using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Usuario> Usuarios { get; }
    IGenericRepository<Cliente> Clientes { get; }
    IGenericRepository<Produto> Produtos { get; }
    IPedidoRepository Pedidos { get; }
    IGenericRepository<PedidoItem> PedidoItens { get; }
    IGenericRepository<HistoricoEvento> HistoricoEventos { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
