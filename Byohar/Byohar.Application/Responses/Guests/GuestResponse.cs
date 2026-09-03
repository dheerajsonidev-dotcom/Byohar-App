using Byohar.Application.Requests.Addresses;
using Byohar.Domain.Entities.Addresses;
using Byohar.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Responses.Guests;

public class GuestResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Event? Event { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? MobileNumber { get; set; }

    public string? Relation { get; set; }

    public int TotalMembers { get; set; } = 1;

    public Guid? AddressId { get; set; }

    public AddressRequest? Address { get; set; }

    public bool IsActive { get; set; } = true;
}
