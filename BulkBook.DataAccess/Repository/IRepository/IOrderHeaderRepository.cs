using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(OrderHeader orderHeader, string orderStatus, string paymentStatus = null);
        void UpdatePaymentId(OrderHeader orderHeader, string PaymentIntentId, string SessionId);
    }
}
