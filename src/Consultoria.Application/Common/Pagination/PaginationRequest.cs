using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Common.Pagination
{
    public class PaginationRequest
    {
        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 10;

        public string? SortBy { get; init; }

        public string SortDir { get; init; } = "asc";

        public int Offset => (Page - 1) * PageSize;
    }
}
