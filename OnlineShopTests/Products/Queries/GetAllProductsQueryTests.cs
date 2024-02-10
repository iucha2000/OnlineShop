using Application.Common.Persistence;
using Application.Product.Queries;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopTests.Product.Queries
{
    public class GetAllProductsQueryTests
    {

        private readonly Mock<IGenericRepository<Domain.Entities.Product>> _productMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.Image>> _imageMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.User>> _userMockRepository;
        private readonly Mock<IExchangeRate> _exchangeRateMock;

        public GetAllProductsQueryTests()
        {
            _productMockRepository = new();
            _imageMockRepository = new();
            _userMockRepository = new();
            _exchangeRateMock = new();
        }

        [Fact]
        public async Task GetAllProductsQuery_Should_Return_One_Product()
        {
            //Arrange
            int count = 1;
            var userId = Guid.NewGuid();

            var products = new List<Domain.Entities.Product>()
            {
                new Domain.Entities.Product()
                {
                    ProductId = 1,
                    Name = "Product 1",
                    Price = 14.99m,
                    Category = Domain.Enums.ProductCategory.Fashion,
                    IsSold = false,
                },

                new Domain.Entities.Product()
                {
                    ProductId = 2,
                    Name = "Product 2",
                    Price = 24.99m,
                    Category = Domain.Enums.ProductCategory.Fashion,
                    IsSold = false,
                }
            };

            var query = new GetAllProductsQuery(userId);

            _productMockRepository.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(), It.IsAny<string>(), It.IsAny<bool>(), null, count))
                .ReturnsAsync(products);

            _exchangeRateMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRatesModel()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates()
                });

            _userMockRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new Domain.Result<User>
                {
                    Value = new User() { Id = userId, Currency = "USD" }
                });

            var handler = new GetAllProductsQueryHandler(_productMockRepository.Object, _imageMockRepository.Object,
                _userMockRepository.Object, _exchangeRateMock.Object);

            //Act
            var result = await handler.Handle(query, default);

            //Assert
            Assert.True(result.Success);
            Assert.Single(result.Value);

        }

        [Fact]
        public async Task GetAllProductsQuery_Should_Return_Two_Products()
        {
            //Arrange
            int count = 2;
            var userId = Guid.NewGuid();

            var products = new List<Domain.Entities.Product>()
            {
                new Domain.Entities.Product()
                {
                    ProductId = 1,
                    Name = "Product 1",
                    Price = 14.99m,
                    Category = Domain.Enums.ProductCategory.Fashion,
                    OrderId = null,
                    IsSold = false,
                },

                new Domain.Entities.Product()
                {
                    ProductId = 3,
                    Name = "Product 2",
                    Price = 9.99m,
                    Category = Domain.Enums.ProductCategory.Grocery,
                    OrderId = null,
                    IsSold = false,
                }
            };

            var query = new GetAllProductsQuery(userId);

            _productMockRepository.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(), It.IsAny<string>(), It.IsAny<bool>(), null, count))
                .ReturnsAsync(products);

            _exchangeRateMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRatesModel()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates()
                });

            _userMockRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new Domain.Result<User>
                {
                    Value = new User() { Currency = "USD" }
                });

            var handler = new GetAllProductsQueryHandler(_productMockRepository.Object, _imageMockRepository.Object,
                _userMockRepository.Object, _exchangeRateMock.Object);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.True(true);
            Assert.Equal(2, result.Value.Count());

        }

        [Fact]
        public async Task GetAllProductsQuery_Should_Return_Empty_List()
        {
            //Arrange
            _productMockRepository.Setup(x => x.ListAsync(null, null, false, null, 0))
                .ReturnsAsync(new List<Domain.Entities.Product>());

            var query = new GetAllProductsQuery(Guid.Empty);

            var handler = new GetAllProductsQueryHandler(_productMockRepository.Object, _imageMockRepository.Object,
                _userMockRepository.Object, _exchangeRateMock.Object);

            //Act
            var result = await handler.Handle(query, default);

            //Assert
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllProductsQuery_Should_Throw_UserNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();

            _productMockRepository.Setup(x => x.ListAsync(null, null, false, null, 0))
                .ReturnsAsync(new List<Domain.Entities.Product>());

            var query = new GetAllProductsQuery(userId);

            var handler = new GetAllProductsQueryHandler(_productMockRepository.Object, _imageMockRepository.Object,
                _userMockRepository.Object, _exchangeRateMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await handler.Handle(query, default));

        }

    }
}
