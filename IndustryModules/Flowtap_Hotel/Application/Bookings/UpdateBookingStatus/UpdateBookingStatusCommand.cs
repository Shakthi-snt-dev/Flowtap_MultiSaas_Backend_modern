using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Hotel.Application.Bookings.UpdateBookingStatus;

public record UpdateBookingStatusCommand(Guid Id, Guid CompanyId, string Status) : IRequest<Result>;
