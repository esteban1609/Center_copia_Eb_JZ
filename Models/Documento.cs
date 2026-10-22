using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentroDeCopias.Models;

namespace CentroDeCopias.Models
{
    public class Documento
    {

        [Key]
        public int IdDocumento { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; }


        public DateTime FechaSubida { get; set; } = DateTime.Now;

        public int IdPedido{get; set; }

        public Pedido pedido {get; set;} = null!;

        // FK hacia Usuarios
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        // Relación 1 a N con Detalle_Pedido
        public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();

        // Archivo que sube el usuario
        [NotMapped]
        public IFormFile? Archivo { get; set; }

        public string RutaArchivo { get; set; } = string.Empty;

        public int Copias { get; set; } = 1;
        public bool Color { get; set; }
        public bool DobleFaz { get; set; }
        public string TamanoHoja { get; set; } = "A4";
        public string Anillado { get; set; } = "Ninguno";
        public decimal Precio { get; set; }
    }
}