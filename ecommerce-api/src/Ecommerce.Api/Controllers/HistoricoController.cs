using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistoricoController : ControllerBase
{
    private readonly IHistoricoService _historicoService;

    public HistoricoController(IHistoricoService historicoService)
    {
        _historicoService = historicoService;
    }

    /// <summary>
    /// Buscar histórico de eventos
    /// </summary>
    /// <param name="entidade">Filtrar por entidade (Cliente, Produto, Pedido)</param>
    /// <param name="entidadeId">Filtrar por ID da entidade</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HistoricoEventoDto>), 200)]
    public async Task<IActionResult> GetHistorico([FromQuery] string? entidade = null, [FromQuery] Guid? entidadeId = null)
    {
        try
        {
            var historico = await _historicoService.GetHistoricoAsync(entidade, entidadeId);
            return Ok(historico);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao buscar histórico",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
}
