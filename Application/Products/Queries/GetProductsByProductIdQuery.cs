using Application.Comments.Models;
using Application.Common.Persistence;
using Application.Product.Models;
using Application.Services;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Product.Queries
{
    public record GetProductsByProductIdQuery(int ProductId, Guid userId) : IRequest<Result<ProductModel>>;

    internal class GetProductByCategoryQueryHandler : IRequestHandler<GetProductsByProductIdQuery, Result<ProductModel>>
    {
        private readonly IGenericRepository<Domain.Entities.Product> _productRepository;
        private readonly IGenericRepository<Image> _imageRepository;
        private readonly IGenericRepository<Domain.Entities.User> _userRepository;
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IExchangeRate _exchangeRate;

        public GetProductByCategoryQueryHandler(IGenericRepository<Domain.Entities.Product> productRepository, IGenericRepository<Image> imageRepository, IGenericRepository<Domain.Entities.User> userRepository, IExchangeRate exchangeRate, IGenericRepository<Comment> commentRepository)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _exchangeRate = exchangeRate;
            _commentRepository = commentRepository;
        }

        public async Task<Result<ProductModel>> Handle(GetProductsByProductIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByExpressionAsync(x => x.ProductId == request.ProductId && x.IsSold == false, trackChanges: false);

            var image = await _imageRepository.GetByExpressionAsync(x => x.ProductId == product.ProductId, trackChanges: false);

            var comments = await _commentRepository.ListAsync(x=> x.ProductId == request.ProductId, trackChanges: false);

            var commentModels = new List<CommentModel>();
            foreach( var comment in comments )
            {
                var commentModel = new CommentModel
                {
                    CommentId = comment.Id,
                    Message = comment.Message,
                    Rating = comment.Rating,
                };
                if(comment.UserId != Guid.Empty)
                {
                    var user = await _userRepository.GetByExpressionAsync(x=> x.Id == comment.UserId, trackChanges: false);
                    commentModel.Name = user.Email;
                }
                commentModels.Add(commentModel);
            }

            var result = new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category,
                Url = image?.Url,
                Comments = commentModels,
            };

            if(request.userId != Guid.Empty)
            {
                var userResult = await _userRepository.GetByIdAsync(request.userId);
                var rates = await _exchangeRate.GetExchangeRates(userResult.Value.Currency);
                result.Price = Math.Round(result.Price / rates.conversion_rates.USD, 2);
            }

            return Result<ProductModel>.Succeed(result);
        }
    }
}
