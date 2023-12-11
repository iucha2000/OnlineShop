using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public string ExceptionMessage { get; set; }
        public int StatusCode { get; set; }
        public Guid UserId { get; set; }

        public OrderNotFoundException(string message, Guid userId, int statusCode = 404)
        {
            ExceptionMessage = message;
            UserId = userId;
            StatusCode = statusCode;
        }
    }
}
