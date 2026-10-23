namespace LearningLanguageApp.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Login {  get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public List<Progress> Progresses { get; set; } = new List<Progress>();

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
