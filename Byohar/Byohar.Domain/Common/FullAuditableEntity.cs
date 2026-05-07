
using System.ComponentModel.DataAnnotations;

namespace Byohar.Domain.Common
{
    public class FullAuditableEntity<TId> : IFullAuditableEntity<TId>
    {
        public TId Id { get; set; }
        public Guid TenantId { get; set; }
        [StringLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [StringLength(450)] public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string? DeletedBy { get; set; }
    }
}