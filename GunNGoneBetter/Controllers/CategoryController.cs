using Microsoft.AspNetCore.Mvc;
using GunNGoneBetter_DataMigrations;
using GunNGoneBetter_Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using GunNGoneBetter_Utility;
using GunNGoneBetter_DataMigrations.Repository.IRepository;

namespace GunNGoneBetter.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class CategoryController : Controller
    {
        private IRepositoryCategory repositoryCategory;

        //private ApplicationDbContext db;

        public CategoryController(IRepositoryCategory repositoryCategory)
        {
            this.repositoryCategory = repositoryCategory;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = repositoryCategory.GetAll();

            return View(categories);
        }

        // GET - Create
        public IActionResult Create()
        {
            return View();
        }

        // POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid) // проверка модели на валидность
            {
                repositoryCategory.Add(category);
                repositoryCategory.Save();

                return RedirectToAction("Index"); // переход на страницу категорий
            }

            return View(category);
        }

        // GET - Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //var category = db.Category.Find(id);
            var category = repositoryCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid) // проверка модели на валидность
            {
                //db.Category.Update(category); // !!!
                //db.SaveChanges();

                repositoryCategory.Update(category);
                repositoryCategory.Save();

                return RedirectToAction("Index"); // переход на страницу категорий
            }

            return View(category);
        }

        // GET - Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //var category = db.Category.Find(id);
            var category = repositoryCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            //var category = db.Category.Find(id);
            var category = repositoryCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            //db.Category.Remove(category);
            //db.SaveChanges();

            repositoryCategory.Remove(category);
            repositoryCategory.Save();

            return RedirectToAction("Index");
        }
    }
}
