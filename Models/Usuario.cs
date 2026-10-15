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
        public string nombre { get; set; }=string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido")]
        public string apellido { get; set; }=string.Empty;

        [Required(ErrorMessage = "El Email es Obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo valido")]
        [Display(Name = "Email")]
        public string email { get; set; } = string.Empty;


        [Display(Name = "Clave")]
        [Required(ErrorMessage = "la contraseña es obligatoria")]
        [MinLength(8,ErrorMessage ="la contraseña debe tener minimo 8 caracteres")]
        public string contrasena { get; set; } = string.Empty;

        [Display(Name = "Avatar")]
        public string? Avatar { get; set; }


        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Cliente"; //"Admin,Cliente,Escuela"

        //estado del usuario
        public bool activo{get;set;}=true;

        // Un usuario puede realizar varios pedidos
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        // Un usuario puede subir varios documentos
        public ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    }
}