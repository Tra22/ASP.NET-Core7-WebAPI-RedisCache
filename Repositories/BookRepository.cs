using System.Linq.Expressions;
using System.Text.Json;
using APICacheWithRedis.Entities;
using APICacheWithRedis.Payload.Filter;
using APICacheWithRedis.Payload.Global;
using APICacheWithRedis.Payload.Pagination;
using APICacheWithRedis.Payload.Search;
using APICacheWithRedis.Payload.Sort;
using APICacheWithRedis.Repositories.Interface;
using APICacheWithRedis.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace APICacheWithRedis.Repositories{
    public class BookRepository : IBookRepository
    {
        private readonly ILogger<BookRepository> _logger;
        private readonly DataContext _context;
        private readonly IDistributedCache _cache;
        public BookRepository(ILogger<BookRepository> logger, DataContext context, IDistributedCache cache){
            this._logger = logger;
            this._context = context;
            this._cache = cache;
        }
        public async Task<bool> BookExistAsync(int BookId)
        {
            return await _context.Books.AnyAsync(Book => Book.Id == BookId);
        }

        public async Task<bool> CreateBookAsync(Book Book)
        {
            await _context.Books.AddAsync(Book);
            await Save();
            string cacheKey = $"book_{Book.Id}";
            //set cache option
            var cacheOptions = new DistributedCacheEntryOptions()
                                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            //add to cache
            await CacheUtils<Book>.SetCacheDataAsync(cacheKey, Book, _cache, cacheOptions);
            return true;
        }

        public async Task<ICollection<Book>> GetAllBooksAsync()
        {
            const string cacheKey = "books";
            List<Book>? Books = await CacheUtils<List<Book>>.GetCacheDataAsync(cacheKey, _cache);
            if(Books is not null){
                _logger.LogInformation($"Fetch data from cache with key: {cacheKey}");
                return Books;
            }else{
                Books = await _context.Books.ToListAsync();
                //set cache option
                var cacheOptions = new DistributedCacheEntryOptions()
                                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                //add to cache 
                await CacheUtils<List<Book>>.SetCacheDataAsync(cacheKey, Books, _cache, cacheOptions);
            }
            return Books??new List<Book>();
        }

        public async Task<Book> GetBookByIDAsync(int BookId)
        {
            string cacheKey = $"book_{BookId}";
            Book? Book = await CacheUtils<Book>.GetCacheDataAsync(cacheKey, _cache);
            if(Book is not null){
                _logger.LogInformation($"Fetch data from cache with key: {cacheKey}");
                return Book;
            }else{
                Book = await _context.Books.FirstAsync(Book => Book.Id == BookId);
                //set cache option
                var cacheOptions = new DistributedCacheEntryOptions()
                                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                //add to cache 
                await CacheUtils<Book>.SetCacheDataAsync(cacheKey, Book, _cache, cacheOptions);
            }
            return Book;
        }

        public async Task<ListWithPagination<ICollection<Book>>> GetBooksAsync(SearchParams searchParams)
        {
            string cacheKey = $"books_{searchParams.PageNumber}_{searchParams.PageSize}_order_{searchParams.OrderBy}_search_{searchParams.SearchTerm}_columnfilter_{searchParams.ColumnFilters}";
            ListWithPagination<ICollection<Book>>? listWithPagination = await CacheUtils<ListWithPagination<ICollection<Book>>>.GetCacheDataAsync(
                cacheKey, _cache
            );
            if(listWithPagination is not null) {
                _logger.LogInformation($"Fetch data from cache with key: {cacheKey}");
                return listWithPagination;
            }else{
                List<ColumnFilter> columnFilters = new List<ColumnFilter>();
                if (!String.IsNullOrEmpty(searchParams.ColumnFilters))
                {
                    try
                    {
                        var colFilters = JsonSerializer.Deserialize<List<ColumnFilter>>(searchParams.ColumnFilters);
                        if(colFilters != null) columnFilters.AddRange(colFilters);
                    }
                    catch (Exception)
                    {
                        columnFilters = new List<ColumnFilter>();
                    }
                }

                List<ColumnSorting> columnSorting = new List<ColumnSorting>();
                if (!String.IsNullOrEmpty(searchParams.OrderBy))
                {
                    try
                    {
                        var orderFilters = JsonSerializer.Deserialize<List<ColumnSorting>>(searchParams.OrderBy);
                        if(orderFilters != null) columnSorting.AddRange(orderFilters);
                    }
                    catch (Exception)
                    {
                        columnSorting = new List<ColumnSorting>();
                    }
                }

                Expression<Func<Book, bool>>? filters = null;
                //First, we are checking our SearchTerm. If it contains information we are creating a filter.
                var searchTerm = "";
                if (!string.IsNullOrEmpty(searchParams.SearchTerm))
                {
                    searchTerm = searchParams.SearchTerm.Trim().ToLower();
                    filters = x => x.Name != null && x.Name.ToLower().Contains(searchTerm);
                }
                // Then we are overwriting a filter if columnFilters has data.
                if (columnFilters.Count > 0)
                {
                    var customFilter = CustomExpressionFilter<Book>.CustomFilter(columnFilters, "books");
                    filters = customFilter;
                }

                var query = filters == null ? _context.Books : _context.Books.AsQueryable().CustomQuery(filters);
                var filteredData = await (
                        columnSorting.Count> 0 ?
                            query
                                .Where(Book => !Book.IsDeleted)
                                .SortBy(columnSorting)
                                .CustomPagination(searchParams.PageNumber, searchParams.PageSize)
                                .ToListAsync():
                            query
                                .Where(Book => !Book.IsDeleted)
                                .OrderBy(Book => Book.Id)
                                .CustomPagination(searchParams.PageNumber, searchParams.PageSize)
                                .ToListAsync()
                    );
                var count = query.Count();
                var totalPage = Math.Floor((Decimal)count / searchParams.PageSize);
                _logger.LogInformation("Query {@Query}", query);
                PaginationResponse paginationResponse = new PaginationResponse(searchParams.PageNumber, searchParams.PageSize, (int) totalPage, count);
                listWithPagination = new ListWithPagination<ICollection<Book>> { List = filteredData, Pagination = paginationResponse };

                var cacheOptions = new DistributedCacheEntryOptions()
                                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                //add to cache 
                await CacheUtils<ListWithPagination<ICollection<Book>>>.SetCacheDataAsync(cacheKey, listWithPagination, _cache, cacheOptions);
            }
            return listWithPagination;
        }

        public async Task<ICollection<Book>> GetDeletedBooksAsync()
        {
            const string cacheKey = "books_deleted";
            ICollection<Book>? Books = await CacheUtils<ICollection<Book>>.GetCacheDataAsync(cacheKey, _cache);
            if(Books is not null){
                _logger.LogInformation($"Fetch data from cache with key: {cacheKey}");
                return Books;
            }
            else{
                Books = await _context.Books.Where(Book => Book.IsDeleted).ToListAsync();
                var cacheOptions = new DistributedCacheEntryOptions()
                                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                //add to cache 
                await CacheUtils<ICollection<Book>>.SetCacheDataAsync(cacheKey, Books, _cache, cacheOptions);
            }
            return Books;
        }

        public async Task<bool> HardDeleteBookAsync(Book Book)
        {
            string cacheKey = $"book_{Book.Id}";
            await CacheUtils<bool>.RemoveCacheDataAsync(cacheKey, _cache);
            _context.Remove(Book);
            return await Save();
        }

        public async Task<bool> SoftDeleteBookAsync(int BookId)
        {
            string cacheKey = $"book_{BookId}";
            var _exisitngBook = await GetBookByIDAsync(BookId);
            if (_exisitngBook != null)
            {
                await CacheUtils<bool>.RemoveCacheDataAsync(cacheKey, _cache);
                _exisitngBook.IsDeleted = true;
                return await Save();
            }
            return false;
        }

        public async Task<bool> UpdateBookAsync(Book Book)
        {
            string cacheKey = $"book_{Book.Id}";
            await CacheUtils<bool>.RemoveCacheDataAsync(cacheKey, _cache);
            _context.Books.Update(Book);
            return await Save();
        }
        private async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0 ? true : false;
        }
    }
}