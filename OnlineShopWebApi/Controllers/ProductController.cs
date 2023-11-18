using MediatR;

namespace OnlineShopWebApi.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(/*ILogger logger,*/ ISender mediator) : base(/*logger,*/ mediator)
        {

        }

    }
}
