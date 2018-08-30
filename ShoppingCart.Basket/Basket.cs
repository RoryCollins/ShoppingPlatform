using System;
using System.Collections.Generic;
using ShoppingCart.Common;

namespace ShoppingCart.Basket
{
    public class Basket
    {
        public IEnumerable<BasketItem> BasketItems { get; }

        public Basket()
        {
            BasketItems = new List<BasketItem>();    
        }
    }
}
