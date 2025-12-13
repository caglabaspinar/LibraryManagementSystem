using LMS.Backend.Data;
using LMS.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentBooksController : ControllerBase
    {
        private readonly LMSDbContext _context;
        public StudentBooksController(LMSDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null) return Unauthorized("Kimlik doğrulanamadı.");

            int studentId = int.Parse(studentIdClaim.Value);
            
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return NotFound("Kitap bulunamadı.");

            var studentBook = new StudentBook
            {
                StudentId = studentId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow 
            };
            _context.StudentBooks.Add(studentBook);
            await _context.SaveChangesAsync();

            return Ok("Kitap başarıyla ödünç alındı.");
        }

        /*
        [HttpGet("my-books")]
        public async Task<IActionResult> GetMyBooks()
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null) return Unauthorized();

            int studentId = int.Parse(studentIdClaim.Value);

            var myBooks = await _context.StudentBooks
                .Where(sb => sb.StudentId == studentId) 
                .Include(sb => sb.Book) 
                .Select(sb => new
                {
                    KitapAdi = sb.Book.Title,
                    Yazar = sb.Book.Author,
                    ISBN = sb.Book.Isbn,
                    OduncTarihi = sb.BorrowDate
                })
                .ToListAsync();
            return Ok(myBooks);
        } */
    }
}
