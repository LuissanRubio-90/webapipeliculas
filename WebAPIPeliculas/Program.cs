using WebAPIPeliculas;

var builder = WebApplication.CreateBuilder(args);

//Instancia de la clase Startup
var startup = new Startup(builder.Configuration);

//Metodo de la clase Startup - ConfigureServices
startup.ConfigureServices(builder.Services);

var app = builder.Build();

var servicioLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));

startup.Configure(app, app.Environment, servicioLogger);

app.MapControllers();
app.Run();
