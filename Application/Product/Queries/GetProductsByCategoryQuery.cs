using Application.Common.Persistence;
using Domain;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Product.Queries
{
    public record GetProductsByCategoryQuery(ProductCategory Category) : IRequest<Result<IEnumerable<Domain.Entities.Product>>>;

    internal class GetProductByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<IEnumerable<Domain.Entities.Product>>>
    {
        private readonly IGenericRepository<Domain.Entities.Product> _repository;

        public GetProductByCategoryQueryHandler(IGenericRepository<Domain.Entities.Product> repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<Domain.Entities.Product>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.ListAsync(x => x.Category == request.Category);
            return Result<IEnumerable<Domain.Entities.Product>>.Succeed(products);
        }
    }
}
