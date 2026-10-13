using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CentroDeCopias.Models
{
    public class Pedido
    {
        [Key]
        public int id_pedido { get; set; }

        [Required(ErrorMessage = "la fecha es obligatoria")]
        public DateTime fecha { get; set; }

        public string estado { get; set; }

        public int id_Usuario { get; set; }

        // Relación con Usuario
        public Usuario Usuario { get; set; } = null!;

        //mostrar el nombre en lugar del id
        [NotMapped]
        public string nombreusuario => Usuario?.nombre ?? string.Empty;

        [Required]
        public decimal total { get; set; }
    }
}