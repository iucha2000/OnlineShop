using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Order.Models
{
    public class AddProductToOrderModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
