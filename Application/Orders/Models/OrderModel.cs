using Application.Product.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Order.Models
{
    public class OrderModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public List<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}
