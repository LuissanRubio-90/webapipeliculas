namespace WebAPIPeliculas.Entidades
{
    public class PeliculasSalasDeCine
    {
        public int PeliculaId { get; set; }
        public int SalaDeCineId { get; set; }
        public Pelicula oPelicula { get; set; }
        public SalaDeCine oSalaDeCine { get; set; }
    }
}
