using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;

namespace CentroDeCopias.Controllers
{
    public class StockController : Controller
    {
        private readonly DataContext _context;

        public StockController(DataContext context)
        {
            _context = context;
        }

        // GET: Stock
        public async Task<IActionResult> Index()
        {
            var stock = _context.Stocks.Include(s => s.Producto);
            return View(await stock.ToListAsync());
        }

        // GET: Stock/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks
                .Include(s => s.Producto)
                .FirstOrDefaultAsync(s => s.IdStock == id);

            if (stock == null) return NotFound();

            return View(stock);
        }

        // GET: Stock/Create
        public IActionResult Create()
        {
            ViewBag.IdProducto = new SelectList(_context.Productos, "IdProducto", "Nombre");
            return View();
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProducto,CantidadDisponible,StockMinimo")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                stock.UltimaActualizacion = DateTime.Now;
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdProducto = new SelectList(_context.Productos, "IdProducto", "Nombre", stock.IdProducto);
            return View(stock);
        }

        // GET: Stock/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return NotFound();

            ViewBag.IdProducto = new SelectList(_context.Productos, "IdProducto", "Nombre", stock.IdProducto);
            return View(stock);
        }

        // POST: Stock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStock,IdProducto,CantidadDisponible,StockMinimo")] Stock stock)
        {
            if (id != stock.IdStock) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    stock.UltimaActualizacion = DateTime.Now;
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Stocks.Any(e => e.IdStock == stock.IdStock)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdProducto = new SelectList(_context.Productos, "IdProducto", "Nombre", stock.IdProducto);
            return View(stock);
        }

        // GET: Stock/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks
                .Include(s => s.Producto)
                .FirstOrDefaultAsync(s => s.IdStock == id);

            if (stock == null) return NotFound();

            return View(stock);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null) _context.Stocks.Remove(stock);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}