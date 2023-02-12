using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GunNGoneBetter_DataMigrations.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        T Find(int id);

        IEnumerable<T> GeyAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
        );

        void Add(T item);

        void Remove(T item);

        void Update(T item); // change!!!

        void Save();
    }
}
