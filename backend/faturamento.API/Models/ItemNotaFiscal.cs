namespace faturamento.API.Models;

public class ItemNotaFiscal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NotaFiscalId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public string ProdutoNome {get; set;} = string.Empty;
    public decimal Preco {get; set;}
}