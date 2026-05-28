using Flowtap_Application.Common.Interfaces;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Entities;
using Flowtap_Infrastructure.Persistence.DbContext;
using Flowtap_Jewelry.DbContext;
using Flowtap_Jewelry.Domain.Entities;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Entities;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowtap_Presentation.Persistence;

/// <summary>
/// The master DbContext for Flowtap_Presentation — the only deployment that serves
/// ALL 5 industries from ONE database.
///
/// Inherits ApplicationDbContext (core tables) and implements every module interface,
/// so handlers for Food, Repair, Hotel, Medical, and Jewelry all resolve to this
/// single context. Its migration creates the FULL schema in one shot — no
/// separate module migrations, no trimming, no conflicts.
///
/// Used ONLY in Flowtap_Presentation. Standalone industry APIs (Food_API, Repair_API,
/// etc.) use their own module DbContext against their own dedicated database.
/// </summary>
public class PresentationDbContext(
    DbContextOptions<PresentationDbContext> options,
    ICurrentUserService currentUser,
    IPublisher publisher)
    : ApplicationDbContext(ChangeOptionsType<ApplicationDbContext>(options), currentUser, publisher),
      IFoodDbContext,
      IRepairDbContext,
      IHotelDbContext,
      IMedicalDbContext,
      IJewelryDbContext
{
    // ── Food ──────────────────────────────────────────────────────────────────
    public DbSet<FoodTable>          FoodTables          => Set<FoodTable>();
    public DbSet<KitchenOrder>       KitchenOrders       => Set<KitchenOrder>();
    public DbSet<KitchenOrderItem>   KitchenOrderItems   => Set<KitchenOrderItem>();
    public DbSet<Recipe>             Recipes             => Set<Recipe>();
    public DbSet<RecipeIngredient>   RecipeIngredients   => Set<RecipeIngredient>();
    public DbSet<StockAlertRule>     StockAlertRules     => Set<StockAlertRule>();

    // ── Repair ────────────────────────────────────────────────────────────────
    public DbSet<ServiceTicket>             ServiceTickets             => Set<ServiceTicket>();
    public DbSet<ServiceTicketItem>         ServiceTicketItems         => Set<ServiceTicketItem>();
    public DbSet<ServiceTicketPartUsage>    ServiceTicketPartUsages    => Set<ServiceTicketPartUsage>();
    public DbSet<TicketTimeLog>             TicketTimeLogs             => Set<TicketTimeLog>();
    public DbSet<Service>                   Services                   => Set<Service>();
    public DbSet<ServiceCategory>           ServiceCategories          => Set<ServiceCategory>();
    public DbSet<ServicePartRequirement>    ServicePartRequirements    => Set<ServicePartRequirement>();
    public DbSet<ServiceDeviceModelMapping> ServiceDeviceModelMappings => Set<ServiceDeviceModelMapping>();
    public DbSet<TechnicalFault>            TechnicalFaults            => Set<TechnicalFault>();
    public DbSet<RepairChecklistTemplate>   RepairChecklistTemplates   => Set<RepairChecklistTemplate>();
    public DbSet<WorkTask>                  WorkTasks                  => Set<WorkTask>();
    public DbSet<WorkTaskTag>               WorkTaskTags               => Set<WorkTaskTag>();
    public DbSet<TaskTimeLog>               TaskTimeLogs               => Set<TaskTimeLog>();
    public DbSet<ActivityLog>               ActivityLogs               => Set<ActivityLog>();
    public DbSet<DeviceBrand>               DeviceBrands               => Set<DeviceBrand>();
    public DbSet<DeviceModel>               DeviceModels               => Set<DeviceModel>();
    public DbSet<ProductDeviceModelMapping> ProductDeviceModelMappings => Set<ProductDeviceModelMapping>();

    // ── Hotel ─────────────────────────────────────────────────────────────────
    public DbSet<HotelRoom>    HotelRooms    => Set<HotelRoom>();
    public DbSet<HotelBooking> HotelBookings => Set<HotelBooking>();

    // ── Medical ───────────────────────────────────────────────────────────────
    public DbSet<Patient>      Patients      => Set<Patient>();
    public DbSet<Appointment>  Appointments  => Set<Appointment>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    // ── Jewelry ───────────────────────────────────────────────────────────────
    public DbSet<MetalRate>               MetalRates               => Set<MetalRate>();
    public DbSet<MetalExchangeTransaction> MetalExchangeTransactions => Set<MetalExchangeTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Core table configurations (from ApplicationDbContext + Flowtap_Infrastructure configs)
        base.OnModelCreating(modelBuilder);

        // ── Food ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<FoodTable>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.KitchenOrders).WithOne(x => x.Table).HasForeignKey(x => x.TableId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<KitchenOrder>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Items).WithOne(x => x.KitchenOrder).HasForeignKey(x => x.KitchenOrderId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<KitchenOrderItem>(b => b.HasKey(x => x.Id));
        modelBuilder.Entity<Recipe>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Ingredients).WithOne(x => x.Recipe).HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<RecipeIngredient>(b => b.HasKey(x => x.Id));
        modelBuilder.Entity<StockAlertRule>(b => b.HasKey(x => x.Id));

        // ── Repair ────────────────────────────────────────────────────────────
        // Repair uses IEntityTypeConfiguration classes — scan the Repair assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Flowtap_Repair.Extensions.RepairServiceExtensions).Assembly);

        // ── Hotel ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<HotelRoom>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Bookings).WithOne(x => x.Room).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<HotelBooking>(b => b.HasKey(x => x.Id));

        // ── Medical ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Patient>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Appointments).WithOne(x => x.Patient).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Appointment>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Consultation).WithOne(x => x.Appointment).HasForeignKey<Consultation>(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Consultation>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Prescriptions).WithOne(x => x.Consultation).HasForeignKey(x => x.ConsultationId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Prescription>(b => b.HasKey(x => x.Id));

        // ── Jewelry ───────────────────────────────────────────────────────────
        modelBuilder.Entity<MetalRate>(b => b.HasKey(x => x.Id));
        modelBuilder.Entity<MetalExchangeTransaction>(b => b.HasKey(x => x.Id));
    }

    // Helper: convert DbContextOptions<TDerived> → DbContextOptions<TBase>
    private static DbContextOptions<TBase> ChangeOptionsType<TBase>(DbContextOptions options)
        where TBase : Microsoft.EntityFrameworkCore.DbContext
    {
        var builder = new DbContextOptionsBuilder<TBase>();
        foreach (var extension in options.Extensions)
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
        return builder.Options;
    }
}
