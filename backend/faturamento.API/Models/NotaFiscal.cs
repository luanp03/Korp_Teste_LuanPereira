using System.Collections.Generic;

namespace faturamento.API.Models;

public enum StatusNotaFiscal { Aberta = 0, Fechada = 1 }

public class NotaFiscal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Numero { get; set; }
    public int QuantidadeTotal { get; set; } 
    public StatusNotaFiscal Status { get; set; } = StatusNotaFiscal.Aberta;
    public DateTime DataEmissao {get; set; } = DateTime.Now;
    public List<ItemNotaFiscal> Itens { get; set; } = new(); 
}