using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.CreateTicket;

public record CreateTicketCommand(
    Guid CompanyId, Guid? LocationId, Guid? ClientId,
    string? ClientName, string? ClientPhone, string? ClientEmail,
    string Type, string Priority, Guid? PrimaryServiceId,
    string? DeviceBrand, string? DeviceModel, string? DeviceSerial,
    string? Appearance, string? Password, string? Equipment,
    string? Reason, string? MastersNotes, Guid? ExecutorEmployeeId, Guid? ManagerEmployeeId,
    decimal EstimatedCost, decimal Prepayment) : IRequest<Result<Guid>>;

