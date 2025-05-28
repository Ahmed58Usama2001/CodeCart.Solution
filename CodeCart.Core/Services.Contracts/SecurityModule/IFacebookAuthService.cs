using CodeCart.Core.Entities.Identity.Facebook;

namespace CodeCart.Core.Services.Contract.AccountModuleContracts;

public interface IFacebookAuthService
{
    Task<FacebookTokenValidationResponse> ValidateFacebookToken(string accessToken);
    Task<FacebookUserInfoResponse> GetFacebookUserInformation(string accessToken);
}
