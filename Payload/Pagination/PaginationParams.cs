namespace APICacheWithRedis.Payload.Pagination{
    public class PaginationParams
    {
        private const int MaxPageSize = 500;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 6;
        public int PageSize { get { return _pageSize; } set { _pageSize = value > MaxPageSize ? MaxPageSize : value; } }
    }
}