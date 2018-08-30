using System.Collections.Generic;

namespace ShoppingCart.Common
{
    public interface IPriceCalculator
    {
        Receipt GetReceipt(Basket basket);
    }
}