using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentroDeCopias.Models;
using Microsoft.EntityFrameworkCore;

namespace CentroDeCopias.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(300)]
        public string? Descripcion { get; set; }

        [Precision(12, 2)]
        public decimal Precio { get; set; }

        [MaxLength(50)]
        public string? Categoria { get; set; }
        public string? RutaImagen { get; set; }
        
        [NotMapped]
        public IFormFile? Imagen {get; set;}
        //baja logica
        public bool activo {get;set;}=true;

        // Relación 1 a 1 con Stock
        public Stock? Stock { get; set; }

        // Relación 1 a N con Detalle_Pedido
        public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    }
}