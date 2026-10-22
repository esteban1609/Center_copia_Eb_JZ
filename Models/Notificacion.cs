
using System.ComponentModel.DataAnnotations;

namespace CentroDeCopias.Models
{
    public class Notificacion
    {
        [Key]
        public int IdNotificacion { get; set; }

        [Required]
        public int id_Usuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Tipo { get; set; } = "Info";

        public bool Leida { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Pedido relacionado (opcional)
        public int? id_pedido { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; } = null!;

        public Pedido? Pedido { get; set; }
    }
}
