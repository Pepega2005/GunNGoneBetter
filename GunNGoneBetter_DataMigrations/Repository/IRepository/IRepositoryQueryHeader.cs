using GunNGoneBetter_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunNGoneBetter_DataMigrations.Repository.IRepository
{
    public interface IRepositoryQueryHeader : IRepository<QueryHeader>
    {
        void Update(QueryHeader obj);
    }
}
