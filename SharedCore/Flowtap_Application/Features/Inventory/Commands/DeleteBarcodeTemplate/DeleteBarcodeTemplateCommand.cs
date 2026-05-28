using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteBarcodeTemplate;

public record DeleteBarcodeTemplateCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
