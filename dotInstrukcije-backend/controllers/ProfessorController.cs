using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using dotInstrukcije.Data;
using dotInstrukcije.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dotInstrukcije.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProfessorController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string _jwtSecret;

        public ProfessorController(DataContext context, string jwtSecret)
        {
            _context = context;
            _jwtSecret = jwtSecret;
        }

        [HttpGet("professors")]
        [Authorize]
        public async Task<IActionResult> GetAllProfessors()
        {
            try
            {
                var professors = await _context.Professors.ToListAsync();
                return Ok(new { success = true, professors, message = "Professors retrieved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving professors", error = ex.Message });
            }
        }

        [HttpGet("professor/{email}")]
        [Authorize]
        public async Task<IActionResult> GetProfessorByEmail(string email)
        {
            try
            {
                var professor = await _context.Professors.FirstOrDefaultAsync(p => p.Email == email);
                if (professor == null)
                {
                    return NotFound(new { success = false, message = "Professor not found" });
                }

                return Ok(new { success = true, professor, message = "Professor retrieved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the professor", error = ex.Message });
            }
        }

        [HttpPost("login/professor")]
        public async Task<IActionResult> Login([FromBody] ProfessorLoginModel request)
        {
            var professorFromDb = await _context.Professors.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (professorFromDb == null)
            {
                return NotFound(new { success = false, message = "Professor not found" });
            }
            if (professorFromDb.Password != request.Password)
            {
                return BadRequest(new { success = false, message = "Invalid password" });
            }

            var token = GenerateJwtToken(professorFromDb.Email);
            return Ok(new { success = true, professor = professorFromDb, token, message = "Login successful" });
        }


        [HttpPost("register/professor")]
        public async Task<IActionResult> Register([FromBody] ProfessorRegistrationModel professor)
        {
            try
            {
                var existingProfessor = await _context.Professors.FirstOrDefaultAsync(p => p.Email == professor.Email);
                if (existingProfessor != null)
                {
                    return BadRequest(new { success = false, message = "Professor with that email already exists" });
                }

                var newProfessor = new Professor
                {
                    Email = professor.Email,
                    Name = professor.Name,
                    Surname = professor.Surname,
                    Password = professor.Password,
                    ProfilePictureUrl = professor.ProfilePictureUrl
                };

                await _context.Professors.AddAsync(newProfessor);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Professor registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while registering the professor", error = ex.Message });
            }
        }

        private string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
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
