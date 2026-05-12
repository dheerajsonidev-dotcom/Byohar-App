using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Responses.Identity;

public class OwnerRegisterResponse
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string TenantName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
