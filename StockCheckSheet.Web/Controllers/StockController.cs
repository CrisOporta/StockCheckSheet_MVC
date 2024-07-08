using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheetWeb.Models;
using System.Security.Claims;

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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var inputList = _unitOfWork.Input.GetAll(u => u.ApplicationUserId == userId).ToList();
            var outputList = _unitOfWork.Output.GetAll(u => u.ApplicationUserId == userId).ToList();
            var stockList = _unitOfWork.Stock.GetAll(u => u.ApplicationUserId == userId).ToList();

            var TotalSum = inputList.Sum(u => u.Amount) - outputList.Sum(u => u.Amount);
            var TotalAmout = inputList.Sum(u => u.TotalCost) - outputList.Sum(u => u.TotalCost);

            ViewBag.InputList = inputList;
            ViewBag.OutputList = outputList;
            ViewBag.TotalSum = TotalSum;
            ViewBag.TotalAmout = TotalAmout;

            return View(stockList);
        }

        public IActionResult ExportToExcel()
        {
            // Obtener los datos como en el método Index
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var inputList = _unitOfWork.Input.GetAll(u => u.ApplicationUserId == userId).ToList();
            var outputList = _unitOfWork.Output.GetAll(u => u.ApplicationUserId == userId).ToList();
            var stockList = _unitOfWork.Stock.GetAll(u => u.ApplicationUserId == userId).OrderBy(u => u.Date).ToList();

            var TotalSum = inputList.Sum(u => u.Amount) - outputList.Sum(u => u.Amount);
            var TotalAmount = inputList.Sum(u => u.TotalCost) - outputList.Sum(u => u.TotalCost);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("StockList");

                // Agregar los encabezados
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Amount";
                worksheet.Cell(1, 3).Value = "Unit Cost";
                worksheet.Cell(1, 4).Value = "Total Cost";
                worksheet.Cell(1, 5).Value = "Type"; 

                // Formato de encabezados
                var headers = worksheet.Range("A1:E1");
                headers.Style.Font.Bold = true;
                headers.Style.Fill.BackgroundColor = XLColor.LightGreen;
                headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headers.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Agregar valores
                int row = 2; // Iniciar en la segunda fila
                foreach (var obj in stockList)
                {
                    bool isInput = inputList.Any(i => i.Date == obj.Date && i.TotalCost == obj.TotalCost);
                    bool isOutput = outputList.Any(o => o.Date == obj.Date && o.TotalCost == obj.TotalCost);

                    worksheet.Cell(row, 1).Value = obj.Date.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 2).Value = obj.Amount;
                    worksheet.Cell(row, 3).Value = obj.UnitCost;
                    worksheet.Cell(row, 4).Value = obj.TotalCost;
                    worksheet.Cell(row, 5).Value = isInput ? "Input" : isOutput ? "Output" : "N/A"; 

                    row++;
                }

                // Agregar totales
                worksheet.Cell(row, 1).Value = "Total";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Value = TotalSum;
                worksheet.Cell(row, 4).Value = TotalAmount;

                // Dar formato a la tabla completamente
                var dataRange = worksheet.Range("A1:E" + row);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Font.FontSize = 14;

                // Dar formato a la fecha
                var date = worksheet.Range("A2:A" + row.ToString());
                date.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Dar formato a los valores
                var body = worksheet.Range("B2:E" + row);
                body.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Dar formato al tipo 
                var type = worksheet.Range("E2" + ":E" + row);
                type.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Formato de pie de página
                var footer = worksheet.Range("B" + row.ToString() + ":E" + row);
                footer.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Ajustar el ancho de las columnas
                worksheet.Columns().AdjustToContents();

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Stocks-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

    }
}
