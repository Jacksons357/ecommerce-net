using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHistoricoService _historicoService;

    public PedidoService(IUnitOfWork unitOfWork, IHistoricoService historicoService)
    {
        _unitOfWork = unitOfWork;
        _historicoService = historicoService;
    }

    public async Task<IEnumerable<PedidoResumoDto>> GetAllAsync(StatusPedido? status = null)
    {
        var pedidos = await _unitOfWork.Pedidos.GetAllWithClienteAsync(status);

        return pedidos.Select(p => new PedidoResumoDto
        {
            Id = p.Id,
            ClienteNome = p.Cliente.Nome,
            DataPedido = p.DataPedido,
            Status = p.Status,
            Total = p.ValorTotal
        });
    }

    public async Task<PedidoDetalheDto?> GetByIdAsync(Guid id)
    {
        var pedido = await _unitOfWork.Pedidos.GetByIdWithDetailsAsync(id);

        if (pedido == null) return null;

        return new PedidoDetalheDto
        {
            Id = pedido.Id,
            Cliente = new ClienteDto
            {
                Id = pedido.Cliente.Id,
                Nome = pedido.Cliente.Nome,
                CPF = pedido.Cliente.CPF,
                DataCriacao = pedido.Cliente.DataCriacao,
                DataAtualizacao = pedido.Cliente.DataAtualizacao
            },
            Itens = pedido.Itens.Select(i => new PedidoItemDetalheDto
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto.Nome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList(),
            DataPedido = pedido.DataPedido,
            Status = pedido.Status,
            Total = pedido.ValorTotal
        };
    }

    public async Task<PedidoDetalheDto> CreateAsync(CreatePedidoDto createPedidoDto, string usuario)
    {
        // Validar cliente
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(createPedidoDto.ClienteId);
        if (cliente == null)
            throw new ArgumentException("Cliente não encontrado");

        // Validar produtos e quantidades
        var produtos = new List<Produto>();
        foreach (var itemDto in createPedidoDto.Itens)
        {
            if (itemDto.Quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");
                
            var produto = await _unitOfWork.Produtos.GetByIdAsync(itemDto.ProdutoId);
            if (produto == null)
                throw new ArgumentException($"Produto com ID {itemDto.ProdutoId} não foi encontrado");
                
            produtos.Add(produto);
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Criar pedido
            var pedido = new Pedido
            {
                ClienteId = createPedidoDto.ClienteId,
                Status = StatusPedido.Criado
            };

            await _unitOfWork.Pedidos.AddAsync(pedido);
            await _unitOfWork.SaveChangesAsync();

            // Criar itens do pedido
            foreach (var itemDto in createPedidoDto.Itens)
            {
                var produto = produtos.First(p => p.Id == itemDto.ProdutoId);
                var item = new PedidoItem
                {
                    PedidoId = pedido.Id,
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.Preco // Congelar preço no momento da criação
                };

                await _unitOfWork.PedidoItens.AddAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();

            // Registrar no histórico - usar dados básicos para evitar referências circulares
            var dadosHistorico = new
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                Status = pedido.Status.ToString(),
                DataPedido = pedido.DataPedido,
                QuantidadeItens = createPedidoDto.Itens.Count
            };
            
            await _historicoService.RegistrarEventoAsync("Pedido", pedido.Id, "Criado", null, dadosHistorico, usuario);

            await _unitOfWork.CommitTransactionAsync();

            // Buscar pedido completo para retorno
            return await GetByIdAsync(pedido.Id) ?? throw new InvalidOperationException("Erro ao buscar pedido criado");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<PedidoDetalheDto?> PagarAsync(Guid id, string usuario)
    {
        var pedido = await _unitOfWork.Pedidos.GetByIdAsync(id);
        if (pedido == null) return null;

        if (!pedido.PodePagar())
            throw new InvalidOperationException("Pedido não pode ser pago no status atual");

        var statusAntes = pedido.Status;
        
        pedido.Pagar();
        _unitOfWork.Pedidos.Update(pedido);
        await _unitOfWork.SaveChangesAsync();

        // Registrar no histórico
        await _historicoService.RegistrarEventoAsync("Pedido", pedido.Id, "StatusAlterado", 
            new { Status = statusAntes }, new { Status = pedido.Status }, usuario);

        return await GetByIdAsync(id);
    }

    public async Task<PedidoDetalheDto?> CancelarAsync(Guid id, string usuario)
    {
        var pedido = await _unitOfWork.Pedidos.GetByIdAsync(id);
        if (pedido == null) return null;

        if (!pedido.PodeCancelar())
            throw new InvalidOperationException("Pedido não pode ser cancelado no status atual");

        var statusAntes = pedido.Status;
        
        pedido.Cancelar();
        _unitOfWork.Pedidos.Update(pedido);
        await _unitOfWork.SaveChangesAsync();

        // Registrar no histórico
        await _historicoService.RegistrarEventoAsync("Pedido", pedido.Id, "StatusAlterado", 
            new { Status = statusAntes }, new { Status = pedido.Status }, usuario);

        return await GetByIdAsync(id);
    }
}
