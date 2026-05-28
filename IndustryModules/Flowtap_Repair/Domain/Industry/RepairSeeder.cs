namespace Flowtap_Domain.BoundedContexts.Industries.Repair;

public static class RepairSeeder
{
    public static IEnumerable<string> GetDefaultServiceCategories()
        => ["Screen Repair", "Battery Replacement", "Water Damage", "Software Issues", "Board Level Repair"];

    public static IEnumerable<string> GetDefaultStatuses()
        => ["New", "Diagnosed", "In Repair", "Waiting Parts", "Ready", "Delivered", "Warranty"];
}
