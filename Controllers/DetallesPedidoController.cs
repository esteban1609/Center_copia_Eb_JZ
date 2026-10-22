using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;

namespace CentroDeCopias.Controllers
{
    [Authorize]
    public class DetallesPedidoController : Controller
    {
        private readonly DataContext _context;

        public DetallesPedidoController(DataContext context)
        {
            _context = context;
        }

        private int UsuarioActualId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // GET: DetallesPedido/Agregar/5  (id = IdProducto)
        [HttpGet]
        public async Task<IActionResult> Agregar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            ViewBag.Producto = producto;
            return View();
        }

        // POST: DetallesPedido/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(int idProducto, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(idProducto);
            if (producto == null) return NotFound();

            if (cantidad <= 0)
            {
                ModelState.AddModelError("", "La cantidad tiene que ser mayor a 0");
                ViewBag.Producto = producto;
                return View();
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.id_Usuario == UsuarioActualId && p.estado == "Pendiente");

            if (pedido == null)
            {
                pedido = new Pedido
                {
                    id_Usuario = UsuarioActualId,
                    fecha = DateTime.Now,
                    estado = "Pendiente",
                    total = 0
                };
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
            }

            var detalleExistente = await _context.DetallesPedido
                .FirstOrDefaultAsync(d => d.id_pedido == pedido.id_pedido && d.id_producto == idProducto);

            decimal subtotal = producto.Precio * cantidad;

            if (detalleExistente != null)
            {
                detalleExistente.cantidad += cantidad;
                detalleExistente.precio += subtotal;
            }
            else
            {
                _context.DetallesPedido.Add(new DetallePedido
                {
                    id_pedido = pedido.id_pedido,
                    id_producto = idProducto,
                    cantidad = cantidad,
                    precio = subtotal
                });
            }

            pedido.total += subtotal;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Producto agregado a tu pedido";
            return RedirectToAction("Details", "Pedidos", new { id = pedido.id_pedido });
        }
    }
}