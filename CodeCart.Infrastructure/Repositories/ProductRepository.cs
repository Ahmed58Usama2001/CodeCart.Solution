using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeCart.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly StoreContext _context;

    public ProductRepository(StoreContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<string>> GetProductBrandsAsync()
    {
        return await _context.Products
            .Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetProductTypesAsync()
    {
        return await _context.Products
            .Select(p => p.Type)
            .Distinct()
            .ToListAsync();
    }
}
