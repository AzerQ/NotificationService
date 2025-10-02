using System.Net;
using System.Net.Mail;

namespace NotificationService.Infrastructure.Providers.Email;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient Create(EmailProviderOptions options)
    {
        var smtpClient = new SmtpClient(options.SmtpHost, options.SmtpPort)
        {
            EnableSsl = options.EnableSsl,
            Credentials = new NetworkCredential(options.UserName, options.Password)
        };

        return new SmtpClientWrapper(smtpClient);
    }
}
