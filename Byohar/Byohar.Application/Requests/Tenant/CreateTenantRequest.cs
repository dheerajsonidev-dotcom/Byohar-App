
using Byohar.Shared.Wrapper;
using MediatR;

namespace Byohar.Application.Requests.Tenant;

public class CreateTenantRequest : CreateEditRequest<Domain.Entities.Tenant.Tenant>, IRequest<Result<string>>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}
