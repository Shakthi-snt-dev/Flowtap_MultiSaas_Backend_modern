using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.BroadcastNotification;

public record BroadcastNotificationCommand(
    Guid CompanyId,
    string Subject,
    string Message,
    string Channel,                              // comma-separated: "Email","SMS","App","Email,SMS","Email,App","Email,SMS,App" etc. Legacy: "Both"=Email+SMS, "InApp"=App
    string Severity = "Information",             // "Information" | "Warning" | "Alert"
    string? SentByName = null,
    Guid? LocationId = null,                     // null = all stores in company
    string Type = "Notification",                // "Banner" | "Popup" | "Notification"
    string TargetType = "AllStores",             // "AllStores" | "SelectedStores" | "Role"
    string? TargetRole = null,
    string Priority = "Normal",                  // "Low" | "Normal" | "High" | "Critical"
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    List<Guid>? TargetLocationIds = null
) : IRequest<Result<int>>;
