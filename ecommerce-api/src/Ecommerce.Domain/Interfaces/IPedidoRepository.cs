using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Interfaces;

public interface IPedidoRepository : IGenericRepository<Pedido>
{
    Task<IEnumerable<Pedido>> GetAllWithClienteAsync(StatusPedido? status = null);
    Task<Pedido?> GetByIdWithDetailsAsync(Guid id);
}
