using H2020.IPMDecisions.UPR.Core.Helpers;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class PaginationMetaData : IPagedList
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
    }
}