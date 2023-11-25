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
    public record AddProductToOrderCommand(Guid UserId, Guid ProductId, int Count): IRequest<Result<Domain.Entities.Order>>;

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
            var user = await _userRepo.GetByIdAsync(request.UserId);
            //TODO
            //check if user exists

            var order = await _orderRepo.GetByExpressionAsync(x=> x.UserId == request.UserId, includes: "Products");
            var product = await _productRepo.GetByIdAsync(request.ProductId);

            if(order is null)
            {
                order = new Domain.Entities.Order
                {
                    UserId = request.UserId,
                };
                order.Products.Add(product.Value);
            }
            else
            {
                var existingProduct = order.Products.FirstOrDefault(x => x.Id == request.ProductId);

                if(existingProduct != null)
                {
                    existingProduct.RemainingCount += request.Count;
                }
                else
                {
                    order.Products.Add(product.Value);
                }
            }

            return Result<Domain.Entities.Order>.Succeed(order);

        }
    }
}
