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

namespace GunNGoneBetter.Controllers
{
    public class OrderController : Controller
    {
        IRepositoryOrderHeader repositoryOrderHeader;
        IRepositoryQueryDetail repositoryQueryDetail;
        IBrainTreeBridge brainTreeBridge;

        public OrderController(IRepositoryOrderHeader repositoryOrderHeader,
            IRepositoryQueryDetail repositoryQueryDetail, IBrainTreeBridge brainTreeBridge)
        {
            this.repositoryOrderHeader = repositoryOrderHeader;
            this.repositoryQueryDetail = repositoryQueryDetail;
            this.brainTreeBridge = brainTreeBridge;
        }

        public IActionResult Index()
        {
            OrderViewModel viewModel = new OrderViewModel()
            {
                OrderHeaderList = repositoryOrderHeader.GetAll(),
                StatusList = PathManager.StatusList.ToList().
                Select(x => new SelectListItem { Text = x, Value = x }),
            };


            return View(viewModel);
        }
    }
}
