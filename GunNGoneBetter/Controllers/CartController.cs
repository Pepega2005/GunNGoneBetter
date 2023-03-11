﻿using GunNGoneBetter_DataMigrations;
using GunNGoneBetter_Models;
using GunNGoneBetter_Models.ViewModels;
using GunNGoneBetter_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GunNGoneBetter_DataMigrations.Data;
using GunNGoneBetter_DataMigrations.Repository.IRepository;
using System.ComponentModel.DataAnnotations.Schema;

namespace GunNGoneBetter.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        //ApplicationDbContext db;

        ProductUserViewModel productUserVIewModel;

        IWebHostEnvironment webHostEnvironment;

        IEmailSender emailSender;

        IRepositoryProduct repositoryProduct;
        IRepositoryApplicationUser repositoryApplicationUser;

        IRepositoryQueryHeader repositoryQueryHeader;
        IRepositoryQueryDetail repositoryQueryDetail;

        public CartController(IWebHostEnvironment webHostEnvironment,
            IEmailSender emailSender, IRepositoryProduct repositoryProduct,
            IRepositoryApplicationUser repositoryApplicationUser, IRepositoryQueryHeader repositoryQueryHeader,
            IRepositoryQueryDetail repositoryQueryDetail)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.emailSender = emailSender;
            this.repositoryApplicationUser = repositoryApplicationUser;
            this.repositoryProduct = repositoryProduct;
            this.repositoryQueryHeader = repositoryQueryHeader;
            this.repositoryQueryDetail = repositoryQueryDetail;
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
            IEnumerable<Product> productListTemp =
                repositoryProduct.GetAll(x => productsIdInCart.Contains(x.Id));

            List<Product> productList = new List<Product>();

            foreach (var item in cartList)
            {
                Product product = productListTemp.FirstOrDefault(x => x.Id == item.ProductId);
                product.TempCount = item.Count;

                productList.Add(product);
            }

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

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear(); // полная очитка сессии


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SummaryPost(ProductUserViewModel productUserViewModel)
        {
            // work with user
            var identityClaims = (ClaimsIdentity)User.Identity;
            var claim = identityClaims.FindFirst(ClaimTypes.NameIdentifier);


            // код для отправки сообщения
            var path = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() +
                    "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

            var subject = "New Order";

            string bodyHtml = "";

            using (StreamReader reader = new StreamReader(path))
            {
                bodyHtml = reader.ReadToEnd();
            }

            string textProducts = "";
            foreach (var item in productUserViewModel.ProductList)
            {
                textProducts += $"- Name: {item.Name}, ID: {item.Id}\n";
            }

            string body = string.Format(bodyHtml, productUserViewModel.ApplicationUser.FullName,
                productUserViewModel.ApplicationUser.Email,
                productUserViewModel.ApplicationUser.PhoneNumber,
                textProducts
                );

            await emailSender.SendEmailAsync(productUserViewModel.ApplicationUser.Email, subject, body);
            await emailSender.SendEmailAsync("elite.clone69@gmail.com", subject, body);

            // добавление данных в БД по заказу

            QueryHeader queryHeader = new QueryHeader()
            {
                ApplicationUserId = claim.Value,
                QueryTime = DateTime.Now,
                FullName = productUserViewModel.ApplicationUser.FullName,
                PhoneNumber = productUserViewModel.ApplicationUser.PhoneNumber,
                Email = productUserViewModel.ApplicationUser.Email,
                ApplicationUser = repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value)
            };

            // получение юзера
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //claim.Value -> getId

            repositoryQueryHeader.Add(queryHeader);
            repositoryQueryHeader.Save();

            // сделать запись деталей - всех продуктов в БД
            foreach (var item in productUserViewModel.ProductList)
            {
                QueryDetail queryDetail = new QueryDetail()
                {
                    ProductId = item.Id,
                    QueryHeaderId = queryHeader.Id,
                    QueryHeader = queryHeader,
                    Product = repositoryProduct.Find(item.Id)
                };

                repositoryQueryDetail.Add(queryDetail);
            }
            repositoryQueryDetail.Save();

            return RedirectToAction("InquiryConfirmation");
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

            IEnumerable<Product> productList = repositoryProduct.GetAll(x => productsIdInCart.Contains(x.Id));

            productUserVIewModel = new ProductUserViewModel()
            {
                ApplicationUser = repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value),
                ProductList = productList.ToList()
            };

            return View(productUserVIewModel);
        }

        [HttpPost]
        public IActionResult Update(IEnumerable<Product> products)
        {
            List<Cart> cartList = new List<Cart>();

            foreach (var product in products)
            {
                cartList.Add(new Cart()
                {
                    ProductId = product.Id,
                    Count = product.TempCount
                });

                HttpContext.Session.Set(PathManager.SessionCart, cartList);
            }

            return RedirectToAction("Index");
        }
    }
}