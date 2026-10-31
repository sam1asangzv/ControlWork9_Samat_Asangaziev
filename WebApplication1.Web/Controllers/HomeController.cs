using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.BLL.Interfaces;
using WebApplication1.BLL.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public sealed class HomeController : Controller
{
    private readonly IWalletService _walletService;

    public HomeController(IWalletService walletService) => _walletService = walletService;

    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        WalletAccount? wallet = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            wallet = await _walletService.GetWalletForUserAsync(userId, cancellationToken);
        }

        return View(new WalletHomeViewModel
        {
            Wallet = wallet,
            Providers = await _walletService.GetServiceProvidersAsync(cancellationToken)
        });
    }

    [AllowAnonymous]
    public IActionResult Error() => View(new ErrorPageViewModel { RequestId = HttpContext.TraceIdentifier });
}
