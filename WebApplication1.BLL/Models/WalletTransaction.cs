namespace WebApplication1.BLL.Models;

public enum WalletTransactionType
{
    TopUp = 1,
    TransferSent = 2,
    TransferReceived = 3,
    ServicePayment = 4
}

public sealed class WalletTransaction
{
    public long Id { get; set; }
    public Guid OperationId { get; set; }
    public int WalletAccountId { get; set; }
    public WalletAccount WalletAccount { get; set; } = null!;
    public WalletTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Counterparty { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
