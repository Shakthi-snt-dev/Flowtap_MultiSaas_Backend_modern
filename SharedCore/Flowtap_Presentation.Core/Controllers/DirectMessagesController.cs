using Flowtap_Application.Features.Notifications.Commands.DeleteDirectMessage;
using Flowtap_Application.Features.Notifications.Commands.MarkMessageAsRead;
using Flowtap_Application.Features.Notifications.Commands.SendDirectMessage;
using Flowtap_Application.Features.Notifications.Queries.GetInbox;
using Flowtap_Application.Features.Notifications.Queries.GetSentMessages;
using Flowtap_Application.Features.Notifications.Queries.GetUnreadMessageCount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/messages")]
public class DirectMessagesController(ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Send a direct message to another user in the same company.
    /// IsComplaint=true flags the message as an employee complaint.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendDirectMessageRequest request, CancellationToken ct)
        => Created(await Sender.Send(new SendDirectMessageCommand(
            CurrentTenantId,
            CurrentUserId,
            request.RecipientId,
            request.Subject,
            request.Body,
            request.IsComplaint), ct));

    /// <summary>
    /// Get the current user's inbox (messages received).
    /// Optionally filter to complaints only with ?isComplaint=true
    /// </summary>
    [HttpGet("inbox")]
    public async Task<IActionResult> GetInbox(
        [FromQuery] bool? isComplaint,
        [FromQuery] Guid? locationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await Sender.Send(
            new GetInboxQuery(CurrentTenantId, CurrentUserId, locationId, isComplaint, page, pageSize), ct));

    /// <summary>
    /// Get the current user's sent messages.
    /// </summary>
    [HttpGet("sent")]
    public async Task<IActionResult> GetSent(
        [FromQuery] Guid? locationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await Sender.Send(
            new GetSentMessagesQuery(CurrentTenantId, CurrentUserId, locationId, page, pageSize), ct));

    /// <summary>
    /// Get unread message count for the current user.
    /// Used by the TopNav bell badge.
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
        => Ok(await Sender.Send(new GetUnreadMessageCountQuery(CurrentTenantId, CurrentUserId), ct));

    /// <summary>
    /// Mark a received message as read.
    /// </summary>
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new MarkMessageAsReadCommand(id, CurrentUserId), ct));

    /// <summary>
    /// Soft-delete a message (hidden from sender's sent or recipient's inbox).
    /// Message is permanently invisible once both sides delete it.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteDirectMessageCommand(id, CurrentUserId), ct));
}

/// <summary>Request body for sending a direct message.</summary>
public record SendDirectMessageRequest(
    Guid   RecipientId,
    string Subject,
    string Body,
    bool   IsComplaint = false);
