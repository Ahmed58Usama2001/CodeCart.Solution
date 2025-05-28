using System.ComponentModel.DataAnnotations;

namespace CodeCart.API.DTOs.AccountDtos;

public class ForgetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
