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
    public record EditCommentCommand(Guid userId, Guid commentId, string? message, int? rating) : IRequest<Result>;

    public class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, Result>
    {
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditCommentCommandHandler(IGenericRepository<Comment> commentRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(EditCommentCommand request, CancellationToken cancellationToken)
        {
            if (request.userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();
            }

            if (string.IsNullOrWhiteSpace(request.message) && request.rating == null)
            {
                return Result.Fail("Incorrect comment parameters", 400);
            }

            var comment = _commentRepository.GetByIdAsync(request.commentId).Result.Value;
            if (comment is null)
            {
                return Result.Fail("Comment not found", 404);
            }

            if (comment.UserId != request.userId)
            {
                throw new UnauthorizedAccessException();
            }

            comment.Message = request.message;
            comment.Rating = request.rating;
            await _commentRepository.UpdateAsync(comment);

            await _unitOfWork.CommitAsync();
            return Result.Succeed();
        }
    }
}
