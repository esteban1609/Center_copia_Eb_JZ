
using CentroDeCopias.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CentroDeCopias.Controllers
{
    [Authorize]
    public class NotificacionesController : Controller
    {
        private readonly DataContext _context;

        public NotificacionesController(DataContext context)
        {
            _context = context;
        }

        private int UsuarioActualId =>
            int.Parse(User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        // GET: Notificaciones
        public async Task<IActionResult> Index(int pagina = 10)
        {
            const int tamanoPagina = 10;

            if (pagina < 1)
                pagina = 1;

            // Consultar solamente las notificaciones del usuario actual
            var query = _context.Notificaciones
                .AsNoTracking()
                .Where(n => n.id_Usuario == UsuarioActualId);

            // Cantidad total de notificaciones del usuario
            int totalNotificaciones = await query.CountAsync();

            // Cantidad de notificaciones sin leer
            int sinLeer = await query.CountAsync(n => !n.Leida);

            // Calcular total de páginas
            int totalPaginas = (int)Math.Ceiling(totalNotificaciones / (double)tamanoPagina);

            // Evitar páginas fuera del rango
            if (totalPaginas > 0 && pagina > totalPaginas)
                pagina = totalPaginas;

            // Obtener solamente las notificaciones de la página actual
            var notificaciones = await query
                .OrderBy(n => n.Leida)
                .ThenByDescending(n => n.FechaCreacion)
                .ThenByDescending(n => n.IdNotificacion)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            // Datos para la vista
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            ViewBag.TotalNotificaciones = totalNotificaciones;
            ViewBag.SinLeer = sinLeer;

            return View(notificaciones);
        }

        // POST: Notificaciones/MarcarLeida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n =>n.IdNotificacion == id &&n.id_Usuario == UsuarioActualId);

            if (notificacion == null)
                return NotFound();

            notificacion.Leida = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Notificaciones/MarcarTodasLeidas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarTodasLeidas()
        {
            var notificaciones = await _context.Notificaciones
                .Where(n =>n.id_Usuario == UsuarioActualId &&!n.Leida)
                .ToListAsync();

            foreach (var notificacion in notificaciones)
            {
                notificacion.Leida = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Notificaciones/IrAPedido/5
        public async Task<IActionResult> IrAPedido(int id)
        {
            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n =>n.IdNotificacion == id && n.id_Usuario == UsuarioActualId);

            if (notificacion == null)
                return NotFound();

            if (notificacion.id_pedido == null)
                return RedirectToAction(nameof(Index));

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p =>p.id_pedido == notificacion.id_pedido);

            if (pedido == null)
                return NotFound();

            if (pedido.id_Usuario != UsuarioActualId &&
                !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            notificacion.Leida = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details","Pedidos",new { id = pedido.id_pedido });
        }
    }
}
