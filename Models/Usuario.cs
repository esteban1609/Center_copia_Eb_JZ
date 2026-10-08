using System.ComponentModel.DataAnnotations;

namespace CenterCopy.models
{
    public class Usuario
    {
        [Key]
        public int id_Usuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [display(name="Nombre")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [display(name="Apellido")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        //[EmailAddress(ErrorMessage = "El Email no es válido")]
        [display(name="Email")]
        public string email { get; set; }=string.empty;


        [display(name="Clave")]
        public string contrasena { get; set; }=string.empty;

        [display(name="Avatar")]
        public string? Avatar { get; set; }


        [display(name="Rol")]
        public string Rol {get; set;}="Cliente" //"Admin,Cliente,Escuela"

    }
}