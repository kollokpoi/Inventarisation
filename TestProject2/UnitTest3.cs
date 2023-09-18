using Inventarisation.Controllers;
using Inventarisation.Interfaces;
using Inventarisation.Models;
using Inventarisation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject2
{
    public class UnitTest3
    {
        //Сценарий создает макет IBDWork и настраивает его для возвращения ожидаемого списка оборудования Equipment
        [Fact]
        public async Task Index_ReturnsViewWithEquipment()
        {
            // Arrange
            var mockBDWork = new Mock<IBDWork>();
            var expectedEquipment = new List<Equipment>();
            mockBDWork.Setup(b => b.GetShortEquipment()).ReturnsAsync(expectedEquipment);

            var controller = new EquipmentController(mockBDWork.Object);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Equipment>>(result.Model);
            var model = result.Model as List<Equipment>;
            Assert.Equal(expectedEquipment, model);
        }
        //Сценарий создает макет IBDWork и настраивает его для возвращения ожидаемой модели Equipment, вызывает Info с определенным id и проверяет, что результат - представление с правильной моделью
        [Fact]
        public async Task Info_ReturnsViewWithEquipmentInfo()
        {
            // Arrange
            var mockBDWork = new Mock<IBDWork>();
            var expectedEquipmentInfo = new Equipment
            {
                InventNumber = Guid.NewGuid(),
                Name = "Equipment 1",
            };
            mockBDWork.Setup(b => b.GetFullEquipment(It.IsAny<Guid>())).ReturnsAsync(expectedEquipmentInfo);

            var controller = new EquipmentController(mockBDWork.Object);

            // Act
            var result = await controller.Info(Guid.NewGuid()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Equipment>(result.Model);
            var model = result.Model as Equipment;
            Assert.Equal(expectedEquipmentInfo, model);
        }
        //Сценарий тестирует метод AddProgram. Он создает макет IBDWork и настраивает его для возвращения успешного результата при добавлении программы
        [Fact]
        public void AddProgram_ValidData_ReturnsOkResult()
        {
            // Arrange
            var mockBDWork = new Mock<IBDWork>();
            mockBDWork.Setup(b => b.AddEquipmentProgram(It.IsAny<Guid>(), It.IsAny<int>()));

            var controller = new EquipmentController(mockBDWork.Object);

            // Act
            var result = controller.AddProgram(1, Guid.NewGuid()) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

    }
}
