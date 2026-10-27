using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentroDeCopias.Models;
using UglyToad.PdfPig;

namespace CentroDeCopias.Controllers
{
    [Authorize]
    public class DocumentosController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentosController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private int UsuarioActualId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }
        private decimal CalcularPrecioImpresion(int paginas, int copias, bool color, bool dobleFaz)
        {
            int hojasPorCopia = dobleFaz ? (int)Math.Ceiling(paginas / 2.0) : paginas;
            int hojasTotales = hojasPorCopia * copias;

            decimal precioPorHoja;

            if (hojasTotales <= 20)
                precioPorHoja = color ? 100 : 80;
            else if (hojasTotales <= 100)
                precioPorHoja = color ? 70 : 60;
            else
                precioPorHoja = color ? 50 : 40;

            return precioPorHoja * hojasTotales;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            string nombre, IFormFile archivo, int copias, bool color,
            bool dobleFaz, string tamanoHoja, string anillado)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("", "Tenés que seleccionar un archivo PDF");
                return View();
            }

            if (Path.GetExtension(archivo.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("", "Solo se permiten archivos PDF");
                return View();
            }

            const long maxBytes = 20 * 1024 * 1024;
            if (archivo.Length > maxBytes)
            {
                ModelState.AddModelError("", "El archivo no puede superar los 20 MB");
                return View();
            }

            if (copias <= 0)
            {
                ModelState.AddModelError("", "La cantidad de copias tiene que ser mayor a 0");
                return View();
            }

            // Busca un pedido Pendiente del usuario, o crea uno nuevo
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

            var carpeta = Path.Combine(_env.WebRootPath, "documentos");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}.pdf";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }
            
            int paginas = ContarPaginas(rutaFisica);

            // TODO: definir precio real según copias/color/tamaño/doble faz
            decimal precio = CalcularPrecioImpresion(paginas, copias, color, dobleFaz);

            var documento = new Documento
            {
                Nombre = string.IsNullOrWhiteSpace(nombre) ? archivo.FileName : nombre,
                RutaArchivo = $"/documentos/{nombreArchivo}",
                FechaSubida = DateTime.Now,
                IdUsuario = UsuarioActualId,
                IdPedido = pedido.id_pedido,
                Copias = copias,
                Color = color,
                DobleFaz = dobleFaz,
                TamanoHoja = tamanoHoja,
                Anillado = anillado,
                Precio = precio,
                Paginas = paginas
            };

            _context.Documentos.Add(documento);
            pedido.total += (decimal)precio;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Documento agregado a tu pedido";
            return RedirectToAction("Details", "Pedidos", new { id = pedido.id_pedido });
        }

        private int ContarPaginas(string rutaFisica)
        {
            using var pdf = PdfDocument.Open(rutaFisica);
            return pdf.NumberOfPages;
        }
    }
}