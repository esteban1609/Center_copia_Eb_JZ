
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;

namespace CentroDeCopias.Controllers.Api
{
    [ApiController]
    [Route("api/productos")]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
    )]
    public class ProductosApiController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductosApiController(DataContext context)
        {
            _context = context;
        }

        // GET: api/productos?pagina=1
        // Lista paginada de productos
        [HttpGet]
        public async Task<IActionResult> Listar(int pagina = 1)
        {
            const int tamanoPagina = 10;

            if (pagina < 1)
                pagina = 1;

            int total = await _context.Productos.CountAsync();

            int totalPaginas = (int)Math.Ceiling(
                total / (double)tamanoPagina
            );

            var productos = await _context.Productos
                .AsNoTracking()
                .OrderBy(p => p.IdProducto)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(p => new
                {
                    p.IdProducto,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.Categoria,
                    p.RutaImagen,
                    p.activo
                })
                .ToListAsync();

            return Ok(new
            {
                datos = productos,
                paginaActual = pagina,
                totalPaginas,
                totalRegistros = total
            });
        }

        // GET: api/productos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .Where(p => p.IdProducto == id)
                .Select(p => new
                {
                    p.IdProducto,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.Categoria,
                    p.RutaImagen,
                    p.activo
                })
                .FirstOrDefaultAsync();

            if (producto == null)
                return NotFound(new
                {
                    mensaje = "Producto no encontrado."
                });

            return Ok(producto);
        }

        // POST: api/productos
        // Solo administradores
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Crear(
            [FromBody] Producto datos)
        {
            if (string.IsNullOrWhiteSpace(datos.Nombre))
            {
                return BadRequest(new
                {
                    mensaje = "El nombre es obligatorio."
                });
            }

            if (datos.Precio < 0)
            {
                return BadRequest(new
                {
                    mensaje = "El precio no puede ser negativo."
                });
            }

            // Creamos una entidad nueva para evitar
            // guardar propiedades no permitidas.
            var producto = new Producto
            {
                Nombre = datos.Nombre.Trim(),
                Descripcion = datos.Descripcion,
                Precio = datos.Precio,
                Categoria = datos.Categoria,
                activo = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Obtener),
                new { id = producto.IdProducto },
                new
                {
                    mensaje = "Producto creado correctamente.",
                    id = producto.IdProducto
                }
            );
        }

        // PUT: api/productos/5
        // Solo administradores
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Modificar(
            int id,
            [FromBody] Producto datos)
        {
            var producto = await _context.Productos
                .FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "Producto no encontrado."
                });
            }

            if (string.IsNullOrWhiteSpace(datos.Nombre))
            {
                return BadRequest(new
                {
                    mensaje = "El nombre es obligatorio."
                });
            }

            if (datos.Precio < 0)
            {
                return BadRequest(new
                {
                    mensaje = "El precio no puede ser negativo."
                });
            }

            producto.Nombre = datos.Nombre.Trim();
            producto.Descripcion = datos.Descripcion;
            producto.Precio = datos.Precio;
            producto.Categoria = datos.Categoria;

            // Conservamos imagen, stock y estado actual.
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto modificado correctamente."
            });
        }

        // DELETE: api/productos/5
        // Baja lógica: no elimina el registro de MySQL
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var producto = await _context.Productos
                .FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "Producto no encontrado."
                });
            }

            producto.activo = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto desactivado correctamente."
            });
        }

        // PATCH: api/productos/5/reactivar
        [HttpPatch("{id:int}/reactivar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var producto = await _context.Productos
                .FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "Producto no encontrado."
                });
            }

            producto.activo = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto reactivado correctamente."
            });
        }
    }
}
