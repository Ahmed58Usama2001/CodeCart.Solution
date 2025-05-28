using CodeCart.Core.Services.Contracts;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace CodeCart.Service;

public class MailingService:IMailingService
{
    private readonly IOptions<MailSettings> _mailSetting;
    private readonly ILogger<MailingService> _logger;

    public MailingService(IOptions<MailSettings> mailSetting, ILogger<MailingService> logger)
    {
        _mailSetting = mailSetting;
        _logger = logger;
    }
    public async Task<bool> SendEmailAsync(string mailTo, string subject, string body)
    {
        try
        {
            using var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_mailSetting.Value.Email),
                Subject = subject
            };

            email.From.Add(new MailboxAddress(_mailSetting.Value.DisplayName, _mailSetting.Value.Email));

            email.To.Add(MailboxAddress.Parse(mailTo));

            var builder = new BodyBuilder();
            builder.HtmlBody = body;

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSetting.Value.Host, _mailSetting.Value.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSetting.Value.Email, _mailSetting.Value.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}


public class MailSettings
{
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Password { get; set; }
}