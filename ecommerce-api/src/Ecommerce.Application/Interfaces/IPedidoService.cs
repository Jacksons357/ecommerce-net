using Ecommerce.Application.DTOs;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Interfaces;

public interface IPedidoService
{
    Task<IEnumerable<PedidoResumoDto>> GetAllAsync(StatusPedido? status = null);
    Task<PedidoDetalheDto?> GetByIdAsync(Guid id);
    Task<PedidoDetalheDto> CreateAsync(CreatePedidoDto createPedidoDto, string usuario);
    Task<PedidoDetalheDto?> PagarAsync(Guid id, string usuario);
    Task<PedidoDetalheDto?> CancelarAsync(Guid id, string usuario);
}
