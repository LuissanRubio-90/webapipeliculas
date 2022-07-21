using System.ComponentModel.DataAnnotations;

namespace WebAPIPeliculas.Validaciones
{
    public class PesoArchivoValidacion : ValidationAttribute
    {
        private readonly int pesoMaximoEnMb;

        public PesoArchivoValidacion(int pesoMaximoEnMb)
        {
            this.pesoMaximoEnMb = pesoMaximoEnMb;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > pesoMaximoEnMb * 1024 * 1024)
            {
                return new ValidationResult($"El peso maximo del arhivo no debe de ser mayor a {pesoMaximoEnMb}mb");
            }

            return ValidationResult.Success;

        }
    }
}
