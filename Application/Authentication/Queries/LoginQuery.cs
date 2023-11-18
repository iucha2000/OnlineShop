using Application.Common.Handlers;
using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Queries
{
    public record LoginQuery(string Email, string Password) : IRequest<Result<string>>;

    internal class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITokenHandler _tokenHandler;
        private readonly IUnitOfWork _unitOfWork;

        public LoginQueryHandler(IGenericRepository<User> userRepository, IPasswordHandler passwordHandler,
            ITokenHandler tokenHandler, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHandler = passwordHandler;
            _tokenHandler = tokenHandler;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByExpressionAsync(x => x.Email == request.Email);
            if (existingUser is null)
            {
                return Result<string>.Fail("Incorrect Credentials", StatusCodes.Status404NotFound);
            }

            if(!_passwordHandler.VerifyPasswordHash(request.Password, existingUser.PasswordHash, existingUser.PasswordSalt))
            {
                return Result<string>.Fail("Incorrect Credentials", StatusCodes.Status400BadRequest);
            }

            var token = _tokenHandler.GenerateToken(existingUser);

            return Result<string>.Succeed(token);
        }
    }
}
