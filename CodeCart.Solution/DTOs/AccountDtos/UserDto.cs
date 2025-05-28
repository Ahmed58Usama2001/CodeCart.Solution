namespace CodeCart.API.DTOs.AccountDtos;

public class UserDto
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public AddressDto Address { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}