using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCheckSheetWeb.Data;
using StockCheckSheetWeb.Models;

namespace StockCheckSheetWeb.Controllers
{
    public class InputController : Controller
    {

        // Make connection to Database
        private readonly ApplicationDbContext _db;
        public InputController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult ExportToExcel()
        {
            var inputs = _db.Inputs.OrderByDescending(u => u.Date).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Inputs");

                // Agregar los encabezados
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Amount";
                worksheet.Cell(1, 3).Value = "Unit Cost";
                worksheet.Cell(1, 4).Value = "Total Cost";

                // Agregar valores
                for (int i = 0; i < inputs.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = inputs[i].Date.ToString("dd/MM/yyyy");
                    worksheet.Cell(i + 2, 2).Value = inputs[i].Amount;
                    worksheet.Cell(i + 2, 3).Value = inputs[i].UnitCost;
                    worksheet.Cell(i + 2, 4).Value = inputs[i].TotalCost;
                }

                // Agregar totales
                worksheet.Cell(inputs.Count + 2, 1).Value = "Total";
                worksheet.Cell(inputs.Count + 2, 2).Value = inputs.Sum(u => u.Amount);
                worksheet.Cell(inputs.Count + 2, 4).Value = inputs.Sum(u => u.TotalCost);

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
            //var objCategoryList = _db.Categories.ToList();
            List<Input> objInputsList = _db.Inputs.ToList();
            return View(objInputsList);
        }


        // Return view Create ---------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }
        // Create a new Input ---------------------------------------------------
        [HttpPost]
        public IActionResult Create(Input obj)
        {
            

            if (ModelState.IsValid)
            {
                _db.Inputs.Add(obj);
                _db.SaveChanges();
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
            Input? inputFromDb = _db.Inputs.Find(id);

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
            if (ModelState.IsValid)
            {
                _db.Inputs.Update(obj);
                _db.SaveChanges();
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
            Input? inputFromDb = _db.Inputs.Find(id);

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
            Input? obj = _db.Inputs.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Inputs.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Input deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
