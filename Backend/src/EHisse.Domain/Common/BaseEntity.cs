namespace EHisse.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
}

public interface IAggregateRoot
{
}
