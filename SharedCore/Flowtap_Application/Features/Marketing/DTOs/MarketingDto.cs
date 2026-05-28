namespace Flowtap_Application.Features.Marketing.DTOs;

public record MarketingCampaignDto(
    Guid Id,
    string Title,
    string Message,
    decimal DiscountPercentage,
    bool IsActive,
    List<Guid> TargetLocationIds,
    DateTime CreatedAt);
