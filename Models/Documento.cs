using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CenterCopy.models;

namespace CentroDeCopias.Models
{
    public class Documento
    {
        public int IdDocumento { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; }


        public DateTime FechaSubida { get; set; } = DateTime.Now;

        // FK hacia Usuarios
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        // Relación 1 a N con Detalle_Pedido
        public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();

        // Archivo que sube el usuario
        [NotMapped]
        public IFormFile? Archivo { get; set; }
    }
}