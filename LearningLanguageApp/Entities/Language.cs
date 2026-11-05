namespace LearningLanguageApp.Entities
{
    public class Language
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Lesson> Lessons { get; set; } = new List<Lesson>();

    }
}
