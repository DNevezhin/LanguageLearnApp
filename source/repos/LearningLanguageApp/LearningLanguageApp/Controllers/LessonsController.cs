using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningLanguageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonsController: ControllerBase
    {

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Вы видите то что не видят не залогиненный юзеры");
        }
    }
}
