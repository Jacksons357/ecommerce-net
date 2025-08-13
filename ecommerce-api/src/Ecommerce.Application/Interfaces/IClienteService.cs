using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> GetAllAsync();
    Task<ClienteDto?> GetByIdAsync(Guid id);
    Task<ClienteDto> CreateAsync(CreateClienteDto createClienteDto, string usuario);
    Task<ClienteDto?> UpdateAsync(Guid id, UpdateClienteDto updateClienteDto, string usuario);
    Task<bool> DeleteAsync(Guid id);
}
