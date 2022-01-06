using FluentValidation;

namespace WebApiAutores.DTO
{
    public class LibroDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }

        public List<int> AutorId { get; set; }
        public List<AutorDto> Autores { get; set; }
        public DateTime FechaCreacion { get; set; }
        // public List<ComentarioDto> Comentarios { get; set; }

    }
    public class LibroValidator : AbstractValidator<LibroDto>
    {
        public LibroValidator()
        {
            RuleFor(p => p.Titulo)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(255)
                .WithMessage("El Titulo es requerido");
            RuleFor(p => p.AutorId)
                .NotEmpty()
                .WithMessage("Autores id es necesario");
            
        }
    }
   
}
