using System.ComponentModel.DataAnnotations;
using CenterCopy.models;

namespace CentroDeCopias.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(300)]
        public string? Descripcion { get; set; }

        public double Precio { get; set; }

        [MaxLength(50)]
        public string? Categoria { get; set; }

        // Relación 1 a 1 con Stock
        public Stock? Stock { get; set; }

        // Relación 1 a N con Detalle_Pedido
        public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    }
}