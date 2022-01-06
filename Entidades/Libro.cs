namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public List<AutorLibro> AutorLibro { get; set; }
        
    }
}
