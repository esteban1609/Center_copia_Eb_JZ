namespace CentroDeCopias.Models
{
    public class Stock
    {
        public int IdStock { get; set; }

        // FK hacia Producto (1 a 1)
        public int IdProducto { get; set; }
        public Producto Producto { get; set; }

        public int CantidadDisponible { get; set; }
        public int StockMinimo { get; set; }
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;
    }
}