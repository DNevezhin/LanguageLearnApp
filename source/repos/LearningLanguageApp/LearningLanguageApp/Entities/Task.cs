namespace LearningLanguageApp.Entities
{
    public class Task
    {
        public Guid Id { get; set; }

        public string Description {  get; set; } = string.Empty;

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; } = null!;

        public List<Progress> Progresses { get; set; } = new List<Progress>();
    }
}
