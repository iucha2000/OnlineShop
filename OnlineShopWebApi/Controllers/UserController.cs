using Application.User.Commands;
using Application.User.Models;
using Application.User.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApi.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineShopWebApi.Controllers
{
    public class UserController : BaseController
    {
        public UserController(ILoggerFactory logger, ISender mediator) : base(logger, mediator)
        {
        }

        [Authorize]
        [HttpGet("get-user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = HttpContext.GetUserId();

            var query = new GetUserProfileQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("edit-user-info")]
        public async Task<IActionResult> EditUserInfo(EditUserInfoModel model)
        {
            var userId = HttpContext.GetUserId();

            var command = new EditUserInfoCommand(userId, model.Email, model.Currency, model.Role, model.Address);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
