using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHistoricoService _historicoService;

    public ClienteService(IUnitOfWork unitOfWork, IHistoricoService historicoService)
    {
        _unitOfWork = unitOfWork;
        _historicoService = historicoService;
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var clientes = await _unitOfWork.Clientes.GetAllAsync();
        return clientes.Select(c => new ClienteDto
        {
            Id = c.Id,
            Nome = c.Nome,
            CPF = c.CPF,
            DataCriacao = c.DataCriacao,
            DataAtualizacao = c.DataAtualizacao
        });
    }

    public async Task<ClienteDto?> GetByIdAsync(Guid id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null) return null;

        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CPF = cliente.CPF,
            DataCriacao = cliente.DataCriacao,
            DataAtualizacao = cliente.DataAtualizacao
        };
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto createClienteDto, string usuario)
    {
        var cliente = new Cliente
        {
            Nome = createClienteDto.Nome,
            CPF = createClienteDto.CPF
        };

        await _unitOfWork.Clientes.AddAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        // Registrar no histórico
        await _historicoService.RegistrarEventoAsync("Cliente", cliente.Id, "Criado", null, cliente, usuario);

        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CPF = cliente.CPF,
            DataCriacao = cliente.DataCriacao,
            DataAtualizacao = cliente.DataAtualizacao
        };
    }

    public async Task<ClienteDto?> UpdateAsync(Guid id, UpdateClienteDto updateClienteDto, string usuario)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null) return null;

        var dadosAntes = new { cliente.Nome, cliente.CPF };

        cliente.Nome = updateClienteDto.Nome;
        cliente.CPF = updateClienteDto.CPF;
        cliente.DataAtualizacao = DateTime.UtcNow;

        _unitOfWork.Clientes.Update(cliente);
        await _unitOfWork.SaveChangesAsync();

        var dadosDepois = new { cliente.Nome, cliente.CPF };

        // Registrar no histórico
        await _historicoService.RegistrarEventoAsync("Cliente", cliente.Id, "Atualizado", dadosAntes, dadosDepois, usuario);

        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CPF = cliente.CPF,
            DataCriacao = cliente.DataCriacao,
            DataAtualizacao = cliente.DataAtualizacao
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null) return false;

        // Verificar se o cliente possui pedidos
        var pedidos = await _unitOfWork.Pedidos.FindAsync(p => p.ClienteId == id);
        if (pedidos.Any())
        {
            throw new InvalidOperationException("Não é possível excluir o cliente pois ele possui um ou mais pedidos.");
        }

        _unitOfWork.Clientes.Delete(cliente);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
