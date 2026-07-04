using FinGuardLite.Models;
using Microsoft.EntityFrameworkCore;

namespace FinGuardLite.Data
{
    public class FinGuardDbContext : DbContext
    {
        public FinGuardDbContext(DbContextOptions<FinGuardDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<TransactionRecord> TransactionRecords { get; set; }
        public DbSet<RiskAlert> RiskAlerts { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    FullName = "Tan Wei Ming",
                    Age = 28,
                    MonthlyIncome = 3200,
                    Country = "Singapore",
                    RiskProfile = "Low",
                    CreatedAt = new DateTime(2026, 6, 1)
                },
                new Customer
                {
                    CustomerId = 2,
                    FullName = "Sarah Lim",
                    Age = 24,
                    MonthlyIncome = 2200,
                    Country = "Singapore",
                    RiskProfile = "Medium",
                    CreatedAt = new DateTime(2026, 6, 1)
                },
                new Customer
                {
                    CustomerId = 3,
                    FullName = "Daniel Ong",
                    Age = 35,
                    MonthlyIncome = 5800,
                    Country = "Singapore",
                    RiskProfile = "Low",
                    CreatedAt = new DateTime(2026, 6, 1)
                }
            );
        }
    }
}