using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIPeliculas.Controllers;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;

namespace PeliculasAPITest.UnitTests
{
    [TestClass]
    public class PeliculasControllerTest : BasePruebas
    {
        private string CrearDataPrueba()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = ConstruirContext(databaseName);
            var genero = new Genero() { Nombre = "Genero 1" };

            var peliculas = new List<Pelicula>()
            {
                new Pelicula(){Titulo = "Pelicula 1", FechaEstreno = new DateTime(2010, 1, 1), EnCines = false},
                new Pelicula(){Titulo = "No estrenada", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false},
                new Pelicula(){Titulo = "Pelicula en cines", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true}

            };

            var peliculaConGenero = new Pelicula()
            {
                Titulo = "Pelicula con Genero",
                FechaEstreno = new DateTime(2010, 1, 1),
                EnCines = false
            };

            peliculas.Add(peliculaConGenero);

            context.Add(genero);
            context.AddRange(peliculas);
            context.SaveChanges();

            var peliculaGenero = new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
            context.Add(peliculaGenero);
            context.SaveChanges();

            return databaseName;
        }

        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            var nombreDB = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDB);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var tituloPelicula = "Pelicula 1";

            var filtroDTO = new FiltroPeliculasDTO()
            {
                Titulo = tituloPelicula,
                CantidadRegistrosPorPagina = 10
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual(tituloPelicula, peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreDB = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDB);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                EnCines = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Pelicula en cines", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            var nombreDB = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDB);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                ProximosEstrenos = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("No estrenada", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            var nombreDB = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDB);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos.Select(x => x.Id).First();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                GeneroId = generoId
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Pelicula con Genero", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarOrdenarTituloAscendente()
        {
            var nombreDB = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDB);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "titulo",
                OrdenAscendente = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDB);
            var peliculasDB = contexto2.Peliculas.OrderBy(x => x.Titulo).ToList();

            Assert.AreEqual(peliculasDB.Count, peliculas.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
            }


        }

        [TestMethod]
        public async Task FiltrarOrdenarTituloDescendente()
        {
            var nombreDB = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDB);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "titulo",
                OrdenAscendente = false
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDB);
            var peliculasDB = contexto2.Peliculas.OrderByDescending(x => x.Titulo).ToList();

            Assert.AreEqual(peliculasDB.Count, peliculas.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
            }


        }

        [TestMethod]
        public async Task FiltrarPorCampoIncorrectoDevuelvePeliculas()
        {
            var nombreDB = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDB);

            var mock = new Mock<ILogger<PeliculasController>>();

            var controller = new PeliculasController(contexto, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "abc",
                OrdenAscendente = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDB);
            var peliculasDB = contexto2.Peliculas.ToList();
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
    }
}
