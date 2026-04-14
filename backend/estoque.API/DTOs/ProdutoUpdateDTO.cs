namespace estoque.API.DTOs;

public class ProdutoUpdateDTO
{
    public string Descricao {get; set;} = string.Empty;
    public int Saldo {get; set;}
}