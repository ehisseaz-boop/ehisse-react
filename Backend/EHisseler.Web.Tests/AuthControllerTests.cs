using System.Threading.Tasks;
using EHisseler.Web.Controllers;
using EHisseler.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EHisseler.Web.Tests
{
    public class AuthControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Register_HashesPassword_And_SavesUser()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new AuthController(dbContext);

            // Act
            var result = await controller.Register(
                "Test", 
                "User", 
                "0501234567", 
                "testuser", 
                "mysecretpassword", 
                UserRole.Usta);

            // Assert
            var userInDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            
            Assert.NotNull(userInDb);
            Assert.NotEqual("mysecretpassword", userInDb.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("mysecretpassword", userInDb.PasswordHash));
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("RegistrationPending", redirectResult.ActionName);
        }
    }
}
