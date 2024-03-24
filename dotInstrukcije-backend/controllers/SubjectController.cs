using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotInstrukcije.Data;
using dotInstrukcije.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace dotInstrukcije.Controllers
{
    [ApiController]
    [Route("api")]
    public class SubjectController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string _jwtSecret;

        public SubjectController(DataContext context, string jwtSecret)
        {
            _context = context;
            _jwtSecret = jwtSecret;
        }

        [HttpGet("subjects")]
        [Authorize]
        public async Task<IActionResult> GetAllSubjects()
        {
            try
            {
                var subjects = await _context.Subjects.ToListAsync();
                return Ok(new { success = true, subjects });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving subjects", error = ex.Message });
            }
        }

        [HttpGet("subject/{url}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectByUrl(string url)
        {
            try
            {
                var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Url == url);
                if (subject == null)
                {
                    return NotFound(new { success = false, message = "Subject not found" });
                }

                return Ok(new { success = true, subject, message = "Subject retrieved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the subject", error = ex.Message });
            }
        }

        //[HttpPost("subject")]
        //[Authorize]
        //public async Task<IActionResult> CreateSubject([FromBody] Subject subject)
        //{
        //    try
        //    {
        //        var existingSubject = await _context.Subjects.FirstOrDefaultAsync(s => s.Url == subject.Url);
        //        if (existingSubject != null)
        //        {
        //            return BadRequest(new { success = false, message = "Subject with that URL already exists" });
        //        }

        //        if (subject.professors != null && subject.professors.Any())
        //        {
        //            foreach (var professor in subject.professors)
        //            {
        //                var existingProfessor = await _context.Professors.FirstOrDefaultAsync(p => p.Email == professor.Email);
        //                if (existingProfessor != null)
        //                {
        //                    existingProfessor.Subjects.Add(subject);
        //                }
        //            }
        //        }

        //        await _context.Subjects.AddAsync(subject);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { success = true, message = "Subject created successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { success = false, message = "An error occurred while creating the subject", error = ex.Message });
        //    }
        //}


        [HttpPost("instructions")]
        [Authorize]
        public async Task<IActionResult> ScheduleInstructionSession([FromBody] InstructionsDate instructions)
        {
            try
            {
                if (instructions.Date == null || instructions.ProfessorId == null)
                {
                    return BadRequest(new { success = false, message = "Date and ProfessorId are required fields" });
                }

                await _context.InstructionsDates.AddAsync(instructions);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Instruction session scheduled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while scheduling the instruction session", error = ex.Message });
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
