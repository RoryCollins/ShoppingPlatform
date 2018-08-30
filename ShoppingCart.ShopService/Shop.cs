using System.Collections.Generic;
using System.Linq;
using ShoppingCart.Common;

namespace ShoppingCart.ShopService
{
    public class Shop : IShop
    {
        private static Shop TheShop;
        private readonly List<ShopItem> Items;
        public IEnumerable<Discount> Discounts { get; private set; }

        private Shop()
        {
            Discounts = new List<Discount>();
            Items = new List<ShopItem>();
        }

        public void AddItem(ShopItem item)
        {
            Items.Add(item);
        }




        public static Shop GetShop()
        {
            return TheShop ?? (TheShop = new Shop());
        }

        public int GetStock(Item item)
        {
            return Items.First(i => i.Item == item).Quantity;
        }

        public void RemoveStock(Item item)
        {
            Items.First(i => i.Item == item).Quantity--;
        }

        public void AddDiscount(Discount discount)
        {
            Discounts = Discounts.Append(discount);
        }
    }
}