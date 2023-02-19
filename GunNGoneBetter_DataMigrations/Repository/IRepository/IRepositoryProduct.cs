using GunNGoneBetter_DataMigrations.Data;
using GunNGoneBetter_Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunNGoneBetter_DataMigrations.Repository.IRepository
{
    public interface IRepositoryProduct : IRepository<Product>
    {
        void Update(Product obj);

        IEnumerable<SelectListItem> GetListItems(string obj);
    }
}
