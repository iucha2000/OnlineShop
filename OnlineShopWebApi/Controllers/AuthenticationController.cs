﻿using Application.Authentication.Commands;
using Application.Authentication.Models;
using Application.Authentication.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApi.Controllers
{
    public class AuthenticationController : BaseController
    {
        public AuthenticationController(/*ILogger logger,*/ ISender mediator) : base(/*logger,*/ mediator)
        {

        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(409)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var command = new RegisterUserCommand(registerModel.EmailAddress, registerModel.Password, registerModel.AdminSecret);

            var result = await _mediator.Send(command);

            if(result.Success)
            {
                return Ok(result.Value);
            }
            else
            {
                return Problem(result.Message);
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var query = new LoginQuery(loginModel.EmailAddress, loginModel.Password);

            var result = await _mediator.Send(query);

            if (result.Success)
            {
                return Ok(result.Value);
            }
            else
            {
                return Problem(result.Message);
            }
        }

    }
}
