using Application.Common.Persistence;
using Application.Product.Queries;
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

namespace OnlineShopTests.Products.Queries
{
    public class GetProductsByProductIdQueryTests
    {
        private readonly Mock<IGenericRepository<Domain.Entities.Product>> _productMockRepository;
        private readonly Mock<IGenericRepository<Image>> _imageMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.User>> _userMockRepository;
        private readonly Mock<IGenericRepository<Comment>> _commentMockRepository;
        private readonly Mock<IExchangeRate> _exchangeRateMock;

        public GetProductsByProductIdQueryTests()
        {
            _productMockRepository = new();
            _imageMockRepository = new();
            _userMockRepository = new();
            _exchangeRateMock = new();
            _commentMockRepository = new();
        }

        [Fact]
        public async Task GetProductByProductId_Should_Return_ProductNotFound()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var productId = 3;
            var command = new GetProductsByProductIdQuery(productId, userId);

            var handler = new GetProductByCategoryQueryHandler
                (_productMockRepository.Object, _imageMockRepository.Object,
                _userMockRepository.Object, _exchangeRateMock.Object, _commentMockRepository.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task GetProductByProductId_Should_Return_Product_Without_Comments()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var productId = 3;
            var query = new GetProductsByProductIdQuery(productId, userId);

            var handler = new GetProductByCategoryQueryHandler
              (_productMockRepository.Object, _imageMockRepository.Object,
              _userMockRepository.Object, _exchangeRateMock.Object, _commentMockRepository.Object);

            _userMockRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new Domain.Result<User>
            {
              Value = new User() { Id = userId, Currency = "USD" }
            });

            _productMockRepository.Setup(x=> x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(), It.IsAny<string>(), false))
                .ReturnsAsync(new Domain.Entities.Product
                {
                    ProductId = productId,
                    Name = "Test",
                });

            _commentMockRepository.Setup(x => x.ListAsync(
                 It.IsAny<Expression<Func<Domain.Entities.Comment, bool>>>(), It.IsAny<string>(), false, null, 0))
                .ReturnsAsync(new List<Domain.Entities.Comment>());

            _exchangeRateMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRatesModel()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates() { USD = 1, EUR = 2 }
                });

            //Act
            var result = await handler.Handle(query, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Empty(result.Value.Comments);
        }

        [Theory]
        [InlineData(4, "Pretty good")]
        [InlineData(2, "Pretty bad")]
        [InlineData(1, "Terrible")]
        public async Task GetProductByProductId_Should_Return_Product_With_Comments(int rating, string comment)
        {
            //Arrange
            var userId = Guid.NewGuid();
            var productId = 3;
            var query = new GetProductsByProductIdQuery(productId, userId);

            var handler = new GetProductByCategoryQueryHandler
              (_productMockRepository.Object, _imageMockRepository.Object,
              _userMockRepository.Object, _exchangeRateMock.Object, _commentMockRepository.Object);

            _userMockRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new Domain.Result<User>
                {
                    Value = new User() { Id = userId, Currency = "USD" }
                });

            _productMockRepository.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(), It.IsAny<string>(), false))
                .ReturnsAsync(new Domain.Entities.Product
                {
                    ProductId = productId,
                    Name = "Test",
                });

            _commentMockRepository.Setup(x => x.ListAsync(
                 It.IsAny<Expression<Func<Domain.Entities.Comment, bool>>>(), It.IsAny<string>(), false, null, 0))
                .ReturnsAsync(new List<Domain.Entities.Comment>()
                {
                    new Domain.Entities.Comment()
                    {
                        Id = Guid.NewGuid(),
                        Rating = rating,
                        Message = comment
                    }
                });

            _exchangeRateMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRatesModel()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates() { USD = 1, EUR = 2 }
                });

            //Act
            var result = await handler.Handle(query, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(rating, result.Value.Comments[0].Rating);
            Assert.Equal(comment, result.Value.Comments[0].Message);
        }
    }
}
