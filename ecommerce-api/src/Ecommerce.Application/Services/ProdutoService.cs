using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHistoricoService _historicoService;

    public ProdutoService(IUnitOfWork unitOfWork, IHistoricoService historicoService)
    {
        _unitOfWork = unitOfWork;
        _historicoService = historicoService;
    }

    public async Task<IEnumerable<ProdutoDto>> GetAllAsync()
    {
        var produtos = await _unitOfWork.Produtos.GetAllAsync();
        return produtos.Select(p => new ProdutoDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Preco = p.Preco,
            DataCriacao = p.DataCriacao,
            DataAtualizacao = p.DataAtualizacao
        });
    }

    public async Task<ProdutoDto?> GetByIdAsync(Guid id)
    {
        var produto = await _unitOfWork.Produtos.GetByIdAsync(id);
        if (produto == null) return null;

        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            DataCriacao = produto.DataCriacao,
            DataAtualizacao = produto.DataAtualizacao
        };
    }

    public async Task<ProdutoDto> CreateAsync(CreateProdutoDto createProdutoDto, string usuario)
    {
        var produto = new Produto
        {
            Nome = createProdutoDto.Nome,
            Preco = createProdutoDto.Preco
        };

        await _unitOfWork.Produtos.AddAsync(produto);
        await _unitOfWork.SaveChangesAsync();

        // Registrar no histórico
        await _historicoService.RegistrarEventoAsync("Produto", produto.Id, "Criado", null, produto, usuario);

        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            DataCriacao = produto.DataCriacao,
            DataAtualizacao = produto.DataAtualizacao
        };
    }

    public async Task<ProdutoDto?> UpdateAsync(Guid id, UpdateProdutoDto updateProdutoDto, string usuario)
    {
        var produto = await _unitOfWork.Produtos.GetByIdAsync(id);
        if (produto == null) return null;

        var dadosAntes = new { produto.Nome, produto.Preco };

        produto.Nome = updateProdutoDto.Nome;
        produto.Preco = updateProdutoDto.Preco;
        produto.DataAtualizacao = DateTime.UtcNow;

        _unitOfWork.Produtos.Update(produto);
        await _unitOfWork.SaveChangesAsync();

        var dadosDepois = new { produto.Nome, produto.Preco };

        // Registrar no histórico
        await _historicoService.RegistrarEventoAsync("Produto", produto.Id, "Atualizado", dadosAntes, dadosDepois, usuario);

        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            DataCriacao = produto.DataCriacao,
            DataAtualizacao = produto.DataAtualizacao
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var produto = await _unitOfWork.Produtos.GetByIdAsync(id);
        if (produto == null) return false;

        // Verificar se o produto está sendo usado em algum pedido
        var pedidoItens = await _unitOfWork.PedidoItens.FindAsync(pi => pi.ProdutoId == id);
        if (pedidoItens.Any())
        {
            throw new InvalidOperationException("Não é possível excluir o produto pois ele está sendo usado em um ou mais pedidos.");
        }

        _unitOfWork.Produtos.Delete(produto);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
