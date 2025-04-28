namespace CodeCart.Core.Specifications.ProductSpecs;

public class ProductSpecificationsParams
{
    public string? sort { get; set; }

    public string? brand { get; set; }

    public string? type { get; set; }

    private string? search;

    public string? Search
    {
        get { return search; }
        set { search = value?.ToLower(); }
    }

    private const int maxPageSize = 10;
	private int pageSize=5;

	public int PageSize
	{
		get { return pageSize; }
		set { pageSize = value>maxPageSize?maxPageSize:value; }
	}

	public int PageIndex { get; set; } = 1;

}
