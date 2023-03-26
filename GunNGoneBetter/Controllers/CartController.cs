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
using GunNGoneBetter_Utility.BrainTree;
using Braintree;

namespace GunNGoneBetter.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        //ApplicationDbContext db;

        ProductUserViewModel productUserViewModel;

        IWebHostEnvironment webHostEnvironment;

        IEmailSender emailSender;

        IRepositoryProduct repositoryProduct;
        IRepositoryApplicationUser repositoryApplicationUser;

        IRepositoryQueryHeader repositoryQueryHeader;
        IRepositoryQueryDetail repositoryQueryDetail;

        IRepositoryOrderHeader repositoryOrderHeader;
        IRepositoryOrderDetail repositoryOrderDetail;

        IBrainTreeBridge brainTreeBridge;

        public CartController(IWebHostEnvironment webHostEnvironment,
            IEmailSender emailSender, IRepositoryProduct repositoryProduct,
            IRepositoryApplicationUser repositoryApplicationUser, IRepositoryQueryHeader repositoryQueryHeader,
            IRepositoryQueryDetail repositoryQueryDetail, IRepositoryOrderHeader repositoryOrderHeader, IRepositoryOrderDetail repositoryOrderDetail,
            IBrainTreeBridge brainTreeBridge)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.emailSender = emailSender;
            this.repositoryApplicationUser = repositoryApplicationUser;
            this.repositoryProduct = repositoryProduct;
            this.repositoryQueryHeader = repositoryQueryHeader;
            this.repositoryQueryDetail = repositoryQueryDetail;
            this.repositoryOrderHeader = repositoryOrderHeader;
            this.repositoryOrderDetail = repositoryOrderDetail;
            this.brainTreeBridge = brainTreeBridge;
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
            IEnumerable<Product> productListTemp =
                repositoryProduct.GetAll(x => productsIdInCart.Contains(x.Id));

            List<Product> productList = new List<Product>();

            foreach (var item in cartList)
            {
                Product product = productListTemp.FirstOrDefault(x => x.Id == item.ProductId);
                product.TempCount = item.Count;

                productList.Add(product);
            }

            return View(productList);
        }

        [HttpPost]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> products)
        {
            List<Cart> carts = new List<Cart>();

            foreach (var product in products)
            {
                carts.Add(new Cart() { ProductId = product.Id, Count = product.TempCount });
            }

            HttpContext.Session.Set(PathManager.SessionCart, carts);

            return RedirectToAction("Summary");
        }

        public IActionResult Remove(int id)
        {
            // удаление из корзины
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            cartList.Remove(cartList.FirstOrDefault(x => x.ProductId == id));

            HttpContext.Session.Set(PathManager.SessionCart, cartList);

            return RedirectToAction("Index");
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = repositoryOrderHeader.FirstOrDefault(
                x => x.Id == id);

            HttpContext.Session.Clear(); // полная очитка сессии

            return View(orderHeader);
        }

        [HttpPost]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, 
            ProductUserViewModel productUserViewModel)
        {
            // work with user
            var identityClaims = (ClaimsIdentity)User.Identity;
            var claim = identityClaims.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(PathManager.AdminRole))
            {
                // Work with ORDER
                double totalPrice = 0;

                foreach (var item in productUserViewModel.ProductList)
                {
                    totalPrice += item.TempCount + item.Price;
                }

                OrderHeader orderHeader = new OrderHeader()
                {
                    AdminId = claim.Value,
                    DateOrder = DateTime.Now,
                    TotalPrice = 0,
                    Status = PathManager.StatusPending,
                    FullName = productUserViewModel.ApplicationUser.FullName,
                    Email = productUserViewModel.ApplicationUser.Email,
                    Phone = productUserViewModel.ApplicationUser.PhoneNumber,
                    City = productUserViewModel.ApplicationUser.City,
                    Street = productUserViewModel.ApplicationUser.Street,
                    House = productUserViewModel.ApplicationUser.House,
                    Apartment = productUserViewModel.ApplicationUser.Apartment,
                    PostalCode = productUserViewModel.ApplicationUser.PostalCode,
                };

                repositoryOrderHeader.Add(orderHeader);
                repositoryOrderHeader.Save();

                foreach (var product in productUserViewModel.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        ProductId = product.Id,
                        Count = product.TempCount,
                        PricePerUnit = (int)product.Price // !!! add new migration (has to be double)
                    };

                    repositoryOrderDetail.Add(orderDetail);
                }

                repositoryOrderDetail.Save();


                string nonce = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = 1,
                    PaymentMethodNonce = nonce,
                    OrderId = "1",
                    Options = new TransactionOptionsRequest { SubmitForSettlement = true } // auto confirmations
                };

                var getway = brainTreeBridge.GetGateway();

                var resultTransaction = getway.Transaction.Sale(request);

                var id = resultTransaction.Target.Id;
                var status = resultTransaction.Target.ProcessorResponseText;


                return RedirectToAction("InquiryConfirmation", new { Id = orderHeader.Id });
            }
            else
            {
                // Work with QUERY

                // код для отправки сообщения
                var path = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() +
                        "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

                var subject = "New Query";

                string bodyHtml = "";

                using (StreamReader reader = new StreamReader(path))
                {
                    bodyHtml = reader.ReadToEnd();
                }

                string textProducts = "";
                foreach (var item in productUserViewModel.ProductList)
                {
                    textProducts += $"- Name: {item.Name}, ID: {item.Id}\n";
                }

                string body = string.Format(bodyHtml, productUserViewModel.ApplicationUser.FullName,
                    productUserViewModel.ApplicationUser.Email,
                    productUserViewModel.ApplicationUser.PhoneNumber,
                    textProducts
                    );

                await emailSender.SendEmailAsync(productUserViewModel.ApplicationUser.Email, subject, body);
                await emailSender.SendEmailAsync("elite.clone69@gmail.com", subject, body);

                // добавление данных в БД по заказу

                QueryHeader queryHeader = new QueryHeader()
                {
                    ApplicationUserId = claim.Value,
                    QueryTime = DateTime.Now,
                    FullName = productUserViewModel.ApplicationUser.FullName,
                    PhoneNumber = productUserViewModel.ApplicationUser.PhoneNumber,
                    Email = productUserViewModel.ApplicationUser.Email,
                    ApplicationUser = repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value)
                };

                // получение юзера
                //var claimsIdentity = (ClaimsIdentity)User.Identity;
                //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //claim.Value -> getId

                repositoryQueryHeader.Add(queryHeader);
                repositoryQueryHeader.Save();

                // сделать запись деталей - всех продуктов в БД
                foreach (var item in productUserViewModel.ProductList)
                {
                    QueryDetail queryDetail = new QueryDetail()
                    {
                        ProductId = item.Id,
                        QueryHeaderId = queryHeader.Id,
                        QueryHeader = queryHeader,
                        Product = repositoryProduct.Find(item.Id)
                    };

                    repositoryQueryDetail.Add(queryDetail);
                }
                repositoryQueryDetail.Save();
            }    

            return RedirectToAction("InquiryConfirmation");
        }

        [HttpPost]
        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(PathManager.AdminRole)) // работа админа в корзине
            {
                // корзина заполняется на основании существующего запроса
                if (HttpContext.Session.Get<int>(PathManager.SessionQuery) != 0)
                {
                    // можем забрать данные из id запроса для юзера

                    QueryHeader queryHeader = repositoryQueryHeader.FirstOrDefault(
                        x => x.Id == HttpContext.Session.Get<int>(PathManager.SessionQuery));

                    applicationUser = new ApplicationUser()
                    {
                        Email = queryHeader.Email,
                        PhoneNumber = queryHeader.PhoneNumber,
                        FullName = queryHeader.FullName
                    };
                }
                else // корзина заполняется админом для юзера 
                {
                    applicationUser = new ApplicationUser();
                }

                // РАБОТА С ОПЛАТОЙ
                var gateway = brainTreeBridge.GetGateway();
                var tokenClient = gateway.ClientToken.Generate();
                ViewBag.TokenClient = tokenClient;
            }
            else // работа юзера с корзиной 
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;

                // если ипользователь вошел в систему, то обьект будет определен
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                applicationUser = repositoryApplicationUser.FirstOrDefault(
                    x => x.Id == claim.Value);
            }

            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            List<int> productsIdInCart = cartList.Select(x => x.ProductId).ToList();

            IEnumerable<Product> productList = repositoryProduct.GetAll(x => productsIdInCart.Contains(x.Id));

            productUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = applicationUser
            };

            foreach (var item in cartList)
            {
                Product product = repositoryProduct.FirstOrDefault(
                    x => x.Id == item.ProductId);
                product.TempCount = item.Count;

                productUserViewModel.ProductList.Add(product);
            }

            return View(productUserViewModel);
        }

        [HttpPost]
        public IActionResult Update(List<Product> products)
        {
            List<Cart> cartList = new List<Cart>();

            foreach (var product in products)
            {
                cartList.Add(new Cart()
                {
                    ProductId = product.Id,
                    Count = product.TempCount
                });

                HttpContext.Session.Set(PathManager.SessionCart, cartList);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}