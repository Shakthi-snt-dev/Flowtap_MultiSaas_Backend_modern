using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Integrations.Commands.DisconnectIntegration;

public class DisconnectIntegrationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DisconnectIntegrationCommand, Result>
{
    public async Task<Result> Handle(DisconnectIntegrationCommand request, CancellationToken ct)
    {
        var integration = await db.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.CompanyId == request.CompanyId, ct);

        if (integration is null)
            return Result.Failure("Integration not found.");

        integration.IsEnabled = false;
        integration.ConnectedAt = null;
        integration.ConfigJson = null;     // clear stored credentials on disconnect
        integration.WebhookUrl = null;
        integration.LastStatusMessage = "Disconnected";
        integration.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
