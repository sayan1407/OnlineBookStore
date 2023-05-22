using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public IEnumerable<T> GetAll(string? IncludeProperties = null)
        {
            IQueryable<T> query = dbSet;
            
            if (IncludeProperties != null)
            {
                string[] Properties = IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string prop in Properties)
                {
                   query =  query.Include(prop);
                }
            }
            return query.ToList();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? IncludeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (IncludeProperties != null)
            {
                foreach (string prop in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query.Include(prop);
                }
            }
            return query.FirstOrDefault();
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

    }
}
