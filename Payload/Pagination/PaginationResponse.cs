namespace APICacheWithRedis.Payload.Pagination{
    public class PaginationResponse{
        public int PageNumber{get;set;}
        public int PageSize {get;set;}
        public int TotalPage {get;set;}
        public int TotalElement{get;set;}
        public PaginationResponse(int PageNumber, int PageSize, int TotalPage, int TotalElement){
            this.PageNumber = PageNumber;
            this.PageSize = PageSize;
            this.TotalPage = TotalPage;
            this.TotalElement = TotalElement;
        }
    }
}