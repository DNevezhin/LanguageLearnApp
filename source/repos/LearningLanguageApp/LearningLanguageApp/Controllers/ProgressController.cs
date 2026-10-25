using LearningLanguageApp.Data;
using LearningLanguageApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LearningLanguageApp.Entities;

namespace LearningLanguageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("solve")]
        public async Task<IActionResult> SolveTask(SolveTaskDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdString))
            //{
            //    return Unauthorized();
            //}
            var userId=Guid.Parse(userIdString);
            var taskExists =await _context.Tasks.AnyAsync(i=>i.Id == dto.TaskId);
            if (!taskExists)
            {
                return NotFound("Задача не найдена");
            }
            var progress= await _context.Progresses.FirstOrDefaultAsync(u => u.UserId == userId && u.TaskId == dto.TaskId);

            if (progress == null)
            {
                progress = new Progress
                { UserId=userId,
                    TaskId=dto.TaskId, 
                    IsSolved= true,
                    SolutionCode= dto.SolutionCode
                };
                _context.Add(progress);
            }

            else
            {
                progress.SolutionCode = dto.SolutionCode;
                progress.IsSolved= true;
            }

            await _context.SaveChangesAsync();
            return Ok("Решение успешно сохранено");

        }


    }
}
