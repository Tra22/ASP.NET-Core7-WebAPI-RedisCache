using APICacheWithRedis.Dtos;
using APICacheWithRedis.Payload.Search;
using APICacheWithRedis.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICacheWithRedis.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BookController : ControllerBase
    {
         private readonly IBookService _bookService;
         private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            this._bookService = bookService;
            this._logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BookDto>))]
        public async Task<IActionResult> GetAll([FromQuery] SearchParams searchParams)
        {
            var students = await _bookService.GetBooksAsync(searchParams);
            _logger.LogInformation("Success {@BookList}", students);
            return Ok(students);
        }

        [HttpGet("{BookId:int}", Name = "GetByBookID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<BookDto>> GetByBookID(int BookId)
        {
            if (BookId <= 0)
            {
                return BadRequest(BookId);
            }
            var BookFound = await _bookService.GetByIdAsync(BookId);

            if (BookFound.Data == null)
            {
                _logger.LogWarning("Not Found {BookId}", BookId);
                return NotFound();
            }
            _logger.LogInformation("Success {@Response}", BookFound);
            return Ok(BookFound);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDto>> Createstudent([FromBody] CreateBookDto createBookDto)
        {
            if (createBookDto == null)
            {
                _logger.LogWarning("Empty Request Body {@CreateBookDto}", createBookDto);
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var _newBook = await _bookService.AddBookAsync(createBookDto);


            if (_newBook.Success == false && _newBook.Message == "RepoError")
            {
                _logger.LogError("RepoError {@createBookDto}",  $"Some thing went wrong in respository layer when adding book {createBookDto}");
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding book {createBookDto}");
                return StatusCode(500, ModelState);
            }

            if (_newBook.Success == false && _newBook.Message == "Error")
            {
                _logger.LogError("Error {@createBookDto}",  $"Some thing went wrong in service layer when adding book {createBookDto}");
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding book {createBookDto}");
                return StatusCode(500, ModelState);
            }
            _logger.LogInformation("Success {@Book}", _newBook);
            //Return new student created
            return CreatedAtRoute("GetByBookID", new { StudentId = _newBook?.Data?.Id }, _newBook);

        }
        [HttpPatch("{BookId:int}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBook(int BookId, [FromBody] UpdateBookDto updateBookDto)
        {
            if (updateBookDto == null || updateBookDto.Id != BookId)
            {
                _logger.LogWarning("Empty Request Body or Empty Id {}", updateBookDto);
                return BadRequest(ModelState);
            }


            var _updateBook = await _bookService.UpdateBookAsync(updateBookDto);

            if (_updateBook.Success == false && _updateBook.Message == "NotFound")
            {
                _logger.LogWarning("Not Found: {@_updateBook}", _updateBook);
                return Ok(_updateBook);
            }

            if (_updateBook.Success == false && _updateBook.Message == "RepoError")
            {
                _logger.LogError("RepoError {@updateBookDto}", $"Some thing went wrong in respository layer when updating book {updateBookDto}");
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating book {updateBookDto}");
                return StatusCode(500, ModelState);
            }

            if (_updateBook.Success == false && _updateBook.Message == "Error")
            {
                _logger.LogError("Error {@updateBookDto}",  $"Some thing went wrong in service layer when updating book {updateBookDto}");
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating book {updateBookDto}");
                return StatusCode(500, ModelState);
            }

            _logger.LogInformation("Success {@Response}", _updateBook);
            return Ok(_updateBook);
        }
        [HttpDelete("{BookId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Not found
        [ProducesResponseType(StatusCodes.Status409Conflict)] //Can not be removed 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBook(int BookId)
        {

            var _deleteBook = await _bookService.SoftDeleteBookAsync(BookId);


            if (_deleteBook.Success == false && _deleteBook.Data == "NotFound")
            {
                _logger.LogWarning("Not Found {@_deleteBook}", _deleteBook);
                ModelState.AddModelError("", "Book Not found");
                return StatusCode(404, ModelState);
            }

            if (_deleteBook.Success == false && _deleteBook.Data == "RepoError")
            {
                _logger.LogError("RepoError {@_deleteBook}", $"Some thing went wrong in Repository when deleting book {_deleteBook}");
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting student");
                return StatusCode(500, ModelState);
            }

            if (_deleteBook.Success == false && _deleteBook.Data == "Error")
            {
                _logger.LogError("RepoError {@_deleteBook}", $"Some thing went wrong in service layer when deleting book {_deleteBook}");
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting book");
                return StatusCode(500, ModelState);
            }
            _logger.LogInformation("Success {Msg}", "deleted");
            return NoContent();

        }
        
    }
}