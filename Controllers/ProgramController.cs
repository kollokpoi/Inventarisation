using Inventarisation.Interfaces;
using Inventarisation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Inventarisation.Controllers
{
    public class ProgramController : Controller
    {

        IBDWork BDWork;

        public ProgramController(IBDWork BDWork)
        {
            this.BDWork = BDWork;
        }

        // GET: ProgramController
        /// <summary>
        /// Список программ
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var model = await BDWork.GetPrograms();

            return View(model);
        }
        /// <summary>
        /// Получение оборудования по программе
        /// </summary>
        /// <param name="programId">id программы</param>
        /// <returns></returns>
        public async Task<List<Equipment>> GetEquipment(int programId)
        {
            var req = await BDWork.GetShortEquipmentByProg(programId);
            return req;
        }
       /// <summary>
       /// Добавление
       /// </summary>
       /// <returns>страница</returns>
        public  IActionResult Add()
        {
            return View();
        }
        /// <summary>
        /// Добавление
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Version">Версия</param>
        /// <param name="Creator">Разработчик</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([Required]string Name, [Required] string Version, [Required] string Creator)
        {
            if (ModelState.IsValid)
            {
                BDWork.AddProgram(Name, Version, Creator);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "";
                foreach (var key in ModelState.Keys)
                {
                    var entry = ModelState[key];
                    if (entry.ValidationState == ModelValidationState.Invalid)
                    {
                        // key содержит имя поля, которое не прошло валидацию
                        ViewBag.ErrorMessage += "Поле " + key + " обязательно к заполнению" + '\n';

                        // Добавьте fieldName в список или выполните другие действия по мере необходимости
                    }
                }
            }

            return View();
        }
        /// <summary>
        /// Обновление
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Страница</returns>
        public async Task<IActionResult> Update(int id)
        {
            var model = await BDWork.GetProgram(id);

            return View(model);
        }
        /// <summary>
        /// Обновление
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="Name">имя</param>
        /// <param name="Version">Версия</param>
        /// <param name="Creator">Разраб</param>
        /// <returns>Переход на индекс</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAsync([Required] int id,[Required] string Name, [Required] string Version, [Required] string Creator)
        {
            if (ModelState.IsValid)
            {
                BDWork.UpdateProgram(id,Name, Version, Creator);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "";
                foreach (var key in ModelState.Keys)
                {
                    var entry = ModelState[key];
                    if (entry.ValidationState == ModelValidationState.Invalid)
                    {
                        // key содержит имя поля, которое не прошло валидацию
                        ViewBag.ErrorMessage += "Поле " + key + " обязательно к заполнению" + '\n';

                        // Добавьте fieldName в список или выполните другие действия по мере необходимости
                    }
                }
            }
            var model = await BDWork.GetProgram(id);

            return View(model);
        }
        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns>Переход на index</returns>
        public IActionResult Delete(int Id)
        {
            BDWork.DeleteProgram(Id);
            return RedirectToAction("Index");
        }
    }
}
