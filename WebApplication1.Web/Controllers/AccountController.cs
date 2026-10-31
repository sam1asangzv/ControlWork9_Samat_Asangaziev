using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebApplication1.BLL.Interfaces;
using WebApplication1.BLL.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public sealed class AccountController : Controller
{
    private readonly UserManager<WalletUser> _userManager;
    private readonly SignInManager<WalletUser> _signInManager;
    private readonly IWalletService _walletService;
    private readonly IStringLocalizer<WalletResources> _text;

    public AccountController(UserManager<WalletUser> userManager, SignInManager<WalletUser> signInManager,
        IWalletService walletService, IStringLocalizer<WalletResources> text)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _walletService = walletService;
        _text = text;
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new WalletUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            DisplayName = model.DisplayName.Trim()
        };
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, _text["RegisterFailed"]);
            return View(model);
        }

        try
        {
            // #1: Create a unique six-digit wallet alongside each Identity account.
            await _walletService.CreateWalletAsync(user.Id, cancellationToken);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            ModelState.AddModelError(string.Empty, _text["RegisterFailed"]);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _signInManager.PasswordSignInAsync(model.Email.Trim(), model.Password, model.RememberMe, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, _text["LoginFailed"]);
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return LocalRedirect(model.ReturnUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Toast"] = _text["SignOut"].Value;
        return RedirectToAction("Index", "Home");
    }
}
