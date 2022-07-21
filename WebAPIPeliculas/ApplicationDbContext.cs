using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using System.Security.Claims;
using WebAPIPeliculas.Entidades;

namespace WebAPIPeliculas
{
    public class ApplicationDbContext : IdentityDbContext //Heredando de IdentityDbContext (manejo de usuarios)
    {
        //Constructor de la clase ApplicationBdContext
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        //Apifluente para determinar las llaves foraneas de las entidades PeliculasGeneros y PeliculasActores
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasGeneros>()
                .HasKey(x => new {x.PeliculaId, x.GeneroId}); //Definicion de llave foranea par PeliculasGeneros

            modelBuilder.Entity<PeliculasActores>()
                 .HasKey(x => new { x.PeliculaId, x.ActorId }); //Definicion de llave foranea par PeliculasActores

            modelBuilder.Entity<PeliculasSalasDeCine>()
                .HasKey(x => new { x.PeliculaId, x.SalaDeCineId}); //Definicion de la llave foranea par PeliculasSalasDeCine

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        //Agregando Datos quemados
        private void SeedData(ModelBuilder modelBuilder)
        {
            var rolAdminId = "f6533ab4-ddf0-4e9a-b74a-e80e78887f2e";
            var usuarioAdminId = "aa2ce5ce-7744-47c1-bdf4-8a2bd01f5ddc";

            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();

            var username = "luisrubio_dev90@outlook.com";

            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = username,
                NormalizedUserName = username,
                Email = username,
                NormalizedEmail = username,
                PasswordHash = passwordHasher.HashPassword(null, "Le29R040o90")
            };

            modelBuilder.Entity<IdentityUser>()
                .HasData(usuarioAdmin);

            modelBuilder.Entity<IdentityRole>()
                .HasData(rolAdmin);

            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>()
                {
                    Id = 1,
                    ClaimType = ClaimTypes.Role,
                    UserId = usuarioAdminId,
                    ClaimValue = "Admin"
                });

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            base.OnModelCreating(modelBuilder);
        }

        //Agregando DbSet por cada clase de la carpeta Entidades
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<SalaDeCine> SalasDeCine { get; set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
        public DbSet<PeliculasActores> PeliculasActores { get; set; }
        public DbSet<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
