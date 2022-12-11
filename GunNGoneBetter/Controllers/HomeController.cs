using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using GunNGoneBetter.Models.ViewModels;
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
            DetailsViewModel detailsViewModel = new DetailsViewModel()
            {
                isInCart = false,
                //Product = db.Product.Find(id)
                Product = db.Product.Include(x => x.Category).
                Where(x => x.Id == id).FirstOrDefault()
            };

            return View(detailsViewModel);
        }

        [HttpPost]
        public IActionResult DetailsPost(int id)
        {
            

            return View();
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