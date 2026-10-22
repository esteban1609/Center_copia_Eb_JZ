
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
        public async Task<IActionResult> Index()
        {
            var notificaciones = await _context.Notificaciones
                .AsNoTracking()
                .Where(n => n.id_Usuario == UsuarioActualId)
                .OrderBy(n => n.Leida)
                .ThenByDescending(n => n.FechaCreacion)
                .ToListAsync();

            ViewBag.TotalNotificaciones = notificaciones.Count;
            ViewBag.SinLeer = notificaciones.Count(n => !n.Leida);

            return View(notificaciones);
        }

        // POST: Notificaciones/MarcarLeida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n =>
                    n.IdNotificacion == id &&
                    n.id_Usuario == UsuarioActualId);

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
                .Where(n =>
                    n.id_Usuario == UsuarioActualId &&
                    !n.Leida)
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
                .FirstOrDefaultAsync(n =>
                    n.IdNotificacion == id &&
                    n.id_Usuario == UsuarioActualId);

            if (notificacion == null)
                return NotFound();

            if (notificacion.id_pedido == null)
                return RedirectToAction(nameof(Index));

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p =>
                    p.id_pedido == notificacion.id_pedido);

            if (pedido == null)
                return NotFound();

            if (pedido.id_Usuario != UsuarioActualId &&
                !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            notificacion.Leida = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "Pedidos",
                new { id = pedido.id_pedido });
        }
    }
}
