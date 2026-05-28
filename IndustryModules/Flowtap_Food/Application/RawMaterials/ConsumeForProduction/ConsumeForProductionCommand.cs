using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.RawMaterials.ConsumeForProduction;

public record ConsumeForProductionCommand(
    Guid CompanyId,
    Guid MenuItemId,        // the finished Product being produced
    Guid WarehouseId,       // kitchen warehouse to deduct stock from
    int QuantityProduced,   // how many portions
    Guid? KitchenOrderId)   // link to KOT (optional)
    : IRequest<Result>;
