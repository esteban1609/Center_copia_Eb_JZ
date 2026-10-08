using System.ComponentModel.DataAnnotations;
namespace CenterCopy.models
{
    public class DetallePedido
    {
        [Key]
        public int id_detalle { get; set; }

        public int id_pedido { get; set; }

        public int id_producto { get; set; }

        public int id_documento { get; set; }
        [Required]
        public int cantidad { get; set; }
        [Required]
        public double precio { get; set; }
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