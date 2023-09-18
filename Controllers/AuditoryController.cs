using Inventarisation.Interfaces;
using Inventarisation.Models;
using Inventarisation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace Inventarisation.Controllers
{
    public class AuditoryController : Controller
    {
        IBDWork BDWork;
        public AuditoryController(IBDWork bDWork)
        {
            BDWork = bDWork;
        }
        // GET: Auditory
        /// <summary>
        /// страница просмотра
        /// </summary>
        /// <returns>страница</returns>
        public async Task<IActionResult> Index()
        {
            var model = await BDWork.GetAuditories();
            return View(model);
        }
        /// <summary>
        /// Просмотр информации
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>страница</returns>
        public async Task<IActionResult> Info(int id)
        {
            var model = await BDWork.GetAuditoryInfo(id);

            return View(model);
        }

        /// <summary>
        /// добавление
        /// </summary>
        /// <returns>страница</returns>
        public async Task<IActionResult> Add()
        {
            var model = await BDWork.GetUsers();

            return View(model);
        }
        /// <summary>
        /// добавление
        /// </summary>
        /// <param name="Name">название</param>
        /// <param name="ShortName">короткое название</param>
        /// <param name="ResponsibleUserId">id ответственного</param>
        /// <param name="TempResponsibleUserId">id временно-ответственного</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add([Required]string Name, [Required] string ShortName, [Required] int ResponsibleUserId, int TempResponsibleUserId=0)
        {
            
            if (ModelState.IsValid)
            {
                BDWork.AddAuditory(Name, ShortName, ResponsibleUserId, TempResponsibleUserId);
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
            var model = await BDWork.GetUsers();

            return View(model);
        }

        /// <summary>
        /// изменение
        /// </summary>
        /// <param name="id">id </param>
        /// <returns>страница</returns>
        public async Task<IActionResult> Update(int id)
        {
            var users = await BDWork.GetUsers();
            var auditory = await BDWork.GetAuditoryInfo(id);

            var model = new AuditoryUpdateViewModel();

            model.Auditory = auditory;
            model.Users = users;

            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name">название</param>
        /// <param name="ShortName">короткое название</param>
        /// <param name="ResponsibleUserId">id ответственного</param>
        /// <param name="TempResponsibleUserId">id временно-ответственного</param>
        /// <returns>Переход на index</returns>
        [HttpPost]
        public async Task<IActionResult> Update([Required]int id, [Required] string Name, [Required] string ShortName, [Required] int ResponsibleUserId, int TempResponsibleUserId = 0)
        {
            Auditory aud = new()
            {
                Id= id,
                Name = Name,
                ShortName = ShortName,
                ResponsibleUserId =ResponsibleUserId,
                TempResponsibleUserId = TempResponsibleUserId
            };

            if (ModelState.IsValid)
            {
                BDWork.UpdateAuditory(aud);
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
            var users = await BDWork.GetUsers();

            var model = new AuditoryUpdateViewModel();

            model.Auditory = aud;
            model.Users = users;

            return View(model);
        }
        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns>
        public IActionResult Delete(int Id)
        {
            BDWork.DeleteAuditory(Id);
            return RedirectToAction("Index");
        }
    }
}
