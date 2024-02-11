using Application.Comments.Commands;
using Application.Common.Persistence;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopTests.Comments.Commands
{
    public class AddCommentToProductCommandTests
    {
        private readonly Mock<IGenericRepository<Comment>> _commentMockRepository;
        private readonly Mock<IGenericRepository<Domain.Entities.Product>> _productMockRepository;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddCommentToProductCommandTests()
        {
            _commentMockRepository = new();
            _productMockRepository = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task AddCommentToProductCommand_Should_Throw_ProductNotFoundException_When_ProductId_Is_Incorrect()
        {
            //Arrange
            var userId = Guid.NewGuid();

            var command = new AddCommentToProductCommand(userId, "some comment", null, 1);

            var handler = new AddCommentToProductCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object, _productMockRepository.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task AddCommentToProductCommand_Should_Return_Fail_When_Message_And_Rating_Are_Null()
        {
            //Arrange
            var userId = Guid.NewGuid();

            var command = new AddCommentToProductCommand(userId, null, null, 1);

            var handler = new AddCommentToProductCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object, _productMockRepository.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("Incorrect comment parameters", result.Message);
            Assert.Equal(400, result.StatusCode);
        }

        [Theory]
        [InlineData("Pretty Great", null, 1)]
        [InlineData(null, 3, 2)]
        [InlineData("Bad", 1, 3)]
        public async Task AddCommentToProductCommand_Should_Return_Success(string? message, int? rating, int productId)
        {
            //Arrange
            var userId = Guid.NewGuid();

            var command = new AddCommentToProductCommand(userId, message, rating, productId);

            var handler = new AddCommentToProductCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object, _productMockRepository.Object);

            _productMockRepository.Setup(x => x.GetByExpressionAsync(
               It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Domain.Entities.Product() { ProductId = productId});

            var expected = new Comment
            {
                Message = message,
                Rating = rating,
                ProductId = productId,
                UserId = userId
            };

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), times: Times.Exactly(1));
        }
    }
}
