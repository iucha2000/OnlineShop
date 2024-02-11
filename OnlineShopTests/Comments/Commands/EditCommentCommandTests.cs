using Application.Comments.Commands;
using Application.Common.Persistence;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopTests.Comments.Commands
{ 
    public class EditCommentCommandTests
    {
        private readonly Mock<IGenericRepository<Comment>> _commentMockRepository;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public EditCommentCommandTests()
        {
            _commentMockRepository = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task EditCommentCommand_Should_Throw_UnauthorizedException_When_UserId_Empty()
        {
            //Arrange
            var userId = Guid.Empty;
            var commentId = Guid.NewGuid();

            var command = new EditCommentCommand(userId, commentId, "message", null);

            var handler = new EditCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task EditCommentCommand_Should_Return_Fail_When_Message_And_Rating_Are_Null()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new EditCommentCommand(userId, commentId, null, null);

            var handler = new EditCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("Incorrect comment parameters", result.Message);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task EditCommentCommand_Should_Return_Fail_When_Comment_Is_Null()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new EditCommentCommand(userId, commentId, "message", null);

            var handler = new EditCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

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
        public async Task EditCommentCommand_Should_Throw_UnauthorizedException_When_UserId_Not_Equal_To_Comment_UserId()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new EditCommentCommand(userId, commentId, "message", null);

            var handler = new EditCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

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
        public async Task EditCommentCommand_Should_Return_Success()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new EditCommentCommand(userId, commentId, "new message", 5);

            var handler = new EditCommentCommandHandler(_commentMockRepository.Object, _unitOfWorkMock.Object);

            _commentMockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
               .ReturnsAsync(new Domain.Result<Comment>
               {
                   Value = new Comment
                   {
                       Id = commentId,
                       Message = "old message",
                       Rating = 1, 
                       UserId = userId,
                   }
               });

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), times: Times.Exactly(1));
        }

    }
}
