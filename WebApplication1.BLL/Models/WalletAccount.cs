namespace WebApplication1.BLL.Models;

public sealed class WalletAccount
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public WalletUser User { get; set; } = null!;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public List<WalletTransaction> Transactions { get; set; } = [];
}
