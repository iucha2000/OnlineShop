using Application.Common.Persistence;
using Application.Product.Queries;
using Application.Services;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopTests
{
    public class ProductTests
    {
        public Mock<IGenericRepository<Product>> mockProductRepo = new Mock<IGenericRepository<Product>>();
        public Mock<IGenericRepository<Image>> mockImageRepo = new Mock<IGenericRepository<Image>>();
        public Mock<IGenericRepository<User>> mockUserRepo = new Mock<IGenericRepository<User>>();
        public Mock<IExchangeRate> mockExchangeRate = new Mock<IExchangeRate>();

        [Fact]
        public void GetAllProductsQuery_Returns_Products()
        {
            //TODO
            var handler = new GetAllProductsQueryHandler(mockProductRepo.Object, mockImageRepo.Object, mockUserRepo.Object, mockExchangeRate.Object);

            var query = new GetAllProductsQuery(Guid.Empty);

            var result = handler.Handle(query, CancellationToken.None);
        }
    }
}
