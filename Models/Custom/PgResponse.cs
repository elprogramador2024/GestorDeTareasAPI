namespace GestorDeTareas.Models.Custom
{
    public class PgResponse
    {
        public int Total { get; set; }
        public int Pgnum { get; set; }
        public int Pgsize { get; set; }
        public int Totpages { get; set; }
        public List<Tarea> Tareas { get; set; }
    }
}
