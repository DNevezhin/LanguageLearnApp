namespace LearningLanguageApp.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? LessonId { get; set; }

        public string Text { get; set; } = string.Empty;

        public bool IsFromAi { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;

        public Lesson? Lesson { get; set; }

    }
}
