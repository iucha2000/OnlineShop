using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class EmailNotVerifiedException : Exception
    {
        public string ExceptionMessage { get; set; }
        public int StatusCode { get; set; }

        public EmailNotVerifiedException(string message, int statusCode = 400)
        {
            ExceptionMessage = message;
            StatusCode = statusCode;
        }
    }
}
