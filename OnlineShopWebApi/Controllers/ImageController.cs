using Application.Images.Commands;
using Application.Images.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApi.Controllers
{
    public class ImageController : BaseController
    {
        public ImageController(ILoggerFactory logger, ISender mediator) : base(logger, mediator)
        {
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadImageModel model)
        {
            var command = new UploadImageCommand(model.File,model.ProductId);

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
