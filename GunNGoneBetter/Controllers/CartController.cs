﻿using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using GunNGoneBetter.Models.ViewModels;
using GunNGoneBetter.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GunNGoneBetter.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        ApplicationDbContext db;

        ProductUserViewModel productUserVIewModel;

        public CartController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);

                // хотим получить каждый товар из корзины
            }

            // получаем лист id товаров
            List<int> productsIdInCart = cartList.Select(x => x.ProductId).ToList();

            // извлекаем сами продукты по списку id
            IEnumerable<Product> productList = db.Product.Where(x => productsIdInCart.Contains(x.Id));

            return View(productList);
        }

        public IActionResult Remove(int id)
        {
            // удаление из корзины
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            cartList.Remove(cartList.FirstOrDefault(x => x.ProductId == id));

            HttpContext.Session.Set(PathManager.SessionCart, cartList);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // если ипользователь вошел в систему, то обьект будет определен
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            List<int> productsIdInCart = cartList.Select(x => x.ProductId).ToList();

            IEnumerable<Product> productList = db.Product.Where(x => productsIdInCart.Contains(x.Id));

            productUserVIewModel = new ProductUserViewModel()
            {
                ApplicationUser = db.ApplicationUser.FirstOrDefault(x => x.Id == claim.Value),
                ProductList = productList
            };

            return View(productUserVIewModel);
        }
    }
}