using System.ComponentModel.DataAnnotations;
using CentroDeCopias.Models;

namespace CentroDeCopias.Models
{
    public class Usuario
    {
        [Key]
        public int id_Usuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "El Email no es valido")]
        [EmailAddress(ErrorMessage = "El Email no es válido")]
        [Display(Name = "Email")]
        public string email { get; set; } = string.Empty;


        [Display(Name = "Clave")]
        [Required(ErrorMessage = "la contraseña es obligatoria")]
        public string contrasena { get; set; } = string.Empty;

        [Display(Name = "Avatar")]
        public string? Avatar { get; set; }


        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Cliente"; //"Admin,Cliente,Escuela"

        // Un usuario puede realizar varios pedidos
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        // Un usuario puede subir varios documentos
        public ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    }
}