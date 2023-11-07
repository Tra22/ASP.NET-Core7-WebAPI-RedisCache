using APICacheWithRedis.Payload.Pagination;

namespace APICacheWithRedis.Payload.Search{
    public class SearchParams: PaginationParams
    {        
        public string? OrderBy { get; set; }
        public string? SearchTerm { get; set; }
        public string? ColumnFilters { get; set; }
    }
}