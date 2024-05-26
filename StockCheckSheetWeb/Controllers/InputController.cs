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
