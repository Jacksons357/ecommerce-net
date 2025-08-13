using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    private string GetCurrentUser() => User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";

    /// <summary>
    /// Listar pedidos com filtro opcional por status
    /// </summary>
    /// <param name="status">Status do pedido (Criado, Pago, Cancelado)</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PedidoResumoDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] StatusPedido? status = null)
    {
        try
        {
            var pedidos = await _pedidoService.GetAllAsync(status);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao buscar pedidos",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Buscar pedido por ID com detalhes completos
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PedidoDetalheDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            
            if (pedido == null)
            {
                return Problem(
                    title: "Pedido não encontrado",
                    detail: $"Pedido com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return Ok(pedido);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao buscar pedido",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Criar novo pedido
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoDetalheDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Create([FromBody] CreatePedidoDto createPedidoDto)
    {
        try
        {
            var pedido = await _pedidoService.CreateAsync(createPedidoDto, GetCurrentUser());
            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
        }
        catch (ArgumentException ex)
        {
            return Problem(
                title: "Dados inválidos",
                detail: ex.Message,
                statusCode: 400
            );
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao criar pedido",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Pagar um pedido (alterar status para Pago)
    /// </summary>
    [HttpPost("{id}/pagar")]
    [ProducesResponseType(typeof(PedidoDetalheDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Pagar(Guid id)
    {
        try
        {
            var pedido = await _pedidoService.PagarAsync(id, GetCurrentUser());
            
            if (pedido == null)
            {
                return Problem(
                    title: "Pedido não encontrado",
                    detail: $"Pedido com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return Ok(pedido);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                title: "Operação inválida",
                detail: ex.Message,
                statusCode: 400
            );
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao pagar pedido",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Cancelar um pedido (alterar status para Cancelado)
    /// </summary>
    [HttpPost("{id}/cancelar")]
    [ProducesResponseType(typeof(PedidoDetalheDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        try
        {
            var pedido = await _pedidoService.CancelarAsync(id, GetCurrentUser());
            
            if (pedido == null)
            {
                return Problem(
                    title: "Pedido não encontrado",
                    detail: $"Pedido com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return Ok(pedido);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                title: "Operação inválida",
                detail: ex.Message,
                statusCode: 400
            );
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao cancelar pedido",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
}
