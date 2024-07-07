using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheetWeb.Models;

namespace StockCheckSheetWeb.Controllers
{
    public class StockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public StockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var inputList = _unitOfWork.Input.GetAll().ToList();
            var outputList = _unitOfWork.Output.GetAll().ToList();
            var stockList = _unitOfWork.Stock.GetAll().ToList();
            
            var TotalSum = inputList.Sum(u => u.Amount) - outputList.Sum(u => u.Amount);
            var TotalAmout = inputList.Sum(u => u.TotalCost) - outputList.Sum(u => u.TotalCost);

            ViewBag.InputList = inputList;
            ViewBag.OutputList = outputList;
            ViewBag.TotalSum = TotalSum;
            ViewBag.TotalAmout = TotalAmout;

            List<Stock> objStockList = _unitOfWork.Stock.GetAll().ToList();
            return View(objStockList);
        }

        public IActionResult ExportToExcel()
        {
            var inputs = _unitOfWork.Stock.GetAll().OrderBy(u => u.Date).ToList();

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

    }
}
