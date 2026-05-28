using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetClientPurchases;

public class GetClientPurchasesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetClientPurchasesQuery, Result<PaginatedList<ClientPurchaseDto>>>
{
    public async Task<Result<PaginatedList<ClientPurchaseDto>>> Handle(
        GetClientPurchasesQuery request, CancellationToken ct)
    {
        var query = db.Sales
            .Where(s => s.CompanyId == request.CompanyId && s.ClientId == request.ClientId);

        var total = await query.CountAsync(ct);

        var sales = await query
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = sales.Select(s =>
        {
            var totalPaid  = s.Payments.Sum(p => p.Amount);
            var balanceDue = s.TotalAmount - totalPaid;

            return new ClientPurchaseDto(
                Id                : s.Id,
                TransactionNumber : s.TransactionNumber,
                Status            : s.Status.ToString(),
                Source            : s.Source.ToString(),
                SubTotal          : s.SubTotal,
                DiscountAmount    : s.DiscountAmount,
                TaxAmount         : s.TaxAmount,
                TotalAmount       : s.TotalAmount,
                TotalPaid         : totalPaid,
                BalanceDue        : balanceDue < 0 ? 0 : balanceDue,
                ItemCount         : s.Items.Count,
                CreatedAt         : s.CreatedAt,
                Payments          : s.Payments.Select(p => new ClientPurchasePaymentDto(
                    Id      : p.Id,
                    Method  : p.Method.ToString(),
                    Amount  : p.Amount,
                    Purpose : p.Purpose.ToString(),
                    PaidAt  : p.PaidAt)).ToList()
            );
        }).ToList();

        return Result<PaginatedList<ClientPurchaseDto>>.Success(
            new PaginatedList<ClientPurchaseDto>(dtos, total, request.Page, request.PageSize));
    }
}
