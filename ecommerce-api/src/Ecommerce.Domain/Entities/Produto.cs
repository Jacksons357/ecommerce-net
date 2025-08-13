namespace Ecommerce.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
    
    // Navigation properties
    public ICollection<PedidoItem> PedidoItens { get; set; } = new List<PedidoItem>();
}
