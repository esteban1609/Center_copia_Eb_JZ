using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CentroDeCopias.Controllers
{

    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        private readonly DataContext _context;

        public UsuarioController(DataContext context)
        {
            _context = context;
        }


        // GET: Usuarios
        public async Task<IActionResult> Index(int pagina = 1)
        {
            const int tamanoPagina = 10;

            if (pagina < 1)
                pagina = 1;

            // Obtener la cantidad total de usuarios
            int totalUsuarios = await _context.Usuarios.CountAsync();

            // Calcular la cantidad de páginas
            int totalPaginas = (int)Math.Ceiling(
                totalUsuarios / (double)tamanoPagina);

            if (totalPaginas > 0 && pagina > totalPaginas)
                pagina = totalPaginas;

            // Obtener los usuarios de la página actual
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.apellido)
                .ThenBy(u => u.nombre)
                .ThenBy(u => u.id_Usuario)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            // Datos para la paginación
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(usuarios);
        }




        // GET: Usuarios/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("nombre,apellido,email,contrasena,Rol")] Usuario usuario)
        {
            if (usuario.Rol != "Admin" &&
                usuario.Rol != "Cliente" &&
                usuario.Rol != "Escuela")
            {
                ModelState.AddModelError("Rol", "Seleccione un rol válido");
            }

            if (await _context.Usuarios.AnyAsync(
                u => u.email == usuario.email))
            {
                ModelState.AddModelError("email", "Este correo ya está registrado");
            }

            if (ModelState.IsValid)
            {
                // Encriptamos la contraseña mediante un hash seguro
                var hasher = new PasswordHasher<Usuario>();

                usuario.contrasena = hasher.HashPassword(
                    usuario,
                    usuario.contrasena
                );

                usuario.activo = true;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }


        // GET: Usuarios/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios
                .FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("nombre,apellido,email,Rol")] Usuario datos)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            // No editamos la contraseña desde este formulario.
            // Quitamos su validación obligatoria para esta operación.
            ModelState.Remove("contrasena");

            // Validar el rol
            if (datos.Rol != "Admin" &&
                datos.Rol != "Cliente" &&
                datos.Rol != "Escuela")
            {
                ModelState.AddModelError("Rol", "Seleccione un rol válido");
            }

            // Verificar que el email no pertenezca a otro usuario
            bool emailExiste = await _context.Usuarios.AnyAsync(
                u => u.email == datos.email &&
                     u.id_Usuario != id
            );

            if (emailExiste)
            {
                ModelState.AddModelError(
                    "email",
                    "Este correo ya está registrado"
                );
            }

            if (ModelState.IsValid)
            {
                usuario.nombre = datos.nombre;
                usuario.apellido = datos.apellido;
                usuario.email = datos.email;
                usuario.Rol = datos.Rol;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            datos.id_Usuario = id;
            return View(datos);
        }

        // POST: Usuarios/Desactivar/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.activo = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // POST: Usuarios/Reactivar/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.activo = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}