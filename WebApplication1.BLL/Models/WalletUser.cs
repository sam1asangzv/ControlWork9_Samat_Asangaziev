using Microsoft.AspNetCore.Identity;

namespace WebApplication1.BLL.Models;

public sealed class WalletUser : IdentityUser<int>
{
    public string DisplayName { get; set; } = string.Empty;
    public WalletAccount? Wallet { get; set; }
}
