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
        public async Task<IActionResult> Index(int pagina = 1)
        {
            const int tamanoPagina = 10;

            if (pagina < 1)
                pagina = 1;

            // Obtener la cantidad total de registros
            int totalStock = await _context.Stocks.CountAsync();

            // Calcular la cantidad de páginas
            int totalPaginas = (int)Math.Ceiling(
                totalStock / (double)tamanoPagina);

            if (totalPaginas > 0 && pagina > totalPaginas)
                pagina = totalPaginas;

            // Obtener los registros de la página actual
            var stock = await _context.Stocks
                .AsNoTracking()
                .Include(s => s.Producto)
                .OrderBy(s => s.Producto.Nombre)
                .ThenBy(s => s.IdStock)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            // Datos necesarios para la paginación
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(stock);
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
            //ViewBag.IdProducto = new SelectList(_context.Productos, "IdProducto", "Nombre");
            return View();
        }

        //GET: Stock/Buscador-Select
        //Busca los productos a medida que vaya escribiendo en el select
        [HttpGet]
        public async Task<IActionResult> BuscarProductos(string? termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return Json(Array.Empty<object>());

            termino = termino.Trim();

            var productos = await _context.Productos
                .AsNoTracking()
                .Where(p =>
                    p.activo &&
                    p.Nombre.Contains(termino) &&
                    !_context.Stocks.Any(s =>
                        s.IdProducto == p.IdProducto))
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    id = p.IdProducto,
                    nombre = p.Nombre
                })
                .Take(10)
                .ToListAsync();

            return Json(productos);
        }


        // // POST: Stock/Create
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("IdProducto,CantidadDisponible,StockMinimo")] Stock stock)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         stock.UltimaActualizacion = DateTime.Now;
        //         _context.Add(stock);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewBag.IdProducto = new SelectList(_context.Productos, "IdProducto", "Nombre", stock.IdProducto);
        //     return View(stock);
        // }



        //POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdProducto,CantidadDisponible,StockMinimo")] Stock stock)
        {
            if (stock.CantidadDisponible < 0 || stock.StockMinimo < 0)
            {
                ModelState.AddModelError(
                    "",
                    "Las cantidades no pueden ser negativas."
                );
            }

            bool productoValido = await _context.Productos
                .AnyAsync(p =>
                    p.IdProducto == stock.IdProducto && p.activo);

            if (!productoValido)
            {
                ModelState.AddModelError(
                    "IdProducto",
                    "Seleccioná un producto válido."
                );
            }

            bool yaTieneStock = await _context.Stocks
                .AnyAsync(s => s.IdProducto == stock.IdProducto);

            if (yaTieneStock)
            {
                ModelState.AddModelError(
                    "IdProducto",
                    "Este producto ya tiene un registro de stock."
                );
            }

            if (!ModelState.IsValid)
                return View(stock);

            stock.UltimaActualizacion = DateTime.Now;

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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


        // GET: Stock/Buscar?q=texto
        [HttpGet]
        public async Task<IActionResult> Buscar(string q)
        {
            var query = _context.Stocks.Include(s => s.Producto).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(s => s.Producto.Nombre.Contains(q));
            }

            var stock = await query.ToListAsync();
            return PartialView("_StockTable", stock);
        }



    }
}