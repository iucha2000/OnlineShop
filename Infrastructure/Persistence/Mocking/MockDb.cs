using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Mocking
{
    public class MockDb
    {
        public Dictionary<Guid, Product> Products;

        public Dictionary<Guid, User> Users;

        public MockDb() 
        {
            Products = new Dictionary<Guid, Product>();
            Users = new Dictionary<Guid, User>();
        }

    }
}
