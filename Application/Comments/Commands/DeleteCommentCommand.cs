using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands
{
    public record DeleteCommentCommand(Guid userId, Guid commentId) : IRequest<Result>;

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result>
    {
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentCommandHandler(IGenericRepository<Comment> commentRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            if(request.userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();
            }

            var comment = _commentRepository.GetByIdAsync(request.commentId).Result.Value;
            if(comment is null)
            {
                return Result.Fail("Comment not found", 404);
            }

            if(comment.UserId != request.userId)
            {
                throw new UnauthorizedAccessException();
            }

            await _commentRepository.DeleteAsync(comment);

            await _unitOfWork.CommitAsync();
            return Result.Succeed();
        }
    }
}
