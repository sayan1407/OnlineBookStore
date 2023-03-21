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
            
            _db.Categories.Add(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}