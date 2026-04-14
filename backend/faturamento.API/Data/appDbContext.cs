using Microsoft.EntityFrameworkCore;
using faturamento.API.Models;

namespace faturamento.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NotaFiscal> NotasFiscais { get; set; }
    public DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; }
}