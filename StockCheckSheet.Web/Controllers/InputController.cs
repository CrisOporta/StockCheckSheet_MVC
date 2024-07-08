using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheet.Utility;
using StockCheckSheetWeb.Models;
using System.Security.Claims;

namespace StockCheckSheetWeb.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class InputController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public InputController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult ExportToExcel()
        {
            // Obtener los datos
            var inputs = _unitOfWork.Input.GetAll().OrderBy(u => u.Date).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Inputs");

                // Agregar los encabezados
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Amount";
                worksheet.Cell(1, 3).Value = "Unit Cost";
                worksheet.Cell(1, 4).Value = "Total Cost";

                // Formato de encabezados
                var headers = worksheet.Range("A1:D1");
                headers.Style.Font.Bold = true;
                headers.Style.Fill.BackgroundColor = XLColor.LightGreen;
                headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headers.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Agregar valores
                int row = 2; // Iniciar en la segunda fila
                foreach (var input in inputs)
                {
                    worksheet.Cell(row, 1).Value = input.Date.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 2).Value = input.Amount;
                    worksheet.Cell(row, 3).Value = input.UnitCost;
                    worksheet.Cell(row, 4).Value = input.TotalCost;

                    row++;
                }

                // Agregar totales
                worksheet.Cell(row, 1).Value = "Total";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Value = inputs.Sum(u => u.Amount);
                worksheet.Cell(row, 4).Value = inputs.Sum(u => u.TotalCost);

                // Dar formato a la tabla completamente
                var dataRange = worksheet.Range("A1:D" + row);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Font.FontSize = 14;

                // Dar formato a la fecha
                var date = worksheet.Range("A2:A" + row);
                date.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Dar formato a los valores
                var body = worksheet.Range("B2:D" + row);
                body.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Formato de pie de página
                var footer = worksheet.Range("B" + row.ToString() + ":D" + row);
                footer.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Ajustar el ancho de las columnas
                worksheet.Columns().AdjustToContents();

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Inputs-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }


        // List all Inputs ---------------------------------------------------
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<Input> objInputsList = _unitOfWork.Input.GetAll(u => u.ApplicationUserId == userId).ToList();
            return View(objInputsList);
        }


        // Return view Create ---------------------------------------------------
        public IActionResult Create()
        {
            var input = new Input();
            input.Date = DateTime.Now;
            return View(input);
        }
        // Create a new Input ---------------------------------------------------
        [HttpPost]
        public IActionResult Create(Input obj)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();

            obj.ApplicationUserId = userId;

            if (ModelState.IsValid)
            {
                Stock stock = new Stock();
                stock.Date = obj.Date;
                stock.Amount = obj.Amount;
                stock.UnitCost = obj.UnitCost;
                stock.TotalCost = obj.TotalCost;
                stock.ApplicationUserId = obj.ApplicationUserId;
                _unitOfWork.Stock.Add(stock);

                _unitOfWork.Input.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Input created successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }


        // Return view Edit ---------------------------------------------------
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Input inputFromDb = _unitOfWork.Input.Get(u => u.Id == id);

            if (inputFromDb == null)
            {
                return NotFound();
            }
            return View(inputFromDb);
        }
        // Update a Input ---------------------------------------------------
        [HttpPost]
        public IActionResult Edit(Input obj)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            obj.ApplicationUserId = userId;

            if (ModelState.IsValid)
            {
                Stock stock = _unitOfWork.Stock.Get(u => u.ApplicationUserId == userId);

                if(stock != null)
                {
                    stock.Date = obj.Date;
                    stock.Amount = obj.Amount;
                    stock.UnitCost = obj.UnitCost;
                    stock.TotalCost = obj.TotalCost;
                    stock.ApplicationUserId = obj.ApplicationUserId;
                    _unitOfWork.Stock.Update(stock);
                }

                _unitOfWork.Input.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Input updated successfully!";
                return RedirectToAction("Index");
            }
            return View();

        }

        // Return view Delete ---------------------------------------------------
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Input? inputFromDb = _unitOfWork.Input.Get(u => u.Id == id);

            if (inputFromDb == null)
            {
                return NotFound();
            }
            return View(inputFromDb);
        }
        // Delete a Input ---------------------------------------------------
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Input? obj = _unitOfWork.Input.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            Stock stock = _unitOfWork.Stock.Get(u => u.Date == obj.Date);

            stock.Date = obj.Date;
            stock.Amount = obj.Amount;
            stock.UnitCost = obj.UnitCost;
            stock.TotalCost = obj.TotalCost;
            stock.ApplicationUserId = obj.ApplicationUserId;

            _unitOfWork.Stock.Remove(stock);
            _unitOfWork.Input.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Input deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
