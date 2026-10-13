using System.ComponentModel.DataAnnotations;
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

        // Relación 1 a 1 con Stock
        public Stock? Stock { get; set; }

        // Relación 1 a N con Detalle_Pedido
        public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    }
}