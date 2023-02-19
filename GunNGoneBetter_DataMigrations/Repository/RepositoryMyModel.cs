using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GunNGoneBetter_DataMigrations.Data;
using GunNGoneBetter_DataMigrations.Repository.IRepository;
using GunNGoneBetter_Models;

namespace GunNGoneBetter_DataMigrations.Repository
{
    public class RepositoryMyModel : Repository<MyModel>, IRepositoryMyModel
    {
        public RepositoryMyModel(ApplicationDbContext db) : base(db) { }

        public void Update(MyModel obj)
        {
            var objFromDb = db.MyModel.FirstOrDefault(x => x.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Number = obj.Number;
            }
        }
    }
}
