namespace Byohar.Domain.Common;

public interface IFullAuditableEntity : IAuditableEntity
{
    public bool IsDeleted { get; set; }
    DateTime? DeletedOn { get; set; }
    string? DeletedBy { get; set; }
}

public interface IFullAuditableEntity<TId> : IFullAuditableEntity, IAuditableEntity<TId>
{
}
