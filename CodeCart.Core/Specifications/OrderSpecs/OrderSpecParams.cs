namespace CodeCart.Core.Specifications.OrderSpecs;

public class OrderSpecParams
{
    private const int maxPageSize = 20;
    private int pageSize = 5;

    public int PageSize
    {
        get { return pageSize; }
        set { pageSize = value > maxPageSize ? maxPageSize : value; }
    }

    public int PageIndex { get; set; } = 1;

    public string? Status { get; set; }
}
