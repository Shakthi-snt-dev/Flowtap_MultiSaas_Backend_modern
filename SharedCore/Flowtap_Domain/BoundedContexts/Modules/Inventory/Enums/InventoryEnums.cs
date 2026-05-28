namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

public enum InventoryTransactionType
{
    Purchase = 1, Sale = 2, TransferOut = 3, TransferIn = 4,
    Adjustment = 5, PurchaseReturn = 6, WriteOff = 7, SaleReturn = 8, Consumed = 9
}

public enum InventoryWriteOffType { Damaged = 1, Scrap = 2, Expired = 3 }

public enum ProductKind { Device = 1, Accessory = 2, SparePart = 3, RawMaterial = 4 }

public enum ProductPublishStatus { Draft = 1, Published = 2, Archived = 3 }

public enum StockAdjustmentReason
{
    Supplier = 1, Damaged = 2, Loss = 3, Correction = 4, Returned = 5, Audit = 6
}

public enum StockAdjustmentType { Add = 1, Remove = 2 }

public enum WriteOffStatus { Pending = 1, Approved = 2, Rejected = 3 }

public enum TransferStatus { Draft = 1, InTransit = 2, Shipped = 3, Completed = 4, Cancelled = 5 }

public enum TransferItemStatus { Pending = 1, Shipped = 2, Receiving = 3, Completed = 4, Discrepancy = 5 }

public enum ItemCondition { Good = 1, Damaged = 2, Missing = 3 }

public enum BinStatus { Empty = 1, Partial = 2, Full = 3 }

public enum RackType { Shelf = 1, Pallet = 2, HeavyDuty = 3 }

public enum WarehouseStatus { None = 0, Active = 1, Maintenance = 2, Critical = 3, Inactive = 4 }

public enum WarehouseType { None = 0, InStore = 1, LocationWarehouse = 2, CentralWarehouse = 3, KitchenStore = 4 }

[Flags]
public enum WorkingDays
{
    None = 0, Monday = 1, Tuesday = 2, Wednesday = 4, Thursday = 8,
    Friday = 16, Saturday = 32, Sunday = 64
}

public enum ZoneType
{
    Standard = 1, ColdStorage = 2, Hazardous = 3, Refrigerated = 4, Frozen = 5, Quarantine = 6
}

public enum ValuationMethod { FIFO = 1, Average = 2 }

public enum StockDeductionMode { OnTicketClose = 1, OnInvoicePayment = 2 }

public enum NegativeStockPolicy { Allow = 1, Block = 2, Warn = 3 }

public enum PricingStatus { Draft = 1, Published = 2, Archived = 3, ReviewNeeded = 4 }

public enum ReorderAlertSeverity { Low = 1, Warning = 2, Critical = 3 }
