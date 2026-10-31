using System.Security.Cryptography;
using WebApplication1.BLL.Interfaces;
using WebApplication1.BLL.Models;

namespace WebApplication1.BLL.Services;

public sealed class WalletService : IWalletService
{
    private const decimal InitialBalance = 100000m;
    private const decimal MaxOperationAmount = 100000000m;
    private readonly IWalletRepository _repository;

    public WalletService(IWalletRepository repository) => _repository = repository;

    public async Task<WalletAccount> CreateWalletAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (await _repository.GetWalletByUserIdAsync(userId, cancellationToken) is { } existing) return existing;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            string number = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            if (await _repository.WalletNumberExistsAsync(number, cancellationToken)) continue;

            var account = new WalletAccount { UserId = userId, AccountNumber = number, Balance = InitialBalance };
            await _repository.AddWalletAsync(account, cancellationToken);
            return account;
        }

        throw new InvalidOperationException();
    }

    public Task<WalletAccount?> GetWalletForUserAsync(int userId, CancellationToken cancellationToken = default) =>
        _repository.GetWalletByUserIdAsync(userId, cancellationToken);

    public async Task<List<WalletTransaction>> GetHistoryAsync(int userId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        WalletAccount? wallet = await _repository.GetWalletByUserIdAsync(userId, cancellationToken);
        if (wallet is null) return [];
        DateTime? fromUtc = from.HasValue ? DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc) : null;
        DateTime? toUtcExclusive = to.HasValue ? DateTime.SpecifyKind(to.Value.Date.AddDays(1), DateTimeKind.Utc) : null;
        return await _repository.GetTransactionsAsync(wallet.Id, fromUtc, toUtcExclusive, cancellationToken);
    }

    public Task<List<ServiceProvider>> GetServiceProvidersAsync(CancellationToken cancellationToken = default) =>
        _repository.GetServiceProvidersAsync(cancellationToken);

    public Task<WalletOperationResult> TopUpAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        if (!IsValidAmount(amount) || string.IsNullOrEmpty(accountNumber) || accountNumber.Length != 6 || !accountNumber.All(IsAsciiDigit))
            return Task.FromResult(WalletOperationResult.WalletNotFound);
        return _repository.TopUpAsync(accountNumber, amount, cancellationToken);
    }

    public async Task<WalletOperationResult> TransferAsync(int senderUserId, string recipientNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        if (!IsValidAmount(amount) || string.IsNullOrEmpty(recipientNumber) || recipientNumber.Length != 6 || !recipientNumber.All(IsAsciiDigit))
            return WalletOperationResult.WalletNotFound;
        WalletAccount? sender = await _repository.GetWalletByUserIdAsync(senderUserId, cancellationToken);
        return sender is null
            ? WalletOperationResult.WalletNotFound
            : await _repository.TransferAsync(sender.AccountNumber, recipientNumber, amount, cancellationToken);
    }

    public async Task<WalletOperationResult> PayServiceAsync(int userId, int providerId, string serviceIdentifier, decimal amount, CancellationToken cancellationToken = default)
    {
        if (!IsValidAmount(amount) || string.IsNullOrWhiteSpace(serviceIdentifier) || serviceIdentifier.Length > 32)
            return WalletOperationResult.ServiceAccountNotFound;
        WalletAccount? wallet = await _repository.GetWalletByUserIdAsync(userId, cancellationToken);
        return wallet is null
            ? WalletOperationResult.WalletNotFound
            : await _repository.PayServiceAsync(wallet.Id, providerId, serviceIdentifier.Trim(), amount, cancellationToken);
    }

    private static bool IsValidAmount(decimal amount) => amount > 0m && amount <= MaxOperationAmount && decimal.Round(amount, 2) == amount;
    private static bool IsAsciiDigit(char value) => value is >= '0' and <= '9';
}
