namespace WebApiAutores.DTO
{
    public class DatoHATEOS
    {
        public string Enlance { get; private set; }
        public string Descripcion { get; private set; }
        public string Metodo { get; private set; }
        public DatoHATEOS(string enlance, string descripcion, string metodo)
        {
            Enlance = enlance;
            Descripcion = descripcion;
            Metodo = metodo;
        }



    }
}
