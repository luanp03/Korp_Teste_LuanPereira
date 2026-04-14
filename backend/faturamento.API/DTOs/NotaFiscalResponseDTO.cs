namespace faturamento.API.DTOs;

public class NotaFiscalResponseDTO
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public int QuantidadeTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public int QuantidadeItens { get; set; }
    public List<ItemNotaFiscalDTO> Itens { get; set; } = new();
}
