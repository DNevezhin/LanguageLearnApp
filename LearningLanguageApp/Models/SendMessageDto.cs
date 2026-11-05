namespace LearningLanguageApp.Models
{
    public class SendMessageDto
    {
        public Guid LessonId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
