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
    public record ForgetPasswordCommand(string Email) : IRequest<Result>;

    internal class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result>
    {
        private readonly IGenericRepository<Domain.Entities.User> _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public ForgetPasswordCommandHandler(IGenericRepository<Domain.Entities.User> userRepository, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x=> x.Email ==  request.Email);
            if (user == null)
            {
                throw new UserNotFoundException($"User with email: {request.Email} does not exist", 404);
            }

            user.VerificationCode = Guid.NewGuid();

            await _emailSender.SendPasswordResetEmail(user.Email, user.VerificationCode);

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
