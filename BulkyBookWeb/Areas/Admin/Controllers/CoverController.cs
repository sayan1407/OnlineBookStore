using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
      [Area("Admin")]
        public class CoverController : Controller
        {
            private readonly IUnitOfWork _db;
            public CoverController(IUnitOfWork Db)
            {
                _db = Db;
            }

            public IActionResult Index()
            {
                IEnumerable<CoverType> lstCoverType = _db.CoverType.GetAll();
                return View(lstCoverType);
            }
            [HttpGet]
            public IActionResult Create()
            {
                return View();
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Create(CoverType coverType)
            {
                
                if (ModelState.IsValid)
                {

                    _db.CoverType.Add(coverType);
                    _db.Save();
                    TempData["success"] = "Cover Type Created Successfully";
                    return RedirectToAction("Index");
                }
                return View();
            }
            [HttpGet]
            public IActionResult Edit(int? id)
            {
                if (id == null || id == 0)
                    return NotFound();
                var coverType = _db.CoverType.GetFirstOrDefault(c => c.ID == id);
                if (coverType == null)
                    return NotFound();
                return View(coverType);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(CoverType coverType)
            {
                if (ModelState.IsValid)
                {

                    _db.CoverType.Update(coverType);
                    _db.Save();
                    TempData["success"] = "Cover Type Updated Successfully";
                    return RedirectToAction("Index");
                }
                return View();
            }
            [HttpGet]
            public IActionResult Delete(int? id)
            {
                if (id == null || id == 0)
                    return NotFound();
                var coverType = _db.CoverType.GetFirstOrDefault(c => c.ID == id);
                if (coverType == null)
                    return NotFound();
                return View(coverType);
            }
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public IActionResult DeleteCoverType(int? id)
            {
                var coverType = _db.CoverType.GetFirstOrDefault(c => c.ID == id);
                if (coverType == null)
                    return NotFound();
                _db.CoverType.Remove(coverType);
                _db.Save();
                TempData["success"] = "Cover Type Deleted Successfully";
                return RedirectToAction("Index");


            }
        }

}