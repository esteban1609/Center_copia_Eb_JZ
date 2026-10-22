
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;
using Microsoft.AspNetCore.Authorization;

namespace CentroDeCopias.Controllers
{
    public class ProductosController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductosController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos.ToListAsync();
            return View(productos);
        }

        // GET: Productos/Buscar?q=texto  (llamado por AJAX)
        [HttpGet]
        public async Task<IActionResult> Buscar(string q)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(q) ||
                    (p.Categoria != null && p.Categoria.Contains(q)));
            }

            var productos = await query.ToListAsync();
            return PartialView("_ProductosCards", productos);
        }

        // GET: Productos/Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            if (producto.Imagen != null && producto.Imagen.Length > 0)
            {
                producto.RutaImagen = await GuardarImagen(producto.Imagen);
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GuardarImagen(IFormFile imagen)
        {
            var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(imagen.FileName).ToLower();

            if (!extensionesValidas.Contains(ext))
                throw new InvalidOperationException("Formato de imagen no soportado");

            var carpeta = Path.Combine(_env.WebRootPath, "images", "productos");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{ext}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return $"/images/productos/{nombreArchivo}";
        }



        // GET: Productos/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto) return NotFound();

            if (!ModelState.IsValid) return View(producto);

            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null) return NotFound();

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Categoria = producto.Categoria;

            if (producto.Imagen != null && producto.Imagen.Length > 0)
            {
                BorrarImagenSiExiste(productoExistente.RutaImagen);
                productoExistente.RutaImagen = await GuardarImagen(producto.Imagen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void BorrarImagenSiExiste(string? rutaImagen)
        {
            if (string.IsNullOrEmpty(rutaImagen)) return;

            var rutaFisica = Path.Combine(_env.WebRootPath, rutaImagen.TrimStart('/'));
            
            if (System.IO.File.Exists(rutaFisica))
            {
                System.IO.File.Delete(rutaFisica);
            }
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
