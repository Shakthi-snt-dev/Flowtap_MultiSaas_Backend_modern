using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Reports;

public record GetMedicineConsumptionQuery(Guid CompanyId, Guid? LocationId, DateTime From, DateTime To)
    : IRequest<Result<MedicineConsumptionDto>>;
