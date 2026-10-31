using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.BLL.Models;

namespace WebApplication1.DAL.Data;

public sealed class MobileContext : IdentityDbContext<WalletUser, IdentityRole<int>, int>
{
    public DbSet<WalletAccount> WalletAccounts => Set<WalletAccount>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();
    public DbSet<ServiceAccount> ServiceAccounts => Set<ServiceAccount>();

    public MobileContext(DbContextOptions<MobileContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WalletAccount>(entity =>
        {
            entity.HasIndex(wallet => wallet.AccountNumber).IsUnique();
            entity.Property(wallet => wallet.AccountNumber).HasMaxLength(6).IsRequired();
            entity.Property(wallet => wallet.Balance).HasPrecision(18, 2);
            entity.HasOne(wallet => wallet.User).WithOne(user => user.Wallet)
                .HasForeignKey<WalletAccount>(wallet => wallet.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<WalletTransaction>(entity =>
        {
            entity.HasIndex(transaction => new { transaction.WalletAccountId, transaction.CreatedAtUtc });
            entity.Property(transaction => transaction.Amount).HasPrecision(18, 2);
            entity.Property(transaction => transaction.Counterparty).HasMaxLength(120).IsRequired();
            entity.HasOne(transaction => transaction.WalletAccount).WithMany(wallet => wallet.Transactions)
                .HasForeignKey(transaction => transaction.WalletAccountId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ServiceProvider>(entity =>
        {
            entity.Property(provider => provider.Name).HasMaxLength(80).IsRequired();
            entity.Property(provider => provider.Category).HasMaxLength(40).IsRequired();
        });

        builder.Entity<ServiceAccount>(entity =>
        {
            entity.HasIndex(account => new { account.ServiceProviderId, account.Identifier }).IsUnique();
            entity.Property(account => account.Identifier).HasMaxLength(32).IsRequired();
            entity.Property(account => account.CustomerName).HasMaxLength(100).IsRequired();
            entity.Property(account => account.Balance).HasPrecision(18, 2);
            entity.HasOne(account => account.Provider).WithMany(provider => provider.Accounts)
                .HasForeignKey(account => account.ServiceProviderId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<WalletUser>().Property(user => user.DisplayName).HasMaxLength(100).IsRequired();
    }
}
