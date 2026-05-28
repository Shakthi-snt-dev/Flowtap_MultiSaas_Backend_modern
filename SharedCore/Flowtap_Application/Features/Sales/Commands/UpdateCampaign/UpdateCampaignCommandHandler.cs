using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.UpdateCampaign;

public class UpdateCampaignCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateCampaignCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateCampaignCommand request, CancellationToken ct)
    {
        var campaign = await db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, ct);

        if (campaign is null)
            return Result<bool>.Failure("Campaign not found.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            campaign.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<CampaignStatus>(request.Status, true, out var status))
            campaign.Status = status;

        if (request.StartDate.HasValue)
            campaign.StartDate = request.StartDate.Value;

        if (request.EndDate.HasValue)
            campaign.EndDate = request.EndDate.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
