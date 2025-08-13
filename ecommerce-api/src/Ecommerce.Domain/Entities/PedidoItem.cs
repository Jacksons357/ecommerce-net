using System.Text.Json.Serialization;

namespace Ecommerce.Domain.Entities;

public class PedidoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    
    // Navigation properties - JsonIgnore para evitar referências circulares
    [JsonIgnore]
    public Pedido Pedido { get; set; } = null!;
    
    [JsonIgnore]
    public Produto Produto { get; set; } = null!;
    
    // Calculated properties
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
