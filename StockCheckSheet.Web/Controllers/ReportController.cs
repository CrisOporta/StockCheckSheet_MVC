using Microsoft.AspNetCore.Mvc;

namespace StockCheckSheetWeb.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
