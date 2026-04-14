using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using faturamento.API.Data;
using faturamento.API.Models;
using faturamento.API.DTOs;
using faturamento.API.Services;
using System.Data.Common;
using System.ComponentModel;

namespace faturamento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotasFiscaisController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EstoqueService _estoqueService;

    public NotasFiscaisController(AppDbContext context, EstoqueService estoqueService)
    {
        _context = context;
        _estoqueService = estoqueService;
    }

    [HttpGet] //Metodo GET NOTAS
    public async Task<ActionResult<IEnumerable<NotaFiscalResponseDTO>>> GetNotas()
    {
        var notas = await _context.NotasFiscais
            .Include(n => n.Itens) 
            .OrderByDescending(n => n.Numero)
            .ToListAsync();

        var response = notas.Select(n => new NotaFiscalResponseDTO
        {
            Id = n.Id,
            Numero = n.Numero,
            QuantidadeTotal = n.QuantidadeTotal,
            Status = n.Status.ToString(),
            DataEmissao = n.DataEmissao,
            QuantidadeItens = n.Itens.Count,
            Itens = n.Itens.Select(i => new ItemNotaFiscalDTO {
                ProdutoCodigo = i.ProdutoCodigo,
                Quantidade = i.Quantidade,
                ProdutoNome = i.ProdutoNome
            }).ToList()
        });
        return Ok(response);
    }

    [HttpGet("proximo-numero")] //controle do proximo N° da nota
    public async Task<ActionResult<int>> GetProximoNumero()
    {
        var ultimoNumero = await _context.NotasFiscais
            .OrderByDescending(n => n.Numero)
            .Select(n => n.Numero)
            .FirstOrDefaultAsync();

        return Ok(ultimoNumero + 1);
    }

    [HttpPost] //metodo POST NOTAS
    public async Task<ActionResult<NotaFiscalResponseDTO>> PostNota(NotaFiscalCreateDTO dto)
    {
        var ultimoNumeroNota = await _context.NotasFiscais
            .OrderByDescending(n => n.Numero)
            .Select(n => n.Numero)
            .FirstOrDefaultAsync();

        var proximoNumero = ultimoNumeroNota + 1;
        var itensNota = new List<ItemNotaFiscal>();
        foreach (var item in dto.Itens)
        {
            var produtoNoEstoque = await _estoqueService.ValidarProdutoEstoque(item.ProdutoCodigo?.Trim() ?? "");

            if (produtoNoEstoque == null)
            {
                return BadRequest($"O produto '{item.ProdutoCodigo}' não foi encontrado no estoque.");
            }

            if (dto.Itens.Any(i => i.Quantidade <= 0)) 
                {
                    return BadRequest("A quantidade de todos os itens deve ser maior que zero.");
                }

            itensNota.Add(new ItemNotaFiscal
            {
                ProdutoNome = item.ProdutoNome ?? produtoNoEstoque.Descricao ?? "Sem Descrição",
                ProdutoCodigo = produtoNoEstoque.Codigo,
                Quantidade = item.Quantidade,
                Preco = item.Preco
            });
        }

        var itensParaBaixar = dto.Itens.Select(i => new ItemBaixaDTO(i.ProdutoCodigo, i.Quantidade)).ToList();
    
        var sucessoEstoque = await _estoqueService.BaixarEstoqueRemoto(itensParaBaixar);
        if (!sucessoEstoque) return BadRequest("Saldo insuficiente no estoque para gerar a nota.");

        var novaNota = new NotaFiscal
        {
            Id = Guid.NewGuid(),
            Numero = proximoNumero,
            QuantidadeTotal = itensNota.Sum(i => i.Quantidade), 
            DataEmissao = DateTime.Now,
            Status = StatusNotaFiscal.Aberta,
            Itens = itensNota
        };

        _context.NotasFiscais.Add(novaNota);
        await _context.SaveChangesAsync();

        var response = new NotaFiscalResponseDTO
        {
            Id = novaNota.Id,
            Numero = novaNota.Numero,
            QuantidadeTotal = novaNota.QuantidadeTotal,
            Status = novaNota.Status.ToString(),
            DataEmissao = novaNota.DataEmissao,
            QuantidadeItens = novaNota.Itens.Count,
            Itens = novaNota.Itens.Select(i => new ItemNotaFiscalDTO 
            { 
                ProdutoCodigo = i.ProdutoCodigo, 
                Quantidade = i.Quantidade,
                ProdutoNome = i.ProdutoNome,
                Preco = i.Preco
            }).ToList()
        };

        return CreatedAtAction("GetNotas", new { id = response.Id }, response);
    }

    
    [HttpPut("{id}")] //metodo PUT NOTAS por ID
    public async Task <IActionResult> PutNota(Guid id, NotaFiscalUpdateDTO dto)
    {
        var nota = await _context.NotasFiscais.FindAsync(id);
        
        if (!Enum.IsDefined(typeof(StatusNotaFiscal), dto.Status))
        {
            return BadRequest("O status informado é inválido. Use 0 para Aberto ou 1 para Fechado.");
        }

        if (dto.QuantidadeTotal <= 0)
        {
            return BadRequest("A quantidade total não pode ser zero ou negativa.");
        }
        if(nota ==null)
        {
            return NotFound("Nota Fiscal não encontrada.");
        }

        nota.QuantidadeTotal = dto.QuantidadeTotal;
        nota.Status = (StatusNotaFiscal)dto.Status;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest("Erro ao atualizar os dados.");
        }

        return NoContent();
    }


    [HttpDelete("{id}")] //metodo DELETE NOTAS por ID
    public async Task<IActionResult> DeleteNota(Guid id)
    {
        var nota = await _context.NotasFiscais
        .Include(n => n.Itens) 
        .FirstOrDefaultAsync(n => n.Id == id);

        if (nota==null)
        {
            return NotFound("Nota Fiscal não encontrada.");
        }

        var itensParaEstornar = nota.Itens.Select(i => new ItemBaixaDTO(i.ProdutoCodigo, i.Quantidade)).ToList();
        var sucessoEstorno = await _estoqueService.EstornarEstoqueRemoto(itensParaEstornar);
        if (!sucessoEstorno)
        {
            return BadRequest("Falha ao estornar estoque do produto(s). Nota não foi excluída.");
        }

        _context.NotasFiscais.Remove(nota);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/imprimir")]
    public async Task<IActionResult> ImprimirNota(Guid id)
    {
        try
        {
            var nota = await _context.NotasFiscais.FirstOrDefaultAsync(n => n.Id == id);
            
            if (nota == null) return NotFound("Nota não encontrada.");
            
            if (nota.Status == StatusNotaFiscal.Fechada) 
                return BadRequest("Esta nota já foi impressa e está fechada.");

            nota.Status = StatusNotaFiscal.Fechada;
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Nota impressa com sucesso!", status = "Fechada" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao processar a impressão: {ex.Message}");
        }
    }
}