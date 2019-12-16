using System.Collections.Generic;
using System.Linq;

namespace Quantis.WorkFlow.Services.Framework
{
    public class PagedList<T>
    {
        public IEnumerable<T> Source { get; set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRows { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1 && TotalPages > 0); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex < TotalPages); }
        }

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalRows)
        {
            TotalRows = totalRows;
            TotalPages = totalRows / pageSize;
            if (totalRows % pageSize > 0)
                TotalPages++;
            PageSize = pageSize;
            PageIndex = pageIndex;
            Source = source.ToList();
        }
    }
}