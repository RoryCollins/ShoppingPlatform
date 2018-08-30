using System.Collections.Generic;

namespace ShoppingCart.Common
{
    public interface IShop
    {
        IEnumerable<Discount> Discounts { get; }
        void AddDiscount(Discount discount);
        void AddItem(ShopItem item);
        int GetStock(Item item);
        void RemoveStock(Item item);
    }
}