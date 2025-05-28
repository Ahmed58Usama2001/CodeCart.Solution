namespace CodeCart.Core.Entities.Identity.Enums;

public enum LoginProvider
{
    Google = 1,
    Facebook
}

public static class LoginProviderExtensions
{
    public static string GetDisplayName(this LoginProvider provider)
    {
        switch (provider)
        {
            case LoginProvider.Google:
                return "Google";
            case LoginProvider.Facebook:
                return "Facebook";
            default:
                throw new ArgumentException("Invalid LoginProvider");
        }
    }
}

