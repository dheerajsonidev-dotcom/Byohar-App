using Byohar.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Domain.Entities.Addresses;

public class Address:FullAuditableEntity<Guid>
{

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PinCode { get; set; }

    public string? Country { get; set; } = "India";
}
