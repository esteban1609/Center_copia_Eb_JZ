using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CentroDeCopias.Controllers
{
    public class PedidosController : Controller
    {
        private readonly DataContext _context;

        public PedidosController(DataContext context)
        {
            _context = context;
        }
        private int UsuarioActualId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [Authorize]
        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            IQueryable<Pedido> query = _context.Pedidos.Include(p => p.Usuario);

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(p => p.id_Usuario == UsuarioActualId);
            }

            var pedidos = await query.OrderByDescending(p => p.fecha).ToListAsync();
            return View(pedidos);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Documentos)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.id_pedido == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        // GET: Pedidos/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create()
        {
            // Si ya tiene un pedido pendiente, lo reutiliza en vez de crear uno nuevo
            var pedidoExistente = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.id_Usuario == UsuarioActualId && p.estado == "Pendiente");

            if (pedidoExistente != null)
            {
                TempData["Mensaje"] = "Ya tenías un pedido pendiente, lo retomamos.";
                return RedirectToAction(nameof(Details), new { id = pedidoExistente.id_pedido });
            }

            var pedido = new Pedido
            {
                id_Usuario = UsuarioActualId,
                fecha = DateTime.Now,
                estado = "Pendiente",
                total = 0
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = pedido.id_pedido });
        }

        private static readonly string[] EstadosValidos =
        {
            "Pendiente", "Confirmado", "En impresión", "Listo para retirar", "Entregado", "Cancelado"
        };

        // GET: Pedidos/EditarEstado/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarEstado(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.id_pedido == id);

            if (pedido == null) return NotFound();

            ViewBag.Estados = new SelectList(EstadosValidos, pedido.estado);
            return View(pedido);
        }

        // POST: Pedidos/EditarEstado/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarEstado(int id, string estado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            if (!EstadosValidos.Contains(estado))
            {
                ModelState.AddModelError("", "Estado no válido");
                ViewBag.Estados = new SelectList(EstadosValidos, pedido.estado);
                return View(pedido);
            }

            pedido.estado = estado;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.id_pedido == id);

            if (pedido == null) return NotFound();

            // Solo se puede eliminar si está Pendiente
            if (pedido.estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden eliminar pedidos en estado Pendiente. Para los demás, cambiá el estado a Cancelado.";
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null) return NotFound();

            if (pedido.estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden eliminar pedidos en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

        // POST: Pedidos/Confirmar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Documentos)
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.id_pedido == id);

            if (pedido == null) return NotFound();

            // Solo el dueño del pedido puede confirmarlo
            if (pedido.id_Usuario != UsuarioActualId) return Forbid();

            if (pedido.estado != "Pendiente")
            {
                TempData["Error"] = "Este pedido ya fue confirmado o no está disponible.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (!pedido.Documentos.Any() && !pedido.Detalles.Any())
            {
                TempData["Error"] = "No podés confirmar un pedido vacío.";
                return RedirectToAction(nameof(Details), new { id });
            }

            pedido.estado = "Confirmado";
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Pedido confirmado correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }
        // GET: Pedidos/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Documentos)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.id_pedido == id);

            if (pedido == null) return NotFound();

            if (pedido.id_Usuario != UsuarioActualId) return Forbid();

            if (pedido.estado != "Pendiente")
            {
                TempData["Error"] = "Solo podés modificar pedidos en estado Pendiente.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(pedido);
        }

        // POST: Pedidos/ActualizarCantidad
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCantidad(int idDetalle, int cantidad)
        {
            var detalle = await _context.DetallesPedido
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(d => d.id_detalle == idDetalle);

            if (detalle == null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(detalle.id_pedido);
            if (pedido == null || pedido.id_Usuario != UsuarioActualId) return Forbid();
            if (pedido.estado != "Pendiente") return Forbid();

            if (cantidad <= 0)
            {
                TempData["Error"] = "La cantidad tiene que ser mayor a 0. Para sacarlo, usá el botón Eliminar.";
                return RedirectToAction(nameof(Edit), new { id = pedido.id_pedido });
            }

            decimal precioUnitario = detalle.Producto.Precio;
            pedido.total -= detalle.precio;

            detalle.cantidad = cantidad;
            detalle.precio = precioUnitario * cantidad;

            pedido.total += detalle.precio;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = pedido.id_pedido });
        }

        // POST: Pedidos/EliminarProducto
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int idDetalle)
        {
            var detalle = await _context.DetallesPedido.FindAsync(idDetalle);
            if (detalle == null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(detalle.id_pedido);
            if (pedido == null || pedido.id_Usuario != UsuarioActualId) return Forbid();
            if (pedido.estado != "Pendiente") return Forbid();

            pedido.total -= detalle.precio;
            _context.DetallesPedido.Remove(detalle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = pedido.id_pedido });
        }

        // POST: Pedidos/EliminarDocumento
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDocumento(int idDocumento)
        {
            var documento = await _context.Documentos.FindAsync(idDocumento);
            if (documento == null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(documento.IdPedido);
            if (pedido == null || pedido.id_Usuario != UsuarioActualId) return Forbid();
            if (pedido.estado != "Pendiente") return Forbid();

            pedido.total -= documento.Precio;
            _context.Documentos.Remove(documento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = pedido.id_pedido });
        }

        

    }
}