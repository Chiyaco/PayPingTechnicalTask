using PayPingTechnicalTask.Entity;

namespace PayPingTechnicalTask.Data;

public static class SeedData
{
    public static async Task Seed(this TransactionDbContext context, IApplicationBuilder app)
    {
        await SaveWallet(context, app);
        await SaveTransactions(context, app);
    }

    static async Task SaveWallet(TransactionDbContext context, IApplicationBuilder app)
    {
        var wallets = new List<Wallet>
        {
            new Wallet { Id = Guid.NewGuid(), Balance = 10, UserId = 1, CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now },
            new Wallet { Id = Guid.NewGuid(), Balance = 0,  UserId = 2, CreatedAt = DateTime.Now ,ModifiedAt = DateTime.Now },
            new Wallet { Id = Guid.NewGuid(), Balance = 50, UserId= 3 , CreatedAt = DateTime.Now ,ModifiedAt = DateTime.Now }
        };

        await context.Wallets.AddRangeAsync(wallets);

        await context.SaveChangesAsync();
    }

    static async Task SaveTransactions(TransactionDbContext context, IApplicationBuilder app)
    {
        var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = 1, TransactionDate = new DateTime(2023, 7, 1), Amount = 50 , CreatedAt=DateTime.Now, ModifiedAt = DateTime.Now},
                new Transaction { Id = Guid.NewGuid(), UserId = 2, TransactionDate = new DateTime(2023, 7, 2), Amount = 100, CreatedAt=DateTime.Now, ModifiedAt = DateTime.Now},
                new Transaction { Id = Guid.NewGuid(), UserId = 1, TransactionDate = new DateTime(2023, 7, 3), Amount = 25 , CreatedAt=DateTime.Now, ModifiedAt = DateTime.Now},
                new Transaction { Id = Guid.NewGuid(), UserId = 3, TransactionDate = new DateTime(2023, 7, 4), Amount = 75 , CreatedAt=DateTime.Now, ModifiedAt = DateTime.Now},
            };

        await context.Transactions.AddRangeAsync(transactions);

        await context.SaveChangesAsync();
    }
}
