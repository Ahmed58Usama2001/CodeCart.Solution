using System.ComponentModel.DataAnnotations;

namespace CodeCart.Core.Entities.Identity.Gmail;

public class FacebookSignInVM
{
    [Required]
    public string AccessToken { get; set; }
}
