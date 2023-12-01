using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Queries
{
    public record GetUserOrderQuery(Guid userId) : IRequest<Result<IEnumerable<Domain.Entities.Order>>>;

    internal class GetUserOrderQueryHandler : IRequestHandler<GetUserOrderQuery, Result<IEnumerable<Domain.Entities.Order>>>
    {
        private readonly IGenericRepository<User> _userRepository;

        public GetUserOrderQueryHandler(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<Domain.Entities.Order>>> Handle(GetUserOrderQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x => x.Id == request.userId, includes: "Orders");

            return Result<IEnumerable<Domain.Entities.Order>>.Succeed(user.Orders);
        }
    }
}
