namespace WebApiAutores.DTO
{
    // Donde el generico T herede de Recurso
    public class ColeccionRecurso<T> : Recurso where T : Recurso
    {
        public List<T> Valores { get; set; }
    }
}
