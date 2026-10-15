
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;

namespace CentroDeCopias.Controllers
{
    public class ProductosController : Controller
    {
        private readonly DataContext _context;

        // Constructor: recibe el contexto de la base de datos
        public ProductosController(DataContext context)
        {
            _context = context;
        }

        // GET: /Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos.ToListAsync();

            return View(productos);
        }

        // GET: /Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }


        // GET: Productos/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            // Buscar el producto por su ID
            var producto = await _context.Productos.FindAsync(id);

            // Si no existe, devolver 404
            if (producto == null)
            {
                return NotFound();
            }

            // Enviar el producto a la vista
            return View(producto);
        }

        // POST: Productos/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdProducto,Nombre,Descripcion,Precio,Categoria")] Producto producto)
        {
            // Verificar que el ID coincida
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Buscar el producto existente en la BD
                var productoBD = await _context.Productos.FindAsync(id);

                if (productoBD == null)
                {
                    return NotFound();
                }

                // Modificar solamente los campos editables
                productoBD.Nombre = producto.Nombre;
                productoBD.Descripcion = producto.Descripcion;
                productoBD.Precio = producto.Precio;
                productoBD.Categoria = producto.Categoria;

                // Guardar los cambios en MySQL
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }



        // POST: Productos/Desactivar/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            // Baja lógica
            producto.activo = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // POST: Productos/Reactivar/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            producto.activo = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}
