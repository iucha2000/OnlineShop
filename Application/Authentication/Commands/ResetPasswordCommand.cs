using Application.Common.Handlers;
using Application.Common.Persistence;
using Application.Services;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Commands
{
    public record ResetPasswordCommand(string password, string email, Guid token) : IRequest<Result>;

    internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork, IPasswordHandler passwordHandler)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHandler = passwordHandler;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x => x.Email == request.email);

            if (user == null)
            {
                throw new UserNotFoundException($"User with email: {request.email} does not exist", 404);
            }

            if(user.VerificationCode == request.token)
            {
                _passwordHandler.CreateHashAndSalt(request.password, out var newHash, out var newSalt);
                user.PasswordSalt = newSalt;
                user.PasswordHash = newHash;

                await _unitOfWork.CommitAsync();
                return Result.Succeed();
            }

            throw new InvalidTokenException("Verification code is invalid", 400);
        }
    }
}
