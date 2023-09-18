using Inventarisation.Interfaces;
using Inventarisation.Models;
using Inventarisation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.NetworkInformation;
using Xceed.Document.NET;

namespace Inventarisation.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        IBDWork BDWork;
        private object excelFile;

        public EquipmentController(IBDWork BDWork)
        {
            this.BDWork = BDWork;
        }
        /// <summary>
        /// Страница просмотра оборудования
        /// </summary>
        /// <returns>Страница</returns>
        // GET: EquipmentController
        public async Task<IActionResult> Index()
        {
            var model = await BDWork.GetShortEquipment();

            return View(model);
        }
        /// <summary>
        /// Просмотр всей информации
        /// </summary>
        /// <param name="id">id оборудования</param>
        /// <returns>Страница</returns>
        public async Task<IActionResult> Info(Guid id)
        {
            var model = await BDWork.GetFullEquipment(id);

            return View(model);
        }
        /// <summary>
        /// Добавление
        /// </summary>
        /// <returns>страница</returns>
        public async Task<IActionResult> Add()
        {
            var model = await BDWork.GetEquipmentAddViewModel();

            return View(model);
        }

        /// <summary>
        /// Добавление
        /// </summary>
        /// <param name="Image">Изображение</param>
        /// <param name="equipmentAddViewModel">Данные</param>
        /// <returns>Переход на index</returns>
        [HttpPost]
        public async Task<IActionResult> Add(IFormFile? Image, EquipmentAddViewModel equipmentAddViewModel)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    byte[] ImageBytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Image.CopyTo(ms);
                        ImageBytes = ms.ToArray();
                    }
                    equipmentAddViewModel.Image = ImageBytes;
                }
                BDWork.AddEquipment(equipmentAddViewModel);
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
                        ViewBag.ErrorMessage+= "Поле " + key + " обязательно к заполнению"+'\n';

                        // Добавьте fieldName в список или выполните другие действия по мере необходимости
                    }
                }
            }


            var model = await BDWork.GetEquipmentAddViewModel();
            equipmentAddViewModel.Directions = model.Directions;
            equipmentAddViewModel.Users = model.Users;
            equipmentAddViewModel.Auditoryes = model.Auditoryes;
            equipmentAddViewModel.Directions = equipmentAddViewModel.Directions;
            equipmentAddViewModel.EquipmentModels = model.EquipmentModels;

            return View(equipmentAddViewModel);
        }
        /// <summary>
        /// Обновление информации
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Страница</returns>
        public async Task<IActionResult> Update(Guid id)
        {
            var model = await BDWork.GetEquipmentAddViewModel();
            var equipment = await BDWork.GetFullEquipment(id);

            EquipmentUpdateViewModel equipmentUpdateViewModel = new EquipmentUpdateViewModel();
            equipmentUpdateViewModel.Auditoryes = model.Auditoryes;
            equipmentUpdateViewModel.Users = model.Users;
            equipmentUpdateViewModel.Equipment = equipment;
            equipmentUpdateViewModel.Statuses = await BDWork.GetEquipmentStatus();
            equipmentUpdateViewModel.Programs = await BDWork.GetPrograms();

            return View(equipmentUpdateViewModel);
        }
        /// <summary>
        /// Добавление программы
        /// </summary>
        /// <param name="programId">Id программы</param>
        /// <param name="Id">id оборудования</param>
        /// <returns>результат запроса</returns>
        [HttpPost]
        public IActionResult AddProgram(int programId, [FromQuery]Guid Id)
        {
            try
            {
                BDWork.AddEquipmentProgram(Id,programId);
                return Ok();
            }
            catch 
            {
                return BadRequest();
            }
        }
        /// <summary>
        /// Обновление
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="Auditory">Аудитория</param>
        /// <param name="Status">Статус</param>
        /// <param name="ResponsibleUserId">ответственный</param>
        /// <param name="TempResponsibleUserId">временно-ответственный</param>
        /// <param name="comment">комментарий</param>
        /// <param name="Count">количество</param>
        /// <returns>переход на index</returns>
        [HttpPost]
        public async Task<IActionResult> Update([Required] Guid id, [Required]int Auditory, [Required] int Status, [Required] int ResponsibleUserId, [Required] int TempResponsibleUserId, string comment = "",int Count = 0)
        {

            if (ModelState.IsValid)
            {
                BDWork.UpdateEquipment(id, Auditory, Status, ResponsibleUserId, TempResponsibleUserId, comment,Count);
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
        /// Добавление настроек
        /// </summary>
        /// <param name="Id">id оборудования</param>
        /// <param name="settings">настройки</param>
        /// <returns>резултат добавления</returns>
        [HttpPost]
        public IActionResult AddSettings(Guid Id, EquipmentSettings settings)
        {
            try
            {
                BDWork.AddEquipmentSettings(Id, settings);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// добавление модели
        /// </summary>
        /// <returns>страница</returns>
        public  IActionResult AddEquipmentModel()
        {
            return View();
        }
        /// <summary>
        /// добавление модели
        /// </summary>
        /// <returns>результат добавления</returns>
        [HttpPost]
        public IActionResult AddEquipmentModel([Required] string Name, [Required] string Type)
        {
            if (ModelState.IsValid)
            {
                int? exists = BDWork.AddEquipmentModel(Name, Type);
                if (exists is null)
                {
                    return RedirectToActionPermanent("Add");
                }
                else
                {
                    return RedirectToAction("AddEquipmentConsumables", new {Id = exists});
                }
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
        /// Добавление направления
        /// </summary>
        /// <returns>страница</returns>
        public IActionResult AddDirection()
        {
            return View();
        }
        /// <summary>
        /// Добавление направления
        /// </summary>
        /// <param name="Name">Название</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddDirection([Required]string Name)
        {
            if (ModelState.IsValid)
            {
                BDWork.AddDirection(Name);
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
            return RedirectToAction("Index");
        }

        /// <summary>
        /// История передвижения
        /// </summary>
        /// <param name="Id">id оборудования</param>
        /// <returns></returns>
        public async Task<IActionResult> StoryOfMove(Guid Id)
        {
            var model = await BDWork.StoryOfMove(Id);


            return View(model);
        }
       /// <summary>
       /// История ответственных
       /// </summary>
       /// <param name="Id">id оборудования</param>
       /// <returns></returns>
        public async Task<IActionResult> StoryOfResponce(Guid Id)
        {
            var model = await BDWork.StoryOfResponce(Id);

            return View(model);
        }

        public IActionResult Delete(Guid Id)
        {
            BDWork.DeleteEquipment(Id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEquipmentConsumables(int Id)
        {
            var model = await BDWork.GetConsumables();
            return View(model);
        }
        public class EquipmentDataModel
        {
            public List<int> EquipmentNumbers { get; set; }
        }
        /// <summary>
        /// Добавление расходников
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddEquipmentConsumables(int Id, [FromBody] EquipmentDataModel data)
        {
            BDWork.AddEquipTypeConsumables(Id, data.EquipmentNumbers);
            return RedirectToActionPermanent("Add");
        }

        /// <summary>
        /// импорт
        /// </summary>
        /// <returns>Страница</returns>
        public IActionResult Import()
        {
            return View();
        }
        /// <summary>
        /// страница
        /// </summary>
        /// <param name="File">Файл импорта</param>
        /// <param name="Direction">направление</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile File, string Direction)
        {
            
            if (File == null || File.Length == 0)
            {
                // Обработка случая, когда файл отсутствует или пуст
                ViewBag.ErrorMessage = "Добавьте файл";
                return View();
            }
            if (Direction == "")
            {
                // Обработка случая, когда файл отсутствует или пуст
                ViewBag.ErrorMessage = "Добавьте направление";
                return View();
            }
            int directionId = await BDWork.GetDirectionId(Direction);
            string fileExtension = Path.GetExtension(File.FileName);
            if (!string.Equals(fileExtension, ".xls", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Файл - не xls";
                return View();
            }

            using (var memoryStream = new MemoryStream())
            {
                await File.CopyToAsync(memoryStream);

                using (var fs = new MemoryStream(memoryStream.ToArray()))
                {
                    try
                    {
                        HSSFWorkbook workbook = new HSSFWorkbook(fs);
                        ISheet sheet = workbook.GetSheetAt(0); // Получаем первый лист (индекс 0)

                        if (sheet != null)
                        {
                            int CurrentUserId = 0;
                            int CurrentType = 0;
                            int CurrentModel = 0;
                            string CurrentName = "";
                            int CurrentCount = 0;
                            // Итерация по строкам
                            for (int rowIndex = 2; rowIndex <= sheet.LastRowNum; rowIndex++)
                            {
                                IRow row = sheet.GetRow(rowIndex);

                                if (row != null)
                                {
                                    bool addEquip = false;
                                    // Итерация по ячейкам
                                    for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                                    {
                                        ICell cell = row.GetCell(cellIndex);

                                        if (cell != null)
                                        {
                                            string cellValue = cell.ToString();

                                            if (cellValue != null)
                                            {
                                                string[] values = cellValue.Split(' ');
                                                bool user = false;
                                                if (cellIndex == 0)
                                                {
                                                    if (char.IsUpper(values[0][0]) && char.IsUpper(values[1][0]) && char.IsUpper(values[2][0]))
                                                    {
                                                        CurrentUserId = await BDWork.GetUserByFullName(values[1], values[0], values[2]);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        values = cellValue.Split(',');
                                                        if (values.Length > 1)
                                                        {
                                                            CurrentType = await BDWork.GetEquipmentTypeByNameAsync(values[values.Length - 1]);
                                                        }
                                                    }
                                                }
                                                if (cellIndex == 1)
                                                {
                                                    if (cellValue != "")
                                                    {
                                                        CurrentModel = await BDWork.GetEquipmentModel(values[values.Length - 2], CurrentType);
                                                        for (int i = 0; i < values.Length - 2; i++)
                                                        {
                                                            CurrentName += values[i] + ' ';
                                                        }
                                                    }
                                                }
                                                if (cellIndex == 3)
                                                {
                                                    if (cellValue != "" && CurrentModel != 0)
                                                    {
                                                        CurrentCount = int.Parse(cellValue);
                                                        addEquip = true;

                                                    }
                                                }
                                            }


                                            // Обработайте значение ячейки по вашему усмотрению
                                        }
                                    }
                                    if (addEquip && CurrentUserId != 0 && CurrentType != 0 && CurrentModel != 0)
                                    {

                                        BDWork.AddImportedEquipmentAsync(CurrentName, CurrentUserId, CurrentModel, directionId, CurrentCount);
                                        CurrentCount = 0;
                                        CurrentName = string.Empty;
                                    }
                                }

                            }
                        }
                    }
                    catch
                    {
                        return View();
                    }
                    
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// экспорт
        /// </summary>
        /// <returns>страница</returns>
        public async Task<IActionResult> Export()
        {
            var model = await BDWork.GetDirections();
            return View(model);
        }

        /// <summary>
        /// Экспорт акта передачи
        /// </summary>
        /// <param name="DirectionID">id направления</param>
        /// <returns>Фаqk</returns>
        [HttpPost]
        public async Task<IActionResult> Export([Required]int DirectionID)
        {
            if (ModelState.IsValid)
            {
                var model = await BDWork.GetDirectionEquipment(DirectionID);

                // Создаем новый документ Word
                var doc = Xceed.Words.NET.DocX.Create("ActOfAcceptance.docx"); // Замените на путь и имя вашего документа

                // Добавляем заголовок акта
                doc.InsertParagraph("АКТ ПРИЕМА ПЕРЕДАЧИ ТОВАРА").FontSize(16).Bold().Alignment = Alignment.center;
                doc.InsertParagraph("");
                doc.InsertParagraph("");
                doc.InsertParagraph("");

                // Добавляем таблицу для товаров
                var table = doc.AddTable(model.Count+2, 5);
                table.Design = TableDesign.TableGrid;
                table.Alignment = Alignment.center;

                // Устанавливаем ширину столбцов таблицы
                float[] columnWidths = { 100, 150, 100, 150 };
                table.SetWidths(columnWidths);

                // Заголовки столбцов
                table.Rows[0].Cells[0].Paragraphs[0].InsertText("№");
                table.Rows[0].Cells[1].Paragraphs[0].InsertText("Наименование");
                table.Rows[0].Cells[2].Paragraphs[0].InsertText("Кол-во");
                table.Rows[0].Cells[3].Paragraphs[0].InsertText("Цена с НДС");
                table.Rows[0].Cells[4].Paragraphs[0].InsertText("Сумма с НДС");

                // Заполняем таблицу данными о товарах (замените на ваши данные)
                double totalPrice = 0;
                for (int i = 0; i < model.Count; i++)
                {
                    table.Rows[i+1].Cells[0].Paragraphs[0].InsertText((i+1).ToString());
                    table.Rows[i+1].Cells[1].Paragraphs[0].InsertText(model[i].Name);
                    table.Rows[i+1].Cells[2].Paragraphs[0].InsertText(model[i].Count.ToString());
                    table.Rows[i+1].Cells[3].Paragraphs[0].InsertText(model[i].Price.ToString());
                    table.Rows[i + 1].Cells[4].Paragraphs[0].InsertText((model[i].Count * model[i].Price).ToString());
                    totalPrice += (model[i].Count * model[i].Price);
                }
                table.Rows[model.Count + 1].Cells[3].Paragraphs[0].InsertText("Итого на сумму:");
                table.Rows[model.Count + 1].Cells[4].Paragraphs[0].InsertText(totalPrice.ToString());
                doc.InsertTable(table);

                // Добавляем пустую строку
                doc.InsertParagraph("");

                // Стоимость товара
                doc.InsertParagraph($"Стоимость Товара поставленного в соответствии с условиями Договора составляет {totalPrice} (___________________), с учетом НДС.")
                    .FontSize(12);

                // Добавляем пустую строку
                doc.InsertParagraph("");

                // Секция для подписей и должностей Покупателя и Продавца (замените на ваши данные)
                doc.InsertParagraph("Покупатель:").FontSize(12);
                doc.InsertParagraph("Должность").FontSize(12);
                doc.InsertParagraph("___________________________ Ф.И.О.").FontSize(12).Bold();
                doc.InsertParagraph("М.П.").FontSize(12);

                doc.InsertParagraph("Продавец:").FontSize(12);
                doc.InsertParagraph("Должность").FontSize(12);
                doc.InsertParagraph("___________________________ Ф.И.О.").FontSize(12).Bold();
                doc.InsertParagraph("М.П.").FontSize(12);

                // Сохраняем документ
                string tempFilePath = Path.Combine(Path.GetTempPath(), "ActOfAcceptance.docx"); // Замените на путь и имя вашего документа
                doc.SaveAs(tempFilePath);

                // Отправляем файл в ответ на запрос
                var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes(tempFilePath));
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "ActOfAcceptance.docx");
            }
            else { return View(); }
        }
    }
}
