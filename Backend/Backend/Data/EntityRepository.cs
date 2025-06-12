using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Tools;
using System.Linq.Expressions;

namespace Backend.Data;

public class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly AppDataConnection _dataConnection;

    public EntityRepository(AppDataConnection dataConnection)
    {
        _dataConnection = dataConnection;
    }

    public virtual IQueryable<TEntity> EntitySet => _dataConnection.GetTable<TEntity>();

    public virtual ITable<TEntity> Table => _dataConnection.GetTable<TEntity>();

    public virtual async Task<TEntity> GetByIdAsync(int id, bool includeDeleted = false)
    {
        if (typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var query = _dataConnection.GetTable<TEntity>().Where(e => ((BaseEntity)(object)e).Id == id);

            if (!includeDeleted && typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !((ISoftDelete)(object)e).IsDeleted);
            }

            return await query.FirstOrDefaultAsync();
        }

        throw new InvalidOperationException($"The type {typeof(TEntity).Name} does not inherit from BaseEntity.");
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (entity is ISoftDelete softDeleteEntity)
        {
            softDeleteEntity.IsDeleted = false;
        }
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.Id = await _dataConnection.InsertWithInt32IdentityAsync(entity);
        }
        return entity;
    }

    public virtual async Task<int> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if(entity is BaseEntity baseEntity)
        {
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }
        return await _dataConnection.UpdateAsync(entity);
    }

    public virtual async Task<int> DeleteAsync(TEntity entity)
    {
        
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (entity is ISoftDelete softDeleteEntity)
        {
            softDeleteEntity.IsDeleted = false;
            return await _dataConnection.UpdateAsync(entity);
        }

        return await _dataConnection.DeleteAsync(entity);
    }

    public virtual async Task<int> InsertAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any())
            throw new ArgumentNullException(nameof(entities));
        var entityList = entities.ToList(); 
        foreach (var entity in entityList)
        {
            if (entity is ISoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = false;
            }
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedAt = DateTime.UtcNow;
                baseEntity.UpdatedAt = DateTime.UtcNow;
            }
        }

        var bulkCopyOptions = new BulkCopyOptions
        {
            KeepIdentity = false 
        };

        var rowsAffected = await _dataConnection.BulkCopyAsync(bulkCopyOptions, entityList);

        return (int)rowsAffected.RowsCopied;
    }

    public virtual async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any())
            throw new ArgumentNullException(nameof(entities));
        var entityList = entities.ToList();
        int rowsAffected = 0;
        using (var transaction = await _dataConnection.BeginTransactionAsync())
        {
            try
            {
                foreach (var entity in entityList)
                {
                    if (entity is BaseEntity baseEntity)
                    {
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                    }
                    rowsAffected += await _dataConnection.UpdateAsync(entity);
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        return rowsAffected;
    }

    public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any())
            throw new ArgumentNullException(nameof(entities));

        var entityList = entities.ToList(); 
        int rowsAffected = 0;
        using (var transaction = await _dataConnection.BeginTransactionAsync())
        {
            try
            {
                if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
                {
                    foreach (var entity in entityList)
                    {
                        if (entity is ISoftDelete softDeleteEntity)
                        {
                            softDeleteEntity.IsDeleted = true;
                        }
                        if (entity is BaseEntity baseEntity)
                        {
                            baseEntity.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                    rowsAffected = await UpdateAsync(entityList);
                }
                else
                {
                    if (typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)))
                    {
                        var ids = entityList
                            .OfType<BaseEntity>()
                            .Select(e => e.Id)
                            .ToList();

                        if (ids.Any())
                        {
                            rowsAffected = await _dataConnection
                                .GetTable<TEntity>()
                                .Where(e => ((BaseEntity)(object)e).Id.In(ids))
                                .DeleteAsync();
                        }
                    }
                    else
                    {
                        foreach (var entity in entityList)
                        {
                            rowsAffected += await _dataConnection.DeleteAsync(entity);
                        }
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        return rowsAffected;
    }
}
