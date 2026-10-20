
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CentroDeCopias.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }

        // GET: Auth/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: Auth/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(
            [Bind("nombre,apellido,email,contrasena,Rol")] Usuario datos)
        {
            // Solo se permiten Cliente y Escuela
            if (datos.Rol != "Cliente" &&
                datos.Rol != "Escuela")
            {
                ModelState.AddModelError(
                    "Rol", "Seleccione un tipo de cuenta válido");
            }

            // Verificar que el correo no exista
            bool existe = await _context.Usuarios.AnyAsync(
                u => u.email == datos.email);

            if (existe)
            {
                ModelState.AddModelError(
                    "email", "Este correo ya está registrado");
            }

            if (!ModelState.IsValid)
            {
                return View(datos);
            }

            // Creamos el usuario que guardaremos en MySQL
            var usuario = new Usuario
            {
                nombre = datos.nombre,
                apellido = datos.apellido,
                email = datos.email,
                Rol = datos.Rol,
                activo = true
            };

            // Guardamos un hash, no la contraseña original
            var hasher = new PasswordHasher<Usuario>();

            usuario.contrasena = hasher.HashPassword(
                usuario, datos.contrasena);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Cuenta creada correctamente. Ya podés iniciar sesión.";

            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [Bind("email,contrasena")] Usuario datos)
        {
            // En el Login no necesitamos estos campos
            ModelState.Remove("nombre");
            ModelState.Remove("apellido");

            if (!ModelState.IsValid)
            {
                return View(datos);
            }

            // Buscar usuario por email
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.email == datos.email);

            if (usuario == null)
            {
                ModelState.AddModelError(
                    "",
                    "Correo o contraseña incorrectos"
                );

                return View(datos);
            }

            // Verificar contraseña
            var hasher = new PasswordHasher<Usuario>();

            var resultado = hasher.VerifyHashedPassword(
                usuario,
                usuario.contrasena,
                datos.contrasena
            );

            if (resultado == PasswordVerificationResult.Failed ||
                !usuario.activo)
            {
                ModelState.AddModelError(
                    "",
                    "Correo o contraseña incorrectos"
                );

                return View(datos);
            }

            // Crear las Claims del usuario
            var claims = new List<Claim>
{
    new Claim(
        ClaimTypes.NameIdentifier,
        usuario.id_Usuario.ToString()
    ),

    new Claim(
        ClaimTypes.Name,
        usuario.nombre + " " + usuario.apellido
    ),

    new Claim(
        ClaimTypes.Email,
        usuario.email
    ),

    new Claim(
        ClaimTypes.Role,
        usuario.Rol
    ),

    new Claim(
        "Avatar",
        usuario.Avatar ?? ""
    )
};

            // Crear la identidad
            var identidad = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // Crear el principal
            var principal = new ClaimsPrincipal(identidad);

            // Iniciar sesión
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Home");
        }

        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login", "Auth");
        }

        // GET: Auth/AccesoDenegado
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }

}
