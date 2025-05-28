namespace CodeCart.Core.Services.Contracts.SecurityModule;

public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string token);
    Task<bool> IsTokenBlacklistedAsync(string token);
}