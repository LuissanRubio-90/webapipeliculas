using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;

namespace WebAPIPeliculas.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            //Mapeando los usuarios
            CreateMap<IdentityUser, UsuarioDTO>();

            //Mapeando las entidades Genero, GeneroDTO y viceversa
            CreateMap<Genero, GeneroDTO>().ReverseMap();

            //Mapeando las entidades GeneroCreacionDTO y Genero para la insercion de datos
            CreateMap<GeneroCreacionDTO, Genero>();

            //Mapeando las entidades Actor y ActorDTO y viceversa
            CreateMap<Actor, ActorDTO>().ReverseMap();

            //Mapeando las entidades ActorCreacionDTO y Actor para la insercion de datos
            CreateMap<ActorCreacionDTO, Actor>().ForMember(x=>x.Foto, options=>options.Ignore());

            //Mapeando las entidades ActorPatchDTO  y Actor / viceversa para el funcionamiento del patch
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            //Mapeando las entidades Pelicula, PeliculaDTO y viceversa
            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();

            //Mapeando las entidades PeliculaCreacionDTO y Pelicula para la insersion de datos
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x=>x.Poster, options=>options.Ignore())
                .ForMember(x=>x.PeliculasGeneros, options=>options.MapFrom(MapPeliculasGeneros))
                .ForMember(x=>x.PeliculasActores, options=>options.MapFrom(MapPeliculasActores));

            //Mapeando las entidades PeliculaPatchDTO Y Pelicula / viceversa para el funcionamiento del patch
            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();

            //Mapeando las entidades Pelicula y PeliculaDetalleDTO
            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));

            //Mapeando las entidades SalaDeCine, SalaDeCineDTO con latitud y longitud
            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember(x => x.Latitud, x => x.MapFrom(y => y.Ubicacion.Y))
                .ForMember(x => x.Longitud, x => x.MapFrom(y => y.Ubicacion.X));


            //Mapeando las entidades SalaDeCineDTO, SalaDeCine con latitud y longitud
            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));

            //Mapeando las entidades Sala SalaDeCineCreacionDTO, SalaDeCine
            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));

            //Mapeando las entidades Review y ReviewDTO
            CreateMap<Review, ReviewDTO>()
                .ForMember(x => x.NombreUsuario, x => x.MapFrom(y => y.oUsuario.UserName));

            CreateMap<ReviewDTO, Review>();

            CreateMap<ReviewCreacionDTO, Review>();

        }

        //Obteniendo la lista de generos en la entidad Peliculas
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIDs == null)
            {
                return resultado;
            }
            foreach(var id in peliculaCreacionDTO.GenerosIDs)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }
            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores == null)
            {
                return resultado;
            }
            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }
            return resultado;
        }

        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<GeneroDTO>();

            if (pelicula.PeliculasGeneros == null) { return resultado; }

            foreach (var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.oGenero.Nombre});
            }

            return resultado;
        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();

            if (pelicula.PeliculasActores == null) { return resultado; }

            foreach (var actorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO() { 
                    ActorId = actorPelicula.ActorId, 
                    Personaje = actorPelicula.Personaje, 
                    NombrePersona = actorPelicula.oActor.Nombre });
            }

            return resultado;
        }
    }
}
