using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Inventarisation.Interfaces;
using Inventarisation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Cryptography.X509Certificates;
using Inventarisation.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Inventarisation.Controllers
{

    public class UserController : Controller
    {

        IBDWork BDWork;
        public UserController(IBDWork bDWork) 
        { 
            BDWork = bDWork; 
        }
        /// <summary>
        /// авторизация
        /// </summary>
        /// <param name="Login">Логие</param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login([Required]string Login, [Required]string Password)
        {

            if (ModelState.IsValid)
            {
                User user;
                try
                {
                    user = await BDWork.Login(Login, Password);
                }
                catch
                {
                    ViewBag.ErrorMessage = "Ошибка базы данных";
                    return View();
                }
                if (user != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Имя пользователя
                    new Claim(ClaimTypes.Role, await BDWork.GetUserRole(user.UserRoleId))
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ErrorMessage = "Данные не верны";
                return View();
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
                return View();
            }
            
        }

        public IActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// Список пользователей
        /// </summary>
        /// <returns>страница</returns>
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Index()
        {
            var model = await BDWork.GetUsers();
            return View(model);
        }


        public ActionResult AccessDenied()
        {
            return View();
        }
        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <returns>Страница</returns>
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Add()
        {
            var model = await BDWork.GetUserRoles();
            return View(model);
        }

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <param name="Login">Логин</param>
        /// <param name="Password">Пароль</param>
        /// <param name="Email">Почта</param>
        /// <param name="Name">Имя</param>
        /// <param name="SecondName">Фамилия</param>
        /// <param name="LastName">Отчество</param>
        /// <param name="UserRoleId">Роль пользователя</param>
        /// <param name="Phone">Телефон</param>
        /// <param name="Adres">Адрес</param>
        /// <returns>Переход на index</returns>
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> Add([Required] int id, [Required] string Login, [Required] string Password, string Email, [Required] string Name, string SecondName, string LastName, int UserRoleId, string Phone, string Adres)
        {
            var user = new User()
            {
                Id = id,
                Login = Login,
                Password = Password,
                Email = Email,
                Name = Name,
                SecondName = SecondName,
                LastName = LastName,
                UserRoleId = UserRoleId,
                Phone = Phone,
                Adres = Adres

            };
            if (ModelState.IsValid)
            {
                BDWork.AddUser(user);
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

            var model = await BDWork.GetUserRoles();
            return View(model);
        }
        /// <summary>
        /// изменение данных пользователя
        /// </summary>
        /// <param name="id">Id пользвателя</param>
        /// <returns>Страница изменения</returns>
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Update(int id)
        {
            var roles = await BDWork.GetUserRoles();
            var user = await BDWork.GetFullUser(id);

            var model = new UserUpdateViewModel()
            {
                User = user,
                UserRoles = roles
            };
            return View(model);
        }

        /// <summary>
        /// добавление пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <param name="Login">Логин</param>
        /// <param name="Password">Пароль</param>
        /// <param name="Email">Почта</param>
        /// <param name="Name">Имя</param>
        /// <param name="SecondName">Фамилия</param>
        /// <param name="LastName">Отчество</param>
        /// <param name="UserRoleId">Роль пользователя</param>
        /// <param name="Phone">Телефон</param>
        /// <param name="Adres">Адрес</param>
        /// <returns>Переход на index</returns>
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> Update([Required] int id, [Required] string Login, [Required] string Password, string Email, [Required] string Name, string SecondName, string LastName, int UserRoleId, string Phone, string Adres)
        {
            var user = new User()
            {
                Id = id,
                Login = Login,
                Password = Password,
                Email = Email,
                Name = Name,
                SecondName = SecondName,
                LastName = LastName,
                UserRoleId = UserRoleId,
                Phone = Phone,
                Adres = Adres

            };

            if (ModelState.IsValid)
            {
                BDWork.UpdateUser(id,user);
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

            var roles = await BDWork.GetUserRoles();

            var model = new UserUpdateViewModel()
            {
                User = user,
                UserRoles = roles
            };
            return View(model);
        }

        /// <summary>
        /// удаление пользователя
        /// </summary>
        /// <param name="Id">id пользователя</param>
        /// <returns>Переход на index</returns>
        [Authorize(Roles = "Администратор")]
        public IActionResult Delete(int Id)
        {
            BDWork.DeleteUser(Id);
            return RedirectToAction("Index");
        }
    }
}
