using Microsoft.EntityFrameworkCore;

namespace SignalFlow.Example.Infrastructure.Dal.Postgres;

public class ProductContext : DbContext
{
    public DbSet<Product.Product> Products { get; set; }

    public ProductContext(DbContextOptions<ProductContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product.Product>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Product.Product>()
            .HasIndex(x => x.Tittle)
            .IsUnique();

        modelBuilder.Entity<Product.Product>()
            .Property(x => x.Cost)
            .IsRequired();
    }
}