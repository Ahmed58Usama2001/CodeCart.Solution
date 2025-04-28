namespace CodeCart.Core.Specifications.ProductSpecs;

public class ProductSpecificationsParams
{
    public string? sort { get; set; }

    public string? brand { get; set; }

    public string? type { get; set; }

    private const int maxPageSize = 10;
	private int pageSize=5;

	public int PageSize
	{
		get { return pageSize; }
		set { pageSize = value>maxPageSize?maxPageSize:value; }
	}

	public int PageIndex { get; set; } = 1;

}
