using System.Text.Json.Serialization;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClienteId { get; set; }
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;
    public StatusPedido Status { get; set; } = StatusPedido.Criado;
    public DateTime? DataAtualizacao { get; set; }
    
    // Navigation properties - JsonIgnore para evitar referências circulares em certos contextos
    [JsonIgnore]
    public Cliente Cliente { get; set; } = null!;
    
    [JsonIgnore]
    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
    
    // Calculated properties
    public decimal ValorTotal => Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
    
    // Business logic methods
    public bool PodePagar() => Status == StatusPedido.Criado;
    public bool PodeCancelar() => Status == StatusPedido.Criado;
    
    public void Pagar()
    {
        if (!PodePagar())
            throw new InvalidOperationException("Pedido não pode ser pago no status atual.");
        
        Status = StatusPedido.Pago;
        DataAtualizacao = DateTime.UtcNow;
    }
    
    public void Cancelar()
    {
        if (!PodeCancelar())
            throw new InvalidOperationException("Pedido não pode ser cancelado no status atual.");
        
        Status = StatusPedido.Cancelado;
        DataAtualizacao = DateTime.UtcNow;
    }
}
