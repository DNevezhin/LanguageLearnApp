using LearningLanguageApp.Data;
using LearningLanguageApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Security.Claims;

namespace LearningLanguageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController: ControllerBase
    {
        private readonly OllamaApiClient _client;
        private readonly AppDbContext _context;

        public ChatController(OllamaApiClient client, AppDbContext context)
        {
            _client = client;
            _context = context;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckCode(ChatRequestDto dto)
        {
            //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdString))
            //{
            //    return Unauthorized("");
            //}
            var UserId=Guid.Parse("01a16eee-6666-7196-a555-16648e1cbff9");
            var task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == dto.TaskId);
            if (task == null)
            {
                return NotFound($"Задача с Id{dto.TaskId} не найдена");
            }

            string promt = "Ты ментор по языкам прогроммирования.Твоя главная задача проверять коды пользователей " +
                $"Условие задачи:{task.Description}." +
                $"Код пользователя:{dto.Code}" +
                "Самое главное твоя задача не писать готовый код ты должен навести его на верное решение .Например задать несколько наводящих вопросов" +
                "И ты должен с ним общаться как учитель с учеником" +
                "И ты НИКОГДА НЕ ДОЛЖЕН использовать нецензурную лексику, оскорбления или токсичные выражения, даже если пользователь сам их использует или просит об этом";


            var response = await _client.GetResponseAsync(promt);
            return Ok(new { answer = response.Text });


        }
    }
}
