using System.Net.Http.Json;
using System.Text.Json;

namespace faturamento.API.Services;

public class ProdutoEstoqueDTO
{
    public Guid Id {get; set;}
    public string Descricao {get; set;} = string.Empty;
    public string Codigo {get; set;} = string.Empty;
    public int Saldo {get; set;}
    }
public class ItemBaixaDTO
{
    public string ProdutoCodigo { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 0;
    
    public ItemBaixaDTO(string produtoCodigo, int quantidade)
    {
        ProdutoCodigo = produtoCodigo;
        Quantidade = quantidade;
    }
}
public class EstoqueService
{
    private readonly HttpClient _HttpClient;
    public EstoqueService (HttpClient HttpClient)
    {
        _HttpClient = HttpClient;
        _HttpClient.BaseAddress = new Uri("http://localhost:5083");
    }
    public async Task<ProdutoEstoqueDTO?> ValidarProdutoEstoque(string nome)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            return await _HttpClient.GetFromJsonAsync<ProdutoEstoqueDTO>($"api/Produtos/buscar-por-nome/{nome}", options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na comunicação: {ex.Message}");
            return null;

        }
    }
    public async Task<bool> BaixarEstoqueRemoto(List<ItemBaixaDTO> itens)
    {
        var response = await _HttpClient.PatchAsJsonAsync("api/Produtos/baixar-estoque", itens);
        return response.IsSuccessStatusCode;
    }
public async Task<bool> EstornarEstoqueRemoto(List<ItemBaixaDTO> itens)
    {
        var response = await _HttpClient.PostAsJsonAsync("api/Produtos/estornar", itens);
        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"FALHA NO ESTORNO: {response.StatusCode} - {msg}");
        }
        return response.IsSuccessStatusCode;
    }
}