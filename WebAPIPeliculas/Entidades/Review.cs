using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebAPIPeliculas.Entidades
{
    public class Review: IId
    {
        public int Id { get; set; }
        public string Comentario { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }
        public int PeliculaId { get; set; }
        public Pelicula oPelicula { get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser oUsuario { get; set; }
        
    }
}
