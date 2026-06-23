using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>(); // <-- ЭТОТ РЯДОК ОБЯЗАТЕЛЕН

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Ноутбук", Price = 25000 },
            new Product { Id = 2, Name = "Бездротова миша", Price = 800 }
        );
    }
}