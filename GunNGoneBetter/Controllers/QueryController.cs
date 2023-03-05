using GunNGoneBetter_DataMigrations;
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
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GunNGoneBetter.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class QueryController : Controller
    {
        private IRepositoryQueryHeader repositoryQueryHeader;
        private IRepositoryQueryDetail repositoryQueryDetail;

        [BindProperty]
        public QueryViewModel QueryViewModel { get; set; }

        public QueryController(IRepositoryQueryHeader repositoryQueryHeader,
            IRepositoryQueryDetail repositoryQueryDetail)
        {
            this.repositoryQueryHeader = repositoryQueryHeader;
            this.repositoryQueryDetail = repositoryQueryDetail;
        }

        public IActionResult Index()
        {           
            return View();
        }

        public IActionResult Details(int id)
        {
            QueryViewModel = new QueryViewModel()
            {
                // извлекаем хедер из репозитория
                QueryHeader = repositoryQueryHeader.FirstOrDefault(x => x.Id == id),

                QueryDetail = repositoryQueryDetail.
                GetAll(x => x.QueryHeader.Id == id,
                includeProperties: "Product")
            };

            return View(QueryViewModel);
        }

        [HttpPost]
        public IActionResult Details()
        {
            List<Cart> carts = new List<Cart>();

            QueryViewModel.QueryDetail = repositoryQueryDetail.GetAll(
                x => x.QueryHeader.Id == QueryViewModel.QueryHeader.Id);

            // создаем корзину покупок и добавляем значения в сессию
            foreach (var item in QueryViewModel.QueryDetail)
            {
                Cart cart = new Cart() { ProductId = item.ProductId };

                carts.Add(cart);
            }

            // работа с сессиями
            HttpContext.Session.Clear();
            HttpContext.Session.Set(PathManager.SessionCart, carts);

            // создаем еще одну сессию для определения того, что мы изменяем заказ
            HttpContext.Session.Set(
                PathManager.SessionQuery, QueryViewModel.QueryHeader.Id);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            QueryHeader queryHeader = repositoryQueryHeader.FirstOrDefault(
                x => x.Id == QueryViewModel.QueryHeader.Id);

            // получаем детали запроса
            IEnumerable<QueryDetail> queryDetails = repositoryQueryDetail.GetAll(
                x => x.QueryHeaderId == QueryViewModel.QueryHeader.Id);

            repositoryQueryDetail.Remove(queryDetails);
            repositoryQueryHeader.Remove(queryHeader);

            repositoryQueryDetail.Save();

            return RedirectToAction("Index");
        }

        public IActionResult GetQueryList()
        {
            JsonResult result = Json(new { data = repositoryQueryHeader.GetAll() });

            return result;
        }
    }
}
