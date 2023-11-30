using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Models
{
    public class CachingOptions
    {
        public int AbsoluteExpiration { get; set; }
        public int SlidingExpiration { get; set; }
        public int Size { get; set; }
    }
}
