using Application.Common.Persistence;
using Application.Order.Models;
using Application.Product.Models;
using Application.Services;
using Application.User.Models;
using Domain;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User.Queries
{
    public record GetUserProfileQuery(Guid userId) :IRequest<Result<UserProfileModel>>;

    internal class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileModel>>
    {
        private readonly IGenericRepository<Domain.Entities.User> _userRepository;
        private readonly IGenericRepository<Domain.Entities.Product> _productRepository;
        private readonly IExchangeRate _exchangeRate;

        public GetUserProfileQueryHandler(IGenericRepository<Domain.Entities.User> userRepository, IGenericRepository<Domain.Entities.Product> productRepository, IExchangeRate exchangeRate)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _exchangeRate = exchangeRate;
        }

        public async Task<Result<UserProfileModel>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x => x.Id == request.userId, includes: "Orders");

            if (user == null)
            {
                throw new UserNotFoundException("User does not exist", 404);
            }

            var orders = new List<OrderModel>();
            foreach(var order in user.Orders)
            {
                var products = await _productRepository.ListAsync(x => x.OrderId == order.Id, trackChanges: false);
                orders.Add(new OrderModel
                {
                    StartDate = order.StartDate,
                    EndDate = order.EndDate,
                    IsCompleted = order.IsCompleted,
                    Products = products.Select(x=> new ProductModel
                    {
                        ProductId = x.ProductId,
                        Name = x.Name,
                        Price = x.Price,
                        Category = x.Category,

                    }).ToList(),
                });
            }

            var exchangeRate = await _exchangeRate.GetExchangeRates(user.Currency);

            var response = new UserProfileModel()
            {
                Email = user.Email,
                Balance = user.Balance,
                Currency = user.Currency,
                EmailVerified = user.EmailVerified,
                ExchangeRates = exchangeRate.conversion_rates,
                Orders = orders
            };

            return Result<UserProfileModel>.Succeed(response);
        }
    }
}
