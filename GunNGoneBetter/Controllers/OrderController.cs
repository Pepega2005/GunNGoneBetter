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
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using GunNGoneBetter_DataMigrations.Repository;

namespace GunNGoneBetter.Controllers
{
    public class OrderController : Controller
    {
        IRepositoryOrderHeader repositoryOrderHeader;
        IRepositoryOrderDetail repositoryOrderDetail;
        IBrainTreeBridge brainTreeBridge;

        [BindProperty]
        public OrderHeaderDetailViewModel OrderViewModel { get; set; }

        public OrderController(IRepositoryOrderHeader repositoryOrderHeader,
            IRepositoryOrderDetail repositoryOrderDetail, IBrainTreeBridge brainTreeBridge)
        {
            this.repositoryOrderHeader = repositoryOrderHeader;
            this.repositoryOrderDetail = repositoryOrderDetail;
            this.brainTreeBridge = brainTreeBridge;
        }

        public IActionResult Index(string searchName = null, string searchEmail = null,
            string searchPhone = null, string status = null)
        {
            OrderViewModel viewModel = new OrderViewModel()
            {
                OrderHeaderList = repositoryOrderHeader.GetAll(),
                StatusList = PathManager.StatusList.ToList().
                Select(x => new SelectListItem { Text = x, Value = x }),
            };

            if (searchName != null)
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.
                    Where(x => x.FullName.ToLower().Contains(searchName.ToLower()));
            }

            if (searchEmail != null)
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.
                    Where(x => x.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (searchPhone != null)
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.
                    Where(x => x.Phone.Contains(searchPhone));
            }

            if (status != "Choose Status" && status != null)
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.
                    Where(x => x.Status.Contains(status));
            }

            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            OrderViewModel = new OrderHeaderDetailViewModel()
            {
                OrderHeader = repositoryOrderHeader.FirstOrDefault(x => x.Id == id),
                OrderDetail = repositoryOrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(OrderViewModel);
        }

        [HttpPost]
        public IActionResult StartInProcessing()
        {
            var model = OrderViewModel;

            OrderHeader orderHeader = repositoryOrderHeader.
                FirstOrDefault(x => x.Id == OrderViewModel.OrderHeader.Id);

            orderHeader.Status = PathManager.StatusInProcess;
            repositoryOrderHeader.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult StartOrderDone()
        {
            OrderHeader orderHeader = repositoryOrderHeader.
                FirstOrDefault(x => x.Id == OrderViewModel.OrderHeader.Id);

            orderHeader.Status = PathManager.StatusOrderDone;
            repositoryOrderHeader.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult StartOrderCancel()
        {
            OrderHeader orderHeader = repositoryOrderHeader.
                FirstOrDefault(x => x.Id == OrderViewModel.OrderHeader.Id);

            var gateway = brainTreeBridge.GetGateway();

            // get transaction
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            // условия при которых не возвращаем
            if (transaction.Status == TransactionStatus.AUTHORIZED ||
                transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                var res = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else // возврат средств
            {
                var res = gateway.Transaction.Refund(orderHeader.TransactionId);
            }

            orderHeader.Status = PathManager.StatusDenied;
            repositoryOrderHeader.Save();

            return RedirectToAction("Index");
        }
    }
}
