namespace CodeCart.Core.Services.Contracts;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshTokenAsync(string userId);
    Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task RevokeAllUserRefreshTokensAsync(string userId);
}

public class RefreshTokenValidationResult
{
    public bool IsValid { get; set; }
    public string UserId { get; set; }
}