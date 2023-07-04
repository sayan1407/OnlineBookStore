using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;
using BulkyBook.DataAccess;
using System.Linq;

namespace BulkyBook.DataAccess.Repository
{
   public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(OrderHeader orderHeader, string orderStatus, string paymentStatus = null)
        {
            var orderHeadreInDb = _db.OrderHeaders.SingleOrDefault(o => o.Id == orderHeader.Id);
            if(orderHeadreInDb != null)
            {
                orderHeadreInDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                    orderHeadreInDb.PaymentStatus = paymentStatus;
            }
        }
        public void UpdatePaymentId(OrderHeader orderHeader,string PaymentIntentId, string SessionId)
        {
            var orderHeadreInDb = _db.OrderHeaders.SingleOrDefault(o => o.Id == orderHeader.Id);
            if(orderHeadreInDb != null)
            {
                orderHeadreInDb.PaymentIntentId = PaymentIntentId;
                orderHeadreInDb.SessionId = SessionId;
            }
        }
    }
}
