using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;
using WebAPIPeliculas.Helpers;

namespace WebAPIPeliculas.Controllers
{
    [Route("api/peliculas/{peliculaId:int}/reviews")]
    [ServiceFilter(typeof(PeliculasExisteAttribute))]
    public class ReviewController: CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ReviewController(ApplicationDbContext context,
                                IMapper mapper)
            : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int peliculaId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Reviews.Include(x => x.oUsuario).AsQueryable();
            queryable = queryable.Where(x => x.PeliculaId == peliculaId);

            return await Get<Review, ReviewDTO>(paginacionDTO, queryable);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int peliculaId, [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            var existePelicula = await context.Peliculas.AnyAsync(x => x.Id == peliculaId);

            if (!existePelicula)
            {
                return NotFound();
            }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var reviewExiste = await context.Reviews.AnyAsync(x => x.PeliculaId == peliculaId && x.UsuarioId == usuarioId);

            if (reviewExiste)
            {
                return BadRequest("El usuario ya ha escrito un review de esta pelicula");
            }

            var review = mapper.Map<Review>(reviewCreacionDTO);
            review.PeliculaId = peliculaId;
            review.UsuarioId = usuarioId;

            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();


        }

        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int reviewId, [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            var reviewDb = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);

            if (reviewDb == null)
            {
                return NotFound();
            }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (reviewDb.UsuarioId != usuarioId)
            {
                return BadRequest("No tiene permisos de editar este review");
            }

            reviewDb = mapper.Map(reviewCreacionDTO, reviewDb);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var reviewDb = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);

            if(reviewDb == null) { return NotFound();  }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if(reviewDb.UsuarioId != usuarioId) { return Forbid();  }

            context.Remove(reviewDb);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
