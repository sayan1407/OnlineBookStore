using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,IncludeProperties:"Product");
            foreach(var cart in shoppingCartVM.ListCart)
            {
                cart.Product.FinalPrice = GetPriceByCount(cart.Product.Price, cart.Product.Price50, cart.Product.Price100, cart.Count);
                shoppingCartVM.Price += cart.Product.FinalPrice * cart.Count;
            }
            
            return View(shoppingCartVM);
        }
        [HttpGet]
        public IActionResult Plus(int id)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id, IncludeProperties: "Product");
            _unitOfWork.ShoppingCart.IncreaseProductCount(shoppingCart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int id)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id, IncludeProperties: "Product");
            if(shoppingCart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecreaseProductCount(shoppingCart, 1);

            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int id)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id, IncludeProperties: "Product");
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        public double GetPriceByCount(double Price,double Price50,double Price100,int Count)
        {
            if (Count <= 50)
                return Price;
            if (Count > 50 && Count <= 100)
                return Price50;
            return Price100;
        }

    }
}