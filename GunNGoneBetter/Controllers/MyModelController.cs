using Microsoft.AspNetCore.Mvc;
using GunNGoneBetter.Data;
using GunNGoneBetter.Models;

namespace GunNGoneBetter.Controllers
{
    public class MyModelController : Controller
    {
        private ApplicationDbContext db;

        public MyModelController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<MyModel> myModels = db.MyModel;

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
                db.MyModel.Add(myModel);
                db.SaveChanges();

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

            var model = db.MyModel.Find(id);

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
                db.MyModel.Update(model);
                db.SaveChanges();

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

            var model = db.MyModel.Find(id);

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
            var model = db.MyModel.Find(id);

            if (model == null)
            {
                return NotFound();
            }

            db.MyModel.Remove(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
