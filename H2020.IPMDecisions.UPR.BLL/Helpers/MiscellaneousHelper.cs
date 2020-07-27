using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class MiscellaneousHelper
    {
        public static PaginationMetaData CreatePaginationMetadata<T>(PagedList<T> pagedList)
        {
            if (pagedList is null) return null;

            return new PaginationMetaData()
            {
                TotalCount = pagedList.TotalCount,
                PageSize = pagedList.PageSize,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                IsFirstPage = pagedList.IsFirstPage,
                IsLastPage = pagedList.IsLastPage
            };
        }
    }
}