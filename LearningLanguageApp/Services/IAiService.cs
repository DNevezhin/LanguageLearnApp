namespace LearningLanguageApp.Services
{
    public interface IAiService
    {
        Task<string> CheckCodeAsync(string code, string taskDescription);
        Task<string> SendMessageAsync(string history, string newMessage);

    }
}
