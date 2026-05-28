namespace Flowtap_Application.Features.Chat.DTOs;

public record ParticipantInfoDto(Guid UserId, string Name);

public record ConversationSummaryDto(
    Guid Id,
    string? Title,
    bool IsGroup,
    string LastMessage,
    string LastSenderName,
    DateTime LastMessageAt,
    int UnreadCount,
    DateTime? LastSeenAt,
    List<ParticipantInfoDto> Participants);

public record ChatMessageDto(
    Guid Id,
    Guid SenderId,
    string SenderName,
    string Body,
    bool IsDeleted,
    DateTime CreatedAt,
    bool IsOwn);
