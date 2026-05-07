using Byohar.Application.Specifications.Base;
using Byohar.Domain.Entities.Identity;

namespace Byohar.Infrastructure.Specifications
{
    public class RoleFilterSpecification : Specification<Role>
    {
        public RoleFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.Description.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}
