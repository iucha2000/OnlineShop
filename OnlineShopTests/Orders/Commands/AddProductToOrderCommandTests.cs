using Application.Comments.Commands;
using Application.Common.Persistence;
using Application.Order.Commands;
using Application.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopTests.Orders.Commands
{
    public class AddProductToOrderCommandTests
    {
        private readonly Mock<IGenericRepository<Domain.Entities.Product>> _productMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.Order>> _orderMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.User>> _userMockRepository;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddProductToOrderCommandTests()
        {
            _productMockRepository = new();
            _orderMockRepository = new();
            _userMockRepository = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task AddProductToOrder_Should_Return_Order()
        {
            //Arrange
            var count = 2;
            var userId = Guid.NewGuid();
            var productId = 1;

            var products = new List<Domain.Entities.Product>()
            {
                new Domain.Entities.Product {Name = "Product 1", ProductId = productId, IsSold = false, OrderId = null},
                new Domain.Entities.Product {Name = "Product 1", ProductId = productId, IsSold = false, OrderId = null},
                new Domain.Entities.Product {Name = "Product 1", ProductId = productId, IsSold = false, OrderId = null}
            };

            _productMockRepository.Setup(x => x.ListAsync(p => p.ProductId == productId && p.IsSold == false
            && p.OrderId == null, null, true, null, count)).ReturnsAsync(products);

            var command = new AddProductToOrderCommand(userId, productId, count);
            var handler = new AddProductToOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object, 
                _userMockRepository.Object, _unitOfWorkMock.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.Success);
        }
    }
}
