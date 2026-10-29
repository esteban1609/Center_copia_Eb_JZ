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






        
        // LISTADO PARA VUE: AJAX + JSON + PAGINACIÓN
        [HttpGet]
        public async Task<IActionResult> ListarVue(
            int pagina = 1, string busqueda = "")
        {
            const int tamanoPagina = 10;
            pagina = Math.Max(1, pagina);
            busqueda = (busqueda ?? "").Trim();

            var consulta = _context.Usuarios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                consulta = consulta.Where(u =>
                    u.nombre.Contains(busqueda) ||
                    u.apellido.Contains(busqueda) ||
                    u.email.Contains(busqueda));
            }

            int total = await consulta.CountAsync();
            int totalPaginas = (int)Math.Ceiling(
                total / (double)tamanoPagina);

            pagina = totalPaginas == 0? 1: Math.Min(pagina, totalPaginas);

            var usuarios = await consulta
                .OrderBy(u => u.apellido)
                .ThenBy(u => u.nombre)
                .ThenBy(u => u.id_Usuario)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(u => new
                {
                    id = u.id_Usuario,
                    nombre = u.nombre,
                    apellido = u.apellido,
                    email = u.email,
                    rol = u.Rol,
                    activo = u.activo
                })
                .ToListAsync();

            return Json(new
            {
                usuarios,
                pagina,
                totalPaginas,
                total
            });
        }

        // ALTA DE USUARIO DESDE VUE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVue(
            [FromBody] Usuario datos)
        {
            if (datos == null)
                return BadRequest(new
                {
                    mensaje = "No se recibieron datos."
                });

            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    mensaje = "Completá los campos correctamente. La contraseña debe tener al menos 8 caracteres."
                });

            string nombre = datos.nombre.Trim();
            string apellido = datos.apellido.Trim();
            string email = datos.email.Trim();

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new
                {
                    mensaje = "Nombre, apellido y email son obligatorios."
                });
            }

            if (datos.Rol != "Admin" &&
                datos.Rol != "Cliente" &&
                datos.Rol != "Escuela")
            {
                return BadRequest(new
                {
                    mensaje = "Seleccioná un rol válido."
                });
            }

            bool emailExiste = await _context.Usuarios
                .AnyAsync(u => u.email.ToLower() == email.ToLower());

            if (emailExiste)
                return BadRequest(new
                {
                    mensaje = "Este correo ya está registrado."
                });

            // Solo copiamos los campos permitidos.
            var usuario = new Usuario
            {
                nombre = nombre,
                apellido = apellido,
                email = email,
                Rol = datos.Rol,
                activo = true
            };

            var hasher = new PasswordHasher<Usuario>();
            usuario.contrasena = hasher.HashPassword(
                usuario, datos.contrasena);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Json(new
            {
                mensaje = "Usuario creado correctamente."
            });
        }

        // MODIFICACIÓN DESDE VUE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarVue(
            int id, [FromBody] Usuario datos)
        {
            if (datos == null)
                return BadRequest(new
                {
                    mensaje = "No se recibieron datos."
                });

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new
                {
                    mensaje = "Usuario no encontrado."
                });

            // La contraseña no se edita desde este formulario.
            ModelState.Remove(nameof(Usuario.contrasena));

            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    mensaje = "Revisá los datos ingresados."
                });

            string nombre = datos.nombre.Trim();
            string apellido = datos.apellido.Trim();
            string email = datos.email.Trim();

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new
                {
                    mensaje = "Completá nombre, apellido y email."
                });
            }

            if (datos.Rol != "Admin" &&
                datos.Rol != "Cliente" &&
                datos.Rol != "Escuela")
            {
                return BadRequest(new
                {
                    mensaje = "Seleccioná un rol válido."
                });
            }

            bool emailExiste = await _context.Usuarios.AnyAsync(u =>
                u.id_Usuario != id &&
                u.email.ToLower() == email.ToLower());

            if (emailExiste)
                return BadRequest(new
                {
                    mensaje = "Este correo pertenece a otro usuario."
                });

            if (usuario.Rol == "Admin" &&
                usuario.activo &&
                datos.Rol != "Admin")
            {
                int administradores = await _context.Usuarios
                    .CountAsync(u => u.Rol == "Admin" && u.activo);

                if (administradores <= 1)
                    return BadRequest(new
                    {
                        mensaje = "No podés quitar el rol al último administrador activo."
                    });
            }

            usuario.nombre = nombre;
            usuario.apellido = apellido;
            usuario.email = email;
            usuario.Rol = datos.Rol;

            // No modificamos contraseña, avatar ni estado.
            await _context.SaveChangesAsync();

            return Json(new
            {
                mensaje = "Usuario actualizado correctamente."
            });
        }

        // BAJA LÓGICA DESDE VUE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarVue(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new
                {
                    mensaje = "Usuario no encontrado."
                });

            if (usuario.Rol == "Admin" && usuario.activo)
            {
                int administradores = await _context.Usuarios
                    .CountAsync(u => u.Rol == "Admin" && u.activo);

                if (administradores <= 1)
                    return BadRequest(new
                    {
                        mensaje = "No podés desactivar al último administrador activo."
                    });
            }

            usuario.activo = false;
            await _context.SaveChangesAsync();

            return Json(new
            {
                mensaje = "Usuario desactivado correctamente."
            });
        }

        // REACTIVACIÓN DESDE VUE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReactivarVue(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new
                {
                    mensaje = "Usuario no encontrado."
                });

            usuario.activo = true;
            await _context.SaveChangesAsync();

            return Json(new
            {
                mensaje = "Usuario reactivado correctamente."
            });
        }


    }
}