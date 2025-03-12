using CraftingServiceApp.AdminAPI.Helpers;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

public class EmailService
{
    private readonly MailSettings _mailSettings;


    public EmailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using (var smtpClient = new SmtpClient(_mailSettings.SmtpServer))
            {
                smtpClient.Port = _mailSettings.Port;
                smtpClient.Credentials = new NetworkCredential(_mailSettings.Email, _mailSettings.Password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_mailSettings.Email, _mailSettings.DisplayedName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);
                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}