using Microsoft.EntityFrameworkCore;
using estoque.API.Models;

namespace estoque.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
}