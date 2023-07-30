using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

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
            shoppingCartVM.OrderHeader = new OrderHeader();
            foreach(var cart in shoppingCartVM.ListCart)
            {
                cart.FinalPrice = GetPriceByCount(cart.Product.Price, cart.Product.Price50, cart.Product.Price100, cart.Count);
                shoppingCartVM.OrderHeader.OrderTotal += cart.FinalPrice * cart.Count;
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
                _unitOfWork.Save();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32("SessionCart", count);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecreaseProductCount(shoppingCart, 1);
                _unitOfWork.Save();

            }
            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int id)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id, IncludeProperties: "Product");
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
            HttpContext.Session.SetInt32("SessionCart", count);
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, IncludeProperties: "Product");
            shoppingCartVM.OrderHeader = new OrderHeader();
            foreach (var cart in shoppingCartVM.ListCart)
            {
                cart.FinalPrice = GetPriceByCount(cart.Product.Price, cart.Product.Price50, cart.Product.Price100, cart.Count);
                shoppingCartVM.OrderHeader.OrderTotal += cart.FinalPrice * cart.Count;
            }
            var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
            shoppingCartVM.OrderHeader.Name = applicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = applicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = applicationUser.City;
            shoppingCartVM.OrderHeader.State = applicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = applicationUser.PostalCode;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Today;

            return View(shoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM Cart)
        {
            if (Cart == null)
                return NotFound();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Cart.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, IncludeProperties: "Product");
            foreach (var cart in Cart.ListCart)
            {
                cart.FinalPrice = GetPriceByCount(cart.Product.Price, cart.Product.Price50, cart.Product.Price100, cart.Count);
                Cart.OrderHeader.OrderTotal += cart.FinalPrice * cart.Count;
            }
            Cart.OrderHeader.OrderStatus = "Pending";
            Cart.OrderHeader.PaymentStatus = "Pending";
            Cart.OrderHeader.OrderDate = DateTime.Today;
            Cart.OrderHeader.ApplicationUserId = claim.Value;
            _unitOfWork.OrderHeader.Add(Cart.OrderHeader);
            _unitOfWork.Save();
            foreach(var shoppingCart in Cart.ListCart)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = shoppingCart.ProductId,
                    Price = shoppingCart.FinalPrice,
                    Count = shoppingCart.Count,
                    OrderId = Cart.OrderHeader.Id,

                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            var domain = "https://localhost:44348/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/OrderConfirmation/?id={Cart.OrderHeader.Id}",
                CancelUrl = domain+$"customer/cart/index",
            };
            foreach(var cart in Cart.ListCart)
            {
                var sessionListItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.FinalPrice*100),
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.Title,
                        },
                    },
                    Quantity = cart.Count,
                };
                options.LineItems.Add(sessionListItem);
                
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdatePaymentId(Cart.OrderHeader, session.PaymentIntentId, session.Id);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if(session.PaymentStatus == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader, "Approved", "Approved");
                _unitOfWork.Save();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            List<ShoppingCart> lstShoppingCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, IncludeProperties: "Product").ToList();
            _unitOfWork.ShoppingCart.RemoveRange(lstShoppingCart);
            _unitOfWork.Save();
            return View(id);
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