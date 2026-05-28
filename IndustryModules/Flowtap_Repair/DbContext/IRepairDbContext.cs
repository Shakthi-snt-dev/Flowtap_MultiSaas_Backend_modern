using Flowtap_Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.DbContext;

using Flowtap_Repair.Domain.Entities;

public interface IRepairDbContext : IApplicationDbContext
{
    DbSet<ServiceTicket> ServiceTickets { get; }
    DbSet<ServiceTicketItem> ServiceTicketItems { get; }
    DbSet<ServiceTicketPartUsage> ServiceTicketPartUsages { get; }
    DbSet<TicketTimeLog> TicketTimeLogs { get; }
    DbSet<Service> Services { get; }
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<ServicePartRequirement> ServicePartRequirements { get; }
    DbSet<ServiceDeviceModelMapping> ServiceDeviceModelMappings { get; }
    DbSet<TechnicalFault> TechnicalFaults { get; }
    DbSet<RepairChecklistTemplate> RepairChecklistTemplates { get; }
    DbSet<WorkTask> WorkTasks { get; }
    DbSet<WorkTaskTag> WorkTaskTags { get; }
    DbSet<TaskTimeLog> TaskTimeLogs { get; }
    DbSet<ActivityLog> ActivityLogs { get; }
    DbSet<DeviceBrand> DeviceBrands { get; }
    DbSet<DeviceModel> DeviceModels { get; }
    DbSet<ProductDeviceModelMapping> ProductDeviceModelMappings { get; }
}

