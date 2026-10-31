namespace WebApplication1.BLL.Models;

public enum WalletOperationResult
{
    Success,
    WalletNotFound,
    ServiceAccountNotFound,
    InsufficientFunds,
    SameWallet
}
