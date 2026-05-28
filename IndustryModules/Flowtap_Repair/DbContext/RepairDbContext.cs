using Flowtap_Application.Common.Interfaces;
using Flowtap_Infrastructure.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowtap_Repair.DbContext;

/// <summary>
/// Extends the shared ApplicationDbContext for the Repair industry module.
/// All ServiceTickets DbSets are inherited from the base ApplicationDbContext.
/// Registered as IRepairDbContext in the Repair API DI container.
/// </summary>
using Flowtap_Repair.Domain.Entities;

public class RepairDbContext(
    DbContextOptions<RepairDbContext> options,
    ICurrentUserService currentUser,
    IPublisher publisher)
    : ApplicationDbContext(ChangeOptionsType<ApplicationDbContext>(options), currentUser, publisher),
      IRepairDbContext
{
    public DbSet<ServiceTicket> ServiceTickets => Set<ServiceTicket>();
    public DbSet<ServiceTicketItem> ServiceTicketItems => Set<ServiceTicketItem>();
    public DbSet<ServiceTicketPartUsage> ServiceTicketPartUsages => Set<ServiceTicketPartUsage>();
    public DbSet<TicketTimeLog> TicketTimeLogs => Set<TicketTimeLog>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServicePartRequirement> ServicePartRequirements => Set<ServicePartRequirement>();
    public DbSet<ServiceDeviceModelMapping> ServiceDeviceModelMappings => Set<ServiceDeviceModelMapping>();
    public DbSet<TechnicalFault> TechnicalFaults => Set<TechnicalFault>();
    public DbSet<RepairChecklistTemplate> RepairChecklistTemplates => Set<RepairChecklistTemplate>();
    public DbSet<WorkTask> WorkTasks => Set<WorkTask>();
    public DbSet<WorkTaskTag> WorkTaskTags => Set<WorkTaskTag>();
    public DbSet<TaskTimeLog> TaskTimeLogs => Set<TaskTimeLog>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<DeviceBrand> DeviceBrands => Set<DeviceBrand>();
    public DbSet<DeviceModel> DeviceModels => Set<DeviceModel>();
    public DbSet<ProductDeviceModelMapping> ProductDeviceModelMappings => Set<ProductDeviceModelMapping>();
    // Helper: convert DbContextOptions<TDerived> → DbContextOptions<TBase>
    private static DbContextOptions<TBase> ChangeOptionsType<TBase>(DbContextOptions options)
        where TBase : Microsoft.EntityFrameworkCore.DbContext
    {
        var builder = new DbContextOptionsBuilder<TBase>();
        foreach (var extension in options.Extensions)
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
        return builder.Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepairDbContext).Assembly);
    }
}

