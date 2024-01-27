using Application.Common.Persistence;
using Application.Order.Commands;
using Application.Services;
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
    public class CheckoutOrderCommandTests
    {

        private readonly Mock<IGenericRepository<Domain.Entities.Product>> _productMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.Order>> _orderMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.User>> _userMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.Image>> _imageMockRepository;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IExchangeRate> _exchangeRateMock;
        private readonly Mock<IEmailSender> _mailSenderMock;

        public CheckoutOrderCommandTests()
        {
            _orderMockRepository = new();
            _userMockRepository = new();
            _unitOfWorkMock = new();
            _productMockRepository = new();
            _imageMockRepository = new();
            _exchangeRateMock = new();
            _mailSenderMock = new();
        }

        [Fact]
        public async Task CheckoutOrderCommand_Should_Throw_EmailNotVerifiedException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var command = new CheckoutOrderCommand(userId, true, "address");

            _userMockRepository.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new User
                {
                    EmailVerified = false,
                });

            var handler = new CheckoutOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object,
                _userMockRepository.Object, _unitOfWorkMock.Object, _exchangeRateMock.Object, _mailSenderMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<EmailNotVerifiedException>(async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task CheckoutOrderCommand_Should_Throw_OrderNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var command = new CheckoutOrderCommand(userId, true, "address");

            _userMockRepository.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new User
                {
                    EmailVerified = true,
                    Orders = new List<Order>()
                });

            var handler = new CheckoutOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object,
               _userMockRepository.Object, _unitOfWorkMock.Object, _exchangeRateMock.Object, _mailSenderMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(async () => await handler.Handle(command, default));
        }

        [Theory]
        [InlineData(100, 200, 150)]
        [InlineData(10, 15, 0)]
        public async Task CheckoutOrderCommand_Should_Return_Fail_When_Balance_Not_Enough(decimal price1, decimal price2, decimal balance)
        {
            //Arrange
            var userId = Guid.NewGuid();
            var command = new CheckoutOrderCommand(userId, true, "address");

            _userMockRepository.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
               .ReturnsAsync(new User
               {
                   EmailVerified = true,
                   Balance = balance,
                   Currency = "USD",
                   Orders = new List<Order>()
                   {
                       new Order 
                       { 
                           IsCompleted = false,
                           Products = new List<Domain.Entities.Product>()
                           {
                               new Domain.Entities.Product
                               {
                                   Price = 200,
                               },
                               new Domain.Entities.Product
                               {
                                   Price = 300
                               }
                           }
                       }
                   }
               });

            _orderMockRepository.Setup(x=> x.GetByExpressionAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Order
                {
                    IsCompleted = false,
                    Products = new List<Domain.Entities.Product>()
                    {
                        new Domain.Entities.Product
                        {
                            Price = price1,
                        },
                        new Domain.Entities.Product
                        {
                            Price = price2
                        }
                    }
                });

            _exchangeRateMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRatesModel()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates() { USD = 1, EUR = 2}
                });

            var handler = new CheckoutOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object,
             _userMockRepository.Object, _unitOfWorkMock.Object, _exchangeRateMock.Object, _mailSenderMock.Object);

            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.False(actual.Success);
        }

        [Theory]
        [InlineData(12.5, 4.5, 20)]
        [InlineData(34, 16, 50)]
        public async Task CheckoutOrderCommand_Should_Return_Success(decimal price1, decimal price2, decimal balance)
        {
            var userId = Guid.NewGuid();
            var command = new CheckoutOrderCommand(userId, true, "address");

            _userMockRepository.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
               .ReturnsAsync(new User
               {
                   EmailVerified = true,
                   Balance = balance,
                   Currency = "USD",
                   Orders = new List<Order>()
                   {
                       new Order
                       {
                           IsCompleted = false,
                           Products = new List<Domain.Entities.Product>()
                           {
                               new Domain.Entities.Product
                               {
                                   Price = 200,
                               },
                               new Domain.Entities.Product
                               {
                                   Price = 300
                               }
                           }
                       }
                   }
               });

            _orderMockRepository.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Order
                {
                    IsCompleted = false,
                    Products = new List<Domain.Entities.Product>()
                    {
                        new Domain.Entities.Product
                        {
                            Price = price1,
                        },
                        new Domain.Entities.Product
                        {
                            Price = price2
                        }
                    }
                });

            _exchangeRateMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRatesModel()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates() { USD = 1, EUR = 2 }
                });

            var handler = new CheckoutOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object,
             _userMockRepository.Object, _unitOfWorkMock.Object, _exchangeRateMock.Object, _mailSenderMock.Object);

            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.True(actual.Success);
            Assert.True(actual.Value.IsCompleted);
        }
    }
}
