using Application.Order.Commands;
using Application.Order.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApi.Extensions;

namespace OnlineShopWebApi.Controllers
{
    public class OrderController : BaseController
    {
        public OrderController(ILoggerFactory logger, ISender mediator) : base(logger, mediator)
        {
        }

        [Authorize]
        [HttpPost("add-product-to-order")]
        public async Task<IActionResult> AddProductToOrder(AddProductToOrderModel model)
        {
            var userId = HttpContext.GetUserId();
            var command = new AddProductToOrderCommand(userId, model.ProductId, model.Quantity);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("remove-product-from-order")]
        public async Task<IActionResult> RemoveProductFromOrder(RemoveProductFromOrderModel model)
        {
            var userId = HttpContext.GetUserId();
            var command = new RemoveProductFromOrderCommand(userId, model.ProductId, model.Quantity);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("checkout-order")]
        public async Task<IActionResult> CheckoutOrder()
        {
            var userId = HttpContext.GetUserId();
            var command = new CheckoutOrderCommand(userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
