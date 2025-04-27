using System.Reflection;
using CodeCart.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeCart.Infrastructure.Data;

public class StoreContext:DbContext
{
  public StoreContext(DbContextOptions options) : base(options)
  {
    
  }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

  public DbSet<Product> Products { get; set; }
}
