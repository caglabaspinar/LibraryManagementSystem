using LMS.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly LMSDbContext _context;
        public ReportsController(LMSDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentBooksReport(int studentId)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
            if (!studentExists)
            {
                return NotFound($"ID {studentId} olan öğrenci bulunamadı.");
            }

            var report = await _context.StudentBooks
                .Where(sb => sb.StudentId == studentId)
                .Include(sb => sb.Book) 
                .Select(sb => new
                {
                    KitapID = sb.BookId,
                    KitapAdi = sb.Book.Title,
                    OduncTarihi = sb.BorrowDate,
                    KutuphaneId = sb.Book.LibraryId 
                })
                .ToListAsync();

            if (report == null || !report.Any())
            {
                return Ok($"ID {studentId} olan öğrencinin ödünç aldığı kitap bulunmamaktadır.");
            }
            return Ok(report);
        }

        [HttpGet("library/{libraryId}")]
        public async Task<IActionResult> GetLibraryBooksReport(int libraryId)
        {
            var libraryExists = await _context.Library.AnyAsync(l => l.Id == libraryId);
            if (!libraryExists)
            {
                return NotFound($"ID {libraryId} olan kütüphane bulunamadı.");
            }

            
            var report = await _context.Books
                .Where(b => b.LibraryId == libraryId)
                .Select(b => new
                {
                    KitapID = b.Id,
                    Baslik = b.Title,
                    Yazar = b.Author,
                    ISBN = b.Isbn
                })
                .ToListAsync();

            if (report == null || !report.Any())
            {
                return Ok($"ID {libraryId} olan kütüphanede kayıtlı kitap bulunmamaktadır.");
            }
            return Ok(report);
        }
    }
}