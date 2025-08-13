using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Interfaces;

public interface IHistoricoService
{
    Task<IEnumerable<HistoricoEventoDto>> GetHistoricoAsync(string? entidade = null, Guid? entidadeId = null);
    Task RegistrarEventoAsync(string entidade, Guid entidadeId, string acao, object? dadosAntes, object? dadosDepois, string usuario);
}
