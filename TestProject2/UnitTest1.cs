using Inventarisation.Interfaces;
using Inventarisation.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Routing;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Inventarisation.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TestProject2
{
    public class UnitTest1
    {
        //Проверяется сценарий, когда пользователь успешно аутентифицирован и введены правильные учетные данные.
        [Fact]
        public async Task Login_ValidCredentials_RedirectsToHome()
        {
            // Arrange
            var bdWorkMock = new Mock<IBDWork>();
            bdWorkMock.Setup(b => b.Login(It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(new UserRepository { UserRoleId = 1, Id = 1 });

            var httpContextMock = new DefaultHttpContext();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, "UserName"),
            new Claim(ClaimTypes.Role, "UserRole")
        },CookieAuthenticationDefaults.AuthenticationScheme));
            httpContextMock.User = claimsPrincipal;

            var controller = new UserController(bdWorkMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContextMock }
            };

            // Act
            var result = await controller.Login("testLogin", "testPassword") as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        //Проверяет сценарий, когда введены неверные учетные данные
        [Fact]
        public async Task Login_InvalidCredentials_ReturnsErrorMessage()
        {
            // Arrange
            var bdWorkMock = new Mock<IBDWork>();
            bdWorkMock.Setup(b => b.Login(It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(new UserRepository { UserRoleId = 1, Id = 1 });

            var controller = new UserController(bdWorkMock.Object);

            // Act
            var result = await controller.Login("invalidLogin", "invalidPassword") as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Данные не верны", result.ViewData["ErrorMessage"]);
        }

        //Проверяет сценарий, когда возникает ошибка базы данных
        [Fact]
        public async Task Login_DatabaseError_ReturnsDatabaseErrorMessage()
        {
            // Arrange
            var bdWorkMock = new Mock<IBDWork>();
            bdWorkMock.Setup(b => b.Login(It.IsAny<string>(), It.IsAny<string>()))
                      .ThrowsAsync(new Exception("Database error"));

            var controller = new UserController(bdWorkMock.Object);

            // Act
            var result = await controller.Login("testLogin", "testPassword") as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ошибка базы данных", result.ViewData["ErrorMessage"]);
        }
    }
}