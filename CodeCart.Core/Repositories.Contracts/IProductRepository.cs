using CodeCart.Core.Entities;

namespace CodeCart.Core.Repositories.Contracts;
public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyList<string>> GetProductBrandsAsync();
    Task<IReadOnlyList<string>> GetProductTypesAsync();
}