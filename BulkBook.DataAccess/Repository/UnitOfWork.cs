using BulkBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(db);

        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
