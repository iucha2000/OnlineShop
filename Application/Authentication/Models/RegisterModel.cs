using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public string AdminSecret { get; set; }
    }
}
