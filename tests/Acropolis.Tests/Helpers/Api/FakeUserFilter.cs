using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Acropolis.Tests.Helpers.Api
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        public readonly List<Claim> Claims;

        public FakeUserFilter(List<Claim> claims)
        {
            Claims = claims;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(Claims));
            await next();
        }
    }
}
