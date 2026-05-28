using Flowtap_Application.Features.Notifications.Commands.BroadcastNotification;
using Flowtap_Application.Features.Notifications.Commands.DismissAdminBroadcast;
using Flowtap_Application.Features.Notifications.Commands.SendNotification;
using Flowtap_Application.Features.Notifications.DTOs;
using Flowtap_Application.Features.Notifications.Queries.GetAdminBroadcasts;
using Flowtap_Application.Features.Notifications.Queries.GetNotifications;
using Flowtap_Presentation.Hubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/notifications")]
public class NotificationController(ISender sender, IHubContext<CommunicationsHub> hub)
    : ApiController(sender)
{
    private string SentByName =>
        User.FindFirstValue("name") ?? User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendNotificationCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    /// <summary>
    /// Broadcast a message to all employees of the company (or a specific store).
    /// Channel is comma-separated: "Email", "SMS", "App", or any combination.
    /// AdminBroadcast (in-app) is created ONLY when "App" is in the channel.
    /// Email/SMS are queued ONLY for the respective channel selections.
    /// Returns the total count of notifications queued (email+sms+1 for in-app).
    /// </summary>
    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastNotificationCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(
            command with { CompanyId = CurrentTenantId, SentByName = SentByName }, ct);

        // Push via SignalR so connected clients receive the announcement immediately.
        // Always push to the company-wide group — the frontend filters by locationId on re-fetch.
        // (location-{id} groups are not auto-joined by users in OnConnectedAsync)
        if (result.IsSuccess)
        {
            var dto = new AdminBroadcastDto(
                Guid.NewGuid(), command.Subject, command.Message,
                command.Severity, SentByName, DateTime.UtcNow,
                command.LocationId, command.Type, command.TargetType,
                command.Priority, command.StartDate, command.EndDate);

            await hub.Clients.Group($"company-{CurrentTenantId}").SendAsync("NewBroadcast", dto, ct);
        }

        return Ok(result);
    }

    /// <summary>
    /// Paginated notification history for the current company.
    /// Filter by channel (Email/Sms) and status (Pending/Sent/Failed).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetHistory(
        [FromQuery] string? channel,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
        => Ok(await Sender.Send(new GetNotificationsQuery(CurrentTenantId, channel, status, page, pageSize), ct));

    /// <summary>
    /// Get active in-app broadcast messages.
    /// Optionally filter by locationId to get store-specific + company-wide messages.
    /// </summary>
    [HttpGet("broadcasts")]
    public async Task<IActionResult> GetBroadcasts(
        [FromQuery] Guid? locationId,
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
        => Ok(await Sender.Send(new GetAdminBroadcastsQuery(CurrentTenantId, CurrentUserId, locationId, limit), ct));

    /// <summary>
    /// Get announcements filtered by Type (Banner, Popup, Notification).
    /// </summary>
    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements(
        [FromQuery] string? type,
        [FromQuery] Guid? locationId,
        [FromQuery] bool activeOnly = true,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
        => Ok(await Sender.Send(new GetAdminBroadcastsQuery(CurrentTenantId, CurrentUserId, locationId, limit, type, activeOnly), ct));

    /// <summary>
    /// Dismiss (soft-delete) an in-app broadcast message.
    /// </summary>
    [HttpDelete("broadcasts/{id:guid}")]
    public async Task<IActionResult> DismissBroadcast(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DismissAdminBroadcastCommand(id, CurrentTenantId, CurrentUserId), ct));
}
