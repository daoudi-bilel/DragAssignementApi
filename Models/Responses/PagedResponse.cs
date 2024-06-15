namespace DragAssignementApi.Models.Responses
{
    public class PagedResponse<T>
{
    public IEnumerable<T> Content { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int page { get; set; }
    public int size { get; set; }
}
}