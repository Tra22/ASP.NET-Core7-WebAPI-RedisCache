using APICacheWithRedis.Entities;
using APICacheWithRedis.Payload.Global;
using APICacheWithRedis.Payload.Search;

namespace APICacheWithRedis.Repositories.Interface{
    public interface IBookRepository {
        Task<ICollection<Book>> GetAllBooksAsync();
        Task<ListWithPagination<ICollection<Book>>> GetBooksAsync(SearchParams searchParams);
        Task<ICollection<Book>> GetDeletedBooksAsync();
        Task<Book> GetBookByIDAsync(int BookId);
        Task<bool> BookExistAsync(int BookId);
        Task<bool> CreateBookAsync(Book Book);
        Task<bool> UpdateBookAsync(Book Book);
        Task<bool> SoftDeleteBookAsync(int BookId);
        Task<bool> HardDeleteBookAsync(Book Book);
    }
}