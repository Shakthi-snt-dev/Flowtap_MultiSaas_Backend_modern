using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Flowtap_Presentation.Hubs;

/// <summary>
/// Real-time hub for broadcasts, direct messages, and chat.
/// Groups:
///   company-{companyId}           — all users in the company
///   location-{locationId}         — all users in a specific store
///   user-{userId}                 — personal channel for a specific user
///   conversation-{conversationId} — all participants of a chat thread
/// Events pushed to clients:
///   NewBroadcast       — AdminBroadcast created (payload: AdminBroadcastDto)
///   NewDirectMessage   — DirectMessage received (payload: { id, subject, senderName })
///   NewChatMessage     — ChatMessage in a conversation (payload: ChatMessageDto)
///   MessageSeen        — Conversation marked seen (payload: { conversationId, userId, seenAt })
///   NewUserNotification — System notification created (payload: UserNotificationDto)
/// </summary>
[Authorize]
public class CommunicationsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var companyId = Context.User?.FindFirstValue("companyId") ?? Context.User?.FindFirstValue("tenantId");
        var locationId = Context.User?.FindFirstValue("locationId");
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(companyId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"company-{companyId}");

        if (!string.IsNullOrEmpty(locationId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"location-{locationId}");

        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

        await base.OnConnectedAsync();
    }

    /// <summary>Join a conversation group to receive real-time chat messages.</summary>
    public async Task JoinConversation(string conversationId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");

    /// <summary>Leave a conversation group.</summary>
    public async Task LeaveConversation(string conversationId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");
}
