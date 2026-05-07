using System.ComponentModel.DataAnnotations;

namespace Byohar.Domain.Common
{
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; }
        public Guid TenantId { get; set; }
        [StringLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [StringLength(450)] public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}