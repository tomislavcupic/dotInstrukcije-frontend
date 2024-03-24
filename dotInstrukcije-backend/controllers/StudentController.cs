using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using dotInstrukcije.Data;
using dotInstrukcije.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace dotInstrukcije.Controllers
{
    [ApiController]
    [Route("api")]
    public class StudentController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public StudentController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("students")]
        [Authorize]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var students = await _context.Students.ToListAsync();
                Response.AppendTrailer("Access-Control-Allow-Origin", "http://localhost:5173");
                return Ok(new { success = true, students, message = "Students retrieved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving students", error = ex.Message });
            }
        }

        [HttpGet("student/{email}")]
        [Authorize]
        public async Task<IActionResult> GetStudentByEmail(string email)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
                if (student == null)
                {
                    return NotFound(new { success = false, message = "Student not found" });
                }
                return Ok(new { success = true, student, message = "Student retrieved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the student", error = ex.Message });
            }
        }

        [HttpPost("login/student")]
        public async Task<IActionResult> Login([FromBody] StudentLoginModel model)
        {
            try
            {
                // Log the received model properties
                Console.WriteLine($"Received email: {model.Email}");
                Console.WriteLine($"Received password: {model.Password}");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var jwtSecret = _configuration["Jwt:Key"];

                var studentFromDb = await _context.Students.FirstOrDefaultAsync(s => s.Email == model.Email && s.Password == model.Password);
                if (studentFromDb == null)
                {
                    return Unauthorized("Wrong username or password. If you don't have an account register.");
                }

                var token = GenerateJwtToken(studentFromDb.Email, jwtSecret);
                if (token == null)
                {
                    return StatusCode(500, new { success = false, message = "An error occurred while generating token" });
                }

                Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex}");
                return StatusCode(500, new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> Register([FromForm] StudentRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePictureUrl.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/slike", fileName);
            
            using (var stream = System.IO.File.Create(filePath))
            {
                await model.ProfilePictureUrl.CopyToAsync(stream);
            }

            var student = new Student
            {
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                Password = model.Password,
                ProfilePictureUrl = "/slike/" + fileName // Update the URL as needed
            };

            // Save the student to the database
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Student registered successfully" });
        }


    private string GenerateJwtToken(string email, string jwtSecret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
