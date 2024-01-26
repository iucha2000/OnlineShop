using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User.Models
{
    public class EditUserInfoModel
    {
        public string? Email { get; set; }
        public string? Currency { get; set; }
        public Role? Role { get; set; }
        public string? Address { get; set; }
    }
}
