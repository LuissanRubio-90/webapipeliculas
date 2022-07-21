using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
    public class ReviewsControllerTest : BasePruebas
    {
        private void CrearPeliculas(string nombreDB)
        {
            var contexto = ConstruirContext(nombreDB);
            contexto.Peliculas.Add(new Pelicula() { Titulo = "pelicula 1" });
            contexto.SaveChanges();
        }

        [TestMethod]
        public async Task UsuarioNoPuedeCrearDosReviewsParaLaMismaPelicula()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaId = contexto.Peliculas.Select(x => x.Id).First();
            var review1 = new Review() { PeliculaId = peliculaId, UsuarioId = usuarioPorDefectoId, Puntuacion = 5};

            contexto.Add(review1);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new ReviewController(contexto2, mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var reviewCreacionDTO = new ReviewCreacionDTO() { Puntuacion = 5};
            var respuesta = await controller.Post(peliculaId, reviewCreacionDTO);

            var valor = respuesta as IStatusCodeActionResult;
            Assert.AreEqual(400, valor.StatusCode.Value);
        }

        [TestMethod]
        public async Task CrearReview()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaId = contexto.Peliculas.Select(x => x.Id).First();
            var contexto2 = ConstruirContext(nombreDB);

            var mapper = ConfigurarAutoMapper();
            var controller = new ReviewController(contexto2, mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var reviewCreacionDTO = new ReviewCreacionDTO() { Puntuacion = 5 };
            var respuesta = await controller.Post(peliculaId, reviewCreacionDTO);

            var valor = respuesta as NoContentResult;
            Assert.IsNotNull(valor);

            var contexto3 = ConstruirContext(nombreDB);
            var reviewDB = contexto3.Reviews.First();
            Assert.AreEqual(usuarioPorDefectoId, reviewDB.UsuarioId);


        }

    }
}
