﻿using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheet.Models.ViewModel;
using StockCheckSheetWeb.Models;

namespace StockCheckSheetWeb.Controllers
{
    public class InputController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public InputController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult ExportToExcel()
        {
            var inputs = _unitOfWork.Input.GetAll().OrderBy(u => u.Date).ToList();

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
            List<Input> objInputsList = _unitOfWork.Input.GetAll().ToList();
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

            if (ModelState.IsValid)
            {
                Stock stock = new Stock();
                stock.Date = obj.Date;
                stock.Amount = obj.Amount;
                stock.UnitCost = obj.UnitCost;
                stock.TotalCost = obj.TotalCost;
                stock.ReportId = obj.ReportId;
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
            Input? inputFromDb = _unitOfWork.Input.Get(u => u.Id == id);

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
                
                Stock stock = _unitOfWork.Stock.Get(u => u.Date == obj.Date);

                stock.Date = obj.Date;
                stock.Amount = obj.Amount;
                stock.UnitCost = obj.UnitCost;
                stock.TotalCost = obj.TotalCost;
                stock.ReportId = obj.ReportId;

                _unitOfWork.Stock.Update(stock);
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
            stock.ReportId = obj.ReportId;


            _unitOfWork.Stock.Remove(stock);
            _unitOfWork.Input.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Input deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
