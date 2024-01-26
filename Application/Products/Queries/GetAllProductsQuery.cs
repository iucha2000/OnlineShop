using Application.Common.Persistence;
using Application.Product.Models;
using Application.Services;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Product.Queries
{
    public record GetAllProductsQuery(Guid userId) : IRequest<Result<IEnumerable<ProductModel>>>;

    internal class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductModel>>>
    {
        private readonly IGenericRepository<Domain.Entities.Product> _productRepository;
        private readonly IGenericRepository<Image> _imageRepository;
        private readonly IGenericRepository<Domain.Entities.User> _userRepository;
        private readonly IExchangeRate _exchangeRate;

        public GetAllProductsQueryHandler(IGenericRepository<Domain.Entities.Product> productRepository, IGenericRepository<Image> imageRepository, IGenericRepository<Domain.Entities.User> userRepository, IExchangeRate exchangeRate)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _exchangeRate = exchangeRate;
        }

        public async Task<Result<IEnumerable<ProductModel>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.ListAsync(trackChanges: false);

            var result = products.Select(x => new ProductModel
            {
                ProductId = x.ProductId,
                Name = x.Name,
                Price = x.Price,
                Category = x.Category,
            }).ToList();

            foreach (var product in result)
            {
                var image = await _imageRepository.GetByExpressionAsync(x => x.ProductId == product.ProductId);
                product.Url = image?.Url;
            }

            if(request.userId != Guid.Empty)
            {
                var userResult = await _userRepository.GetByIdAsync(request.userId);

                if(userResult is null)
                {
                    throw new UserNotFoundException("User not found");
                }

                var rates = await _exchangeRate.GetExchangeRates(userResult.Value.Currency);
                result.ForEach(x => Math.Round(x.Price /= rates.conversion_rates.USD, 2));
            }

            return Result<IEnumerable<ProductModel>>.Succeed(result);
        }
    }
}
