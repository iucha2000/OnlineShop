using Application.Order.Models;
using Application.Services.Models;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User.Models
{
    public class UserProfileModel
    {
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public bool EmailVerified { get; set; }
        public List<OrderModel> Orders { get; set; }
        public Conversion_Rates ExchangeRates { get; set; }
    }
}
