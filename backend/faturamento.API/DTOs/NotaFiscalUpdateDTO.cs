using System.ComponentModel.DataAnnotations;
using faturamento.API.Models;

namespace faturamento.API.DTOs;
public class NotaFiscalUpdateDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int QuantidadeTotal {get; set;}

    [EnumDataType(typeof(StatusNotaFiscal), ErrorMessage = "Status inválido.")]
    public int Status {get; set;}
    
    
}