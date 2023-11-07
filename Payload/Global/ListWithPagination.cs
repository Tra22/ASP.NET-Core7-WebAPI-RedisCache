using APICacheWithRedis.Payload.Pagination;

namespace APICacheWithRedis.Payload.Global{
    public class ListWithPagination<T>{
        public T? List { get; set; }
        public PaginationResponse? Pagination { get; set; }
    }
}