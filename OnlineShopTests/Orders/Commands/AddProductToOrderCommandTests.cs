using Application.Comments.Commands;
using Application.Common.Persistence;
using Application.Order.Commands;
using Application.Services;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public async Task AddProductToOrder_Should_Return_New_Order()
        {
            //Arrange
            var count = 2;
            var userId = Guid.NewGuid();
            var productId = 1;

            var products = new List<Domain.Entities.Product>()
            {
                new Domain.Entities.Product {Name = "Product 1", ProductId = productId, IsSold = false, OrderId = null},
                new Domain.Entities.Product {Name = "Product 1", ProductId = productId, IsSold = false, OrderId = null},
            };

            _productMockRepository.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Domain.Entities.Product,bool>>>(), It.IsAny<string>(), It.IsAny<bool>(), null, count))
                .ReturnsAsync(products);

            var command = new AddProductToOrderCommand(userId, productId, count);
            var handler = new AddProductToOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object, 
                _userMockRepository.Object, _unitOfWorkMock.Object);

            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.True(actual.Success);
            Assert.Equal(actual.Value.Products.Count, count);
            Assert.False(actual.Value.IsCompleted);
        }

        [Fact]
        public async Task AddProductToOrder_Should_Update_Existing_Order()
        {
            //Arrange
            var count = 2;
            var userId = Guid.NewGuid();
            var productId = 1;

            var products = new List<Domain.Entities.Product>()
            {
                new Domain.Entities.Product {Name = "Product 1", ProductId = productId, IsSold = false, OrderId = null},
                new Domain.Entities.Product {Name = "Product 2", ProductId = productId, IsSold = false, OrderId = null},
            };

            _productMockRepository.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(), It.IsAny<string>(), It.IsAny<bool>(), null, count))
                .ReturnsAsync(products);

            _orderMockRepository.Setup(x=> x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Order
                {
                    IsCompleted = false,
                    Products = new List<Domain.Entities.Product>()
                    {
                        new Domain.Entities.Product
                        {
                            Name = "Product 3"
                        }
                    }
                });

            var command = new AddProductToOrderCommand(userId, productId, count);
            var handler = new AddProductToOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object,
                _userMockRepository.Object, _unitOfWorkMock.Object);

            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.True(actual.Success);
            Assert.Equal(actual.Value.Products.Count, count + 1);
            Assert.False(actual.Value.IsCompleted);
        }
    }
}
