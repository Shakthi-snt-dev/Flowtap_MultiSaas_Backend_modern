using Flowtap_Application.Features.UserNotifications.Commands.DeleteNotification;
using Flowtap_Application.Features.UserNotifications.Commands.MarkAllNotificationsRead;
using Flowtap_Application.Features.UserNotifications.Commands.MarkNotificationRead;
using Flowtap_Application.Features.UserNotifications.Queries.GetUnreadNotificationCount;
using Flowtap_Application.Features.UserNotifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/user-notifications")]
public class UserNotificationsController(ISender sender) : ApiController(sender)
{
    /// <summary>Get notifications for current user. Filter by isRead.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isRead,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
        => Ok(await Sender.Send(
            new GetUserNotificationsQuery(CurrentTenantId, CurrentUserId, isRead, page, pageSize), ct));

    /// <summary>Get unread notification count for bell badge.</summary>
    [HttpGet("count")]
    public async Task<IActionResult> GetCount(CancellationToken ct)
        => Ok(await Sender.Send(new GetUnreadNotificationCountQuery(CurrentTenantId, CurrentUserId), ct));

    /// <summary>Mark a specific notification as read.</summary>
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new MarkNotificationReadCommand(id, CurrentUserId), ct));

    /// <summary>Mark all notifications as read for current user.</summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
        => FromResult(await Sender.Send(
            new MarkAllNotificationsReadCommand(CurrentTenantId, CurrentUserId), ct));

    /// <summary>Delete a notification (hard delete).</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteNotificationCommand(id, CurrentUserId), ct));
}
