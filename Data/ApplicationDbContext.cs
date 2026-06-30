using Microsoft.EntityFrameworkCore;
using LOGIN.Models;

namespace LOGIN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuarios
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Ciudad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Edad).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuración de Productos
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.Precio).IsRequired().HasColumnType("numeric(18,2)");
                entity.Property(e => e.Categoria).HasMaxLength(100);
                entity.Property(e => e.Marca).HasMaxLength(100);
                entity.Property(e => e.ModeloAuto).HasMaxLength(100);
                entity.Property(e => e.FechaRegistro)
                     .HasColumnType("timestamp with time zone")
                     .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuración de CarritoItems
            modelBuilder.Entity<CarritoItem>(entity =>
            {
                entity.ToTable("CarritoItems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.FechaAgregado)
                     .HasColumnType("timestamp with time zone")
                     .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Pedidos
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("Pedidos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Total).IsRequired().HasColumnType("numeric(18,2)");
                entity.Property(e => e.Estado).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.DireccionEnvio).HasMaxLength(200);
                entity.Property(e => e.MetodoPago).HasMaxLength(50);
                entity.Property(e => e.NumeroReferencia).HasMaxLength(20);
                entity.Property(e => e.FechaPedido)
                       .HasColumnType("timestamp with time zone")
                       .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de PedidoDetalles
            modelBuilder.Entity<PedidoDetalle>(entity =>
            {
                entity.ToTable("PedidoDetalles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.PrecioUnitario).IsRequired().HasColumnType("numeric(18,2)");

                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.PedidoDetalles)
                    .HasForeignKey(e => e.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}