using System.ComponentModel.DataAnnotations;

namespace CodeCart.API.DTOs.AccountDtos;

public class AddressDto
{
    [Required(ErrorMessage = "Address Line 1 is required")]
    public string Line1 { get; set; } = string.Empty;

    public string? Line2 { get; set; }

    [Required(ErrorMessage = "City is required")]
    public  string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal Code is required")]
    public  string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required")]
    public  string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    public required string Country { get; set; } = string.Empty;
}
