using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GunNGoneBetter_Models;

namespace GunNGoneBetter_DataMigrations.Repository.IRepository
{
    public interface IRepositoryMyModel : IRepository<MyModel>
    {
        void Update(MyModel obj);
    }
}
