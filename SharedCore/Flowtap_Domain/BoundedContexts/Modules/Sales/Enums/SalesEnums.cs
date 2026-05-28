namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;

public enum ClientType { Individual = 1, Company = 2 }

public enum SaleItemType { Service = 1, Part = 2, Product = 3 }

public enum SaleSource { POS = 1, Ticket = 2 }

public enum SaleStatus { Draft = 1, Completed = 2, Cancelled = 3, Refunded = 4 }

public enum PaymentMethod { Cash = 1, Card = 2, UPI = 3, NetBanking = 4, Wallet = 5 }

public enum PaymentPurpose { Advance = 1, Final = 2, Partial = 3 }

public enum PaymentAccountType { Cash = 1, Bank = 2, Gateway = 3 }

public enum TransactionStatus { Pending = 1, Success = 2, Failed = 3 }

public enum GatewayProvider { Razorpay = 1, Stripe = 2, PayPal = 3 }

public enum CampaignStatus { Scheduled = 1, Active = 2, Ended = 3, Paused = 4 }

public enum CampaignType { Discount = 1, Bundle = 2, FlashSale = 3, BOGO = 4 }

public enum FoodOrderType { DineIn = 1, Takeaway = 2, Delivery = 3 }
