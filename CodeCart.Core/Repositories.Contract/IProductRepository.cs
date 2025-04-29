using CodeCart.Core.Entities;

namespace CodeCart.Core.Repositories.Contract;
public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyList<string>> GetProductBrandsAsync();
    Task<IReadOnlyList<string>> GetProductTypesAsync();
}