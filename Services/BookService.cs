using APICacheWithRedis.Dtos;
using APICacheWithRedis.Entities;
using APICacheWithRedis.Payload;
using APICacheWithRedis.Payload.Global;
using APICacheWithRedis.Payload.Search;
using APICacheWithRedis.Repositories.Interface;
using AutoMapper;

namespace APICacheWithRedis.Services {
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
         public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            this._bookRepository = bookRepository;
            this._mapper = mapper;
        }
        public async Task<Response<BookDto>> AddBookAsync(CreateBookDto createBookDto)
        {
             Response<BookDto> _response = new();
            try
            {
                Book _newBook = new()
                {
                    Name = createBookDto.Name,
                    Price = createBookDto.Price,
                    IsDeleted = false
                };

                //Add new record
                if (!await _bookRepository.CreateBookAsync(_newBook))
                {
                    _response.Error = "RepoError";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<BookDto>(_newBook);
                _response.Message = "Created";

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };

            }
            return _response;
        }

        public async Task<Response<ListWithPagination<List<BookDto>>>> GetBooksAsync(SearchParams searchParams)
        {
            Response<ListWithPagination<List<BookDto>>> _response = new();

            try
            {

                var BookList = await _bookRepository.GetBooksAsync(searchParams);

                var BookListDto = new List<BookDto>();
                if(BookList.List != null){
                    foreach (var item in BookList.List)
                    {
                        BookListDto.Add(_mapper.Map<BookDto>(item));
                    }
                }
                _response.Success = true;
                _response.Message = "ok";
                _response.Data = new ListWithPagination<List<BookDto>>{ List = BookListDto, Pagination = BookList.Pagination};

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<Response<BookDto>> GetByIdAsync(int BookId)
        {
            Response<BookDto> _response = new();

            try
            {

                var _Book = await _bookRepository.GetBookByIDAsync(BookId);

                if (_Book == null)
                {
                    _response.Success = false;
                    _response.Message = "NotFound";
                    return _response;
                }

                var _BookDto = _mapper.Map<BookDto>(_Book);

                _response.Success = true;
                _response.Message = "ok";
                _response.Data = _BookDto;


            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<Response<string>> SoftDeleteBookAsync(int BookId)
        {
            Response<string> _response = new();

            try
            {
                //check if record exist
                var _existingBook = await _bookRepository.BookExistAsync(BookId);

                if (_existingBook == false)
                {
                    _response.Success = false;
                    _response.Message = "NotFound";
                    _response.Data = null;
                    return _response;

                }

                if (!await _bookRepository.SoftDeleteBookAsync(BookId))
                {
                    _response.Success = false;
                    _response.Message = "RepoError";
                    return _response;
                }



                _response.Success = true;
                _response.Message = "SoftDeleted";

            }
            catch (Exception ex)
            {

                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }
            return _response;
        }

        public async Task<Response<BookDto>> UpdateBookAsync(UpdateBookDto updateBookDto)
        {
            Response<BookDto> _response = new();

            try
            {
                //check if record exist
                var _existingBook = await _bookRepository.GetBookByIDAsync(updateBookDto.Id);

                if (_existingBook == null)
                {
                    _response.Success = false;
                    _response.Message = "NotFound";
                    _response.Data = null;
                    return _response;

                }

                //Update
                _existingBook.Name = updateBookDto.Name;
                _existingBook.Price = updateBookDto.Price;
                _existingBook.IsDeleted = updateBookDto.IsDeleted;

                if (!await _bookRepository.UpdateBookAsync(_existingBook))
                {
                    _response.Success = false;
                    _response.Message = "RepoError";
                    _response.Data = null;
                    return _response;
                }

                var _BookDto = _mapper.Map<BookDto>(_existingBook);
                _response.Success = true;
                _response.Message = "Updated";
                _response.Data = _BookDto;

            }
            catch (Exception ex)
            {

                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }
            return _response;
        }
    }
}