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
    public class RepositoryOrderDetail : Repository<OrderDetail>, IRepositoryOrderDetail
    {
        public RepositoryOrderDetail(ApplicationDbContext db) : base(db) { }

        public void Update(OrderDetail obj)
        {
            db.OrderDetail.Update(obj);
        }
    }
}
