using LearningLanguageApp.Models;

namespace LearningLanguageApp.Services
{
    public interface ILessonService
    {
        Task<List<LessonResponseDto>> ReturnAllLessons();
        Task<LessonResponseDto?> GetLessonById(Guid LessonID);
        Task<List<LessonResponseDto>> GetLessonByLanguage(Guid languageId);

    }
}
