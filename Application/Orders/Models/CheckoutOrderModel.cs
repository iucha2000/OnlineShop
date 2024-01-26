using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Order.Models
{
    public class CheckoutOrderModel
    {
        public bool Delivery {  get; set; }
        public string? Address { get; set; }
    }
}
