using Inventarisation.Interfaces;
using Inventarisation.Models;
using Inventarisation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Inventarisation.Controllers
{
    [Authorize]
    public class ConsumablesController : Controller
    {
        IBDWork BDWork;
        public ConsumablesController(IBDWork bDWork)
        {
            BDWork = bDWork;
        }

        // GET: ConsumablesController
        /// <summary>
        /// Страница просмотра
        /// </summary>
        /// <returns>страница</returns>
        public async Task<IActionResult> Index()
        {
            var model = await BDWork.GetConsumables();
            return View(model);
        }
        /// <summary>
        /// Страница добавления
        /// </summary>
        /// <returns>страница</returns>
        public async Task<IActionResult> Add()
        {
            AddConsumablesViewModel viewModel = new AddConsumablesViewModel();
            viewModel.Users = await BDWork.GetUsers();
            viewModel.ConsumableTypes = await BDWork.GetConsumableTypes();
            if (viewModel.ConsumableTypes.Count>0)
            {
                viewModel.Specifications = await BDWork.GetConsumableSpecifications(viewModel.ConsumableTypes[0].Id);
            }
            

            return View(viewModel);
        }
        /// <summary>
        /// добавление
        /// </summary>
        /// <param name="Image">изображение</param>
        /// <param name="Name">имя</param>
        /// <param name="Description">описание</param>
        /// <param name="DateOfCame">дата получения</param>
        /// <param name="Count">количество</param>
        /// <param name="ResponsibleUserId">ответственный</param>
        /// <param name="TempResponsibleUserId">временно-ответственный</param>
        /// <param name="ConsumableType">тип расходника</param>
        /// <param name="SpecificationValues">спецификации</param>
        /// <returns>переход на index</returns>
        [HttpPost]
        public async Task<IActionResult> Add(IFormFile Image,[Required]string Name, string Description, [Required] DateTime DateOfCame, [Required] int Count, [Required] int ResponsibleUserId, int TempResponsibleUserId, int ConsumableType, string SpecificationValues)
        {

            if (ModelState.IsValid)
            {
                Consumables consumable = new Consumables();
                List<ConsumablesSpecificationsValues>? ConsumablesSpecificationsValues = new List<ConsumablesSpecificationsValues>();
                try
                {
                    ConsumablesSpecificationsValues = JsonSerializer.Deserialize<List<ConsumablesSpecificationsValues>>(SpecificationValues);
                }
                catch
                {
                    ConsumablesSpecificationsValues = new List<ConsumablesSpecificationsValues>();
                }
                if (Image != null && Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        Image.CopyTo(memoryStream);
                        consumable.Image = memoryStream.ToArray();

                    }
                }


                consumable.Name = Name;
                consumable.Description= Description;
                consumable.DateOfCame= DateOfCame;
                consumable.Count= Count;
                consumable.ResponsibleUserId =ResponsibleUserId;
                consumable.TempResponsibleUserId = TempResponsibleUserId;
                consumable.ConsumableTypeId = ConsumableType;

                BDWork.AddConsumable(consumable, ConsumablesSpecificationsValues);
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// просмотр деталей
        /// </summary>
        /// <param name="Id">id расходника</param>
        /// <returns>страница</returns>
        public async Task<IActionResult> Details(int Id)
        {
            var model = await BDWork.GetConsumable(Id);
            return View(model);
        }
        /// <summary>
        /// изменение
        /// </summary>
        /// <param name="Id">id</param>
        /// <returns>страница</returns>
        public async Task<IActionResult> Update(int Id)
        {
            var model = await BDWork.GetConsumable(Id);

            UpdateConsumablesViewModel viewModel = new UpdateConsumablesViewModel();
            viewModel.Consumable = model;
            viewModel.Users = await BDWork.GetUsers();

            return View(viewModel);
        }
        /// <summary>
        /// Изменение
        /// </summary>
        /// <param name="Id">id</param>
        /// <param name="Count">количество</param>
        /// <param name="ResponsibleUserId">ответственный</param>
        /// <param name="TempResponsibleUserId">временно ответственный</param>
        /// <returns>переход на index</returns>
        [HttpPost]
        public IActionResult Update(int Id, int Count, int ResponsibleUserId, int TempResponsibleUserId)
        {
            BDWork.UpdateConsumable(Id, Count, ResponsibleUserId, TempResponsibleUserId);
            return RedirectToAction("Index");
        }

        public async Task<List<Specifications>> GetSpecifications(int Id)
        {
            var model = await BDWork.GetConsumableSpecifications(Id);
            return model;
        }
        /// <summary>
        /// Добавление типа расходника
        /// </summary>
        /// <param name="Name">название</param>
        /// <returns>Тип расходника</returns>
        [HttpPost]
        public async Task<ConsumablesType> AddConsumableType(string Name)
        {
            int id = await BDWork.AddConsumableTypeAsync(Name);
            ConsumablesType type = new()
            {
                Id = id,
                Name = Name
            };
            return type;
        }
        /// <summary>
        /// добавление спецификации расходника
        /// </summary>
        /// <param name="Name">название</param>
        /// <param name="ConsumableId">расходникд</param>
        /// <returns>спецификация</returns>
        [HttpPost]
        public async Task<Specifications> AddConsumableSpecifications(string Name, int ConsumableId)
        {
            int id = await BDWork.AddConsumableSpecifications(Name, ConsumableId);
            Specifications type = new()
            {
                Id = id,
                Name = Name
            };
            return type;
        }
        /// <summary>
        /// удаление
        /// </summary>
        /// <param name="Id">id</param>
        /// <returns>переход на index</returns>
        public IActionResult Delete(int Id)
        {
            BDWork.DeleteConsumable(Id);
            return RedirectToAction("Index");
        }
    }
}
