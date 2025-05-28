using System.ComponentModel.DataAnnotations;

namespace CodeCart.API.DTOs.AccountDtos;

public class ResetPasswordDto
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password Not Match")]
    public string RetryPassword { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}
