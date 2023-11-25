using Application.Common.Persistence;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Product.Queries
{
    public record GetAllProductsQuery : IRequest<Result<IEnumerable<Domain.Entities.Product>>>;

    internal class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<Domain.Entities.Product>>>
    {
        private readonly IGenericRepository<Domain.Entities.Product> _repository;

        public GetAllProductsQueryHandler(IGenericRepository<Domain.Entities.Product> repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<Domain.Entities.Product>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.ListAsync(x=> true);

            return Result<IEnumerable<Domain.Entities.Product>>.Succeed(products);
        }
    }
}
