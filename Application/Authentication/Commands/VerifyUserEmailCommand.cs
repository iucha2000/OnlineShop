using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Commands
{
    public record VerifyUserEmailCommand(string email, Guid verificationCode) : IRequest<Result>;

    internal class VerifyUserEmailCommandHandler : IRequestHandler<VerifyUserEmailCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyUserEmailCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(VerifyUserEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x => x.Email == request.email && x.EmailVerified == false);

            if (user == null)
            {
                //TODO
                throw new ArgumentException();
            }

            if(user.VerificationCode == request.verificationCode)
            {
                user.EmailVerified = true;
                await _unitOfWork.CommitAsync();
                return Result.Succeed();
            }

            return Result.Fail("Verification code is incorrect", 500);
        }
    }
}
