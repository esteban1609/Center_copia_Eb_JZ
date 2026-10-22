
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CentroDeCopias.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CentroDeCopias.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public PerfilController(
            DataContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Perfil/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var idClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out int idUsuario))
                return Challenge();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.id_Usuario == idUsuario && u.activo);

            if (usuario == null)
                return Forbid();

            return View(usuario);
        }

        // POST: Perfil/SubirAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirAvatar(IFormFile? archivo)
        {
            var idClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out int idUsuario))
                return Challenge();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.id_Usuario == idUsuario && u.activo);

            if (usuario == null)
                return Forbid();

            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Seleccioná una imagen.";
                return RedirectToAction(nameof(Index));
            }

            // Tamaño máximo: 2 MB
            if (archivo.Length > 2 * 1024 * 1024)
            {
                TempData["Error"] = "La imagen no puede superar los 2 MB.";

                return RedirectToAction(nameof(Index));
            }

            // Validar el contenido real de la imagen.
            // No confiamos en la extensión enviada por el navegador.
            string? extension = null;

            using (var stream = archivo.OpenReadStream())
            {
                byte[] encabezado = new byte[12];
                int leidos = await stream.ReadAsync(
                    encabezado.AsMemory(0, encabezado.Length));

                if (leidos >= 3 &&
                    encabezado[0] == 0xFF &&
                    encabezado[1] == 0xD8 &&
                    encabezado[2] == 0xFF)
                {
                    extension = ".jpg";
                }
                else if (leidos >= 8 &&
                    encabezado.Take(8).SequenceEqual(
                        new byte[]
                        {
                            137, 80, 78, 71, 13, 10, 26, 10
                        }))
                {
                    extension = ".png";
                }
            }

            if (extension == null)
            {
                TempData["Error"] = "Solo se permiten imágenes JPG, PNG.";

                return RedirectToAction(nameof(Index));
            }

            string carpeta = Path.Combine(_environment.WebRootPath, "avatars");

            Directory.CreateDirectory(carpeta);

            // Nombre aleatorio para evitar sobrescribir archivos
            string nombreArchivo = Guid.NewGuid().ToString("N") + extension;

            string rutaFisica = Path.Combine(carpeta, nombreArchivo);

            // Guardar imagen físicamente
            using (var stream = new FileStream(
                rutaFisica, FileMode.CreateNew))
            {
                await archivo.CopyToAsync(stream);
            }

            // Guardar la ruta de la fotografía anterior
            string? avatarAnterior = usuario.Avatar;

            // Guardar la ruta de la nueva fotografía
            usuario.Avatar = "/avatars/" + nombreArchivo;

            // Actualizar el usuario en MySQL
            await _context.SaveChangesAsync();

            // Eliminar la fotografía anterior
            //esto puede traer problemas al tomar que cada  imagen pertenece a un solo usuario y que todas estan guardadas en wwroot/avatars.
            if (!string.IsNullOrEmpty(avatarAnterior))
            {
                // Obtener solamente el nombre del archivo
                string archivoAnterior = Path.GetFileName(avatarAnterior);

                // Construir la ruta física
                string rutaAnterior = Path.Combine(
                    _environment.WebRootPath,
                    "avatars",
                    archivoAnterior
                );

                // Verificar que exista antes de eliminar
                if (System.IO.File.Exists(rutaAnterior))
                {
                    System.IO.File.Delete(rutaAnterior);
                }
            }

            // Obtener las Claims actuales del usuario
            var claims = User.Claims
                .Where(c => c.Type != "Avatar")
                .ToList();

            // Agregar la nueva foto de perfil
            claims.Add(new Claim(
                "Avatar",
                usuario.Avatar!
            ));

            // Crear una nueva identidad con las Claims actualizadas
            var identidad = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // Crear el principal
            var principal = new ClaimsPrincipal(identidad);

            // Actualizar la cookie de autenticación
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            TempData["Mensaje"] = "Foto de perfil actualizada correctamente.";

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            string nombre,
            string apellido,
            string email)
        {
            // Obtener el ID del usuario autenticado
            var idClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out int idUsuario))
                return Challenge();

            // Buscar al usuario en MySQL
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.id_Usuario == idUsuario && u.activo);

            if (usuario == null)
                return Forbid();

            // Limpiar los datos ingresados
            nombre = nombre?.Trim() ?? "";
            apellido = apellido?.Trim() ?? "";
            email = email?.Trim() ?? "";

            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            // Validar longitud
            if (nombre.Length > 50 ||
                apellido.Length > 50 ||
                email.Length > 100)
            {
                TempData["Error"] = "Los datos superan la longitud permitida.";

                return RedirectToAction(nameof(Index));
            }

            // Validar correo
            var emailValidator =
                new System.ComponentModel.DataAnnotations
                    .EmailAddressAttribute();

            if (!emailValidator.IsValid(email))
            {
                TempData["Error"] = "Ingresá un correo electrónico válido.";

                return RedirectToAction(nameof(Index));
            }

            // Verificar que el correo no esté registrado
            // por otro usuario
            bool existe = await _context.Usuarios.AnyAsync(u =>
                u.email == email &&
                u.id_Usuario != idUsuario);

            if (existe)
            {
                TempData["Error"] = "Este correo ya pertenece a otro usuario.";

                return RedirectToAction(nameof(Index));
            }

            // Actualizar únicamente los campos permitidos
            usuario.nombre = nombre;
            usuario.apellido = apellido;
            usuario.email = email;

            await _context.SaveChangesAsync();

            // Actualizar el nombre y correo de la sesión
            var claims = User.Claims
                .Where(c =>
                    c.Type != ClaimTypes.Name &&
                    c.Type != ClaimTypes.Email)
                .ToList();

            claims.Add(new Claim(
                ClaimTypes.Name,
                usuario.nombre + " " + usuario.apellido));

            claims.Add(new Claim(
                ClaimTypes.Email,
                usuario.email));

            var identidad = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identidad);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            TempData["Mensaje"] = "Información personal actualizada correctamente.";

            return RedirectToAction(nameof(Index));
        }

    }
}
