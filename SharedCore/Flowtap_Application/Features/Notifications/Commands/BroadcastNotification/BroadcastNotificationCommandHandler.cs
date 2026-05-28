using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Commands.BroadcastNotification;

public class BroadcastNotificationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<BroadcastNotificationCommand, Result<int>>
{
    public async Task<Result<int>> Handle(BroadcastNotificationCommand request, CancellationToken ct)
    {
        // Parse channel — supports comma-separated values: "Email", "SMS", "App", "Email,SMS", etc.
        // Legacy values: "InApp" (→ App only), "Both" (→ Email + SMS, no App).
        var rawChannel = request.Channel?.ToLowerInvariant() ?? "app";
        var parts      = rawChannel.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var sendApp   = parts.Contains("app") || rawChannel == "inapp";
        var sendEmail = parts.Contains("email");
        var sendSms   = parts.Contains("sms") || rawChannel == "both" || parts.Contains("both");

        // Create in-app AdminBroadcast ONLY when App channel is selected
        if (sendApp)
        {
            var adminBroadcast = new AdminBroadcast
            {
                CompanyId  = request.CompanyId,
                LocationId = request.LocationId,
                Subject    = request.Subject,
                Message    = request.Message,
                Severity   = request.Severity,
                SentBy     = request.SentByName ?? "Admin",
                IsActive   = true,
                CreatedAt  = DateTime.UtcNow,
                Type       = request.Type,
                TargetType = request.TargetType,
                TargetRole = request.TargetRole,
                Priority   = request.Priority,
                StartDate  = request.StartDate?.ToUniversalTime(),
                EndDate    = request.EndDate?.ToUniversalTime(),
            };
            db.AdminBroadcasts.Add(adminBroadcast);
            await db.SaveChangesAsync(ct);

            // Add per-store targets if SelectedStores
            if (request.TargetType == "SelectedStores" && request.TargetLocationIds?.Any() == true)
            {
                db.AnnouncementTargets.AddRange(
                    request.TargetLocationIds.Select(lid => new AnnouncementTarget
                    {
                        AdminBroadcastId = adminBroadcast.Id,
                        LocationId       = lid,
                    })
                );
                await db.SaveChangesAsync(ct);
            }
        }

        // If neither Email nor SMS selected, return (in-app was handled above)
        if (!sendEmail && !sendSms)
            return Result<int>.Success(sendApp ? 1 : 0);

        // Build employee query
        var employeesQ = db.Employees
            .Where(e => e.CompanyId == request.CompanyId && e.Status == EmployeeStatus.Active);

        if (request.TargetType == "SelectedStores" && request.TargetLocationIds?.Any() == true)
        {
            employeesQ = employeesQ.Where(e => 
                (e.DefaultLocationId.HasValue && request.TargetLocationIds.Contains(e.DefaultLocationId.Value)) ||
                e.LocationAccess.Any(la => request.TargetLocationIds.Contains(la.LocationId))
            );
        }
        else if (request.LocationId.HasValue)
        {
            employeesQ = employeesQ.Where(e => 
                e.DefaultLocationId == request.LocationId.Value ||
                e.LocationAccess.Any(la => la.LocationId == request.LocationId.Value)
            );
        }

        var contacts = await employeesQ
            .Join(db.UserProfiles,
                  e => e.UserAccountId,
                  p => p.UserAccountId,
                  (e, p) => new { p.Email, p.Phone })
            .ToListAsync(ct);

        var queued = new List<NotificationQueue>();

        foreach (var contact in contacts)
        {
            if (sendEmail && !string.IsNullOrWhiteSpace(contact.Email))
            {
                queued.Add(new NotificationQueue
                {
                    Type      = "Email",
                    Recipient = contact.Email,
                    Subject   = request.Subject,
                    Payload   = request.Message,
                    Status    = "Pending"
                });
            }

            if (sendSms && !string.IsNullOrWhiteSpace(contact.Phone))
            {
                queued.Add(new NotificationQueue
                {
                    Type      = "Sms",
                    Recipient = contact.Phone,
                    Subject   = request.Subject,
                    Payload   = request.Message,
                    Status    = "Pending"
                });
            }
        }

        if (queued.Count == 0)
        {
            // No contacts found for email/SMS — in-app was already created if App was selected.
            return Result<int>.Success(sendApp ? 1 : 0);
        }

        db.NotificationQueues.AddRange(queued);
        await db.SaveChangesAsync(ct);

        return Result<int>.Success(queued.Count + (sendApp ? 1 : 0));
    }
}
