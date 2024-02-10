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

namespace Application.Order.Commands
{
    public record RemoveProductFromOrderCommand(Guid UserId, int ProductId, int Count) : IRequest<Result<Domain.Entities.Order>>;
    public class RemoveProductFromOrderCommandHandler : IRequestHandler<RemoveProductFromOrderCommand, Result<Domain.Entities.Order>>
    {
        private readonly IGenericRepository<Domain.Entities.Order> _orderRepo;
        private readonly IGenericRepository<Domain.Entities.Product> _productRepo;
        private readonly IGenericRepository<Domain.Entities.User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveProductFromOrderCommandHandler(IGenericRepository<Domain.Entities.Order> orderRepo, IGenericRepository<Domain.Entities.Product> productRepo,
            IGenericRepository<Domain.Entities.User> userRepo, IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Domain.Entities.Order>> Handle(RemoveProductFromOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByExpressionAsync(x => x.Id == request.UserId, includes: "Orders");

            if(user is null)
            {
                throw new UserNotFoundException("User not found", 404);
            }

            var ongoingOrder = user.Orders.FirstOrDefault(o=> o.IsCompleted == false);

            if (ongoingOrder == null)
            {
                throw new OrderNotFoundException("Ongoing order not found", request.UserId, 404);
            }

            var orderFromDb = await _orderRepo.GetByExpressionAsync(x => x.Id == ongoingOrder.Id, includes: "Products");

            var productsFromOrder = orderFromDb.Products.Where(x => x.ProductId == request.ProductId).ToList();
            if (productsFromOrder.Count() < request.Count)
            {
                return Result<Domain.Entities.Order>.Fail("Not enough product in order");
            }

            var productsToRemove = productsFromOrder.Take(request.Count).ToList();

            for (int i = 0; i < productsToRemove.Count(); i++)
            {
                ongoingOrder.Products.Remove(productsToRemove[i]);
                productsToRemove[i].OrderId = null;
            }

            await _unitOfWork.CommitAsync();

            return Result<Domain.Entities.Order>.Succeed(ongoingOrder);
        }
    }
}
