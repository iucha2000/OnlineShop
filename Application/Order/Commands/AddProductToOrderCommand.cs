using Application.Common.Persistence;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Order.Commands
{
    public record AddProductToOrderCommand(Guid UserId, int ProductId, int Count) : IRequest<Result<Domain.Entities.Order>>;

    internal class AddProductToOrderCommandHandler : IRequestHandler<AddProductToOrderCommand, Result<Domain.Entities.Order>>
    {
        private readonly IGenericRepository<Domain.Entities.Order> _orderRepo;
        private readonly IGenericRepository<Domain.Entities.Product> _productRepo;
        private readonly IGenericRepository<Domain.Entities.User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductToOrderCommandHandler(IGenericRepository<Domain.Entities.Order> orderRepo, IGenericRepository<Domain.Entities.Product> productRepo,
            IGenericRepository<Domain.Entities.User> userRepo, IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Domain.Entities.Order>> Handle(AddProductToOrderCommand request, CancellationToken cancellationToken)
        {
            //TODO
            //check if user exists

            var order = await _orderRepo.GetByExpressionAsync(x => x.UserId == request.UserId && x.IsCompleted == false, includes: "Products");
            var orderIsNew = order is null;

            var products = await _productRepo
                .ListAsync(p => p.ProductId == request.ProductId 
                && p.OrderId == null 
                && p.IsSold == false, 
                count: request.Count);

            if (orderIsNew)
            {
                order = new Domain.Entities.Order
                {
                    UserId = request.UserId,
                };
            }

            foreach (var product in products)
            {
                product.OrderId = order.Id;
                order.Products.Add(product);
            }

            if(orderIsNew)
            {
                await _orderRepo.AddAsync(order);
            }
            else
            {
                await _orderRepo.UpdateAsync(order);
            }

            await _unitOfWork.CommitAsync();
            return Result<Domain.Entities.Order>.Succeed(order);
        }
    }
}
