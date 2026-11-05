using System;
using System.Collections.Generic;
using System.Text;
using LearningLanguageApp.Controllers;
using LearningLanguageApp.Data;
using LearningLanguageApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LearninLanguageApp.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_WithNewUser_ReturnOk()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestBd").Options;
            using var context = new AppDbContext(options);
            var controller = new AuthController(context, null!);

            var dto = new RegisterDto { Login = "testuser", Password = "123" };
            var result = controller.Register(dto);
            Assert.IsType<OkObjectResult>(result);

            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Login == "testuser");
            Assert.NotNull(userInDb);
        }
        [Fact]
        public async Task Authorization_WithDublicateLogin_ReturnBadReques()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestBd_Dublicat").Options;
            using var context = new AppDbContext(options);
            var controller = new AuthController(context, null!);

            context.Users.Add(new LearningLanguageApp.Entities.User { Login = "user", Password = "1245dgfg" });
            await context.SaveChangesAsync();

            var dto = new RegisterDto { Login = "user", Password = "1414235" };

            var result = controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);

        }
    }
}
