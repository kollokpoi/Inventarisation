using Inventarisation.Interfaces;
using Inventarisation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventarisation.Controllers
{
    [Authorize]
    public class InventarisationController : Controller
    {

        IBDWork BDWork;
        public InventarisationController(IBDWork bDWork)
        {
            BDWork = bDWork;
        }
        // GET: InventorisationController
        /// <summary>
        /// Страница просмотра
        /// </summary>
        /// <returns>страница</returns>
        public async Task<ActionResult> Index()
        {
            var model = await BDWork.GetInventariations();

            InventarisationViewModel inventarisationViewModel = new();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int id = int.Parse(userId);
            foreach (var item in model)
            {
                bool contains = await BDWork.GetInventariationContain(item.Id, id);
                if (contains)
                {
                    inventarisationViewModel.Contains.Add(item);
                }
                else
                {
                    inventarisationViewModel.Other.Add(item);
                }
            }

            return View(inventarisationViewModel);
        }

        // GET: InventorisationController/Details/5
        /// <summary>
        /// Просмотр деталей
        /// </summary>
        /// <param name="InventId">id</param>
        /// <returns>страница</returns>
        public async Task<ActionResult> Details(int InventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? id = int.Parse(userId);
            if (User.IsInRole("Администратор"))
            {
                id = null;
            }
            var model = await BDWork.GetInventariation(InventId, id);

            return View(model);
        }

        // GET: InventorisationController/Create
        /// <summary>
        /// Создание
        /// </summary>
        /// <returns>страница</returns>
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult> Create()
        {
            var model = await BDWork.GetShortEquipment();

            return View(model);
        }

        // POST: InventorisationController/Create
        /// <summary>
        /// Создание
        /// </summary>
        /// <param name="addInventarisationViewModel">модель с информацией</param>
        /// <returns>переход на index</returns>
        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public ActionResult Create([FromBody]AddInventarisationViewModel addInventarisationViewModel)
        {
            try
            {
                BDWork.AddInventarisation(addInventarisationViewModel);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Create");
            }

        }
        /// <summary>
        /// Участие в инвентаризации
        /// </summary>
        /// <param name="InventId">id</param>
        /// <returns>страница</returns>
        public ActionResult JoinInventarisation(int InventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int id = int.Parse(userId);
            BDWork.AddUserToInventarisation(InventId, id);
            return RedirectToAction("Details",new { InventId });
        }
        /// <summary>
        /// Обновление оборудования
        /// </summary>
        /// <param name="id">id инвентаризации</param>
        /// <param name="count">количество</param>
        /// <param name="InventId">id оборудования</param>
        /// <returns></returns>
        public ActionResult UpdateInventarisationEquipment(int id, int count, int InventId)
        {
            BDWork.UpdateInventarisationEquipment(id, count);
            return RedirectToAction("Details", new { InventId });
        }


        public IActionResult Delete(int Id)
        {
            BDWork.DeleteInventarisation(Id);
            return RedirectToAction("Index");
        }
    }
}
