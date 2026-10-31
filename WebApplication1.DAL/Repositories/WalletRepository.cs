using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication1.BLL.Interfaces;
using WebApplication1.BLL.Models;
using WebApplication1.DAL.Data;

namespace WebApplication1.DAL.Repositories;

public sealed class WalletRepository : IWalletRepository
{
    private readonly MobileContext _context;

    public WalletRepository(MobileContext context) => _context = context;

    public Task<WalletAccount?> GetWalletByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        _context.WalletAccounts.AsNoTracking().SingleOrDefaultAsync(wallet => wallet.UserId == userId, cancellationToken);

    public Task<WalletAccount?> GetWalletByNumberAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        _context.WalletAccounts.AsNoTracking().SingleOrDefaultAsync(wallet => wallet.AccountNumber == accountNumber, cancellationToken);

    public Task<bool> WalletNumberExistsAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        _context.WalletAccounts.AnyAsync(wallet => wallet.AccountNumber == accountNumber, cancellationToken);

    public async Task AddWalletAsync(WalletAccount account, CancellationToken cancellationToken = default)
    {
        await _context.WalletAccounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<WalletTransaction>> GetTransactionsAsync(int walletId, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default)
    {
        IQueryable<WalletTransaction> query = _context.WalletTransactions.AsNoTracking()
            .Where(transaction => transaction.WalletAccountId == walletId);
        if (fromUtc.HasValue) query = query.Where(transaction => transaction.CreatedAtUtc >= fromUtc.Value);
        if (toUtc.HasValue) query = query.Where(transaction => transaction.CreatedAtUtc < toUtc.Value);
        return query.OrderByDescending(transaction => transaction.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public Task<List<ServiceProvider>> GetServiceProvidersAsync(CancellationToken cancellationToken = default) =>
        _context.ServiceProviders.AsNoTracking().OrderBy(provider => provider.Name).ToListAsync(cancellationToken);

    // #2: Anonymous account top-up commits the balance change and ledger entry atomically.
    public async Task<WalletOperationResult> TopUpAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var wallet = await _context.WalletAccounts.SingleOrDefaultAsync(item => item.AccountNumber == accountNumber, cancellationToken);
        if (wallet is null) return WalletOperationResult.WalletNotFound;

        wallet.Balance += amount;
        _context.WalletTransactions.Add(new WalletTransaction
        {
            WalletAccountId = wallet.Id,
            OperationId = Guid.NewGuid(),
            Type = WalletTransactionType.TopUp,
            Amount = amount,
            Counterparty = string.Empty,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return WalletOperationResult.Success;
    }

    // #3: Both wallets and both journal rows commit or roll back together.
    public async Task<WalletOperationResult> TransferAsync(string senderNumber, string recipientNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        if (senderNumber == recipientNumber) return WalletOperationResult.SameWallet;

        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var sender = await _context.WalletAccounts.SingleOrDefaultAsync(item => item.AccountNumber == senderNumber, cancellationToken);
        var recipient = await _context.WalletAccounts.SingleOrDefaultAsync(item => item.AccountNumber == recipientNumber, cancellationToken);
        if (sender is null || recipient is null) return WalletOperationResult.WalletNotFound;
        if (sender.Balance < amount) return WalletOperationResult.InsufficientFunds;

        sender.Balance -= amount;
        recipient.Balance += amount;
        Guid operationId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        _context.WalletTransactions.AddRange(
            new WalletTransaction { WalletAccountId = sender.Id, OperationId = operationId, Type = WalletTransactionType.TransferSent, Amount = -amount, Counterparty = recipientNumber, CreatedAtUtc = now },
            new WalletTransaction { WalletAccountId = recipient.Id, OperationId = operationId, Type = WalletTransactionType.TransferReceived, Amount = amount, Counterparty = senderNumber, CreatedAtUtc = now });
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return WalletOperationResult.Success;
    }

    // #4: Service lookup, wallet debit, provider credit and journal entry are atomic.
    public async Task<WalletOperationResult> PayServiceAsync(int walletId, int providerId, string serviceIdentifier, decimal amount, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var wallet = await _context.WalletAccounts.SingleOrDefaultAsync(item => item.Id == walletId, cancellationToken);
        if (wallet is null) return WalletOperationResult.WalletNotFound;
        var serviceAccount = await _context.ServiceAccounts
            .Include(account => account.Provider)
            .SingleOrDefaultAsync(account => account.ServiceProviderId == providerId && account.Identifier == serviceIdentifier, cancellationToken);
        if (serviceAccount is null) return WalletOperationResult.ServiceAccountNotFound;
        if (wallet.Balance < amount) return WalletOperationResult.InsufficientFunds;

        wallet.Balance -= amount;
        serviceAccount.Balance += amount;
        _context.WalletTransactions.Add(new WalletTransaction
        {
            WalletAccountId = wallet.Id,
            OperationId = Guid.NewGuid(),
            Type = WalletTransactionType.ServicePayment,
            Amount = -amount,
            Counterparty = $"{serviceAccount.Provider.Name}: {serviceIdentifier}",
            CreatedAtUtc = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return WalletOperationResult.Success;
    }
}
