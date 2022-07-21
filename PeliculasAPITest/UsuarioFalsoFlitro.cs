using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPITest
{
    public class UsuarioFalsoFlitro : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Email, "example@hotmail.com"),
                new Claim(ClaimTypes.Name, "example@hotmail.com"),
                new Claim(ClaimTypes.NameIdentifier, "76b93e77-606d-4321-8c18-80c5ecafd99e")

            }, "prueba"));

            await next();
        }
    }
}
