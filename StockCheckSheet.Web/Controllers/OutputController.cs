using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockCheckSheet.DataAccess.Data;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheet.Models.ViewModel;
using StockCheckSheet.Utility;
using StockCheckSheetWeb.Models;
using System.Security.Claims;

namespace StockCheckSheetWeb.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class OutputController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public OutputController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult ExportToExcel()
        {
            var outputs = _unitOfWork.Output.GetAll().OrderBy(u => u.Date).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Outputs");

                // Agregar los encabezados
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Amount";
                worksheet.Cell(1, 3).Value = "Unit Cost";
                worksheet.Cell(1, 4).Value = "Total Cost";

                // Agregar valores
                for (int i = 0; i < outputs.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = outputs[i].Date.ToString("dd/MM/yyyy");
                    worksheet.Cell(i + 2, 2).Value = outputs[i].Amount;
                    worksheet.Cell(i + 2, 3).Value = outputs[i].UnitCost;
                    worksheet.Cell(i + 2, 4).Value = outputs[i].TotalCost;
                }

                // Agregar totales
                worksheet.Cell(outputs.Count + 2, 1).Value = "Total";
                worksheet.Cell(outputs.Count + 2, 2).Value = outputs.Sum(u => u.Amount);
                worksheet.Cell(outputs.Count + 2, 4).Value = outputs.Sum(u => u.TotalCost);

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Outputs-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        // List all Outputs ---------------------------------------------------
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //var objCategoryList = _db.Categories.ToList();
            List<Output> objOutputsList = _unitOfWork.Output.GetAll(u => u.ApplicationUserId == userId).ToList();
            return View(objOutputsList);
        }


        // Return view Create ---------------------------------------------------
        public IActionResult Create()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<Input> InputList = _unitOfWork.Input.GetAll(u => u.ApplicationUserId == userId.ToString()).ToList();

            OutputVM outputVM = new()
            {
                InputList = InputList,
                Output = new Output(),
            };

            outputVM.Output.Date = DateTime.Now;


            return View(outputVM);
        }
        // Create a new Output ---------------------------------------------------
        [HttpPost]
        public IActionResult Create(OutputVM obj)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            obj.Output.ApplicationUserId = userId;

            if (ModelState.IsValid)
            {
                Stock stock = new Stock();
                stock.Date = obj.Output.Date;
                stock.Amount = obj.Output.Amount;
                stock.UnitCost = obj.Output.UnitCost;
                stock.TotalCost = obj.Output.TotalCost;
                stock.ApplicationUserId = obj.Output.ApplicationUserId;
                _unitOfWork.Stock.Add(stock);

                _unitOfWork.Output.Add(obj.Output);
                _unitOfWork.Save();
                TempData["success"] = "Output created successfully!";
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
            Output outputFromDb = _unitOfWork.Output.Get(u => u.Id == id);

            if (outputFromDb == null)
            {
                return NotFound();
            }
            return View(outputFromDb);
        }
        // Update a Output ---------------------------------------------------
        [HttpPost]
        public IActionResult Edit(Output obj)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            obj.ApplicationUserId = userId;

            if (ModelState.IsValid)
            {

                Stock stock = _unitOfWork.Stock.Get(u => u.Date == obj.Date);

                if (stock != null)
                {
                    stock.Date = obj.Date;
                    stock.Amount = obj.Amount;
                    stock.UnitCost = obj.UnitCost;
                    stock.TotalCost = obj.TotalCost;
                    stock.ApplicationUserId = obj.ApplicationUserId;
                    _unitOfWork.Stock.Update(stock);
                }

                _unitOfWork.Output.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Output updated successfully!";
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
            Output? outputFromDb = _unitOfWork.Output.Get(u => u.Id == id);

            if (outputFromDb == null)
            {
                return NotFound();
            }
            return View(outputFromDb);
        }
        // Delete a Output ---------------------------------------------------
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Output? obj = _unitOfWork.Output.Get(u => u.Id == id);
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
            _unitOfWork.Output.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Output deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
