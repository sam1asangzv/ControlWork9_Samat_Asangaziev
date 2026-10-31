using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels;

public sealed class RegisterViewModel
{
    [Required(ErrorMessage = "FieldRequired"), EmailAddress(ErrorMessage = "InvalidEmail"), Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "FieldRequired"), StringLength(100, MinimumLength = 2, ErrorMessage = "FieldRequired"), Display(Name = "DisplayName")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "FieldRequired"), MinLength(8, ErrorMessage = "PasswordLength"), DataType(DataType.Password), Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "FieldRequired"), Compare(nameof(Password), ErrorMessage = "PasswordMismatch"), DataType(DataType.Password), Display(Name = "ConfirmPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
