using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.SendDirectMessage;

public class SendDirectMessageCommandHandler(IApplicationDbContext db)
    : IRequestHandler<SendDirectMessageCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SendDirectMessageCommand request, CancellationToken ct)
    {
        var message = new DirectMessage
        {
            CompanyId   = request.CompanyId,
            SenderId    = request.SenderId,
            RecipientId = request.RecipientId,
            Subject     = request.Subject.Trim(),
            Body        = request.Body.Trim(),
            IsComplaint = request.IsComplaint,
            CreatedAt   = DateTime.UtcNow,
        };

        db.DirectMessages.Add(message);
        await db.SaveChangesAsync(ct);

        return Result<Guid>.Success(message.Id);
    }
}
