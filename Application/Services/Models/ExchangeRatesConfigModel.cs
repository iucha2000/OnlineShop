using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Models
{
    public class ExchangeRatesConfigModel
    {
        public string BaseUrl { get; set; }
        public string Param { get; set; }
        public string ApiKey { get; set; }
    }
}
