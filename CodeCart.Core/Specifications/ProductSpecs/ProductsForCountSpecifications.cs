using CodeCart.Core.Entities;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications.ProductSpecs;

public class ProductsForCountSpecifications : BaseSpecifications<Product>
{
    public ProductsForCountSpecifications(ProductSpecificationsParams specParams)
        : base(GetCriteria(specParams))
    {
    }




    private static Expression<Func<Product, bool>> GetCriteria(ProductSpecificationsParams specParams)
    {
        return p =>
            (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
            (specParams.Brands == null || !specParams.Brands.Any() || specParams.Brands.Contains(p.Brand.ToLower())) &&
            (specParams.Types == null || !specParams.Types.Any() || specParams.Types.Contains(p.Type.ToLower()));
    }
}
