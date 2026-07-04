using FinGuardLite.Models;
using FinGuardLite.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinGuardLite.Data
{
    public static class FinGuardDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<FinGuardDbContext>();
            var riskScoringService = scope.ServiceProvider.GetRequiredService<RiskScoringService>();

            // Prevent duplicate sample data from being added every time the app runs
            if (await context.AuditLogs.AnyAsync(a => a.Action == "Sample Data Seeded"))
            {
                return;
            }

            List<Customer> customersToAdd = new()
            {
                new Customer { FullName = "Aisha Rahman", Age = 31, MonthlyIncome = 4200, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2021, 3, 12) },
                new Customer { FullName = "Daniel Ong", Age = 26, MonthlyIncome = 6000, Country = "Singapore", RiskProfile = "Medium", CreatedAt = new DateTime(2020, 11, 20) },
                new Customer { FullName = "Ethan Koh", Age = 29, MonthlyIncome = 4800, Country = "Singapore", RiskProfile = "Medium", CreatedAt = new DateTime(2022, 5, 3) },
                new Customer { FullName = "Marcus Lee", Age = 22, MonthlyIncome = 1800, Country = "Singapore", RiskProfile = "Medium", CreatedAt = new DateTime(2024, 1, 15) },
                new Customer { FullName = "Nur Aisyah", Age = 27, MonthlyIncome = 3000, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2022, 8, 9) },
                new Customer { FullName = "Priya Nair", Age = 34, MonthlyIncome = 3600, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2019, 7, 25) },
                new Customer { FullName = "Sarah Lim", Age = 24, MonthlyIncome = 2800, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2023, 4, 18) },
                new Customer { FullName = "Tan Wei Ming", Age = 38, MonthlyIncome = 9000, Country = "Singapore", RiskProfile = "High", CreatedAt = new DateTime(2018, 10, 2) },
                new Customer { FullName = "Chloe Teo", Age = 30, MonthlyIncome = 5200, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2020, 2, 7) },
                new Customer { FullName = "Hafiz Ismail", Age = 33, MonthlyIncome = 4100, Country = "Singapore", RiskProfile = "Medium", CreatedAt = new DateTime(2021, 9, 14) },
                new Customer { FullName = "Ivan Goh", Age = 41, MonthlyIncome = 7500, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2019, 12, 1) },
                new Customer { FullName = "Mei Lin", Age = 28, MonthlyIncome = 3900, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2022, 1, 30) },
                new Customer { FullName = "Ravi Kumar", Age = 36, MonthlyIncome = 5600, Country = "Singapore", RiskProfile = "Medium", CreatedAt = new DateTime(2020, 6, 21) },
                new Customer { FullName = "Siti Aminah", Age = 25, MonthlyIncome = 3100, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2023, 2, 11) },
                new Customer { FullName = "Brandon Chua", Age = 32, MonthlyIncome = 6800, Country = "Singapore", RiskProfile = "Medium", CreatedAt = new DateTime(2021, 5, 5) },
                new Customer { FullName = "Grace Wong", Age = 45, MonthlyIncome = 8200, Country = "Singapore", RiskProfile = "Low", CreatedAt = new DateTime(2018, 3, 17) }
            };

            foreach (var customer in customersToAdd)
            {
                bool customerExists = await context.Customers
                    .AnyAsync(c => c.FullName == customer.FullName);

                if (!customerExists)
                {
                    context.Customers.Add(customer);
                }
            }

            await context.SaveChangesAsync();

            List<Customer> customers = await context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();

            var random = new Random(2815);

            string[] normalMerchants =
            {
                "FairPrice", "Cold Storage", "Sheng Siong", "Grab", "Gojek", "Foodpanda",
                "Shopee", "Lazada", "Amazon", "Apple Store", "Challengers", "Courts",
                "Harvey Norman", "Singapore Airlines", "Agoda", "Steam", "Netflix",
                "StarHub", "Singtel", "SP Utilities"
            };

            string[] normalCategories =
            {
                "Groceries", "Food", "Transport", "Shopping", "Electronics", "Travel",
                "Entertainment", "Education", "Healthcare", "Utilities", "Insurance"
            };

            string[] highRiskMerchants =
            {
                "Binance", "Crypto.com", "Unknown", "Wise Transfer", "Offshore Exchange",
                "Global Remit", "Casino Royale", "Unregistered Vendor"
            };

            string[] highRiskCategories =
            {
                "Crypto", "Money Transfer", "Gambling", "Unknown"
            };

            string[] countries =
            {
                "Singapore", "Malaysia", "Indonesia", "Thailand", "Vietnam", "India",
                "Philippines", "China", "Hong Kong SAR", "United States", "United Kingdom",
                "Australia", "Switzerland", "United Arab Emirates", "Unknown"
            };

            string[] paymentTypes =
            {
                "Card", "Debit Card", "Credit Card", "PayNow", "Bank Transfer", "FAST Transfer",
                "E-Wallet", "Mobile Wallet", "Buy Now Pay Later", "Cash Withdrawal", "Cheque",
                "Crypto", "Other"
            };

            DateTime startDate = new DateTime(2022, 1, 1);
            DateTime endDate = new DateTime(2026, 6, 29);
            int totalDays = (endDate - startDate).Days;

            List<TransactionRecord> transactions = new();

            for (int i = 0; i < 120; i++)
            {
                Customer customer = customers[random.Next(customers.Count)];

                DateTime transactionDate = startDate
                    .AddDays(random.Next(totalDays))
                    .AddHours(random.Next(8, 23))
                    .AddMinutes(random.Next(0, 60));

                bool makeRisky = i % 5 == 0 || i % 11 == 0;

                decimal amount;
                string merchant;
                string category;
                string country;
                string paymentType;

                if (makeRisky)
                {
                    merchant = highRiskMerchants[random.Next(highRiskMerchants.Length)];
                    category = highRiskCategories[random.Next(highRiskCategories.Length)];
                    country = countries[random.Next(1, countries.Length)];
                    paymentType = category == "Crypto" ? "Crypto" : paymentTypes[random.Next(paymentTypes.Length)];

                    int riskType = random.Next(4);

                    amount = riskType switch
                    {
                        0 => random.Next(850, 2500),
                        1 => random.Next(2500, 7000),
                        2 => random.Next(7000, 15000),
                        _ => random.Next(15000, 75000)
                    };
                }
                else
                {
                    merchant = normalMerchants[random.Next(normalMerchants.Length)];
                    category = normalCategories[random.Next(normalCategories.Length)];
                    country = random.Next(100) < 85 ? "Singapore" : countries[random.Next(1, countries.Length - 1)];
                    paymentType = paymentTypes[random.Next(0, 9)];

                    amount = category switch
                    {
                        "Groceries" => random.Next(20, 250),
                        "Food" => random.Next(8, 120),
                        "Transport" => random.Next(5, 80),
                        "Shopping" => random.Next(30, 800),
                        "Electronics" => random.Next(200, 3500),
                        "Travel" => random.Next(300, 4500),
                        "Healthcare" => random.Next(50, 1500),
                        "Insurance" => random.Next(100, 1200),
                        _ => random.Next(20, 1000)
                    };
                }

                TransactionRecord transaction = new()
                {
                    CustomerId = customer.CustomerId,
                    TransactionDate = transactionDate,
                    Amount = amount,
                    Merchant = merchant,
                    Category = category,
                    Country = country,
                    PaymentType = paymentType,
                    CreatedAt = transactionDate.AddMinutes(1)
                };

                transactions.Add(transaction);
            }

            transactions = transactions
                .OrderBy(t => t.TransactionDate)
                .ToList();

            foreach (var transaction in transactions)
            {
                bool transactionExists = await context.TransactionRecords.AnyAsync(t =>
                    t.CustomerId == transaction.CustomerId &&
                    t.TransactionDate == transaction.TransactionDate &&
                    t.Merchant == transaction.Merchant &&
                    t.Amount == transaction.Amount);

                if (transactionExists)
                {
                    continue;
                }

                Customer? customer = await context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == transaction.CustomerId);

                DateTime startOfDay = transaction.TransactionDate.Date;
                DateTime endOfDay = startOfDay.AddDays(1);

                int transactionsTodayCount = await context.TransactionRecords
                    .CountAsync(t =>
                        t.CustomerId == transaction.CustomerId &&
                        t.TransactionDate >= startOfDay &&
                        t.TransactionDate < endOfDay);

                RiskScoringResult result = riskScoringService.CalculateRisk(
                    transaction,
                    customer,
                    transactionsTodayCount);

                transaction.RiskScore = result.RiskScore;
                transaction.RiskLevel = result.RiskLevel;
                transaction.FlagReason = result.FlagReason;

                context.TransactionRecords.Add(transaction);
                await context.SaveChangesAsync();

                DateTime auditTimestamp = transaction.TransactionDate.AddMinutes(random.Next(2, 20));

                context.AuditLogs.Add(new AuditLog
                {
                    Username = "System",
                    Action = "Transaction Ingested",
                    Details = $"Mock transaction {transaction.TransactionRecordId} ingested from simulated payment feed with risk level {transaction.RiskLevel}.",
                    Timestamp = auditTimestamp
                });

                if (transaction.RiskScore >= 30)
                {
                    string status = transaction.RiskScore >= 80
                        ? "Open"
                        : random.Next(100) < 50 ? "Under Review" : "Open";

                    DateTime alertCreatedAt = transaction.TransactionDate.AddMinutes(random.Next(5, 45));

                    RiskAlert alert = new()
                    {
                        TransactionRecordId = transaction.TransactionRecordId,
                        RiskScore = transaction.RiskScore,
                        RiskLevel = transaction.RiskLevel,
                        Reason = transaction.FlagReason,
                        Status = status,
                        AnalystComment = status == "Under Review"
                            ? "Initial review started based on risk score and transaction pattern."
                            : "Pending analyst review.",
                        CreatedAt = alertCreatedAt,
                        UpdatedAt = status == "Under Review"
                            ? alertCreatedAt.AddHours(random.Next(1, 48))
                            : alertCreatedAt
                    };

                    context.RiskAlerts.Add(alert);
                    await context.SaveChangesAsync();

                    context.AuditLogs.Add(new AuditLog
                    {
                        Username = "System",
                        Action = "Risk Alert Generated",
                        Details = $"Risk alert {alert.RiskAlertId} generated for transaction {transaction.TransactionRecordId}.",
                        Timestamp = alertCreatedAt
                    });

                    if (status == "Under Review")
                    {
                        context.AuditLogs.Add(new AuditLog
                        {
                            Username = "Risk Analyst",
                            Action = "Risk Alert Reviewed",
                            Details = $"Risk alert {alert.RiskAlertId} status changed to Under Review.",
                            Timestamp = alert.UpdatedAt ?? alertCreatedAt.AddHours(1)
                        });
                    }
                }
            }

            context.AuditLogs.Add(new AuditLog
            {
                Username = "System",
                Action = "Sample Data Seeded",
                Details = "Five-year mock dataset created for customers, transactions, risk alerts, and audit logs.",
                Timestamp = new DateTime(2026, 6, 29, 17, 0, 0)
            });

            await context.SaveChangesAsync();
        }
    }
}