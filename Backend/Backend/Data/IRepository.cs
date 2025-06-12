using LinqToDB;

namespace Backend.Data;

public interface IRepository<TEntity> where TEntity : IEntity
{
    IQueryable<TEntity> EntitySet { get; }

    ITable<TEntity> Table { get; }

    Task<TEntity> GetByIdAsync(int id , bool includeDeleted);
    Task<TEntity> InsertAsync(TEntity entity);
    Task<int> UpdateAsync(TEntity entity);
    Task<int> DeleteAsync(TEntity entity);

    Task<int> InsertAsync(IEnumerable<TEntity> entities);
    Task<int> UpdateAsync(IEnumerable<TEntity> entities);
    Task<int> DeleteAsync(IEnumerable<TEntity> entities);
}

