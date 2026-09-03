using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Requests.Events;

public class EventRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public string? BannerImageUrl { get; set; }
}
