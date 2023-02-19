using GunNGoneBetter_DataMigrations;
using GunNGoneBetter_Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GunNGoneBetter_Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using GunNGoneBetter_Models.ViewModels;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GunNGoneBetter_Utility;
using GunNGoneBetter_DataMigrations.Data;

using GunNGoneBetter_DataMigrations.Repository.IRepository;
using GunNGoneBetter_DataMigrations.Repository;

namespace GunNGoneBetter.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class ProductController : Controller
    {
        //private ApplicationDbContext db;
        private IRepositoryProduct repositoryProduct;

        private IWebHostEnvironment webHostEnvironment;

        public ProductController(IRepositoryProduct repositoryProduct, IWebHostEnvironment webHostEnvironment)
        {
            this.repositoryProduct = repositoryProduct;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET INDEX
        public IActionResult Index()
        {
            IEnumerable<Product> objList = repositoryProduct.GetAll();

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
                CategoriesList = repositoryProduct.GetListItems(PathManager.NameCategory),
                MyModelsList = repositoryProduct.GetListItems(PathManager.NameMyModel)
        };

            if (id == null)
            {
                // create
                return View(productViewModel);
            }
            else
            {
                // edit
                //productViewModel.Product = db.Product.Find(id);
                productViewModel.Product = repositoryProduct.Find(id.GetValueOrDefault());

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

            string wwwRoot = webHostEnvironment.WebRootPath;

            if (productViewModel.Product.Id == 0)
            {
                // create
                string upload = wwwRoot + PathManager.ImageProductPath;
                string imageName = Guid.NewGuid().ToString();

                string extension = Path.GetExtension(files[0].FileName);

                string path = upload + imageName + extension;

                // скопируем файл на сервер
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productViewModel.Product.Image = imageName + extension;

                repositoryProduct.Add(productViewModel.Product);
            }
            else
            {
                // update
                // AsNoTracking() - important!!!
                // db.Product.AsNoTracking().FirstOrDefault( u => u.Id == productViewModel.Product.Id);
                var product = repositoryProduct.FirstOrDefault(u => u.Id == productViewModel.Product.Id, isTracking: false);

                if (files.Count > 0) // юзер загружает другой файл
                {
                    string upload = wwwRoot + PathManager.ImageProductPath;
                    string imageName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    string path = upload + imageName + extension;

                    // delete old file
                    var oldFile = upload + product.Image;

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productViewModel.Product.Image = imageName + extension;
                }
                else // фотка не поменялась
                {
                    productViewModel.Product.Image = product.Image; // оставляем имя прежним
                }

                //db.Product.Update(productViewModel.Product);
                repositoryProduct.Update(productViewModel.Product);
            }

            //db.SaveChanges();
            repositoryProduct.Save();

            return RedirectToAction("Index");

            //return View();
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product product = repositoryProduct.FirstOrDefault
                (x => x.Id == id, includeProperties: "Category,MyModel"); // ???

            if (product == null)
            {
                return NotFound();
            }

            //product.Category = db.Category.Find(product.CategoryId);

            return View(product);
        }

        // POST - DELETE
        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            string wwwRoot = webHostEnvironment.WebRootPath;
            string upload = wwwRoot + PathManager.ImageProductPath;

            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product product = repositoryProduct.Find((int)id);

            var oldFile = upload + product.Image;

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            repositoryProduct.Remove(product);
            repositoryProduct.Save();

            return RedirectToAction("Index");
        }
    }

}