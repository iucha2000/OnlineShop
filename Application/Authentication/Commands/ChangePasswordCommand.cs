using Application.Common.Handlers;
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

namespace Application.Authentication.Commands
{
    public record ChangePasswordCommand(Guid userId, string oldPassword, string newPassword) : IRequest<Result>;

    internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IGenericRepository<Domain.Entities.User> _userRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IGenericRepository<Domain.Entities.User> userRepository, IPasswordHandler passwordHandler, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHandler = passwordHandler;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetByIdAsync(request.userId);

            if (userResult.Success == false)
            {
                throw new UserNotFoundException($"User does not exist", 404);
            }

            var user = userResult.Value;

            if(_passwordHandler.VerifyPasswordHash(request.oldPassword, user.PasswordHash, user.PasswordSalt) == false)
            {
                throw new IncorrectPasswordException("Old password is incorrect", 400);
            }

            _passwordHandler.CreateHashAndSalt(request.newPassword, out var newHash, out var newSalt);
            user.PasswordSalt = newSalt;
            user.PasswordHash = newHash;

            await _unitOfWork.CommitAsync();
            return Result.Succeed();
        }
    }
}
