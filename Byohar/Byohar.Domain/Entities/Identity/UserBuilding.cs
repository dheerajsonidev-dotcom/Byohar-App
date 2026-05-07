using Byohar.Domain.Common;

namespace Byohar.Domain.Entities.Identity
{
    public class UserBuilding : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid BuildingId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
