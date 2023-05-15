using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _db;
        public CategoryController(IUnitOfWork Db)
        {
            _db = Db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> lstCategories = _db.Category.GetAll();
            return View(lstCategories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("CustomErr", "Category name and display order cannot be same");
            if (ModelState.IsValid)
            {

                _db.Category.Add(category);
                _db.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var category = _db.Category.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("CustomErr", "Category name and display order cannot be same");
            if (ModelState.IsValid)
            {

                _db.Category.Update(category);
                _db.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var category = _db.Category.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            var category = _db.Category.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();
            _db.Category.Remove(category);
            _db.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
            
           
        }
    }
}