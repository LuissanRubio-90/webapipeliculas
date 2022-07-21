using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;

namespace PeliculasAPITest.PruebasDeIntegracion
{
    [TestClass]
    public class ReviewControllerTests : BasePruebas
    {
        private static readonly string url = "/api/peliculas/1/reviews";

        [TestMethod]
        public async Task ObtenerReviewsDevuelve404PeliculaInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreBD);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);
            Assert.AreEqual(404, (int)respuesta.StatusCode);
        }

        [TestMethod]
        public async Task ObtenerReviewDevuelveListadoVacio()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var context = ConstruirContext(nombreBD);

            context.Peliculas.Add(new Pelicula() { Titulo = "Pelicula 1"});
            context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var reviews = JsonConvert.DeserializeObject<List<ReviewDTO>>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual(0, reviews.Count);
        }
    }
}
