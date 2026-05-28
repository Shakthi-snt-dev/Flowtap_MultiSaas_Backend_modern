using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.CreateTicket;

public class CreateTicketCommandHandler(IRepairDbContext db)
    : IRequestHandler<CreateTicketCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<TicketType>(request.Type, true, out var type))
            type = TicketType.Paid;

        if (!Enum.TryParse<TicketPriority>(request.Priority, true, out var priority))
            priority = TicketPriority.Medium;

        var locationId = request.LocationId ?? Guid.Empty;

        var clientId = request.ClientId;

        // Verify the provided clientId actually exists in this company's Clients table.
        // Callers may pass a placeholder/fake GUID (e.g. from Swagger defaults), so we
        // validate existence before trusting the value. If invalid, fall through to auto-create.
        var clientIdIsValid = clientId.HasValue
            && clientId.Value != Guid.Empty
            && await db.Clients.AnyAsync(c => c.Id == clientId.Value && c.CompanyId == request.CompanyId, ct);

        if (!clientIdIsValid)
        {
            if (!string.IsNullOrWhiteSpace(request.ClientName))
            {
                var client = new Flowtap_Domain.BoundedContexts.Modules.Sales.Entities.Client
                {
                    CompanyId = request.CompanyId,
                    LocationId = locationId,
                    Name = request.ClientName,
                    Phone = request.ClientPhone,
                    Email = request.ClientEmail,
                    Type = Flowtap_Domain.BoundedContexts.Modules.Sales.Enums.ClientType.Individual
                };
                db.Clients.Add(client);
                await db.SaveChangesAsync(ct);
                clientId = client.Id;
            }
            else
            {
                var walkIn = await db.Clients.FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId && c.Name == "Walk-in Customer", ct);
                if (walkIn is null)
                {
                    walkIn = new Flowtap_Domain.BoundedContexts.Modules.Sales.Entities.Client { CompanyId = request.CompanyId, LocationId = locationId, Name = "Walk-in Customer", Type = Flowtap_Domain.BoundedContexts.Modules.Sales.Enums.ClientType.Individual };
                    db.Clients.Add(walkIn);
                    await db.SaveChangesAsync(ct);
                }
                clientId = walkIn.Id;
            }
        }

        var count = await db.ServiceTickets.CountAsync(t => t.CompanyId == request.CompanyId, ct);

        var brandName = request.DeviceBrand;
        if (Guid.TryParse(brandName, out var brandId))
        {
            var brand = await db.DeviceBrands.FirstOrDefaultAsync(b => b.Id == brandId, ct);
            if (brand != null) brandName = brand.Name;
        }

        var modelName = request.DeviceModel;
        if (Guid.TryParse(modelName, out var modelId))
        {
            var model = await db.DeviceModels.FirstOrDefaultAsync(m => m.Id == modelId, ct);
            if (model != null) modelName = model.Name;
        }

        var ticket = new ServiceTicket
        {
            CompanyId = request.CompanyId,
            LocationId = locationId,
            ClientId = clientId.Value,
            TicketNumber = $"TKT-{count + 1:D6}",
            PrimaryServiceId = request.PrimaryServiceId,
            Type = type,
            Status = TicketStatus.New,
            Priority = priority,
            ExecutorEmployeeId = request.ExecutorEmployeeId,
            ManagerEmployeeId = request.ManagerEmployeeId,
            Reason = request.Reason,
            MastersNotes = request.MastersNotes,
            DeviceDetails = new DeviceDetails
            {
                Brand = brandName,
                Model = modelName,
                Serial = request.DeviceSerial,
                Appearance = request.Appearance,
                Password = request.Password,
                Equipment = request.Equipment
            },
            Financials = new ServiceFinancials
            {
                EstimatedCost = request.EstimatedCost,
                Prepayment = request.Prepayment,
                TotalCost = request.EstimatedCost
            }
        };

        db.ServiceTickets.Add(ticket);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(ticket.Id);
    }
}

