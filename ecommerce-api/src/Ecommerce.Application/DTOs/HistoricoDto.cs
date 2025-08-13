namespace Ecommerce.Application.DTOs;

public class HistoricoEventoDto
{
    public Guid Id { get; set; }
    public string Entidade { get; set; } = string.Empty;
    public Guid EntidadeId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? DadosAntes { get; set; }
    public string? DadosDepois { get; set; }
    public DateTime DataOcorrencia { get; set; }
    public string Usuario { get; set; } = string.Empty;
}
