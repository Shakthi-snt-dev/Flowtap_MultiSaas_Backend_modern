using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.DeleteCampaign;

public class DeleteCampaignCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteCampaignCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCampaignCommand request, CancellationToken ct)
    {
        var campaign = await db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, ct);

        if (campaign is null)
            return Result<bool>.Failure("Campaign not found.");

        campaign.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
