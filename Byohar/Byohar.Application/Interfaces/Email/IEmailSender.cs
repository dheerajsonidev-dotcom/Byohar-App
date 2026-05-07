using Byohar.Application.Models.Email;

namespace Byohar.Application.Interfaces.Email;

public interface IEmailSender
{
    void SendEmail(EmailMessage email);
}
