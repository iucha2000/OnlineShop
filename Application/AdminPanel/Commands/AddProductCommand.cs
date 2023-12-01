using Application.Common.Persistence;
using Domain;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Commands
{
    public record AddProductCommand(string Name, decimal Price, int ProductId, ProductCategory Category, int Count): IRequest<Result>;

    internal class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result>
    {
        private readonly IGenericRepository<Domain.Entities.Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductCommandHandler(IGenericRepository<Domain.Entities.Product> productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            for (int i = 0; i < request.Count; i++)
            {
                var product = new Domain.Entities.Product
                {
                    Name = request.Name,
                    Price = request.Price,
                    ProductId = request.ProductId,
                    Category = request.Category,
                    OrderId = null,
                    IsSold = false
                };

                await _productRepository.AddAsync(product);
            }

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
