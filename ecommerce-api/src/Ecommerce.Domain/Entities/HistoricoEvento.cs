namespace Ecommerce.Domain.Entities;

public class HistoricoEvento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Entidade { get; set; } = string.Empty;
    public Guid EntidadeId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? DadosAntes { get; set; }
    public string? DadosDepois { get; set; }
    public DateTime DataOcorrencia { get; set; } = DateTime.UtcNow;
    public string Usuario { get; set; } = string.Empty;
}
