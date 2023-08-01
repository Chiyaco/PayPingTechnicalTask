using PayPingTechnicalTask.Entity;
using PayPingTechnicalTask.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace PayPingTechnicalTask.Data;

public class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        //modelBuilder.ApplyConfiguration(new WalletConfiguration());

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        // Get all the entities that inherit from AuditableEntity
        // and have a state of Added or Modified
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditEntity && (
                e.State is EntityState.Added or EntityState.Modified));

        // For each entity we will set the Audit properties
        foreach (var entityEntry in entries)
        {
            // If the entity state is Added let's set
            // the CreatedAt and CreatedBy properties
            if (entityEntry.State == EntityState.Added)
            {
                ((IAuditEntity)entityEntry.Entity).CreatedAt = DateTime.Now;
            }
            else
            {
                // If the state is Modified then we don't want
                // to modify the CreatedAt and CreatedBy properties
                // so we set their state as IsModified to false
                Entry((IAuditEntity)entityEntry.Entity).Property(p => p.CreatedAt).IsModified = false;
            }

            // In any case we always want to set the properties
            // ModifiedAt and ModifiedBy
            ((IAuditEntity)entityEntry.Entity).ModifiedAt = DateTime.Now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Wallet> Wallets { get; set; }

}
