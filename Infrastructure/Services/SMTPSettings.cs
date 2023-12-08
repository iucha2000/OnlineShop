using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SMTPSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationAddress { get; set; }
    }
}
