using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xceed.Document.NET;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheet.Models.ViewModel;
using StockCheckSheet.Utility;
using StockCheckSheetWeb.Models;
using System.Security.Claims;
using Xceed.Words.NET;
using Alignment = Xceed.Document.NET.Alignment;

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

                // Formato de encabezados
                var headers = worksheet.Range("A1:D1");
                headers.Style.Font.Bold = true;
                headers.Style.Fill.BackgroundColor = XLColor.LightGreen;
                headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headers.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Agregar valores
                int row = 2; // Iniciar en la segunda fila
                foreach (var output in outputs)
                {
                    worksheet.Cell(row, 1).Value = output.Date.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 2).Value = output.Amount;
                    worksheet.Cell(row, 3).Value = output.UnitCost;
                    worksheet.Cell(row, 4).Value = output.TotalCost;

                    row++;
                }

                // Agregar totales
                worksheet.Cell(row, 1).Value = "Total";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Value = outputs.Sum(u => u.Amount);
                worksheet.Cell(row, 4).Value = outputs.Sum(u => u.TotalCost);

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

                string excelName = $"Outputs-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }



        public IActionResult ExportToWord()
        {
            var outputs = _unitOfWork.Output.GetAll().OrderBy(u => u.Date).ToList();

            using (var document = DocX.Create("Outputs.docx"))
            {
                // Agregar un título
                document.InsertParagraph("Outputs")
                    .FontSize(20)
                    .Bold()
                    .Alignment = Alignment.center;

                // Agregar una tabla
                var table = document.AddTable(outputs.Count + 2, 4);
                table.Design = TableDesign.TableGrid;

                // Agregar los encabezados
                table.Rows[0].Cells[0].Paragraphs[0].Append("Date").Bold();
                table.Rows[0].Cells[1].Paragraphs[0].Append("Amount").Bold();
                table.Rows[0].Cells[2].Paragraphs[0].Append("Unit Cost").Bold();
                table.Rows[0].Cells[3].Paragraphs[0].Append("Total Cost").Bold();

                // Agregar valores
                for (int i = 0; i < outputs.Count; i++)
                {
                    table.Rows[i + 1].Cells[0].Paragraphs[0].Append(outputs[i].Date.ToString("dd/MM/yyyy"));
                    table.Rows[i + 1].Cells[1].Paragraphs[0].Append(outputs[i].Amount.ToString());
                    table.Rows[i + 1].Cells[2].Paragraphs[0].Append(outputs[i].UnitCost.ToString());
                    table.Rows[i + 1].Cells[3].Paragraphs[0].Append(outputs[i].TotalCost.ToString());
                }

                // Agregar totales
                table.Rows[outputs.Count + 1].Cells[0].Paragraphs[0].Append("Total").Bold();
                table.Rows[outputs.Count + 1].Cells[1].Paragraphs[0].Append(outputs.Sum(u => u.Amount).ToString());
                table.Rows[outputs.Count + 1].Cells[3].Paragraphs[0].Append(outputs.Sum(u => u.TotalCost).ToString());

                // Insertar la tabla en el documento
                document.InsertTable(table);

                // Guardar el documento en un MemoryStream
                var stream = new MemoryStream();
                document.SaveAs(stream);
                stream.Position = 0;

                string wordName = $"Outputs-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.docx";
                return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordName);
            }
        }


        // List all Outputs ---------------------------------------------------
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

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

            var inputList = _unitOfWork.Input.GetAll(u => u.ApplicationUserId == userId).ToList();
            var outputList = _unitOfWork.Output.GetAll(u => u.ApplicationUserId == userId).ToList();
            var minInputsByDate = inputList.Where(u => u.Date < obj.Output.Date);

            OutputVM outputVM = new()
            {
                InputList = inputList,
                Output = new Output(),
            };

            if (inputList.Count == 0)
            {
                TempData["error"] = "You must have at least one input to create an output!";
                return View(outputVM);
            }

            var totalAmountOutputList = outputList.Sum(u => u.Amount);
            var minDateTimeInput = inputList.Min(u => u.Date);
            var totalAmountInputList = inputList.Sum(u => u.Amount);
            var minAmountInputsByDate = minInputsByDate.Sum(u => u.Amount);

            obj.Output.ApplicationUserId = userId;

            if (obj.Output.Date < minDateTimeInput)
            {
                TempData["error"] = "Date entered is invalid!";
                return View(outputVM);
            }

            if (obj.Output.Amount > totalAmountInputList || (obj.Output.Amount + totalAmountOutputList) > totalAmountInputList)
            {
                TempData["error"] = "Amount overflows total inputs!";
                return View(outputVM);
            }

            if (obj.Output.Amount > minAmountInputsByDate)
            {
                TempData["error"] = "Amount overflows inputs by its date!";
                return View(outputVM);
            }

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

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<Input> InputList = _unitOfWork.Input.GetAll(u => u.ApplicationUserId == userId.ToString()).ToList();

            OutputVM outputVM = new()
            {
                InputList = InputList,
                Output = outputFromDb,
            };

            return View(outputVM);
        }
        // Update a Output ---------------------------------------------------
        [HttpPost]
        public IActionResult Edit(OutputVM obj)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var inputList = _unitOfWork.Input.GetAll(u => u.ApplicationUserId == userId).ToList();
            var outputList = _unitOfWork.Output.GetAll(u => u.ApplicationUserId == userId).ToList();
            var minInputsByDate = inputList.Where(u => u.Date < obj.Output.Date);

            if (outputList.Count > 0)
            {
                outputList.RemoveAt(outputList.Count - 1);
            }

            var totalAmountOutputList = outputList.Sum(u => u.Amount);
            var minDateTimeInput = inputList.Min(u => u.Date);
            var totalAmountInputList = inputList.Sum(u => u.Amount);
            var minAmountInputsByDate = minInputsByDate.Sum(u => u.Amount);

            OutputVM outputVM = new()
            {
                InputList = inputList,
                Output = obj.Output,
            };

            obj.Output.ApplicationUserId = userId;

            if (obj.Output.Date < minDateTimeInput)
            {
                TempData["error"] = "Date entered is invalid!";
                return View(outputVM);
            }

            if (obj.Output.Amount > totalAmountInputList || (obj.Output.Amount + totalAmountOutputList) > totalAmountInputList)
            {
                TempData["error"] = "Amount overflows total inputs!";
                return View(outputVM);
            }

            if (obj.Output.Amount > minAmountInputsByDate)
            {
                TempData["error"] = "Amount overflows inputs by its date!";
                return View(outputVM);
            }

            if (ModelState.IsValid)
            {
                Stock stock = _unitOfWork.Stock.Get(u => u.ApplicationUserId == userId);
                if (stock != null)
                {
                    stock.Date = obj.Output.Date;
                    stock.Amount = obj.Output.Amount;
                    stock.UnitCost = obj.Output.UnitCost;
                    stock.TotalCost = obj.Output.TotalCost;
                    stock.ApplicationUserId = obj.Output.ApplicationUserId;
                    _unitOfWork.Stock.Update(stock);
                }

                _unitOfWork.Output.Update(obj.Output);
                _unitOfWork.Save();
                TempData["success"] = "Output updated successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
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
