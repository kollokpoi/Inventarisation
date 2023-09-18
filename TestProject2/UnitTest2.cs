using Inventarisation.Controllers;
using Inventarisation.Interfaces;
using Inventarisation.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject2
{
    public class UnitTest2
    {
        //Сценарий теста метода Index. Он создает макет IBDWork и настраивает его для возвращения ожидаемых объектов Auditory
        [Fact]
        public async Task Index_ReturnsViewWithModel()
        {
            // Arrange
            var mockBDWork = new Mock<IBDWork>();
            var expectedModel = new List<Auditory>
        {
            new Auditory { Id = 1, Name = "Auditory 1" },
            new Auditory { Id = 2, Name = "Auditory 2" }
        };
            mockBDWork.Setup(b => b.GetAuditories()).ReturnsAsync(expectedModel);

            var controller = new AuditoryController(mockBDWork.Object);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Auditory>>(result.Model);
            var model = result.Model as List<Auditory>;
            Assert.Equal(expectedModel, model);
        }
        //Сценарий теста метода Info. Он создает макет IBDWork и настраивает его для возвращения ожидаемой модели Auditory
        [Fact]
        public async Task Info_ReturnsViewWithAuditoryInfo()
        {
            // Arrange
            var mockBDWork = new Mock<IBDWork>();
            var expectedAuditoryInfo = new Auditory
            {
                Id = 1,
                Name = "Auditory 1",
                ShortName = "209",
            };
            mockBDWork.Setup(b => b.GetAuditoryInfo(It.IsAny<int>())).ReturnsAsync(expectedAuditoryInfo);

            var controller = new AuditoryController(mockBDWork.Object);

            // Act
            var result = await controller.Info(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Auditory>(result.Model);
            var model = result.Model as Auditory;
            Assert.Equal(expectedAuditoryInfo, model);
        }
    }
}
