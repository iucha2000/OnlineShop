using Domain;
using Domain.Entities;
using Domain.Enums;
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

        public Dictionary<Guid, Order> Orders;

        public MockDb() 
        {
            Products = new Dictionary<Guid, Product>();
            Users = new Dictionary<Guid, User>();
            Orders = new Dictionary<Guid, Order>();
            SeedProducts();
        }

        void SeedProducts()
        {
            for(int i=0; i<10; i++)
            {
                var prod1 = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Tablet",
                    Price = 99.99m,
                    Category = ProductCategory.Electronics,
                    ProductId = ProductIdentifier.Tablet
                };
                Products.Add(prod1.Id, prod1);
            }

            for (int i = 0; i < 20; i++)
            {
                var prod2 = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Sweater",
                    Price = 49.99m,
                    Category = ProductCategory.Fashion,
                    ProductId = ProductIdentifier.Sweater
                };
                Products.Add(prod2.Id, prod2);
            }

            for (int i = 0; i < 15; i++)
            {
                var prod3 = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Lamp",
                    Price = 19.99m,
                    Category = ProductCategory.Home,
                    ProductId = ProductIdentifier.Lamp
                };
                Products.Add(prod3.Id, prod3);
            }
        }
    }
}
