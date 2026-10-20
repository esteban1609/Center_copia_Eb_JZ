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

        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            var pedidos = _context.Pedidos.Include(p => p.Usuario);
            return View(await pedidos.ToListAsync());
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

    // GET: Pedidos/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.id_pedido == id);

        if (pedido == null) return NotFound();

        ViewBag.Estados = new SelectList(EstadosValidos, pedido.estado);
        return View(pedido);
    }

    // POST: Pedidos/Edit/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string estado)
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
    

    }
}