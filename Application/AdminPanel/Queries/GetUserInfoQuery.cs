using Application.AdminPanel.Models;
using Application.Common.Persistence;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Queries
{
    public record GetUserInfoQuery(Guid userId) : IRequest<GetUserInfoResponseModel>;

    internal class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, GetUserInfoResponseModel>
    {
        private readonly IGenericRepository<Domain.Entities.User> _userRepo;
        private readonly IGenericRepository<Domain.Entities.Order> _orderRepo;
        private readonly IGenericRepository<Domain.Entities.Product> _productRepo;

        public GetUserInfoQueryHandler(IGenericRepository<User> userRepo, IGenericRepository<Domain.Entities.Order> orderRepo, IGenericRepository<Domain.Entities.Product> productRepo)
        {
            _userRepo = userRepo;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        public async Task<GetUserInfoResponseModel> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByExpressionAsync(x => x.Id == request.userId, includes: "Orders", trackChanges: false);

            foreach(var order in user.Orders)
            {
                order.Products = await _productRepo.ListAsync(x => x.OrderId == order.Id);
            }

            var response = new GetUserInfoResponseModel()
            {
                UserId = user.Id,
                Currency = user.Currency,
                Email = user.Email,
                Balance = user.Balance,
                Orders = user.Orders
            };

            return response;
        }
    }
}
