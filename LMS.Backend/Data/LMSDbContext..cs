using Microsoft.EntityFrameworkCore;
using LMS.Backend.Models;

namespace LMS.Backend.Data
{
    public class LMSDbContext : DbContext
    {
        public LMSDbContext(DbContextOptions<LMSDbContext> options) : base(options)
        {
        }
        public DbSet<Library> Library { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<StudentBook> StudentBooks { get; set; } = null!;
    }
}