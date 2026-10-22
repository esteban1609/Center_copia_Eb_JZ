using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CentroDeCopias.Models
{
    public class Stock
    {
        public int IdStock { get; set; }

        public int IdProducto { get; set; }

        [ValidateNever]
        public Producto Producto { get; set; }

        public int CantidadDisponible { get; set; }
        public int StockMinimo { get; set; }
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;
    }
}