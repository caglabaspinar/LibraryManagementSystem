using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LMS.Backend.Data;
using LMS.Backend.DTOs;
using LMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly LMSDbContext _context;
    private readonly IConfiguration _configuration; 
    public StudentsController(LMSDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration; 
    }
#pragma warning disable CS8604 
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password!));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
    private string CreateToken(Student student)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); 

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, student.Id.ToString() ?? throw new ArgumentNullException(nameof(student.Id))),
            new Claim(ClaimTypes.Email, student.Email ?? throw new ArgumentNullException(nameof(student.Email)))
        }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [HttpPost]
    public async Task<ActionResult<Student>> Register(StudentRegisterDto request)
    {
        if (await _context.Students.AnyAsync(s => s.Email == request.Email))
        {
            return BadRequest("Bu e-posta adresi zaten kayıtlıdır.");
        }

        var passwordHash = HashPassword(request.Password);

        var student = new Student
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = passwordHash 
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(StudentLoginDto request)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == request.Email);

        if (student == null)
        {
            return Unauthorized("Kullanıcı adı veya şifre hatalı.");
        }
        var enteredPasswordHash = HashPassword(request.Password);

        if (enteredPasswordHash != student.Password)
        {
            return Unauthorized("Kullanıcı adı veya şifre hatalı.");
        }
        var token = CreateToken(student);
        return Ok(new { token = token }); 
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }
        student.Password = null;
        return student;
    }
}