using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Requests.Addresses;

public class AddressRequest
{
    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PinCode { get; set; }

    public string? Country { get; set; } = "India";
}
