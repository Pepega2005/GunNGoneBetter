using GunNGoneBetter_DataMigrations;
using GunNGoneBetter_Models;
using GunNGoneBetter_Models.ViewModels;
using GunNGoneBetter_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;

namespace GunNGoneBetter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            this.db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel()
            {
                Products = db.Product.Include(u => u.Category),
                Categories = db.Category
            };

            return View(homeViewModel);
        }

        public IActionResult Details(int id)
        {
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            DetailsViewModel detailsViewModel = new DetailsViewModel()
            {
                isInCart = false,
                //Product = db.Product.Find(id)
                Product = db.Product.Include(x => x.Category).
                Where(x => x.Id == id).FirstOrDefault()
            };

            // проверка на наличие товара в корзине
            // если товар есть, то меняем свойство
            foreach (var item in cartList)
            {
                if (item.ProductId == id)
                {
                    detailsViewModel.isInCart = true;
                }
            }

            return View(detailsViewModel);
        }

        [HttpPost]
        public IActionResult DetailsPost(int id)
        {
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            cartList.Add(new Cart() { ProductId = id});

            HttpContext.Session.Set(PathManager.SessionCart, cartList);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            /*for (int i = 0; i < cartList.Count; i++)
            {
                if (id == cartList[i].ProductId)
                {
                    cartList.Remove(new Cart() { ProductId = id });
                }
            }*/

            var item = cartList.Single(x => x.ProductId == id);

            if (item != null)
            {
                cartList.Remove(item);
            }

            HttpContext.Session.Set(PathManager.SessionCart, cartList);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}