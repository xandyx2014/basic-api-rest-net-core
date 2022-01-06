namespace WebApiAutores.DTO
{
    public class PaginacionDto
    {
        public int Pagina { get; set; }
        private int recordPorPagina = 10;
        private readonly int cantidadMaximaPorPagina = 50;
        public int RecordsPorPagina
        {
            get
            {
                return recordPorPagina;
            }
            set
            {
                recordPorPagina = value > cantidadMaximaPorPagina ? cantidadMaximaPorPagina : value;
            }
        }
    }
}
