using LearningLanguageApp.Data;
using LearningLanguageApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningLanguageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonsController: ControllerBase
    {

        private readonly AppDbContext _context;

        public LessonsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ReturnAllLessons()
        {
            var result =await _context.Lessons
                .Include(l => l.Language)
                .Include(l => l.Tasks)
                .Select(l => new LessonResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Theory = l.Theory,
                    LanguageName = l.Language.Name,
                    Tasks = l.Tasks.Select(t => new TaskResponseDto
                    {
                        Id = t.Id,
                        Description = t.Description
                    }).ToList()
                })
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonsById(Guid id)
        {
            var result =await _context.Lessons
                .Include(l => l.Language)
                .Include(l => l.Tasks)
                .Select(l => new LessonResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Theory = l.Theory,
                    LanguageName = l.Language.Name,
                    Tasks = l.Tasks.Select(t => new TaskResponseDto
                    {
                        Id = t.Id,
                        Description = t.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync(l => l.Id == id);
            if (result == null)
            {
                return NotFound("Урок не найден");
            }
            return Ok(result);
        }
    }
}
