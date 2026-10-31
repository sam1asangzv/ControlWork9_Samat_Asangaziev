using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "FieldRequired"), EmailAddress(ErrorMessage = "InvalidEmail"), Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "FieldRequired"), DataType(DataType.Password), Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "RememberMe")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
