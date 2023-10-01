namespace backend.Data;

public class SearchParameters
{
    private int _pageSize = 2;
    private const int MAX_PAGE_SIZE = 50;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < MAX_PAGE_SIZE ? value : MAX_PAGE_SIZE;
    }
}