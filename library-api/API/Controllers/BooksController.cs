using library_api.Application.DTOs.Book;
using library_api.Application.Interfaces;
using library_api.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace library_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound(new { message = $"Libro con ID {id} no encontrado." });
            }
            return Ok(book);
        }

        [HttpGet("with-stock")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksWithStock()
        {
            var books = await _bookService.GetBooksWithStockAsync();
            return Ok(books);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new { message = "El término de búsqueda es requerido." });
            }
            var books = await _bookService.SearchAsync(term);
            return Ok(books);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            try
            {
                var book = await _bookService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
            }
            catch (DuplicateEntityException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> Update(int id, [FromBody] UpdateBookDto dto)
        {
            try
            {
                var book = await _bookService.UpdateAsync(id, dto);
                return Ok(book);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DuplicateEntityException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _bookService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
    