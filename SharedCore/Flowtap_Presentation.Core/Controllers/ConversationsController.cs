using Flowtap_Application.Features.Chat.Commands.DeleteChatMessage;
using Flowtap_Application.Features.Chat.Commands.MarkConversationSeen;
using Flowtap_Application.Features.Chat.Commands.SendChatMessage;
using Flowtap_Application.Features.Chat.Commands.StartOrGetConversation;
using Flowtap_Application.Features.Chat.DTOs;
using Flowtap_Application.Features.Chat.Queries.GetChatMessages;
using Flowtap_Application.Features.Chat.Queries.GetConversations;
using Flowtap_Presentation.Hubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/conversations")]
public class ConversationsController(ISender sender, IHubContext<CommunicationsHub> hub)
    : ApiController(sender)
{
    // Note: SenderName resolved from UserProfiles in SendChatMessageCommandHandler
    // when this claim is empty — "Unknown" is just an interim fallback.
    private string CurrentUserName =>
        User.FindFirstValue("name") ?? User.FindFirstValue(ClaimTypes.Name) ?? "";

    /// <summary>True when the caller is an Owner or Admin — enables full conversation visibility.</summary>
    private bool IsAdminOrOwner
    {
        get
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "";
            return role == "Owner" || role == "Admin";
        }
    }

    /// <summary>Get conversations. Admins/Owners see all conversations in the store; employees see only their own.</summary>
    [HttpGet]
    public async Task<IActionResult> GetConversations([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetConversationsQuery(CurrentTenantId, CurrentUserId, locationId, IsAdminOrOwner), ct));

    /// <summary>Start or get existing 1-on-1 conversation. Body: { otherUserId, otherUserName, locationId }</summary>
    [HttpPost]
    public async Task<IActionResult> StartConversation(
        [FromBody] StartConversationRequest request, CancellationToken ct)
    {
        return Ok(await Sender.Send(new StartOrGetConversationCommand(
            CurrentTenantId, CurrentUserId, CurrentUserName,
            request.OtherUserId, request.OtherUserName ?? "Unknown",
            request.LocationId), ct));
    }

    /// <summary>Get messages in a conversation (paginated). Admins can read any conversation.</summary>
    [HttpGet("{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(
        Guid id, [FromQuery] int page = 1, CancellationToken ct = default)
        => Ok(await Sender.Send(new GetChatMessagesQuery(id, CurrentUserId, page, IsAdmin: IsAdminOrOwner), ct));

    /// <summary>Send a message in a conversation. Body: { body }</summary>
    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid id, [FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var result = await Sender.Send(new SendChatMessageCommand(
            id, CurrentUserId, CurrentUserName, request.Body, CurrentTenantId), ct);

        if (result.IsSuccess)
        {
            // Push to all conversation participants via SignalR
            var msgDto = new ChatMessageDto(
                result.Value, CurrentUserId, CurrentUserName, request.Body,
                false, DateTime.UtcNow, false);
            await hub.Clients.Group($"conversation-{id}").SendAsync("NewChatMessage", msgDto, ct);

            // Also push to all user-{participantId} groups so the conversation list updates
            // (the controller doesn't have participant IDs here, the hub group handles this)
        }

        return Ok(result);
    }

    /// <summary>Mark conversation as seen by current user.</summary>
    [HttpPut("{id:guid}/seen")]
    public async Task<IActionResult> MarkSeen(Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new MarkConversationSeenCommand(id, CurrentUserId), ct);
        if (result.IsSuccess)
        {
            await hub.Clients.Group($"conversation-{id}").SendAsync("MessageSeen", new
            {
                conversationId = id,
                userId = CurrentUserId,
                seenAt = DateTime.UtcNow,
            }, ct);
        }
        return FromResult(result);
    }

    /// <summary>Soft-delete a message (own messages only).</summary>
    [HttpDelete("{id:guid}/messages/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(
        Guid id, Guid messageId, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteChatMessageCommand(messageId, CurrentUserId), ct));
}

public record StartConversationRequest(Guid OtherUserId, string? OtherUserName, Guid? LocationId = null);
public record SendMessageRequest(string Body);
