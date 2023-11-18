using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Product.Queries
{
    public record GetProductByIdQuery(Guid productId) : IRequest<Result>;

    internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result>
    {
        public Task<Result> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
