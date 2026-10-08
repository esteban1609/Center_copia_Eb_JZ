
using CenterCopy.models;
using CentroDeCopias.Models;
using Microsoft.EntityFrameworkCore;

namespace TuProyecto.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<DetallePedido> DetallesPedido { get; set; }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Documento> Documentos { get; set; }

        public DbSet<Stock> Stocks { get; set; }
    }
}
