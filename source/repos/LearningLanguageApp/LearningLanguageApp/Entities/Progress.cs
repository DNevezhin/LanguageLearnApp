namespace LearningLanguageApp.Entities
{
    public class Progress
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid TaskId { get; set; }

        public bool IsSolved { get; set; }

        public User User { get; set; } = null!;

        public Task Task { get; set; } = null!;
        
    }
}
