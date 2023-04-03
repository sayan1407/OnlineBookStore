using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BulkyBookWeb.Data;
using BulkyBookWeb.Models;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext Db)
        {
            _db = Db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> lstCategories = _db.Categories;
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

                _db.Categories.Add(category);
                _db.SaveChanges();
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
            var category = _db.Categories.SingleOrDefault(c => c.Id == id);
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

                _db.Categories.Update(category);
                _db.SaveChanges();
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
            var category = _db.Categories.SingleOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            var category = _db.Categories.SingleOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();
            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
            
           
        }
    }
}