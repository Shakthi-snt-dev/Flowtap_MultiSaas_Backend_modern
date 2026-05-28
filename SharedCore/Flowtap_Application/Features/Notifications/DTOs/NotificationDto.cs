namespace Flowtap_Application.Features.Notifications.DTOs;

public record NotificationSettingsDto(
    bool AssignedToTicketEmail, bool AssignedToTicketSms, bool TicketStatusChangedPush,
    bool TaskAssignedEmail, bool TaskStatusChangedPush, bool LowStockAlert, bool PaymentReceivedEmail);

public record NotificationListItemDto(
    Guid Id, string Type, string Recipient, string Subject,
    string Status, string? Error, DateTime CreatedAt, DateTime? SentAt);

public record AdminBroadcastDto(
    Guid Id, string Subject, string Message, string Severity,
    string SentBy, DateTime CreatedAt, Guid? LocationId,
    string Type = "Notification", string TargetType = "AllStores",
    string Priority = "Normal", DateTime? StartDate = null, DateTime? EndDate = null);

public record DirectMessageDto(
    Guid Id,
    string Subject,
    string Body,
    Guid SenderId,
    string SenderName,
    Guid RecipientId,
    string RecipientName,
    bool IsRead,
    bool IsComplaint,
    DateTime CreatedAt,
    DateTime? ReadAt);
