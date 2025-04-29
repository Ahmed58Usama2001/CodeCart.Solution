namespace CodeCart.Core.Specifications.ProductSpecs;

public class ProductSpecificationsParams
{
    public string? sort { get; set; }

    private List<string>? _brands;
    public List<string>? Brands
    {
        get => _brands;
        set => _brands = ProcessListInput(value);
    }

    private List<string>? _types;
    public List<string>? Types
    {
        get => _types;
        set => _types = ProcessListInput(value);
    }

    private static List<string>? ProcessListInput(List<string>? input)
    {
        if (input == null) return null;

        // Handle both cases:
        // 1. When ASP.NET Core binds multiple values (["Nike", "Adidas"])
        // 2. When a single comma-separated string is passed (["Nike,Adidas"])
        return input
            .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Trim().ToLower())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();
    }

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
