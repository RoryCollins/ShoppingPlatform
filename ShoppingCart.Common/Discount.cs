using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Common
{
    public class Discount
    {
        public string Name { get; set; }
        public Dictionary<Item, int> Items { get; set; }
        public decimal DiscountPrice { get; set; }
        
    }
}
