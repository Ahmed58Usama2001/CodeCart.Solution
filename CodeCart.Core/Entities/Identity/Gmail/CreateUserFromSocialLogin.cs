namespace CodeCart.Core.Entities.Identity.Gmail;

public class CreateUserFromSocialLogin
{
    public string UserName { get; set; }

    public string? ProfilePicture { get; set; }

    public string Email { get; set; }

    public string LoginProviderSubject { get; set; }
}
