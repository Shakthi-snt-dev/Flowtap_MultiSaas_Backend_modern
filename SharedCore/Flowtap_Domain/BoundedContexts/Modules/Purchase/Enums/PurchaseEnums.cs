namespace Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;

public enum PurchaseOrderStatus
{
    Draft = 1, Submitted = 2, Sent = 3, PartiallyReceived = 4, Received = 5, Cancelled = 6
}

public enum PurchaseOrderItemStatus { Pending = 1, Processing = 2, Completed = 3 }

public enum PurchaseReturnStatus { Draft = 1, Approved = 2, Cancelled = 3 }

public enum PaymentStatus { Unpaid = 1, PartiallyPaid = 2, Paid = 3 }
