using System.Text.Json;
using System.Text.Json.Serialization;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Application.Services;

public class HistoricoService : IHistoricoService
{
    private readonly IUnitOfWork _unitOfWork;

    public HistoricoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<HistoricoEventoDto>> GetHistoricoAsync(string? entidade = null, Guid? entidadeId = null)
    {
        var historicos = await _unitOfWork.HistoricoEventos.GetAllAsync();
        
        var query = historicos.AsQueryable();
        
        if (!string.IsNullOrEmpty(entidade))
            query = query.Where(h => h.Entidade == entidade);
            
        if (entidadeId.HasValue)
            query = query.Where(h => h.EntidadeId == entidadeId.Value);
            
        return query.OrderByDescending(h => h.DataOcorrencia)
                   .Select(h => new HistoricoEventoDto
                   {
                       Id = h.Id,
                       Entidade = h.Entidade,
                       EntidadeId = h.EntidadeId,
                       Acao = h.Acao,
                       DadosAntes = h.DadosAntes,
                       DadosDepois = h.DadosDepois,
                       DataOcorrencia = h.DataOcorrencia,
                       Usuario = h.Usuario
                   });
    }

    public async Task RegistrarEventoAsync(string entidade, Guid entidadeId, string acao, object? dadosAntes, object? dadosDepois, string usuario)
    {
        // Configurar opções de serialização para evitar referências circulares
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var historico = new HistoricoEvento
        {
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = acao,
            DadosAntes = dadosAntes != null ? JsonSerializer.Serialize(dadosAntes, jsonOptions) : null,
            DadosDepois = dadosDepois != null ? JsonSerializer.Serialize(dadosDepois, jsonOptions) : null,
            Usuario = usuario
        };

        await _unitOfWork.HistoricoEventos.AddAsync(historico);
        await _unitOfWork.SaveChangesAsync();
    }
}
