﻿namespace CodeCart.Core.Entities.Identity.Facebook;

public class FacebookAuthConfig
{
    public string TokenValidationUrl { get; set; }
    public string UserInfoUrl { get; set; }
    public string AppId { get; set; }
    public string AppSecret { get; set; }
}
