using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Common
{
    public class Basket
    {
        private readonly Dictionary<Item, int> _items;
        private readonly IShop _shop;

        public Basket(IShop shop)
        {
            _shop = shop;
            _items = new Dictionary<Item, int>();
        }

        public IDictionary<Item, int> GetItems()
        {
            return _items;
        }

        public void Add(Item item)
        {
            if (_shop.GetStock(item) > 0)
            {
                if (!_items.ContainsKey(item))
                {
                    _items.Add(item, 0);
                }

                _items[item]++;
                _shop.RemoveStock(item);
            }
        }
    }
}
