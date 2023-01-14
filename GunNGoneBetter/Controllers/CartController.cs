using GunNGoneBetter.Data;
using GunNGoneBetter.Models;
using GunNGoneBetter.Utility;
using Microsoft.AspNetCore.Mvc;

namespace GunNGoneBetter.Controllers
{
    public class CartController : Controller
    {
        ApplicationDbContext db;

        public CartController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);

                // хотим получить каждый товар из корзины
            }

            // получаем лист id товаров
            List<int> productsIdInCart = cartList.Select(x => x.ProductId).ToList();

            // извлекаем сами продукты по списку id
            IEnumerable<Product> productList = db.Product.Where(x => productsIdInCart.Contains(x.Id));

            return View(productList);
        }

        public IActionResult Remove(int id)
        {
            // удаление из корзины

            return RedirectToAction("Index");
        }
    }
}
