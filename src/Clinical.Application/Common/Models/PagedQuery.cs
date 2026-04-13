namespace Clinical.Application.Common.Models;

public abstract record PagedQuery(int PageNumber = 1, int PageSize = 10, string? SortBy = null, bool Descending = false);
