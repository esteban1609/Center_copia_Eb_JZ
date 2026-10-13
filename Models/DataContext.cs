
using CentroDeCopias.Models;
using Microsoft.EntityFrameworkCore;

namespace CentroDeCopias.Models
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







protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Usuario 1:N Pedido
    modelBuilder.Entity<Pedido>()
        .HasOne(p => p.Usuario)
        .WithMany(u => u.Pedidos)
        .HasForeignKey(p => p.id_Usuario);

    // Usuario 1:N Documento
    modelBuilder.Entity<Documento>()
        .HasOne(d => d.Usuario)
        .WithMany(u => u.Documentos)
        .HasForeignKey(d => d.IdUsuario);

    // Pedido 1:N DetallePedido
    modelBuilder.Entity<DetallePedido>()
        .HasOne<Pedido>()
        .WithMany()
        .HasForeignKey(d => d.id_pedido)
        .IsRequired();
    
    //pedido 1:N Documento
    modelBuilder.Entity<Documento>()
    .HasOne(d => d.pedido)
    .WithMany(p => p.Documentos)
    .HasForeignKey(d => d.IdPedido);

    // Producto 1:N DetallePedido
    modelBuilder.Entity<DetallePedido>()
        .HasOne<Producto>()
        .WithMany(p => p.DetallesPedido)
        .HasForeignKey(d => d.id_producto)
        .IsRequired();

    // Documento 1:N DetallePedido
    modelBuilder.Entity<DetallePedido>()
        .HasOne<Documento>()
        .WithMany(d => d.DetallesPedido)
        .HasForeignKey(d => d.id_documento)
        .IsRequired(false);

    // Producto 1:1 Stock
    modelBuilder.Entity<Stock>()
        .HasOne(s => s.Producto)
        .WithOne(p => p.Stock)
        .HasForeignKey<Stock>(s => s.IdProducto);

    // Claves primarias
    modelBuilder.Entity<Documento>()
        .HasKey(d => d.IdDocumento);

    modelBuilder.Entity<Stock>()
        .HasKey(s => s.IdStock);

    // Propiedad solo para mostrar el nombre
    modelBuilder.Entity<Pedido>()
        .Ignore(p => p.nombreusuario);

}


    }
}
