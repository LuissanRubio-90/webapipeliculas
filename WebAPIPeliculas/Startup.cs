using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text;
using WebAPIPeliculas.Helpers;
using WebAPIPeliculas.Servicios;

namespace WebAPIPeliculas
{
    public class Startup
    {
        //Inyectando deoendencias de IConfiguration
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration;

        public Action<SqlServerDbContextOptionsBuilder> SqlServerOptions { get; private set; }

        //Metodo para la publicacion de la configuracion de los servicios
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(FiltroErrores));
            })
                .AddNewtonsoftJson(); //Agregando el servicio de NewtonSoftJson para el patch

            //Publicando y configurando servicios para la gestion de usuarios con Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Publicando y configurando servicios JwtBearer para la autenticacion de usuarios
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtkey"])),
                       ClockSkew = TimeSpan.Zero
                   }
                );

            //Publicando servicios para la string de conexion a la base de datos
            services.AddDbContext<ApplicationDbContext>(options => 
                                                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                                        sqlServerOptions => sqlServerOptions.UseNetTopologySuite() //Configuracion para usar la suite de topologia
                                                        ));

            //Configurando AutoMapper
            services.AddAutoMapper(typeof(Startup));

            //Configurando servicios (Interfaz y Clase) para almacenar archivos en azure
            services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

            //Configurando servicios (Interfaz y Clase) para almacenar archivos locales
            services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
            services.AddHttpContextAccessor();
            //Configurando los servicios para el manejo de Ubicaciones
            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

            //Configurando los servicios de filtrado Mvc
            services.AddScoped<PeliculasExisteAttribute>();

            services.AddSingleton(provider =>
                new MapperConfiguration(config =>
                {
                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutoMapperProfiles(geometryFactory));
                }).CreateMapper()
            );
        }

        //Metodo para la publicacion de los middlewares
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
        }
    }
}
