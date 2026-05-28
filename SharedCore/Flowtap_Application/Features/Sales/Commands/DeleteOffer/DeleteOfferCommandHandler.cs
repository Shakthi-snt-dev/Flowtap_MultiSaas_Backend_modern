using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.DeleteOffer;

public class DeleteOfferCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteOfferCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOfferCommand request, CancellationToken ct)
    {
        var offer = await db.Offers
            .FirstOrDefaultAsync(o => o.Id == request.Id && o.CompanyId == request.CompanyId, ct);

        if (offer is null)
            return Result<bool>.Failure("Offer not found.");

        offer.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
