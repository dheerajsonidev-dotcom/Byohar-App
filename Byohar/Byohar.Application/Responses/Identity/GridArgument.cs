using Byohar.Application.Requests;
using Byohar.Shared.Wrapper;
using MediatR;

namespace Byohar.Application.Responses.Identity
{
    public class GridArgument : PagedRequest, IRequest<PaginatedResult<UserGridResponse>>
    {
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int TotalRecords { get; set; }
        public string Status { get; set; }
    }
}
