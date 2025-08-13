using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;

namespace Ecommerce.Tests.Services;

public class PedidoServiceTests : IDisposable
{
    private readonly EcommerceDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IHistoricoService> _mockHistoricoService;
    private readonly PedidoService _pedidoService;

    public PedidoServiceTests()
    {
        var options = new DbContextOptionsBuilder<EcommerceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EcommerceDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
        _mockHistoricoService = new Mock<IHistoricoService>();
        _pedidoService = new PedidoService(_unitOfWork, _mockHistoricoService.Object);
    }

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveCriarPedido()
    {
        // Arrange
        var cliente = new Cliente { Nome = "João Silva", CPF = "123.456.789-00" };
        var produto1 = new Produto { Nome = "Produto 1", Preco = 100.00m };
        var produto2 = new Produto { Nome = "Produto 2", Preco = 50.00m };

        await _context.Clientes.AddAsync(cliente);
        await _context.Produtos.AddRangeAsync(produto1, produto2);
        await _context.SaveChangesAsync();

        var createPedidoDto = new CreatePedidoDto
        {
            ClienteId = cliente.Id,
            Itens = new List<CreatePedidoItemDto>
            {
                new() { ProdutoId = produto1.Id, Quantidade = 2 },
                new() { ProdutoId = produto2.Id, Quantidade = 1 }
            }
        };

        // Act
        var result = await _pedidoService.CreateAsync(createPedidoDto, "Teste");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cliente.Id, result.Cliente.Id);
        Assert.Equal(2, result.Itens.Count);
        Assert.Equal(250.00m, result.Total); // (100 * 2) + (50 * 1)
        Assert.Equal(StatusPedido.Criado, result.Status);

        // Verificar se o histórico foi registrado
        _mockHistoricoService.Verify(x => x.RegistrarEventoAsync(
            "Pedido", result.Id, "Criado", null, It.IsAny<object>(), "Teste"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ComClienteInexistente_DeveThrowArgumentException()
    {
        // Arrange
        var createPedidoDto = new CreatePedidoDto
        {
            ClienteId = Guid.NewGuid(),
            Itens = new List<CreatePedidoItemDto>
            {
                new() { ProdutoId = Guid.NewGuid(), Quantidade = 1 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _pedidoService.CreateAsync(createPedidoDto, "Teste"));
    }

    [Fact]
    public async Task CreateAsync_ComQuantidadeZero_DeveThrowArgumentException()
    {
        // Arrange
        var cliente = new Cliente { Nome = "João Silva", CPF = "123.456.789-00" };
        var produto = new Produto { Nome = "Produto 1", Preco = 100.00m };

        await _context.Clientes.AddAsync(cliente);
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();

        var createPedidoDto = new CreatePedidoDto
        {
            ClienteId = cliente.Id,
            Itens = new List<CreatePedidoItemDto>
            {
                new() { ProdutoId = produto.Id, Quantidade = 0 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _pedidoService.CreateAsync(createPedidoDto, "Teste"));
    }

    [Fact]
    public async Task PagarAsync_ComPedidoCriado_DeveAlterarStatusParaPago()
    {
        // Arrange
        var cliente = new Cliente { Nome = "João Silva", CPF = "123.456.789-00" };
        var produto = new Produto { Nome = "Produto 1", Preco = 100.00m };
        var pedido = new Pedido { ClienteId = cliente.Id, Status = StatusPedido.Criado };

        await _context.Clientes.AddAsync(cliente);
        await _context.Produtos.AddAsync(produto);
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();

        // Act
        var result = await _pedidoService.PagarAsync(pedido.Id, "Teste");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusPedido.Pago, result.Status);

        // Verificar se o histórico foi registrado
        _mockHistoricoService.Verify(x => x.RegistrarEventoAsync(
            "Pedido", pedido.Id, "StatusAlterado", 
            It.IsAny<object>(), It.IsAny<object>(), "Teste"), Times.Once);
    }

    [Fact]
    public async Task CancelarAsync_ComPedidoCriado_DeveAlterarStatusParaCancelado()
    {
        // Arrange
        var cliente = new Cliente { Nome = "João Silva", CPF = "123.456.789-00" };
        var produto = new Produto { Nome = "Produto 1", Preco = 100.00m };
        var pedido = new Pedido { ClienteId = cliente.Id, Status = StatusPedido.Criado };

        await _context.Clientes.AddAsync(cliente);
        await _context.Produtos.AddAsync(produto);
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();

        // Act
        var result = await _pedidoService.CancelarAsync(pedido.Id, "Teste");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusPedido.Cancelado, result.Status);

        // Verificar se o histórico foi registrado
        _mockHistoricoService.Verify(x => x.RegistrarEventoAsync(
            "Pedido", pedido.Id, "StatusAlterado", 
            It.IsAny<object>(), It.IsAny<object>(), "Teste"), Times.Once);
    }

    [Fact]
    public async Task PagarAsync_ComPedidoPago_DeveThrowInvalidOperationException()
    {
        // Arrange
        var cliente = new Cliente { Nome = "João Silva", CPF = "123.456.789-00" };
        var pedido = new Pedido { ClienteId = cliente.Id, Status = StatusPedido.Pago };

        await _context.Clientes.AddAsync(cliente);
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _pedidoService.PagarAsync(pedido.Id, "Teste"));
    }

    [Fact]
    public async Task CancelarAsync_ComPedidoPago_DeveThrowInvalidOperationException()
    {
        // Arrange
        var cliente = new Cliente { Nome = "João Silva", CPF = "123.456.789-00" };
        var pedido = new Pedido { ClienteId = cliente.Id, Status = StatusPedido.Pago };

        await _context.Clientes.AddAsync(cliente);
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _pedidoService.CancelarAsync(pedido.Id, "Teste"));
    }

    [Fact]
    public async Task GetAllAsync_ComFiltroStatus_DeveRetornarPedidosCorretos()
    {
        // Arrange
        var cliente = new Cliente { Nome = "João Silva", CPF = "123.456.789-00" };
        var pedido1 = new Pedido { ClienteId = cliente.Id, Status = StatusPedido.Criado };
        var pedido2 = new Pedido { ClienteId = cliente.Id, Status = StatusPedido.Pago };
        var pedido3 = new Pedido { ClienteId = cliente.Id, Status = StatusPedido.Cancelado };

        await _context.Clientes.AddAsync(cliente);
        await _context.Pedidos.AddRangeAsync(pedido1, pedido2, pedido3);
        await _context.SaveChangesAsync();

        // Act
        var resultCriados = await _pedidoService.GetAllAsync(StatusPedido.Criado);
        var resultPagos = await _pedidoService.GetAllAsync(StatusPedido.Pago);

        // Assert
        Assert.Single(resultCriados);
        Assert.Single(resultPagos);
        Assert.Equal(StatusPedido.Criado, resultCriados.First().Status);
        Assert.Equal(StatusPedido.Pago, resultPagos.First().Status);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
