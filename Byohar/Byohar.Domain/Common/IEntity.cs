namespace Byohar.Domain.Common;

public interface IEntity
{
    public Guid TenantId { get; set; }
}

public interface IEntity<TId> : IEntity
{
    public TId Id { get; set; }
}