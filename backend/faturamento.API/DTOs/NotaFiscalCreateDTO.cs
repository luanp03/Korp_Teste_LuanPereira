using System.ComponentModel.DataAnnotations;
namespace faturamento.API.DTOs;

public class NotaFiscalCreateDTO
{

    public List<ItemNotaFiscalDTO> Itens {get; set;} = new();
}

public class ItemNotaFiscalDTO
{
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome {get; set;} = string.Empty;

    //[Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
    public int Quantidade {get; set;}
    //[Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco {get; set;}
}