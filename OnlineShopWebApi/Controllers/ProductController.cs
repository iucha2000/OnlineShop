using Application.AdminPanel.Models;
using Application.Comments.Commands;
using Application.Comments.Models;
using Application.Product.Queries;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApi.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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


        [HttpPost("add-comment-to-product")]
        public async Task<IActionResult> AddComment(AddCommentModel model)
        {
            var userId = HttpContext.GetUserId();

            var command = new AddCommentToProductCommand(userId, model.Message, model.Rating, model.ProductId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("delete-comment")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var userId = HttpContext.GetUserId();

            var command = new DeleteCommentCommand(userId, commentId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("edit-comment")]
        public async Task<IActionResult> EditComment(EditCommentModel model)
        {
            var userId = HttpContext.GetUserId();

            var command = new EditCommentCommand(userId, model.CommentId, model.Message, model.Rating);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
