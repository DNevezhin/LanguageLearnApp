using LearningLanguageApp.Data;
using LearningLanguageApp.Models;
using LearningLanguageApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<OllamaSharp.OllamaApiClient>(a =>
    new OllamaSharp.OllamaApiClient(new Uri("http://localhost:11434"), "qwen2.5-coder:7b"));

builder.Services.AddScoped<IAiService, OllamaAiService>();
builder.Services.AddScoped<ILessonService, LessonService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
    };
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

using(var scope = app.Services.CreateScope()){
    var context= scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Lessons.Any())
    {
        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var charpJson = await File.ReadAllTextAsync("SeedData/csharp_lessons.json");
        var pythonJson = await File.ReadAllTextAsync("SeedData/python_lessons.json");
        var cppJson = await File.ReadAllTextAsync("SeedData/cpp_lessons.json");
        var csharpLessons=System.Text.Json.JsonSerializer.Deserialize<List<SeedLessonDto>>(charpJson,options);
        var pythonLessons = System.Text.Json.JsonSerializer.Deserialize<List<SeedLessonDto>>(pythonJson,options);
        var cppLessons=System.Text.Json.JsonSerializer.Deserialize<List<SeedLessonDto>>(cppJson, options);
        var csharpLang = context.Languages.FirstOrDefault(l => l.Name == "C#");
        var pythonLang = context.Languages.FirstOrDefault(l => l.Name == "Python");
        var cppLang = context.Languages.FirstOrDefault(l => l.Name == "C++");
        if (csharpLang == null)
        {
            csharpLang = new LearningLanguageApp.Entities.Language { Id = Guid.NewGuid(), Name = "C#" };
            context.Languages.Add(csharpLang);
        }
        if (pythonLang == null)
        {
            pythonLang = new LearningLanguageApp.Entities.Language { Id = Guid.NewGuid(), Name = "Python" };
            context.Languages.Add(pythonLang);
        }
        if (cppLang == null)
        {
            cppLang = new LearningLanguageApp.Entities.Language { Id = Guid.NewGuid(), Name = "C++" };
            context.Languages.Add(cppLang);
        }
        if(csharpLessons != null)
        {
            foreach (var l in csharpLessons!)
            {
                var newLessonsId= Guid.NewGuid();
                var lesson = new LearningLanguageApp.Entities.Lesson
                {
                    Id=newLessonsId,
                    Title=l.Title,
                    Theory=l.Theory,
                    LanguageId=csharpLang.Id
                };
                context.Lessons.Add(lesson);

                foreach (var t in l.Tasks)
                {
                    var task = new LearningLanguageApp.Entities.Task
                    {
                        Id = Guid.NewGuid(),
                        Description = t.Description,
                        LessonId = newLessonsId,
                    };
                    context.Tasks.Add(task);
                }
            }
        }
        if (pythonLessons != null)
        {
            foreach (var l in pythonLessons!)
            {
                var newLessonsId = Guid.NewGuid();
                var lesson = new LearningLanguageApp.Entities.Lesson
                {
                    Id = newLessonsId,
                    Title = l.Title,
                    Theory = l.Theory,
                    LanguageId = pythonLang.Id
                };
                context.Lessons.Add(lesson);

                foreach (var t in l.Tasks)
                {
                    var task = new LearningLanguageApp.Entities.Task
                    {
                        Id = Guid.NewGuid(),
                        Description = t.Description,
                        LessonId = newLessonsId,
                    };
                    context.Tasks.Add(task);
                }
            }
        }
        if (cppLang != null)
        {
            foreach (var l in cppLessons!)
            {
                var newLessonsId = Guid.NewGuid();
                var lesson = new LearningLanguageApp.Entities.Lesson
                {
                    Id = newLessonsId,
                    Title = l.Title,
                    Theory = l.Theory,
                    LanguageId = cppLang.Id
                };
                context.Lessons.Add(lesson);

                foreach (var t in l.Tasks)
                {
                    var task = new LearningLanguageApp.Entities.Task
                    {
                        Id = Guid.NewGuid(),
                        Description = t.Description,
                        LessonId = newLessonsId,
                    };
                    context.Tasks.Add(task);
                }
            }
        }
        await context.SaveChangesAsync();
    }
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
//мой id который я зарегестрировал для [Authorize] - 01a16eee-6666-7196-a555-16648e1cbff9