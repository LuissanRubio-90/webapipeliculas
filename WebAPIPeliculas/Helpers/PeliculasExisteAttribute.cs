using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace WebAPIPeliculas.Helpers
{
    public class PeliculasExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext dbcontext;

        public PeliculasExisteAttribute(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];

            if (peliculaIdObject == null)
            {
                return;
            }

            var peliculaId = int.Parse(peliculaIdObject.ToString());
            var existePelicula = await dbcontext.Peliculas.AnyAsync(x => x.Id == peliculaId);

            if (!existePelicula)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }
        }
    }
}
