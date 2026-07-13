using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Common.Pagination
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyCollection<T> Items { get; }

        public int TotalCount { get; }

        public int Page { get; }

        public int PageSize { get; }

        public int TotalPages { get; }

        public PagedResult(
            IEnumerable<T> items,
            int totalCount,
            int page,
            int pageSize)
        {
            Items = items.ToList().AsReadOnly();
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;

            TotalPages = pageSize > 0
                ? (int)Math.Ceiling(totalCount / (double)pageSize)
                : 0;
        }
    }
}
