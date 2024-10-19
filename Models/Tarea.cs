using GestorDeTareas.Models.Custom;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeTareas.Models
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        [Required]
        public string UserName { get; set; }
        
        [NotMapped]
        [ValidEnumDataType(typeof(EstadoTarea))]
        public EstadoTarea Estado { get; set; }

        [Column("Estado")]
        public string EstadoString
        {
            get { return Estado.ToString(); }
            private set { Estado = (EstadoTarea)Enum.Parse(typeof(EstadoTarea), value); }
        }
    }

    public enum EstadoTarea
    {
        PENDIENTE,
        ENPROCESO,
        COMPLETADA
    }

}
