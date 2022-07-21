using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;
using WebAPIPeliculas.Helpers;
using WebAPIPeliculas.Servicios;
using System.Linq.Dynamic.Core;

namespace WebAPIPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController :CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context,
                                   IMapper mapper,
                                   IAlmacenadorArchivos almacenadorArchivos,
                                   ILogger<PeliculasController> logger):base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }
                 
        //Endpoint para obtener todas las peliculas de la entidad Pelicula
        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(x => x.FechaEstreno > hoy)
                .OrderBy(x => x.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var enCines = await context.Peliculas
                .Where(x => x.EnCines)
                .Take(top)
                .ToListAsync();

            //Filtro para los proximos estrenos
            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

            return resultado;
        }

        //Endpoint para filtrar peliculas
        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }

            if (filtroPeliculasDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }

            if (filtroPeliculasDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                    .Contains(filtroPeliculasDTO.GeneroId));
            }

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculasDTO.OrdenAscendente ? "ascending" : "descending";
                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculasDTO.CampoOrdenar} {tipoOrden}");
                }
                catch (Exception ex)
                {

                    logger.LogError(ex.Message, ex);
                }
                
                //if (filtroPeliculasDTO.CampoOrdenar == "titulo")
                //{
                //    if (filtroPeliculasDTO.OrdenAscendente)
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderBy(x => x.Titulo);
                //    }
                //    else
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderByDescending(x => x.Titulo);
                //    }
                //}
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable,
                filtroPeliculasDTO.CantidadRegistrosPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);


        }

        //Endpoint para obtener peliculas por ID
        [HttpGet ("{id}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasActores)
                .ThenInclude(x => x.oActor)
                .Include(x => x.PeliculasGeneros)
                .ThenInclude(x => x.oGenero)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

            return mapper.Map<PeliculaDetallesDTO>(pelicula);
        }

        //Endpoint para guardar registro de peliculas en la entidad Pelicula
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

            //Implementando los servicios de almacenamiento en azure
            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido,
                                                                            extension,
                                                                            contenedor,
                                                                            peliculaCreacionDTO.Poster.ContentType);
                }
            }
            AsignarOrdenActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);

        }

        //Endpoint para modificar el registro de peliculas en la entidad Pelicula
        [HttpPut ("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaDb = await context.Peliculas
                .Include(x => x.PeliculasGeneros)
                .Include(x => x.PeliculasActores)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaDb == null)
            {
                return NotFound();
            }

            peliculaDb = mapper.Map(peliculaCreacionDTO, peliculaDb);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    peliculaDb.Poster = await almacenadorArchivos.EditarArchivo(contenido,
                                                                            extension,
                                                                            contenedor,
                                                                            peliculaDb.Poster,
                                                                            peliculaCreacionDTO.Poster.ContentType);
                }
            }

            AsignarOrdenActores(peliculaDb);
            await context.SaveChangesAsync();

            return NoContent();
        }

        //Endpoint para patch
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            //if (patchDocument == null) { return BadRequest(); }

            //var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            //if (entidadDB == null) { return NotFound(); }

            //var entidadDTO = mapper.Map<PeliculaPatchDTO>(entidadDB);
            //patchDocument.ApplyTo(entidadDTO, ModelState);

            //var esValido = TryValidateModel(entidadDTO);

            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}

            //mapper.Map(entidadDTO, entidadDB);
            //await context.SaveChangesAsync();
            //return NoContent();
            return await Patch<Pelicula, PeliculaPatchDTO>(id, patchDocument);
        }

        //Endpoint para eliminar registros en la entidad Pelicula
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            //var existe = await context.Peliculas.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Pelicula() { Id = id });
            //await context.SaveChangesAsync();

            //return NoContent();
            return await Delete<Pelicula>(id);
        }

        //Endpoint para asignar orden de actores
        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }
    }
}
