using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using GunNGoneBetter.Models.ViewModels;

namespace GunNGoneBetter.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db;

        public ProductController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET INDEX
        public IActionResult Index()
        {
            IEnumerable<Product> objList = db.Product;

            // получаем ссылки на сущности категорий
            foreach (var item in objList)
            {
                // сопоставление таблицы категорий и таблицы product
                item.Category = db.Category.FirstOrDefault(x => x.Id == item.CategoryId);
            }

            return View(objList);
        }

        // GET - CreateEdit
        public IActionResult CreateEdit(int? id)
        {
            // получаем лист категорий для отправки его во View
            /*IEnumerable<SelectListItem> CategoriesList = db.Category.Select(x =>
            new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            // отправляем лист категорий во View
            //ViewBag.CategoriesList = CategoriesList;
            ViewData["CategoriesList"] = CategoriesList;*/

            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = new Product(),
                CategoriesList = db.Category.Select(x =>
                new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (id == null)
            {
                // create
                return View(productViewModel);
            }
            else
            {
                // edit
                productViewModel.Product = db.Product.Find(id);
                if (productViewModel.Product == null)
                {
                    return NotFound();
                }

                return View(productViewModel);
            }

            return View();
        }
    }

}