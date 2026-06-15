using Byohar.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Domain.Entities.Events;

public class Event : AuditableEntity<Guid>
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? TenantId { get; set; }

    public string? BannerImageUrl { get; set; }
}
