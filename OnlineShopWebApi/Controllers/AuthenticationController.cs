using Application.Authentication.Commands;
using Application.Authentication.Models;
using Application.Authentication.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApi.Extensions;

namespace OnlineShopWebApi.Controllers
{
    public class AuthenticationController : BaseController
    {
        public AuthenticationController(ILoggerFactory logger, ISender mediator) : base(logger, mediator)
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

            if (result.Success)
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

        [HttpGet("Verify")]
        public async Task<IActionResult> Verify(string email, Guid verificationCode)
        {
            var command = new VerifyUserEmailCommand(email, verificationCode);

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("Forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordModel model)
        {
            var command = new ForgetPasswordCommand(model.Email);

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("Reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var command = new ResetPasswordCommand(model.Password, model.Email, model.Token);

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [Authorize]
        [HttpPost("Change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var userId = HttpContext.GetUserId();
            var command = new ChangePasswordCommand(userId, model.oldPassword, model.newPassword);

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
