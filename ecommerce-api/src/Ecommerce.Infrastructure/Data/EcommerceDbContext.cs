using Microsoft.EntityFrameworkCore;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Infrastructure.Data;

public class EcommerceDbContext : DbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Produto> Produtos { get; set; } = null!;
    public DbSet<Pedido> Pedidos { get; set; } = null!;
    public DbSet<PedidoItem> PedidoItens { get; set; } = null!;
    public DbSet<HistoricoEvento> HistoricoEventos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario Configuration
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(500);
        });

        // Cliente Configuration
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CPF).IsRequired().HasMaxLength(14);
            entity.HasIndex(e => e.CPF).IsUnique();
        });

        // Produto Configuration
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Preco).HasColumnType("decimal(18,2)");
        });

        // Pedido Configuration
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClienteId).IsRequired();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.DataPedido).IsRequired();
            entity.Ignore(e => e.ValorTotal); // Propriedade computada
            
            // Configuração explícita da relação
            entity.HasOne(e => e.Cliente)
                  .WithMany()
                  .HasForeignKey("ClienteId")
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_Pedidos_Clientes_ClienteId");
        });

        // PedidoItem Configuration
        modelBuilder.Entity<PedidoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PedidoId).IsRequired();
            entity.Property(e => e.ProdutoId).IsRequired();
            entity.Property(e => e.PrecoUnitario).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Quantidade).IsRequired();
            
            // Configuração explícita das relações
            entity.HasOne(e => e.Pedido)
                  .WithMany(p => p.Itens)
                  .HasForeignKey("PedidoId")
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_PedidoItens_Pedidos_PedidoId");
                  
            entity.HasOne(e => e.Produto)
                  .WithMany()
                  .HasForeignKey("ProdutoId")
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_PedidoItens_Produtos_ProdutoId");
        });

        // HistoricoEvento Configuration
        modelBuilder.Entity<HistoricoEvento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Entidade).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Acao).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Usuario).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DadosAntes).HasColumnType("text");
            entity.Property(e => e.DadosDepois).HasColumnType("text");
        });
    }
}
