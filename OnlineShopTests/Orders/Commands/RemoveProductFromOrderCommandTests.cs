using Application.Common.Persistence;
using Application.Order.Commands;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopTests.Orders.Commands
{
    public class RemoveProductFromOrderCommandTests
    {
        private readonly Mock<IGenericRepository<Domain.Entities.Order>> _orderMockRepo;
        private readonly Mock<IGenericRepository<Domain.Entities.Product>> _productMockRepo;
        private readonly Mock<IGenericRepository<Domain.Entities.User>> _userMockRepo;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        private List<Domain.Entities.Product> _productsInOrder;

        public RemoveProductFromOrderCommandTests()
        {
            _orderMockRepo = new();
            _productMockRepo = new();
            _userMockRepo = new();
            _unitOfWorkMock = new();

            SeedProductsInOrderData();
        }

        private void SeedProductsInOrderData()
        {
            _productsInOrder = new List<Domain.Entities.Product>();

            for(int i=0 ; i < 5; i++)
            {
                _productsInOrder.Add(new Domain.Entities.Product {  Id = Guid.NewGuid(), ProductId = 1, Name = "Product 1" });
            }

            for (int i = 0; i < 15; i++)
            {
                _productsInOrder.Add(new Domain.Entities.Product { Id = Guid.NewGuid(), ProductId = 2, Name = "Product 2" });
            }

            for (int i = 0; i < 45; i++)
            {
                _productsInOrder.Add(new Domain.Entities.Product { Id = Guid.NewGuid(), ProductId = 3, Name = "Product 3" });
            }
        }

        [Fact]
        public async Task RemoveProductFromOrder_Should_Fail_When_User_IsNull()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var count = 1;
            var productId = 4;

            var command = new RemoveProductFromOrderCommand(userId, productId, count);

            var handler = new RemoveProductFromOrderCommandHandler(
                _orderMockRepo.Object, _productMockRepo.Object,
                _userMockRepo.Object, _unitOfWorkMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task RemoveProductFromOrder_Should_Fail_When_Order_IsNull()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var count = 1;
            var productId = 4;

            var command = new RemoveProductFromOrderCommand(userId, productId, count);

            var handler = new RemoveProductFromOrderCommandHandler(
                _orderMockRepo.Object, _productMockRepo.Object,
                _userMockRepo.Object, _unitOfWorkMock.Object);

            _userMockRepo.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), "Orders", true))
                .ReturnsAsync(new Domain.Entities.User
                {
                    Orders = new List<Domain.Entities.Order>()
                });

            //Act
            //Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(async () => await handler.Handle(command, default));
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 20)]
        [InlineData(3, 50)]
        public async Task RemoveProductFromOrder_Should_Fail_When_Not_Enough_Products(int productId, int count)
        {
            //Arrange
            var userId = Guid.NewGuid();
            string errorMessage = "Not enough product in order";
            var command = new RemoveProductFromOrderCommand(userId, productId, count);

            var handler = new RemoveProductFromOrderCommandHandler(
               _orderMockRepo.Object, _productMockRepo.Object,
               _userMockRepo.Object, _unitOfWorkMock.Object);

            _userMockRepo.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), "Orders", true))
                .ReturnsAsync(new Domain.Entities.User
                {
                    Orders = new List<Domain.Entities.Order>()
                    {
                        new Order
                        {
                            IsCompleted = false,
                        }
                    }
                });

            _orderMockRepo.Setup(x => x.GetByExpressionAsync(
                 It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Domain.Entities.Order
                {
                    Products = _productsInOrder
                });

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal(errorMessage, result.Message);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 10)]
        [InlineData(3, 20)]
        public async Task RemoveProductFromOrder_Should_Remove_Products(int productId, int count)
        {
            //Arrange
            var expected = _productsInOrder.Count - count;
            var userId = Guid.NewGuid();
            var command = new RemoveProductFromOrderCommand(userId, productId, count);

            var handler = new RemoveProductFromOrderCommandHandler(
               _orderMockRepo.Object, _productMockRepo.Object,
               _userMockRepo.Object, _unitOfWorkMock.Object);

            _userMockRepo.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), "Orders", true))
                .ReturnsAsync(new Domain.Entities.User
                {
                    Orders = new List<Domain.Entities.Order>()
                    {
                        new Order
                        {
                            IsCompleted = false,
                            Products = _productsInOrder
                        }
                    }
                });

            _orderMockRepo.Setup(x => x.GetByExpressionAsync(
                 It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Domain.Entities.Order
                {
                    Products = _productsInOrder
                });

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(expected, result.Value.Products.Count);
        }
    }
}
