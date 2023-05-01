using BulkBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>,ICoverTypeRepository
    {
        private ApplicationDbContext _db;
        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CoverType coverType)
        {
            _db.CoverType.Update(coverType);
        }
    }
}
