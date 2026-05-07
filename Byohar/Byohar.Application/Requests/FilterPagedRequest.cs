namespace Byohar.Application.Requests
{
    public class FilterPagedRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string StringSearch { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        public List<Guid>? BuildingIds { get; set; }
        public List<Guid>? SiteIds { get; set; }
        public DateTime? From { get; set; }
        public virtual DateTime? To { get; set; }
        public bool IsHealthRequired { get; set; } = false;

    }
}
