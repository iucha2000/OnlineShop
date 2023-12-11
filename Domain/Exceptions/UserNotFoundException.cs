using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserNotFoundException :Exception
    {
        public string ExceptionMessage { get; set; }
        public int StatusCode { get; set; }
        public UserNotFoundException(string message, int statuscode = 404)
        {
            ExceptionMessage = message;
            StatusCode = statuscode;
        }
    }
}
