using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.Backend.Models;

[Table("Libraries")]
public class Library
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string Location { get; set; } = string.Empty; 
    public ICollection<Book> Books { get; set; } = new List<Book>();
}