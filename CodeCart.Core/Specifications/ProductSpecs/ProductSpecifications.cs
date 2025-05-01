using CodeCart.Core.Entities;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications.ProductSpecs;

public class ProductSpecifications : BaseSpecifications<Product>
{
    public ProductSpecifications(ProductSpecificationsParams specParams)
        : base(GetCriteria(specParams))
    {
        ApplyOrdering(specParams);
        ApplyPagination(specParams);
    }

    private static Expression<Func<Product, bool>> GetCriteria(ProductSpecificationsParams specParams)
    {
        return p =>
            (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
            (specParams.Brands == null || !specParams.Brands.Any() || specParams.Brands.Contains(p.Brand.ToLower())) &&
            (specParams.Types == null || !specParams.Types.Any() || specParams.Types.Contains(p.Type.ToLower()));
    }

    private void ApplyOrdering(ProductSpecificationsParams specParams)
    {
        if (!string.IsNullOrEmpty(specParams.sort))
        {
            switch (specParams.sort.ToLower())
            {
                case "priceasc":
                    AddOrderBy(p => p.Price);
                    break;
                case "pricedesc":
                    AddOrderByDesc(p => p.Price);
                    break;
                default:
                    AddOrderBy(p => p.Name);
                    break;
            }
        }
        else
        {
            AddOrderBy(p => p.Name);
        }
    }

    private void ApplyPagination(ProductSpecificationsParams specParams)
    {
        ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
    }
}