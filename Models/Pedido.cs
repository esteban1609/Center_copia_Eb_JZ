using System.ComponentModel.DataAnnotations;
namespace CenterCopy.models
{
    public class Pedido
    {
        [Key]
        public int id_pedido { get; set; }

        [Required(ErrorMessage="la fecha es obligatoria")]
        public DateTime fecha { get; set; }
        
        public string estado { get; set; }
        
        public int id_Usuario { get; set; }

        //mostrar el nombre en lugar del id
        public string nombreusuario{get;set;}

        [Required]
        public double total { get; set; }
    }
}