using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebApplication1.BLL.Interfaces;
using WebApplication1.BLL.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public sealed class WalletController : Controller
{
    private readonly IWalletService _walletService;
    private readonly IStringLocalizer<WalletResources> _text;

    public WalletController(IWalletService walletService, IStringLocalizer<WalletResources> text)
    {
        _walletService = walletService;
        _text = text;
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopUp(TopUpViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Json(new WalletOperationResponse(false, _text["InvalidAmount"].Value, null));
        WalletOperationResult result = await _walletService.TopUpAsync(model.AccountNumber.Trim(), model.Amount, cancellationToken);
        return Json(new WalletOperationResponse(result == WalletOperationResult.Success,
            result == WalletOperationResult.Success ? _text["OperationSuccessTopUp"].Value : ResultMessage(result), null));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(TransferViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Json(new WalletOperationResponse(false, _text["InvalidAmount"].Value, null));
        WalletOperationResult result = await _walletService.TransferAsync(CurrentUserId(), model.RecipientNumber.Trim(), model.Amount, cancellationToken);
        decimal? balance = result == WalletOperationResult.Success
            ? (await _walletService.GetWalletForUserAsync(CurrentUserId(), cancellationToken))?.Balance
            : null;
        return Json(new WalletOperationResponse(result == WalletOperationResult.Success,
            result == WalletOperationResult.Success ? _text["OperationSuccessTransfer"].Value : ResultMessage(result), balance));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayService(ServicePaymentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Json(new WalletOperationResponse(false, _text["InvalidAmount"].Value, null));
        WalletOperationResult result = await _walletService.PayServiceAsync(CurrentUserId(), model.ProviderId, model.ServiceIdentifier, model.Amount, cancellationToken);
        decimal? balance = result == WalletOperationResult.Success
            ? (await _walletService.GetWalletForUserAsync(CurrentUserId(), cancellationToken))?.Balance
            : null;
        return Json(new WalletOperationResponse(result == WalletOperationResult.Success,
            result == WalletOperationResult.Success ? _text["OperationSuccessPayment"].Value : ResultMessage(result), balance));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> History(DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date)
        {
            TempData["Toast"] = _text["DateRangeInvalid"].Value;
            return RedirectToAction(nameof(History));
        }

        WalletAccount? wallet = await _walletService.GetWalletForUserAsync(CurrentUserId(), cancellationToken);
        if (wallet is null) return RedirectToAction("Index", "Home");
        var transactions = await _walletService.GetHistoryAsync(CurrentUserId(), from, to, cancellationToken);
        return View(new WalletHistoryViewModel { Wallet = wallet, Transactions = transactions, From = from, To = to });
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string ResultMessage(WalletOperationResult result) => result switch
    {
        WalletOperationResult.WalletNotFound => _text["WalletNotFound"].Value,
        WalletOperationResult.ServiceAccountNotFound => _text["ServiceAccountNotFound"].Value,
        WalletOperationResult.InsufficientFunds => _text["InsufficientFunds"].Value,
        WalletOperationResult.SameWallet => _text["SameWallet"].Value,
        _ => _text["InvalidAmount"].Value
    };
}
