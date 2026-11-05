using Microsoft.EntityFrameworkCore;
using LearningLanguageApp.Entities;

namespace LearningLanguageApp.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options) 
        { 
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Entities.Task> Tasks { get; set; }

        public DbSet<Progress> Progresses { get; set; }

        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<ChatMessage> ChatMessages {  get; set; }
    }
}
