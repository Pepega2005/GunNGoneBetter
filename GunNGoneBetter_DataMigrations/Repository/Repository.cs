﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GunNGoneBetter_DataMigrations.Data;
using GunNGoneBetter_DataMigrations.Repository.IRepository;
using Microsoft.EntityFrameworkCore; // dbSet

namespace GunNGoneBetter_DataMigrations.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext db;

        private DbSet<T> dbSet; // ???
        
        // пользуемся внедрением зависимостей
        public Repository(ApplicationDbContext db)
        {
            this.db = db;
            dbSet = this.db.Set<T>();
        }

        public void Add(T item)
        {
            dbSet.Add(item);
        }

        public T Find(int id)
        {
            return dbSet.Find(id);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter = null,
            string includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> quiry = dbSet;

            // filter
            if (filter != null)
            {
                quiry = quiry.Where(filter);
            }

            // properties
            if (includeProperties != null)
            {
                // LINQ Include
                foreach (var item in includeProperties.Split(','))
                {
                    quiry = quiry.Include(item);
                }
            }

            // isTracking
            if (!isTracking)
            {
                quiry = quiry.AsNoTracking();
            }

            return quiry.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> quiry = dbSet;

            // filter
            if (filter != null)
            {
                quiry = quiry.Where(filter);
            }

            // properties
            if (includeProperties != null)
            {
                // LINQ Include
                foreach (var item in includeProperties.Split(','))
                {
                    quiry = quiry.Include(item);
                }
            }

            // orderBy
            if (orderBy != null)
            {
                quiry = orderBy(quiry);
            }

            // isTracking
            if (!isTracking)
            {
                quiry = quiry.AsNoTracking();
            }

            return quiry.ToList(); // !!!
        }

        public void Remove(T item)
        {
            dbSet.Remove(item);
        }

        public void Remove(IEnumerable<T> items)
        {
            dbSet.RemoveRange(items);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
