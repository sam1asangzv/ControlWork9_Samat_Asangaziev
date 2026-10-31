using WebApplication1.BLL.Models;

namespace WebApplication1.BLL.Interfaces;

public interface IWalletService
{
    Task<WalletAccount> CreateWalletAsync(int userId, CancellationToken cancellationToken = default);
    Task<WalletAccount?> GetWalletForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<WalletTransaction>> GetHistoryAsync(int userId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<List<ServiceProvider>> GetServiceProvidersAsync(CancellationToken cancellationToken = default);
    Task<WalletOperationResult> TopUpAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default);
    Task<WalletOperationResult> TransferAsync(int senderUserId, string recipientNumber, decimal amount, CancellationToken cancellationToken = default);
    Task<WalletOperationResult> PayServiceAsync(int userId, int providerId, string serviceIdentifier, decimal amount, CancellationToken cancellationToken = default);
}
