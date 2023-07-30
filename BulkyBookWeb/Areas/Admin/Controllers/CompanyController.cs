using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Company> lstCompanies = _unitOfWork.Company.GetAll();
            return Json(new { data = lstCompanies });
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company;
            if (id == 0 || id == null)
                company = new Company();
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            }
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["success"] = "Company updated successfully";
                }

            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return Json(new { success = false, message = "Company is not deleted" });
            }
            Company company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (company == null)
                return Json(new { success = false, message = "Company is not deleted" });
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company deleted successfully" });
        }
    }
}