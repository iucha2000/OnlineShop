using Application.Order.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApi.Controllers
{
    public class OrderController : BaseController
    {
        public OrderController(/*ILogger logger,*/ ISender mediator) : base(/*logger,*/ mediator)
        {
        }

        [HttpPost("add-product-to-order")]
        public async Task<IActionResult> AddProductToOrder(AddProductToOrderModel model)
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
