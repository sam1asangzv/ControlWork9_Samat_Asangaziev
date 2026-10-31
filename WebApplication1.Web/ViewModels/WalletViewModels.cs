using System.ComponentModel.DataAnnotations;
using WebApplication1.BLL.Models;

namespace WebApplication1.ViewModels;

public sealed class WalletHomeViewModel
{
    public WalletAccount? Wallet { get; init; }
    public IReadOnlyList<WebApplication1.BLL.Models.ServiceProvider> Providers { get; init; } = [];
}

public sealed class WalletHistoryViewModel
{
    public WalletAccount Wallet { get; init; } = null!;
    public IReadOnlyList<WalletTransaction> Transactions { get; init; } = [];
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}

public sealed class TopUpViewModel
{
    [Required(ErrorMessage = "FieldRequired"), StringLength(6, MinimumLength = 6, ErrorMessage = "WalletNotFound"), Display(Name = "WalletNumber")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "FieldRequired"), Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "InvalidAmount"), Display(Name = "Amount")]
    public decimal Amount { get; set; }
}

public sealed class TransferViewModel
{
    [Required(ErrorMessage = "FieldRequired"), StringLength(6, MinimumLength = 6, ErrorMessage = "WalletNotFound"), Display(Name = "RecipientNumber")]
    public string RecipientNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "FieldRequired"), Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "InvalidAmount"), Display(Name = "Amount")]
    public decimal Amount { get; set; }
}

public sealed class ServicePaymentViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "ServiceAccountNotFound"), Display(Name = "ServiceProvider")]
    public int ProviderId { get; set; }

    [Required(ErrorMessage = "FieldRequired"), StringLength(32, ErrorMessage = "ServiceAccountNotFound"), Display(Name = "ServiceIdentifier")]
    public string ServiceIdentifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "FieldRequired"), Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "InvalidAmount"), Display(Name = "Amount")]
    public decimal Amount { get; set; }
}

public sealed record WalletOperationResponse(bool Success, string Message, decimal? Balance);
