﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public ICollection<Product> Products { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Order() 
        {
            Products = new List<Product>();
            StartDate = DateTime.Now;
        }
    }
}
