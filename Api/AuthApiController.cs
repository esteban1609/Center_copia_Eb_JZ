
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CentroDeCopias.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CentroDeCopias.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthApiController(
            DataContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginApiRequest datos)
        {
            if (string.IsNullOrWhiteSpace(datos.Email) ||
                string.IsNullOrWhiteSpace(datos.Contrasena))
            {
                return BadRequest(new
                {
                    mensaje = "Ingresá el correo y la contraseña."
                });
            }

            // Buscar usuario por correo
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.email == datos.Email);

            if (usuario == null || !usuario.activo)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }

            // Verificar contraseña igual que en AuthController
            var hasher = new PasswordHasher<Usuario>();

            var resultado = hasher.VerifyHashedPassword(
                usuario,
                usuario.contrasena,
                datos.Contrasena
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }

            // Obtener configuración JWT
            var jwtKey = _configuration["Jwt:Key"]!;
            var jwtIssuer = _configuration["Jwt:Issuer"]!;
            var jwtAudience = _configuration["Jwt:Audience"]!;

            // Crear claims del usuario
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
                )
            };

            // Crear clave de firma
            var clave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credenciales = new SigningCredentials(
                clave,
                SecurityAlgorithms.HmacSha256
            );

            // El token durará 1 hora
            var vencimiento = DateTime.UtcNow.AddHours(1);

            // Generar JWT
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: vencimiento,
                signingCredentials: credenciales
            );

            var tokenGenerado =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            // Devolver respuesta JSON
            return Ok(new
            {
                mensaje = "Autenticación correcta.",
                token = tokenGenerado,
                expiracion = vencimiento,
                usuario = new
                {
                    id = usuario.id_Usuario,
                    nombre = usuario.nombre,
                    apellido = usuario.apellido,
                    email = usuario.email,
                    rol = usuario.Rol
                }
            });
        }
    }

    // Modelo para recibir las credenciales JSON.
    // No modifica la entidad Usuario ni crea una tabla.
    public class LoginApiRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}
