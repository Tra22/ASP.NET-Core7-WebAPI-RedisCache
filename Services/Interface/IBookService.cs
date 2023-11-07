using APICacheWithRedis.Dtos;
using APICacheWithRedis.Payload;
using APICacheWithRedis.Payload.Global;
using APICacheWithRedis.Payload.Search;

namespace APICacheWithRedis.Services {
    public interface IBookService {
        Task<Response<ListWithPagination<List<BookDto>>>> GetBooksAsync(SearchParams searchParams);
        Task<Response<BookDto>> GetByIdAsync(int BookId);
        Task<Response<BookDto>> AddBookAsync(CreateBookDto createBookDto);        
        Task<Response<BookDto>> UpdateBookAsync(UpdateBookDto updateBookDto);
        Task<Response<string>> SoftDeleteBookAsync(int BookId);
    }
}