using Byohar.Application.Interfaces.Email;
using Byohar.Application.Models.Email;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Byohar.Infrastructure.Services.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public void SendEmail(EmailMessage email)
    {
        Task.Run(async () =>
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Connect to the SMTP server
                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                    // Authenticate with the server
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                    // Create the email message
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromAddress));
                    message.To.Add(MailboxAddress.Parse(email.To));
                    message.Subject = email.Subject;

                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = email.Body
                    };
                    message.Body = bodyBuilder.ToMessageBody();

                    // Send the email asynchronously
                    await client.SendAsync(message);
                }
                finally
                {
                    // Ensure the client is always disconnected
                    await client.DisconnectAsync(true);
                }
            }
        });
    }
}