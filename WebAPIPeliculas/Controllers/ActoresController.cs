using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;
using WebAPIPeliculas.Helpers;
using WebAPIPeliculas.Servicios;

namespace WebAPIPeliculas.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : CustomBaseController //Heredando del controlador CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        //Inyeccion de dependencias para la entidad Actores
        public ActoresController(ApplicationDbContext context,
                                 IMapper mapper,
                                 IAlmacenadorArchivos almacenadorArchivos)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        //Endpoint para mostrar los registros de la entidad Actor
        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            //Retorna el endpoint centralizado en CustomBaseController
            return await Get<Actor, ActorDTO>(paginacionDTO);
        }

        //Endpoint para mostrar el registro de la entidad Actor por ID

        [HttpGet("{id}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            //Retorna el endpoint centralizado en CustomBaseController
            return await Get<Actor, ActorDTO>(id);
        }

        //Endpoint para agregar un registro a la entidad Actor
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var entidad = mapper.Map<Actor>(actorCreacionDTO);

            //Implementando los servicios de almacenamiento en azure
            if (actorCreacionDTO.Foto != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    entidad.Foto = await almacenadorArchivos.GuardarArchivo(contenido, 
                                                                            extension, 
                                                                            contenedor,
                                                                            actorCreacionDTO.Foto.ContentType);
                }
            }

            context.Add(entidad);
            await context.SaveChangesAsync();
            var dto = mapper.Map<ActorDTO>(entidad);

            return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
        }

        //Endpoint para modificar un registro a la entidad Actor
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actorDb = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDb == null)
            {
                return NotFound();
            }

            actorDb = mapper.Map(actorCreacionDTO, actorDb);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorDb.Foto = await almacenadorArchivos.EditarArchivo(contenido,
                                                                            extension,
                                                                            contenedor,
                                                                            actorDb.Foto,
                                                                            actorCreacionDTO.Foto.ContentType);
                }
            }


            //var entidad = mapper.Map<Actor>(actorCreacionDTO);
            //entidad.Id = id;
            //context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        //EndPoint para Patch
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
        }

        //Endpoint para eliminar un registro de la entidad Actor
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
        }
    }
}
