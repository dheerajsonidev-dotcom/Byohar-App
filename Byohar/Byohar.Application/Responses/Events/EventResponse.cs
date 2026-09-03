using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Responses.Events;

public class EventResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public string? Location { get; set; }

    public bool IsActive { get; set; }

    public string? BannerImageUrl { get; set; }
}
