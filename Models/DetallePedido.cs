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
        [Required]
        public int copias { get; set; }
        [Required]
        public bool color { get; set; }
        [Required]
        public bool doble_faz { get; set; }
        [Required]
        public string tamano_hoja { get; set; }
        [Required]
        public string anillado { get; set; }
    }
    
}