namespace CodeCart.Core.Services.Contracts;

public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string token);
    Task<bool> IsTokenBlacklistedAsync(string token);
}