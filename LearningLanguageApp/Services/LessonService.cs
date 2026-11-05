using LearningLanguageApp.Data;
using LearningLanguageApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningLanguageApp.Services
{
    public class LessonService : ILessonService
    {
        private readonly AppDbContext _context;

        public LessonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LessonResponseDto>> ReturnAllLessons()
        {
            var result = await _context.Lessons
                .Select(l => new LessonResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Theory = l.Theory,
                    LanguageName = l.Language.Name,
                    Tasks = l.Tasks.Select(t => new TaskResponseDto
                    {
                        Id = t.Id,
                        Description = t.Description
                    }).ToList()
                })
                .ToListAsync();

            return result;
        }

        public async Task<LessonResponseDto?> GetLessonById(Guid id)
        {
            var result = await _context.Lessons
                .Select(l => new LessonResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Theory = l.Theory,
                    LanguageName = l.Language.Name,
                    Tasks = l.Tasks.Select(t => new TaskResponseDto
                    {
                        Id = t.Id,
                        Description = t.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync(l => l.Id == id);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public async Task<List<LessonResponseDto>> GetLessonByLanguage(Guid languageId)
        {
            var language = await _context.Lessons
                .Where(i => i.LanguageId == languageId)
                .OrderBy(l => l.Title)
                .Select(l => new LessonResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Theory = l.Theory,
                    LanguageName = l.Language.Name,
                    Tasks = l.Tasks.Select(t => new TaskResponseDto
                    {
                        Id = t.Id,
                        Description = t.Description
                    }).ToList()
                })
                .ToListAsync();

            return language;
        }
    }
}
