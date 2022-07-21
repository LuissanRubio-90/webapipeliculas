using Microsoft.EntityFrameworkCore;
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
    public class GenerosControllerTests : BasePruebas
    {
        private static readonly string url = "/api/generos";

        [TestMethod]
        public async Task ObtenerTodosLosGenerosListadoVacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync());

            Assert.AreEqual(0, generos.Count());
        }

        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var contexto = ConstruirContext(nombreDB);

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await contexto.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync());

            Assert.AreEqual(2, generos.Count());
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var contexto = ConstruirContext(nombreDB);
            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await contexto.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.DeleteAsync($"{url}/1");
            respuesta.EnsureSuccessStatusCode();

            var contexto2 = ConstruirContext(nombreDB);
            var existe = await contexto2.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }

        //[TestMethod]
        //public async Task BorrarGeneroRetorna401()
        //{
        //    var nombreDB = Guid.NewGuid().ToString();
        //    var factory = ConstruirWebApplicationFactory(nombreDB, ignorarSeguridad: false);

        //    var cliente = factory.CreateClient();
        //    var respuesta = await cliente.DeleteAsync($"{url}/1");
        //    Assert.AreEqual("Unauthorized", respuesta.ReasonPhrase);
        //}
    }
}
