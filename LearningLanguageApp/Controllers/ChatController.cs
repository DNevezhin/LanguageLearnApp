using LearningLanguageApp.Data;
using LearningLanguageApp.Entities;
using LearningLanguageApp.Models;
using LearningLanguageApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System.Security.Claims;
using System.Text;

namespace LearningLanguageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController: ControllerBase
    {
        private readonly IAiService _service;
        private readonly AppDbContext _context;

        public ChatController(IAiService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckCode(ChatRequestDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("");
            }
            var UserId=Guid.Parse(userIdString);
            var task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == dto.TaskId);
            if (task == null)
            {
                return NotFound($"Задача с Id{dto.TaskId} не найдена");
            }

            var answer = await _service.CheckCodeAsync(dto.Code,task.Description);
            return Ok(new { answer = answer });
        }

        [HttpPost("message")]
        public async Task<IActionResult> Chat(SendMessageDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdString);

            var userMessage = new LearningLanguageApp.Entities.ChatMessage
            {
                UserId = userId,
                LessonId = dto.LessonId,
                Text = dto.Text,
                IsFromAi = false,
                CreatedAt = DateTime.UtcNow

            };

            _context.ChatMessages.Add(userMessage);
            await _context.SaveChangesAsync();

            var history = await _context.ChatMessages
                .Where(i => i.UserId == userId && i.LessonId.HasValue && i.LessonId.Value == dto.LessonId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .ToListAsync();

            history.Reverse();

            var historyString = new StringBuilder();
            foreach (var el in history)
            {
                string role = el.IsFromAi ? "Ментор" : "Пользователь";
                historyString.AppendLine($"{role}: {el.Text}");
            }

            var answer = await _service.SendMessageAsync(historyString.ToString(),dto.Text);
            string aiAnswer = answer;

            var aiMessage = new LearningLanguageApp.Entities.ChatMessage
            {
                UserId = userId,
                LessonId = dto.LessonId,
                Text = aiAnswer,
                IsFromAi = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(aiMessage);
            await _context.SaveChangesAsync();
            return Ok(new {answer=aiAnswer});
        }

        [HttpGet("history/{lessonId}")]
        public async Task<IActionResult> GetChatHistoty(Guid lessonId)
        {
            var userIdString=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("");
            }
            var userId=Guid.Parse(userIdString);
            var history = await _context.ChatMessages
                .Where(u => u.UserId == userId && u.LessonId == lessonId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(history);
        }

        [HttpDelete("history/{lessonId}")]
        public async Task<IActionResult> ClearHistory(Guid lessonId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("");
            }
            var userId = Guid.Parse(userIdString);
            var messages = await _context.ChatMessages
                .Where(u => u.UserId == userId && u.LessonId == lessonId)
                .ToListAsync();

            _context.ChatMessages.RemoveRange(messages);
            await _context.SaveChangesAsync();
            return Ok("История очищена");
        }

    }
}
