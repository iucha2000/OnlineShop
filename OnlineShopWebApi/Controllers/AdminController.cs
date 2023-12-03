using Application.AdminPanel.Commands;
using Application.AdminPanel.Models;
using Application.AdminPanel.Queries;
using Application.Order.Models;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApi.Extensions;

namespace OnlineShopWebApi.Controllers
{
    
    public class AdminController : BaseController
    {
        public AdminController(/*ILogger logger,*/ ISender mediator) : base(/*logger,*/ mediator)
        {
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(AddProductModel model)
        {
            await Task.CompletedTask;
            var userId = HttpContext.GetUserId();
            var command = new AddProductCommand(model.Name, model.Price, model.ProductId, model.Category, model.Count);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("get-user-orders")]
        public async Task<IActionResult> GetUserOrders(Guid userId)
        {
            await Task.CompletedTask;
            var query = new GetUserOrderQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
