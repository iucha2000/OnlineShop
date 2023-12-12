using Application.Product.Queries;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApi.Extensions;

namespace OnlineShopWebApi.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(ILoggerFactory logger, ISender mediator) : base(logger, mediator)
        {

        }

        [HttpGet("products")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllProducts()
        {
            var userId = HttpContext.GetUserId();

            var query = new GetAllProductsQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("get-product-by-product-id/{productId}")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProductsByProductId(int productId)
        {
            var userId = HttpContext.GetUserId();

            var query = new GetProductsByProductIdQuery(productId, userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
