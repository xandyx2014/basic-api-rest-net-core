using AutoMapper;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;
namespace WebApiAutores.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Example 
            /*
             CreateMap<Curso, CursoDto>()
            .ForMember(x => x.Instructores, y => y.MapFrom(z => z.InstructoresLink.Select(a => a.Instructor).ToList()))
            .ForMember(x => x.Comentarios, y => y.MapFrom(z => z.ComentarioLista))
            .ForMember(x => x.Precio, y => y.MapFrom(y => y.PrecioPromocion));
            */
            // AutorDto hacia Autor
            CreateMap<AutorDto, Autor>();
            CreateMap<LibroPatchDto, Libro>().ReverseMap();
            CreateMap<Autor, AutorDto>()
                .ForMember(autor => autor.Libros, options => options.MapFrom(MapAutorToAutorDto));
            CreateMap<LibroDto, Libro>()
                // Para la propiedad Autor Libro, Mapea de la sgte Forma
                .ForMember(libro => libro.AutorLibro, options => options.MapFrom(MapAutorLibro));


            CreateMap<Libro, LibroDto>()
                .ForMember(libro => libro.Autores, options => options.MapFrom( e => e.AutorLibro.Select( a=> a.Autor).ToList()));

            CreateMap<Comentario, ComentarioDto>();
            CreateMap<ComentarioDto, Comentario>();
        }
        private List<LibroDto> MapAutorToAutorDto(Autor autor, AutorDto autorDto)
        {
            var result = new List<LibroDto>();
            if (autor.AutorLibro == null) return result;
            autor.AutorLibro.ForEach((autorlibro) => {
                result.Add(new LibroDto() { 
                    Id = autorlibro.LibroId,
                    Titulo = autorlibro.Libro.Titulo
                });
            });
            return result;
        
        }
        private List<AutorDto> MapLibroDtoAutores(Libro libro, LibroDto libroDto)
        {
            var result = new List<AutorDto>();
            if (libro.AutorLibro == null)
            {
                return result;
            }
            libro.AutorLibro.ForEach(autorLibro =>
            {
                AutorDto autor = new()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                };
                result.Add(autor);
            });
            return result;
        }
        private List<AutorLibro> MapAutorLibro(LibroDto libroDto, Libro libro)
        {
            var result = new List<AutorLibro>();

            if (libroDto.AutorId == null)
            {
                return result;
            }
            libroDto.AutorId.ForEach(autor =>
            {
                result.Add(new AutorLibro() { AutorId = autor });
            });
            return result;
        }
    }
}
