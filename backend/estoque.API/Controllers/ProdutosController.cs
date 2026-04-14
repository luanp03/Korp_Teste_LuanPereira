using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using estoque.API.Data;
using estoque.API.Models;
using estoque.API.DTOs;

namespace estoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

   [HttpGet] //metodo GET
public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetProdutos()
{
    try
    {
        var produtos = await _context.Produtos.ToListAsync();
        
        var dtos = produtos.Select(p => new ProdutoResponseDTO
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Descricao = p.Descricao,   
            Saldo = p.Saldo,  
            Preco = 0              
        }).ToList();

        return Ok(dtos);
    }
    catch (Exception)
    {
        return StatusCode(500, "Erro ao consultar o estoque.");
    }
}

    [HttpGet("buscar-por-nome/{nome}")] //metodo GET por Nome
    public async Task<ActionResult<ProdutoResponseDTO>> GetPorNome(string nome)
    {
        try
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Descricao.ToLower() == nome.ToLower() || p.Codigo == nome);

            if (produto == null) return NotFound("Produto não encontrado.");

            return Ok(new ProdutoResponseDTO 
            { 
                Id = produto.Id, 
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo,
                Preco = produto.Preco
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro na integração de estoque.");
        }
    }

    [HttpPost] //metodo POST
public async Task<ActionResult<ProdutoResponseDTO>> PostProduto(ProdutoCreateDTO dto)
{
    var codigoExiste = await _context.Produtos
        .AnyAsync(p => p.Codigo == dto.Codigo);

    if (codigoExiste)
    {
        return BadRequest($"O código '{dto.Codigo}' já está cadastrado.");
    }
    try
    {
        if (dto.Saldo < 0) return BadRequest("O saldo não pode ser negativo.");

        var novoProduto = new Produto
        {
            Id = Guid.NewGuid(),
            Codigo = dto.Codigo,
            Descricao = dto.Descricao,
            Saldo = dto.Saldo
        };

        _context.Produtos.Add(novoProduto);
        await _context.SaveChangesAsync();

        var response = new ProdutoResponseDTO 
        { 
            Id = novoProduto.Id, 
            Descricao = novoProduto.Descricao,
            Saldo = novoProduto.Saldo,
            Preco = 0
        };

        return CreatedAtAction(nameof(GetPorNome), new {nome = response.Descricao}, response);
    }
    catch (Exception)
    {
        return StatusCode(500, "Erro ao cadastrar produto.");
    }
}

    [HttpPost("estornar")] //devolve saldo
    public async Task<IActionResult> EstornarEstoque([FromBody] List<ItemBaixaDTO> itens)
    {
        foreach (var item in itens)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Codigo == item.ProdutoCodigo);
            if (produto != null)
            {
                produto.Saldo += item.Quantidade;
            }
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id}")]  //metodo PUT
    public async Task<IActionResult> PutProduto(Guid id, ProdutoUpdateDTO dto)
    {
        try
        {
            var produto = await _context.Produtos.FindAsync(id);
            
            if (produto == null) 
                return NotFound("Produto não encontrado para atualização.");

            produto.Descricao = dto.Descricao;
            produto.Saldo = dto.Saldo;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("O produto foi alterado por outro utilizador. Atualize e tente novamente.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro ao atualizar o produto.");
        }
    }


    [HttpDelete("{id}")] //metodo DELETE por ID
    public async Task<IActionResult> DeleteProduto(Guid id)
    {
        try
        {
            var produto = await _context.Produtos.FindAsync(id);
            
            if (produto == null) 
                return NotFound("O produto já foi excluído ou não existe.");

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception)
        {

            return StatusCode(500, "Erro ao excluir o produto. Verifique se ele não possui vínculos ativos.");
        }
    }
    [HttpPatch("baixar-estoque")] //metodo baixa do estoque
    public async Task<IActionResult> BaixarEstoque([FromBody] List<ItemBaixaDTO> itens)
    {
        foreach (var item in itens)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Codigo == item.ProdutoCodigo);
            if (produto == null) return NotFound($"Produto {item.ProdutoCodigo} não encontrado.");
            
            if (produto.Saldo < item.Quantidade)
                return BadRequest($"Saldo insuficiente para o produto {produto.Descricao}.");
 
            produto.Saldo -= item.Quantidade;
        }

        await _context.SaveChangesAsync();
        return Ok();
}
    public record ItemBaixaDTO(string ProdutoCodigo, int Quantidade);
}