using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;

namespace WebAPIPeliculas.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GeneroController:CustomBaseController //Hereda del controlador CustomBaseController
    {
        //Construyendo inyeccion de dependencias
        public GeneroController(ApplicationDbContext context,
                                IMapper mapper)
            :base(context, mapper) //Definiendo la base para la implementacion de los endpoints genericos
        {

        }

        //Endpoint para obtener listado de Generos
        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            //Retorna el endpoint centralizado, definido en CustomBaseController
            return await Get<Genero, GeneroDTO>();
        }

        //Endpoint para obtener Genero por ID
        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            //Retorna el endpoint centralizado, definido en CustomBaseController
            return await Get<Genero, GeneroDTO>(id);
        }

        //Endpoint para agregar Genero a la BD
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            //Retorna el endpoint centralizado, definido en CustomBaseController
            return await Post<GeneroCreacionDTO, Genero, GeneroDTO>(generoCreacionDTO, "obtenerGenero");
        }

        //EndPoint para actualizar registros en la entidad Genero
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            //Retorna el endpoint centralizado, definido en CustomBaseController
            return await Put<GeneroCreacionDTO, Genero>(id, generoCreacionDTO);
        }

        //Endpoint para eliminar registros de la entidad Genero
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            //Retorna el endpoint centralizado, definido en CustomBaseController
            return await Delete<Genero>(id);
        }

        //NOTA: Centralizar codigo en un controlador base es efectivo en funciones de CRUD
    }
}
