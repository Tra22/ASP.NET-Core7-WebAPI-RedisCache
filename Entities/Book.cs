using System.ComponentModel.DataAnnotations;

namespace APICacheWithRedis.Entities{
    public class Book {
        [Key]
        public int Id {get;set;}
        [Required]
        [StringLength(200, MinimumLength =2, ErrorMessage ="The {0} must be at lease {2} and at max {1} characters long.")]
        public required string Name {get;set;}
        public decimal Price {get;set;}
        public bool IsDeleted {get;set;}
    }
}