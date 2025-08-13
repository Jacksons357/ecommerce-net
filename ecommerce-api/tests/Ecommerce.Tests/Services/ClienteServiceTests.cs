using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;

namespace Ecommerce.Tests.Services;

public class ClienteServiceTests : IDisposable
{
    private readonly EcommerceDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IHistoricoService> _mockHistoricoService;
    private readonly ClienteService _clienteService;

    public ClienteServiceTests()
    {
        var options = new DbContextOptionsBuilder<EcommerceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EcommerceDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
        _mockHistoricoService = new Mock<IHistoricoService>();
        _clienteService = new ClienteService(_unitOfWork, _mockHistoricoService.Object);
    }

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveCriarCliente()
    {
        // Arrange
        var createClienteDto = new CreateClienteDto
        {
            Nome = "João Silva",
            CPF = "123.456.789-00"
        };

        // Act
        var result = await _clienteService.CreateAsync(createClienteDto, "Teste");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createClienteDto.Nome, result.Nome);
        Assert.Equal(createClienteDto.CPF, result.CPF);
        Assert.True(result.DataCriacao <= DateTime.UtcNow);

        // Verificar se o histórico foi registrado
        _mockHistoricoService.Verify(x => x.RegistrarEventoAsync(
            "Cliente", result.Id, "Criado", null, It.IsAny<object>(), "Teste"), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizarCliente()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            CPF = "123.456.789-00"
        };

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        var updateClienteDto = new UpdateClienteDto
        {
            Nome = "João Santos",
            CPF = "123.456.789-01"
        };

        // Act
        var result = await _clienteService.UpdateAsync(cliente.Id, updateClienteDto, "Teste");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateClienteDto.Nome, result.Nome);
        Assert.Equal(updateClienteDto.CPF, result.CPF);
        Assert.NotNull(result.DataAtualizacao);

        // Verificar se o histórico foi registrado
        _mockHistoricoService.Verify(x => x.RegistrarEventoAsync(
            "Cliente", cliente.Id, "Atualizado", 
            It.IsAny<object>(), It.IsAny<object>(), "Teste"), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        var updateClienteDto = new UpdateClienteDto
        {
            Nome = "João Santos",
            CPF = "123.456.789-01"
        };

        // Act
        var result = await _clienteService.UpdateAsync(Guid.NewGuid(), updateClienteDto, "Teste");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdExistente_DeveRetornarCliente()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            CPF = "123.456.789-00"
        };

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _clienteService.GetByIdAsync(cliente.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cliente.Id, result.Id);
        Assert.Equal(cliente.Nome, result.Nome);
        Assert.Equal(cliente.CPF, result.CPF);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Act
        var result = await _clienteService.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosClientes()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            new() { Nome = "João Silva", CPF = "123.456.789-00" },
            new() { Nome = "Maria Santos", CPF = "123.456.789-01" },
            new() { Nome = "Pedro Costa", CPF = "123.456.789-02" }
        };

        await _context.Clientes.AddRangeAsync(clientes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _clienteService.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, c => c.Nome == "João Silva");
        Assert.Contains(result, c => c.Nome == "Maria Santos");
        Assert.Contains(result, c => c.Nome == "Pedro Costa");
    }

    [Fact]
    public async Task DeleteAsync_ComIdExistente_DeveExcluirCliente()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "João Silva",
            CPF = "123.456.789-00"
        };

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _clienteService.DeleteAsync(cliente.Id);

        // Assert
        Assert.True(result);
        
        var clienteExcluido = await _context.Clientes.FindAsync(cliente.Id);
        Assert.Null(clienteExcluido);
    }

    [Fact]
    public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
    {
        // Act
        var result = await _clienteService.DeleteAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
