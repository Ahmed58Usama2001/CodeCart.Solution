using CodeCart.Core.Entities;

namespace CodeCart.Core.Specifications.ProductSpecs;

public class ProductSpecifications:BaseSpecifications<Product>
{
    public ProductSpecifications(ProductSpecificationsParams specificationParams):
        base(p=>
        (string.IsNullOrEmpty(specificationParams.brand) || p.Brand == specificationParams.brand) &&
        (string.IsNullOrEmpty(specificationParams.type) || p.Type == specificationParams.type) 
        )
    {
        if (!string.IsNullOrEmpty(specificationParams.sort))
        {
            switch (specificationParams.sort)
            {
                case "priceAsc":
                    AddOrderBy(p => p.Price);
                    break;

                case "priceDesc":
                    AddOrderByDesc(p => p.Price);
                    break;

                default:
                    AddOrderBy(p => p.Name);
                    break;
            }
        }
        else
            AddOrderBy(p => p.Name);

        ApplyPagination(specificationParams.PageSize * (specificationParams.PageIndex - 1), specificationParams.PageSize);
    }
}
