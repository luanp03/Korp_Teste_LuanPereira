using System.ComponentModel.DataAnnotations;

namespace estoque.API.DTOs;
public class ProdutoCreateDTO
{
    [Required (ErrorMessage ="O código é obrigatório,")]
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Saldo { get; set; }
}