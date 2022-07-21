namespace WebAPIPeliculas.Entidades
{
    public class PeliculasGeneros
    {
        public int GeneroId { get; set; }
        public int PeliculaId { get; set; }
        public Genero oGenero { get; set; }
        public Pelicula oPelicula { get; set; }
    }
}
