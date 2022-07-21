
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIPeliculas.Controllers;
using WebAPIPeliculas.DTOs;
using WebAPIPeliculas.Entidades;
using WebAPIPeliculas.Servicios;

namespace PeliculasAPITest.UnitTests
{
    [TestClass]
    public class ActoresControllerTest : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerPersonasPaginadas()
        {
            var  nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            contexto.Actores.Add(new Actor() { Nombre = "Actor 1" });
            contexto.Actores.Add(new Actor() { Nombre = "Actor 2" });
            contexto.Actores.Add(new Actor() { Nombre = "Actor 3" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);

            var controller = new ActoresController(contexto2, mapper, null);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina1 = await controller.Get(new PaginacionDTO() { Pagina = 1, CantidadRegistrosPorPagina = 2 });
            var actoresPagina1 = pagina1.Value;

            Assert.AreEqual(2, actoresPagina1.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina2 = await controller.Get(new PaginacionDTO() { Pagina = 2, CantidadRegistrosPorPagina = 2 });
            var actoresPagina2 = pagina2.Value;

            Assert.AreEqual(1, actoresPagina2.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina3 = await controller.Get(new PaginacionDTO() { Pagina = 3, CantidadRegistrosPorPagina = 2 });
            var actoresPagina3 = pagina3.Value;

            Assert.AreEqual(0, actoresPagina3.Count);
        }

        [TestMethod]
        public async Task CrearActorSinFoto()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var actor = new ActorCreacionDTO() { Nombre = "Luis Rubio", FechaNacimiento = DateTime.Now };

            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(null, null, null, null))
                .Returns(Task.FromResult("url"));

            var controller = new ActoresController(contexto, mapper, mock.Object);
            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.AreEqual(201, resultado.StatusCode);

            var contexto2 = ConstruirContext(nombreDB);
            var listado = await contexto2.Actores.ToListAsync();
            Assert.AreEqual(1, listado.Count);
            Assert.IsNull(listado[0].Foto);

            Assert.AreEqual(0, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CrearActorConFoto()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var content = Encoding.UTF8.GetBytes("Imagen de prueba");
            var archivo = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");
            archivo.Headers = new HeaderDictionary();
            archivo.ContentType = "image/jpg";

            var actor = new ActorCreacionDTO()
            {
                Nombre = "nuevo actor",
                FechaNacimiento = DateTime.Now,
                Foto = archivo
            };

            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(content, ".jpg", "actores", archivo.ContentType))
                .Returns(Task.FromResult("url"));

            var controller = new ActoresController(contexto, mapper, mock.Object);
            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;

            Assert.AreEqual(201, resultado.StatusCode);

            var contexto2 = ConstruirContext(nombreDB);
            var listado = await contexto2.Actores.ToListAsync();
            Assert.AreEqual(1, listado.Count);
            Assert.AreEqual("url", listado[0].Foto);
            Assert.AreEqual(1, mock.Invocations.Count);

        }

        [TestMethod]
        public async Task PatchRetorna404SiActorNoExiste()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new ActoresController(contexto, mapper, null);
            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            var respuesta = await controller.Patch(1, patchDoc);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);

        }

        [TestMethod]
        public async Task PatchActualizarUnSoloCampo()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var fechaNacimiento = DateTime.Now;
            var actor = new Actor() { Nombre = "Ronald Alexander", FechaNacimiento = fechaNacimiento};
            contexto.Add(actor);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var controller = new ActoresController(contexto2, mapper, null);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            controller.ObjectValidator = objectValidator.Object;

            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            patchDoc.Operations.Add(new Operation<ActorPatchDTO>("replace", "/nombre", null, "Veronica Abigail"));

            var respuesta = await controller.Patch(1, patchDoc);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var actorDB = await contexto3.Actores.FirstAsync();
            Assert.AreEqual("Veronica Abigail", actorDB.Nombre);
            Assert.AreEqual(fechaNacimiento, actorDB.FechaNacimiento);


        }
    }
}
