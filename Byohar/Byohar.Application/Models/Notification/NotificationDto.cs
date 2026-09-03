using Byohar.Shared.Notification;

namespace Byohar.Application.Models.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string? Message { get; set; }
    public string LinkToNotification { get; set; }
    public string EntityId { get; set; }
    public NotificationType NotificationType { get; set; }
    public bool IsStatusChanged { get; set; } = false;
    public DateTime? ReadOn { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedOn { get; set; }
}