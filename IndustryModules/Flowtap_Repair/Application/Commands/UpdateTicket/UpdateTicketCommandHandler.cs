using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.UpdateTicket;

public class UpdateTicketCommandHandler(IRepairDbContext db)
    : IRequestHandler<UpdateTicketCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateTicketCommand request, CancellationToken ct)
    {
        var ticket = await db.ServiceTickets
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("ServiceTicket", request.Id);

        // Client
        if (request.ClientId.HasValue && request.ClientId.Value != Guid.Empty)
            ticket.ClientId = request.ClientId.Value;

        // Assignment
        if (request.ExecutorEmployeeId is not null)
            ticket.ExecutorEmployeeId = request.ExecutorEmployeeId == Guid.Empty ? null : request.ExecutorEmployeeId;

        if (request.ManagerEmployeeId is not null)
            ticket.ManagerEmployeeId = request.ManagerEmployeeId == Guid.Empty ? null : request.ManagerEmployeeId;

        // Priority & scheduling
        if (!string.IsNullOrWhiteSpace(request.Priority)
            && Enum.TryParse<TicketPriority>(request.Priority, true, out var priority))
            ticket.Priority = priority;

        if (request.Deadline is not null)
            ticket.Deadline = request.Deadline;

        // Device details — initialise owned object if first time
        ticket.DeviceDetails ??= new DeviceDetails();
        if (request.DeviceType   is not null) ticket.DeviceDetails.DeviceType   = request.DeviceType;
        if (request.DeviceBrand  is not null)
        {
            var brandName = request.DeviceBrand;
            if (Guid.TryParse(brandName, out var brandId))
            {
                var brand = await db.DeviceBrands.FirstOrDefaultAsync(b => b.Id == brandId, ct);
                if (brand != null) brandName = brand.Name;
            }
            ticket.DeviceDetails.Brand = brandName;
        }
        if (request.DeviceModel  is not null)
        {
            var modelName = request.DeviceModel;
            if (Guid.TryParse(modelName, out var modelId))
            {
                var model = await db.DeviceModels.FirstOrDefaultAsync(m => m.Id == modelId, ct);
                if (model != null) modelName = model.Name;
            }
            ticket.DeviceDetails.Model = modelName;
        }
        if (request.DeviceSerial is not null) ticket.DeviceDetails.Serial       = request.DeviceSerial;
        if (request.DeviceModification is not null) ticket.DeviceDetails.Modification = request.DeviceModification;
        if (request.Appearance   is not null) ticket.DeviceDetails.Appearance   = request.Appearance;
        if (request.Password     is not null) ticket.DeviceDetails.Password     = request.Password;
        if (request.Equipment    is not null) ticket.DeviceDetails.Equipment    = request.Equipment;

        // Notes & checklists
        if (request.Reason            is not null) ticket.Reason            = request.Reason;
        if (request.MastersNotes      is not null) ticket.MastersNotes      = request.MastersNotes;
        if (request.PreRepairChecklist is not null) ticket.PreRepairChecklist = request.PreRepairChecklist;
        if (request.AccessoryList     is not null) ticket.AccessoryList     = request.AccessoryList;

        // Financials — initialise owned object if first time
        ticket.Financials ??= new ServiceFinancials();
        if (request.EstimatedCost.HasValue) ticket.Financials.EstimatedCost = request.EstimatedCost.Value;
        if (request.Prepayment.HasValue)    ticket.Financials.Prepayment    = request.Prepayment.Value;
        if (request.TotalCost.HasValue)     ticket.Financials.TotalCost     = request.TotalCost.Value;
        if (request.IsPaid.HasValue)        ticket.Financials.IsPaid        = request.IsPaid.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

