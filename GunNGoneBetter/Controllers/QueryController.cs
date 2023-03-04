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

namespace GunNGoneBetter.Controllers
{
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

        public IActionResult GetQueryList()
        {
            JsonResult result = Json(new { data = repositoryQueryHeader.GetAll() });

            return result;
        }
    }
}
