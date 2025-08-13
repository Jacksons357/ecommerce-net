using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Interfaces;

public interface IProdutoService
{
    Task<IEnumerable<ProdutoDto>> GetAllAsync();
    Task<ProdutoDto?> GetByIdAsync(Guid id);
    Task<ProdutoDto> CreateAsync(CreateProdutoDto createProdutoDto, string usuario);
    Task<ProdutoDto?> UpdateAsync(Guid id, UpdateProdutoDto updateProdutoDto, string usuario);
    Task<bool> DeleteAsync(Guid id);
}
