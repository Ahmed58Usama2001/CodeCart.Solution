namespace CodeCart.Core.Services.Contracts;

public interface IMailingService
{
    Task<bool> SendEmailAsync(string mailTo, string subject, string body);

}
