using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Queries.GetUnreadMessageCount;

public class GetUnreadMessageCountQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetUnreadMessageCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadMessageCountQuery request, CancellationToken ct)
    {
        var count = await db.DirectMessages
            .CountAsync(m =>
                m.CompanyId   == request.CompanyId &&
                m.RecipientId == request.UserId    &&
                !m.IsRead                          &&
                !m.IsDeletedByRecipient, ct);

        return Result<int>.Success(count);
    }
}
