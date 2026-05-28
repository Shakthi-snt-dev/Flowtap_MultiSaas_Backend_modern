using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Marketing.Commands.DeleteMarketingCampaign;

public class DeleteMarketingCampaignCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteMarketingCampaignCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteMarketingCampaignCommand request, CancellationToken ct)
    {
        var campaign = await db.MarketingCampaigns
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, ct);

        if (campaign is null)
            return Result<bool>.Failure("Marketing campaign not found.");

        campaign.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
