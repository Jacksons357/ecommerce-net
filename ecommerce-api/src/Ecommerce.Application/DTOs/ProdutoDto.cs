namespace Ecommerce.Application.DTOs;

public class CreateProdutoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
}

public class UpdateProdutoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
}

public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
