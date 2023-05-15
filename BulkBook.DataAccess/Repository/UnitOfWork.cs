using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get;private set; }
        public IProductRepository Product { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(db);
            CoverType = new CoverTypeRepository(db);
            Product = new ProductRepository(db);

        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
