namespace Byohar.Application.Requests
{
    public class PagedRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string StringSearch { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}