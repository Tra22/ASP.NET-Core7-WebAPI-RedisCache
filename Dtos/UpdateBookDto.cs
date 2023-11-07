namespace APICacheWithRedis.Dtos {
    public class UpdateBookDto {
        public int Id {get;set;}
        public required string Name {get;set;}
        public decimal Price {get;set;}
        public bool IsDeleted {get;set;}        
    }
}