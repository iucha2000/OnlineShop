using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Models
{
    public record ForgetPasswordModel(string Email);

    public record ResetPasswordModel(string Email, string Password, Guid Token);

    public record ChangePasswordModel(string oldPassword, string newPassword);
}
