namespace Base.Public.DTO;

public class RestApiResponseWithPaging<TRestApiResponse>
{
    public int Page { get; set; }
    public int TotalPageCount { get; set; }
    public IEnumerable<TRestApiResponse> Data { get; set; } = default!;
}