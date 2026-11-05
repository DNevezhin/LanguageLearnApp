namespace LearningLanguageApp.Entities
{
    public class Lesson
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Theory {  get; set; } = string.Empty;

        public Guid LanguageId { get; set; }

        public Language Language { get; set; } = null!;

        public List<Task> Tasks { get; set; } = new List<Task>();

    }
}
