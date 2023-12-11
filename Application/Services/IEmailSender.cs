using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body, bool isHtml = false);

        Task SendVerificationEmail(string email, Guid verificationCode);

        Task SendPasswordResetEmail(string email, Guid verificationCode);
    }
}
