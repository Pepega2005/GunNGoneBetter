using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GunNGoneBetter_Models;

namespace GunNGoneBetter_DataMigrations.Repository.IRepository
{
    public interface IRepositoryOrderHeader : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
    }
}
