namespace access_control.core.Shared
{
    public class Paginator<T>
    {
        public static PaginationResult<T> GetPagedData(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            int totalItems = source.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PaginationResult<T>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
    }

    public class PaginationResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
