namespace LearningLanguageApp.Models
{
    public class LessonResponseDto
    {
        public Guid Id { get; set; }

        public string Title {  get; set; } = string.Empty;    

        public string Theory {  get; set; } = string.Empty;   

        public string LanguageName {  get; set; } = string.Empty;

        public List <TaskResponseDto> Tasks= new List<TaskResponseDto>();
    }
}
