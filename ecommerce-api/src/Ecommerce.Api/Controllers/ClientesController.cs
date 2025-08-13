using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    private string GetCurrentUser() => User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";

    /// <summary>
    /// Listar todos os clientes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao buscar clientes",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Buscar cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClienteDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            
            if (cliente == null)
            {
                return Problem(
                    title: "Cliente não encontrado",
                    detail: $"Cliente com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao buscar cliente",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Criar novo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Create([FromBody] CreateClienteDto createClienteDto)
    {
        try
        {
            var cliente = await _clienteService.CreateAsync(createClienteDto, GetCurrentUser());
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao criar cliente",
                detail: ex.Message,
                statusCode: 400
            );
        }
    }

    /// <summary>
    /// Atualizar cliente existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClienteDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClienteDto updateClienteDto)
    {
        try
        {
            var cliente = await _clienteService.UpdateAsync(id, updateClienteDto, GetCurrentUser());
            
            if (cliente == null)
            {
                return Problem(
                    title: "Cliente não encontrado",
                    detail: $"Cliente com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao atualizar cliente",
                detail: ex.Message,
                statusCode: 400
            );
        }
    }

    /// <summary>
    /// Excluir cliente
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _clienteService.DeleteAsync(id);
            
            if (!result)
            {
                return Problem(
                    title: "Cliente não encontrado",
                    detail: $"Cliente com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                title: "Não é possível excluir",
                detail: ex.Message,
                statusCode: 400
            );
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro interno",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
}
