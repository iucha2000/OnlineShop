using Domain.Enums;
using System.Security.Claims;

namespace OnlineShopWebApi.Extensions
{
    public static class UserClaims
    {
        public static Guid GetUserId(this HttpContext context)
        {
            var idAsString = context.User.Claims.ToList().FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier)?.Value;

            var id = string.IsNullOrEmpty(idAsString) ? Guid.Empty : Guid.Parse(idAsString);

            return id;
        }
    }
}
