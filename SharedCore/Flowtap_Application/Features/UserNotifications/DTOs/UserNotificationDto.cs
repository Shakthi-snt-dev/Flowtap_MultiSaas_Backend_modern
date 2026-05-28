namespace Flowtap_Application.Features.UserNotifications.DTOs;

public record UserNotificationDto(
    Guid Id,
    string Type,
    string Title,
    string Message,
    Guid? ReferenceId,
    string? ReferenceType,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt);
