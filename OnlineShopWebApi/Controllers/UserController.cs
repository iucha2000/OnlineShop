using Application.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApi.Extensions;

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
    }
}
