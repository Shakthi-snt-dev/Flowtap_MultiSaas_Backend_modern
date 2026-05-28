using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.UpdateTicket;

public record UpdateTicketCommand(
    Guid Id,
    Guid CompanyId,
    // Client
    Guid? ClientId,
    // Assignment
    Guid? ExecutorEmployeeId,
    Guid? ManagerEmployeeId,
    // Priority & scheduling
    string? Priority,
    DateTime? Deadline,
    // Device details
    string? DeviceType,
    string? DeviceBrand,
    string? DeviceModel,
    string? DeviceSerial,
    string? DeviceModification,
    string? Appearance,
    string? Password,
    string? Equipment,
    // Notes & checklists
    string? Reason,
    string? MastersNotes,
    string? PreRepairChecklist,
    string? AccessoryList,
    // Financials
    decimal? EstimatedCost,
    decimal? Prepayment,
    decimal? TotalCost,
    bool? IsPaid
) : IRequest<Result<bool>>;

