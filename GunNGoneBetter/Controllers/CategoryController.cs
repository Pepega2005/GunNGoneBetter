﻿using Microsoft.AspNetCore.Mvc;
using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace GunNGoneBetter.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db;

        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = db.Category;

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
                db.Category.Add(category);
                db.SaveChanges();

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

            var category = db.Category.Find(id);

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
                db.Category.Update(category);
                db.SaveChanges();

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

            var category = db.Category.Find(id);

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
            var category = db.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            db.Category.Remove(category);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
