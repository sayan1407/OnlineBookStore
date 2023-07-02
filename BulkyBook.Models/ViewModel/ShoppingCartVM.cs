using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public double Price { get; set; }
    }
}
