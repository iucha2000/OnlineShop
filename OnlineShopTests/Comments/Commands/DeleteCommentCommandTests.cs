using Application.Comments.Commands;
using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopTests.Comments.Commands
{
    public class DeleteCommentCommandTests
    {

        private readonly Mock<IGenericRepository<Comment>> _commentMockRepository;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public DeleteCommentCommandTests()
        {
            _commentMockRepository = new();
            _unitOfWorkMock = new();

        }

        [Fact]
        public async Task DeleteCommentCommand_Should_Throw_UnauthorizedException_When_UserId_Empty()
        {
            //Arrange
            var userId = Guid.Empty;
            var commentId = Guid.NewGuid();

            var command = new DeleteCommentCommand(userId, commentId);

            var handler = new DeleteCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async()=> await handler.Handle(command, default));   
        }

        [Fact]
        public async Task DeleteCommentCommand_Should_Return_Fail_When_Comment_Is_Null()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new DeleteCommentCommand(userId, commentId);

            var handler = new DeleteCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

            _commentMockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Domain.Result<Comment>
                {
                    Value = null
                });

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("Comment not found", result.Message);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task DeleteCommentCommand_Should_Throw_UnauthorizedException_When_UserId_Not_Equal_To_Comment_UserId()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new DeleteCommentCommand(userId, commentId);

            var handler = new DeleteCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

            _commentMockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
               .ReturnsAsync(new Domain.Result<Comment>
               {
                   Value = new Comment
                   {
                       Id = commentId,
                       UserId = Guid.NewGuid(),
                   }
               });

            //Act
            //Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task DeleteCommentCommand_Should_Return_Success()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new DeleteCommentCommand(userId, commentId);

            var handler = new DeleteCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

            _commentMockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
               .ReturnsAsync(new Domain.Result<Comment>
               {
                   Value = new Comment
                   {
                       Id = commentId,
                       UserId = userId,
                   }
               });

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
        }

    }
}
