using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    private string GetCurrentUser() => User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";

    /// <summary>
    /// Listar todos os produtos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var produtos = await _produtoService.GetAllAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao buscar produtos",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Buscar produto por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProdutoDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var produto = await _produtoService.GetByIdAsync(id);
            
            if (produto == null)
            {
                return Problem(
                    title: "Produto não encontrado",
                    detail: $"Produto com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return Ok(produto);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao buscar produto",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    /// <summary>
    /// Criar novo produto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Create([FromBody] CreateProdutoDto createProdutoDto)
    {
        try
        {
            var produto = await _produtoService.CreateAsync(createProdutoDto, GetCurrentUser());
            return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao criar produto",
                detail: ex.Message,
                statusCode: 400
            );
        }
    }

    /// <summary>
    /// Atualizar produto existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProdutoDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProdutoDto updateProdutoDto)
    {
        try
        {
            var produto = await _produtoService.UpdateAsync(id, updateProdutoDto, GetCurrentUser());
            
            if (produto == null)
            {
                return Problem(
                    title: "Produto não encontrado",
                    detail: $"Produto com ID {id} não foi encontrado",
                    statusCode: 404
                );
            }

            return Ok(produto);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Erro ao atualizar produto",
                detail: ex.Message,
                statusCode: 400
            );
        }
    }

    /// <summary>
    /// Excluir produto
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _produtoService.DeleteAsync(id);
            
            if (!result)
            {
                return Problem(
                    title: "Produto não encontrado",
                    detail: $"Produto com ID {id} não foi encontrado",
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
