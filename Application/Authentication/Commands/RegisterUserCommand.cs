﻿using Application.Common.Handlers;
using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Commands
{

    public record RegisterUserCommand(string Email, string Password, string AdminSecret) : IRequest<Result<string>>;

    internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<string>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITokenHandler _tokenHandler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public RegisterUserCommandHandler(IGenericRepository<User> userRepository, IPasswordHandler passwordHandler,
            ITokenHandler tokenHandler, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHandler = passwordHandler;
            _tokenHandler = tokenHandler;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByExpressionAsync(x=> x.Email == request.Email);
            if (existingUser is not null)
            {
                return Result<string>.Fail("User already exists", StatusCodes.Status409Conflict);
            }

            _passwordHandler.CreateHashAndSalt(request.Password, out var passwordHash, out var passwordSalt);

            var newUser = new User()
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = request.AdminSecret == _configuration.GetSection("AdminPanel:AdminSecret").Value ? Domain.Enums.UserRole.Admin : Domain.Enums.UserRole.Client,
                Orders = new List<Domain.Entities.Order>()
            };

            var user = await _userRepository.AddAsync(newUser);
            await _unitOfWork.CommitAsync();

            var token = _tokenHandler.GenerateToken(newUser);

            return Result<string>.Succeed(token);
        }
    }
}
