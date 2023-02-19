using Microsoft.AspNetCore.Mvc;
using GunNGoneBetter_DataMigrations;
using GunNGoneBetter_Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using GunNGoneBetter_Utility;
using GunNGoneBetter_DataMigrations.Data;

using GunNGoneBetter_DataMigrations.Repository.IRepository;
using GunNGoneBetter_DataMigrations.Repository;

namespace GunNGoneBetter.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class MyModelController : Controller
    {
        //private ApplicationDbContext db;
        private IRepositoryMyModel repositoryMyModel;

        public MyModelController(IRepositoryMyModel repositoryMyModel)
        {
            this.repositoryMyModel = repositoryMyModel;
        }

        public IActionResult Index()
        {
            IEnumerable<MyModel> myModels = repositoryMyModel.GetAll();

            return View(myModels);
        }

        // GET - Create
        public IActionResult Create()
        {
            return View();
        }

        // POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MyModel myModel)
        {
            if (ModelState.IsValid) // проверка модели на валидность
            {
                repositoryMyModel.Add(myModel);
                repositoryMyModel.Save();

                return RedirectToAction("Index");
            }

            return View(myModel);
        }

        // GET - Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var model = repositoryMyModel.Find((int)id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MyModel model)
        {
            if (ModelState.IsValid) // проверка модели на валидность
            {
                repositoryMyModel.Update(model);
                repositoryMyModel.Save();

                return RedirectToAction("Index"); // переход на страницу категорий
            }

            return View(model);
        }

        // GET - Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var model = repositoryMyModel.Find((int)id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var model = repositoryMyModel.Find((int)id);

            if (model == null)
            {
                return NotFound();
            }

            repositoryMyModel.Remove(model);
            repositoryMyModel.Save();

            return RedirectToAction("Index");
        }
    }
}
