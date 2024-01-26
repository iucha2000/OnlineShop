using Application.Common.Persistence;
using Application.Services;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User.Commands
{
    public record EditUserInfoCommand(Guid userId, string? Email, string? Currency, Role? Role, string? Address) : IRequest<Result>;

    internal class EditUserInfoCommandHandler : IRequestHandler<EditUserInfoCommand, Result>
    {
        private readonly IGenericRepository<Domain.Entities.User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public EditUserInfoCommandHandler(IGenericRepository<Domain.Entities.User> userRepository, IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task<Result> Handle(EditUserInfoCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetByIdAsync(request.userId);
            if(userResult.Success == false)
            {
                throw new UserNotFoundException("User not found", 404);
            }

            var user = userResult.Value;

            if(!string.IsNullOrWhiteSpace(user.Currency))
            {
                user.Currency = request.Currency;
            }
            if (request.Role.HasValue)
            {
                user.Role = request.Role.Value;
            }
            if(!string.IsNullOrWhiteSpace(user.Address))
            {
                user.Address = request.Address;
            }
            if(!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
                user.EmailVerified = false;
                user.VerificationCode = Guid.NewGuid();
                await _emailSender.SendVerificationEmail(request.Email, user.VerificationCode);
            }

            await _unitOfWork.CommitAsync();
            return Result.Succeed();
        }
    }
}
