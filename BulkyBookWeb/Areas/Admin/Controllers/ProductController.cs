﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models.ViewModel;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();
            
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });
            if(id == null || id == 0)
            {
                productVM.Product = new Product();
            }
            else
            {
                Product product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
                productVM.Product = product;
            }
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj,IFormFile file)
        {
            
            if(ModelState.IsValid)
            {
                if(file != null)
                {
                    var rootPath = _hostEnvironment.WebRootPath;
                    var fileName = Guid.NewGuid().ToString();
                    var uploadPath = Path.Combine(rootPath, @"images/products/");
                    var fileExtension = Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName + fileExtension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    if(obj.Product.ImageUrl != null)
                    {
                        string oldImagePath = Path.Combine(rootPath, obj.Product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }
                    obj.Product.ImageUrl = @"/images/products/" + fileName + fileExtension;

                }
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product added successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();
               
                return RedirectToAction("Index");

            }
            return View();

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(IncludeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
            if (obj == null)
                return Json(new { success = false, message = "Cannot delete the product" });
            var rootPath = _hostEnvironment.WebRootPath;
            if (obj.ImageUrl != null)
            {
                string oldImagePath = Path.Combine(rootPath, obj.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "The product is deleted successfully" });

        }

    }
}