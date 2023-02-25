using GunNGoneBetter_DataMigrations.Data;
using GunNGoneBetter_DataMigrations.Repository.IRepository;
using GunNGoneBetter_Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GunNGoneBetter_Utility;

namespace GunNGoneBetter_DataMigrations.Repository
{
    public class RepositoryProduct : Repository<Product>, IRepositoryProduct
    {
        public RepositoryProduct(ApplicationDbContext db) : base(db) { }

        public IEnumerable<SelectListItem> GetListItems(string obj)
        {
            if (obj == PathManager.NameCategory)
            {
                return db.Category.Select(x =>
                new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }

            if (obj == PathManager.NameMyModel)
            {
                return db.MyModel.Select(x =>
                new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }

            return null;
        }

        public void Update(Product obj)
        {
            db.Update(obj); // !!! CHECK !!!
        }
    }
}
