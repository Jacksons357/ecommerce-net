using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.DTOs;

public class CreatePedidoItemDto
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class CreatePedidoDto
{
    public Guid ClienteId { get; set; }
    public List<CreatePedidoItemDto> Itens { get; set; } = new();
}

public class PedidoResumoDto
{
    public Guid Id { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; }
    public StatusPedido Status { get; set; }
    public decimal Total { get; set; }
}

public class PedidoItemDetalheDto
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class PedidoDetalheDto
{
    public Guid Id { get; set; }
    public ClienteDto Cliente { get; set; } = null!;
    public List<PedidoItemDetalheDto> Itens { get; set; } = new();
    public DateTime DataPedido { get; set; }
    public StatusPedido Status { get; set; }
    public decimal Total { get; set; }
}
