namespace Flowtap_Domain.BoundedContexts.Industries.Repair;

public static class RepairRules
{
    public static bool RequiresDeviceDetails(string ticketType) => ticketType != "General";
    public static bool RequiresEstimate(decimal totalCost) => totalCost > 0;
    public static int DefaultWarrantyDays => 30;
}
