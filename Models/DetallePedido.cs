using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace CentroDeCopias.Models
{
    public class DetallePedido
    {
        [Key]
        public int id_detalle { get; set; }

        public int id_pedido { get; set; }

        public int id_producto { get; set; }
        public Producto? Producto { get; set; }

        public int? id_documento { get; set; }
        public Documento? Documento { get; set; }
       
        [Required]
        public int cantidad { get; set; }
        [Required]
        [Precision(12, 2)]
        public decimal precio { get; set; }

    
    }
    
}