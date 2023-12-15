using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public int ProductId { get; set; }
        public string ExceptionMessage { get; set; }
        public int StatusCode { get; set; }

        public ProductNotFoundException(string message, int productId, int statusCode = 400)
        {
            ExceptionMessage = message;
            ProductId = productId;
            StatusCode = statusCode;
        }
    }
}
