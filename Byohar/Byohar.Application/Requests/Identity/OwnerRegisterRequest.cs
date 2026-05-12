using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Requests.Identity;

public class OwnerRegisterRequest
{
    public string TenantName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
