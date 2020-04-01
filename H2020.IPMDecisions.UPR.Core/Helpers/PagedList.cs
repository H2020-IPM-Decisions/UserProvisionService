using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Core.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(List<T> items, int currentPage, int pageSize, int count)
        {
            this.CurrentPage = currentPage;
            this.PageSize = pageSize;
            this.TotalCount = count;
            this.TotalPages = (int)System.Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedList<T>(items, pageNumber, pageSize, count);
        }
    }
}