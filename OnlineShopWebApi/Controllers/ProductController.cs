using Application.Product.Queries;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApi.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(/*ILogger logger,*/ ISender mediator) : base(/*logger,*/ mediator)
        {

        }

        [HttpGet("products")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllProducts()
        {
            await Task.CompletedTask;
            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("get-products-by-category/{category}")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProductsByCategory(ProductCategory category)
        {
            var query = new GetProductsByCategoryQuery(category);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
