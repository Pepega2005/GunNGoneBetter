using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using GunNGoneBetter.Models.ViewModels;
using System.Text.RegularExpressions;

namespace GunNGoneBetter.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db;
        private IWebHostEnvironment webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET INDEX
        public IActionResult Index()
        {
            IEnumerable<Product> objList = db.Product;

            // получаем ссылки на сущности категорий
            /*foreach (var item in objList)
            {
                // сопоставление таблицы категорий и таблицы product
                item.Category = db.Category.FirstOrDefault(x => x.Id == item.CategoryId);
            }*/

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

        // POST - CreateEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEdit(ProductViewModel productViewModel)
        {
            var files = HttpContext.Request.Form.Files;

            string wwRoot = webHostEnvironment.WebRootPath;

            if (productViewModel.Product.Id == 0)
            {
                // create
                string upload = wwRoot + PathManager.ImageProductPath;
                string imageName = Guid.NewGuid().ToString();

                string extension = Path.GetExtension(files[0].FileName);

                string path = upload + imageName + extension;

                // скопируем фацл на сервер
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productViewModel.Product.Image = imageName + extension;

                db.Product.Add(productViewModel.Product);
            }
            else
            {
                // update
            }
            
            db.SaveChanges();

            return RedirectToAction("Index");

            //return View();
        }
    }

}