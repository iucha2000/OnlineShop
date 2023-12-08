using Application.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class GmailSender : IEmailSender
    {
        private readonly SMTPSettings _smtpSettings;

        public GmailSender(IOptions<SMTPSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string body, bool isHtml = false)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(_smtpSettings.Email),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendVerificationEmail(string email, Guid verificationCode)
        {
            string linkLocation = $"{_smtpSettings.VerificationAddress}?email={email}&verificationCode={verificationCode}";
            var messageBody = $"Please follow this link to verify your email <a href=\"{linkLocation}\">Verify Email</a>";

            await SendEmailAsync(email, "Please confirm your email", messageBody, true);
        }

        public async Task SendPasswordChangeEmail(string email, Guid verificationCode)
        {
            throw new NotImplementedException();
        }
    }
}
