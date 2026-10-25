namespace LearningLanguageApp.Models
{
    public class SolveTaskDto
    {
        public Guid TaskId { get; set; }

        public string SolutionCode {  get; set; }=string.Empty;
    }
}
