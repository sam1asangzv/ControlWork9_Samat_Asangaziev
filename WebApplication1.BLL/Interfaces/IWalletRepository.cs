using WebApplication1.BLL.Models;

namespace WebApplication1.BLL.Interfaces;

public interface IWalletRepository
{
    Task<WalletAccount?> GetWalletByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<WalletAccount?> GetWalletByNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<bool> WalletNumberExistsAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task AddWalletAsync(WalletAccount account, CancellationToken cancellationToken = default);
    Task<List<WalletTransaction>> GetTransactionsAsync(int walletId, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default);
    Task<List<ServiceProvider>> GetServiceProvidersAsync(CancellationToken cancellationToken = default);
    Task<WalletOperationResult> TopUpAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default);
    Task<WalletOperationResult> TransferAsync(string senderNumber, string recipientNumber, decimal amount, CancellationToken cancellationToken = default);
    Task<WalletOperationResult> PayServiceAsync(int walletId, int providerId, string serviceIdentifier, decimal amount, CancellationToken cancellationToken = default);
}
