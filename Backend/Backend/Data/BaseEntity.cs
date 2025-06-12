using LinqToDB.Mapping;

namespace Backend.Data;

public abstract class BaseEntity : IEntity
{
    [Column, PrimaryKey, Identity]
    public int Id { get; set; }
   
    [Column]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}