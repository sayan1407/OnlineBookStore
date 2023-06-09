﻿using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncreaseProductCount(ShoppingCart shoppingCart, int count);
        int DecreaseProductCount(ShoppingCart shoppingCart, int count);

    }
}
