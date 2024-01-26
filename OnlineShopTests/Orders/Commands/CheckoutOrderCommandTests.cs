using Application.Common.Persistence;
using Application.Order.Commands;
using Application.Services;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [Theory]
        [InlineData(false, true, "Address")]
        public async Task CheckoutOrderCommand_Should_Throw_Exception(bool verified, bool delivery, string address)
        {
            //Arrange
            var userId = Guid.NewGuid();
            var command = new CheckoutOrderCommand(userId, delivery, address);


            _userMockRepository.Setup(x => x.GetByExpressionAsync(x => x.Id == userId, "Orders", true))
                .ReturnsAsync(new Domain.Entities.User
                {
                    EmailVerified = verified,
                });


            var handler = new CheckoutOrderCommandHandler(_orderMockRepository.Object, _productMockRepository.Object,
                _userMockRepository.Object, _unitOfWorkMock.Object, _exchangeRateMock.Object, _mailSenderMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<EmailNotVerifiedException>(async () => await handler.Handle(command, default));
        }
    }
}
