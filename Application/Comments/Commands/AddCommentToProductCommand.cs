using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands
{
    public record AddCommentToProductCommand(Guid userId, string? message, int? rating, int productId) : IRequest<Result>;

    public class AddCommentToProductCommandHandler : IRequestHandler<AddCommentToProductCommand, Result>
    {
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IGenericRepository<Domain.Entities.Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddCommentToProductCommandHandler(IGenericRepository<Comment> commentRepository, IUnitOfWork unitOfWork, IGenericRepository<Domain.Entities.Product> productRepository)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public async Task<Result> Handle(AddCommentToProductCommand request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(request.message) && request.rating == null)
            {
                return Result.Fail("Incorrect comment parameters", 400);
            }

            var product = await _productRepository.GetByExpressionAsync(x => x.ProductId == request.productId);
            if (product == null)
            {
                throw new ProductNotFoundException("Product not found", request.productId, 404);
            }

            var comment = new Comment
            {
                Message = request.message,
                ProductId = request.productId,
                Rating = request.rating,
                UserId = request.userId,
            };

            await _commentRepository.AddAsync(comment);

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
