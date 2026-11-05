using LearningLanguageApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LearningLanguageApp.Services;

namespace LearningLanguageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonsController: ControllerBase
    {

        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        public async Task<IActionResult> ReturnAllLessons()
        {
            var result = await _lessonService.ReturnAllLessons();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonsById(Guid id)
        {
            var result = await _lessonService.GetLessonById(id);
            if (result == null)
            {
                return NotFound("Урок не найден");
            }
            return Ok(result);
        }

        [HttpGet("language/{languageId}")]
        public async Task<IActionResult> GetLessonByLanguage(Guid languageId)
        {
            var language = await _lessonService.GetLessonByLanguage(languageId);
            return Ok(language);
        }
    }
}
