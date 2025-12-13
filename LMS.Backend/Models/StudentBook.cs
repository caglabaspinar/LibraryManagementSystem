using System;

namespace LMS.Backend.Models
{
    public class StudentBook
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!; 
        public int BookId { get; set; }
        public Book Book { get; set; } = null!; 
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; } 
    }
}