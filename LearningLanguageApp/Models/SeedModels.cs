namespace LearningLanguageApp.Models
{
        public class SeedTaskDto
        {
            public string Description { get; set; } = string.Empty;
        }

        public class SeedLessonDto
        {
            public string Title {  get; set; }  = string.Empty;
            
            public string Theory { get; set; } = string.Empty;

            public List<SeedTaskDto> Tasks { get; set; } = new List<SeedTaskDto>();
        }

}

