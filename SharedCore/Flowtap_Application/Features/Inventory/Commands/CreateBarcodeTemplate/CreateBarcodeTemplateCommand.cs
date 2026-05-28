using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateBarcodeTemplate;

public record CreateBarcodeTemplateCommand(
    Guid CompanyId,
    string Name,
    string CodeType = "Code128",
    bool IsDefault = false,
    int LabelsPerRow = 3,
    decimal LabelWidthMm = 60,
    decimal LabelHeightMm = 40,
    bool ShowProductName = true,
    bool ShowSKU = true) : IRequest<Result<Guid>>;
