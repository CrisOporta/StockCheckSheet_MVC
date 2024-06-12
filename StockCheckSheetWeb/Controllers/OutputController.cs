using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheet.Models.ViewModel;
using StockCheckSheetWeb.Models;

namespace StockCheckSheetWeb.Controllers
{
    public class OutputController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public OutputController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            //var objCategoryList = _db.Categories.ToList();
            List<Output> objOutputsList = _unitOfWork.Output.GetAll().ToList();
            return View(objOutputsList);
        }


        // Return view Create ---------------------------------------------------
        public IActionResult Create()
        {
            List<Input> InputList = _unitOfWork.Input.GetAll().ToList();

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
            var amoutInputs = _unitOfWork.Input.GetAll().Sum(u=> u.Amount);

            List<Input> InputList = _unitOfWork.Input.GetAll().ToList();
            DateTime minDateInput = _unitOfWork.Input.GetAll().Min(u=>u.Date);

            OutputVM outputVM = new()
            {
                InputList = InputList,
                Output = new Output()
            };

            if (amoutInputs < obj.Output.Amount)
            {
                TempData["error"] = "La cantidad ingresada excede al número de existencias!";
                return View(outputVM);
            }

            if (minDateInput > obj.Output.Date)
            {
                TempData["error"] = "La fecha no es valida!";
                return View(outputVM);
            }


            if (ModelState.IsValid)
            {
                Stock stock = new Stock();
                stock.Date = obj.Output.Date;
                stock.Amount = obj.Output.Amount;
                stock.UnitCost = obj.Output.UnitCost;
                stock.TotalCost = obj.Output.TotalCost;
                stock.ReportId = obj.Output.ReportId;
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
            Output? inputFromDb = _unitOfWork.Output.Get(u => u.Id == id);

            if (inputFromDb == null)
            {
                return NotFound();
            }
            return View(inputFromDb);
        }
        // Update a Output ---------------------------------------------------
        [HttpPost]
        public IActionResult Edit(Output obj)
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
            Output? inputFromDb = _unitOfWork.Output.Get(u => u.Id == id);

            if (inputFromDb == null)
            {
                return NotFound();
            }
            return View(inputFromDb);
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
            stock.ReportId = obj.ReportId;


            _unitOfWork.Stock.Remove(stock);
            _unitOfWork.Output.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Output deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
