using LMS.Backend.Data;
using LMS.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LMS.Backend.DTOs;

[Route("api/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BooksController : ControllerBase
{
    private readonly LMSDbContext _context;
    public BooksController(LMSDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(BookCreateDto bookDto)
    {
        if (!await _context.Library.AnyAsync(l => l.Id == bookDto.LibraryId))
        {
            return BadRequest("Belirtilen kütüphane ID'si bulunamadı.");
        }
        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Isbn = bookDto.Isbn,
            LibraryId = bookDto.LibraryId
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        return await _context.Books
                             .Include(b => b.Library)
                             .ToListAsync();
    }

    /*[HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBooks(int id)
    {
        var book = await _context.Books
                                 .Include(b => b.Library)
                                 .FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }
        return book;
    }
    */
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
    {
        if (!await _context.Library.AnyAsync(l => l.Id == bookDto.LibraryId))
        {
            return BadRequest("Belirtilen kütüphane ID'si bulunamadı.");
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound("Güncellenecek kitap bulunamadı.");

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Isbn = bookDto.Isbn;
        book.LibraryId = bookDto.LibraryId;

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Books.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return NoContent(); 
    }
}
