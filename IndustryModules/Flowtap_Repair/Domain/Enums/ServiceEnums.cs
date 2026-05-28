namespace Flowtap_Repair.Domain.Enums;

public enum TicketStatus { New = 1, InProgress = 2, WaitingForParts = 3, Done = 4, Canceled = 5 ,Ready = 6}


public enum TicketType { Paid = 1, Offsite = 2, Warranty = 3 }

public enum TicketPriority { Low = 1, Medium = 2, High = 3, Urgent = 4 }

public enum TicketItemType { Service = 1, Part = 2, Product = 3 }

public enum TaskStatus { New = 1, Done = 2, Expired = 3, Cancelled = 4, InProgress = 5, OnHold = 6, Testing = 7, Rejected = 8 }

public enum TaskPriority { Low = 1, Medium = 2, High = 3, Urgent = 4 }

public enum ActivityEntityType
{
    Ticket = 1, Sale = 2, Payment = 3, Task = 4,
    Client = 5, Employee = 6, Product = 7, Service = 8
}
