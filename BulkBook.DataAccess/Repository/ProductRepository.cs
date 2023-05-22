using System;
using System.Collections.Generic;
using System.Text;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess;
using System.Linq;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>,IProductRepository
    {
         public ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            var productInDb = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if(productInDb != null)
            {
                productInDb.Author = product.Author;
                productInDb.CategoryId= product.CategoryId;
                productInDb.CoverTypeId = product.CoverTypeId;
                productInDb.Description = product.Description;
                productInDb.ISBN = product.ISBN;
                productInDb.ListPrice = product.ListPrice;
                productInDb.Price = product.Price;
                productInDb.Price100 = product.Price100;
                productInDb.Price50 = product.Price50;
                productInDb.Title = product.Title;
                if (product.ImageUrl != null)
                    productInDb.ImageUrl = product.ImageUrl;
            }

        }
    }
}
