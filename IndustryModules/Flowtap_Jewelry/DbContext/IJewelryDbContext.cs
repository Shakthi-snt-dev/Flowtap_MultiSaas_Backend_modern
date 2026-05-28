using Flowtap_Application.Common.Interfaces;
using Flowtap_Jewelry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Jewelry.DbContext;

public interface IJewelryDbContext : IApplicationDbContext
{
    DbSet<MetalRate> MetalRates { get; }
    DbSet<MetalExchangeTransaction> MetalExchangeTransactions { get; }
}
