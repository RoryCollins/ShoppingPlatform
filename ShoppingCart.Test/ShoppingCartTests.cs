using System.Collections.Generic;
using ShoppingCart.Common;
using ShoppingCart.PricingService;
using ShoppingCart.ShopService;
using Xunit;

namespace ShoppingCart.Test
{
    public class ShoppingCartTests
    {
        private readonly Basket _basket;
        private readonly Shop _shop;
        private readonly IPriceCalculator _priceCalculator;

        public ShoppingCartTests()
        {
            _shop = Shop.GetShop();
            _basket = new Basket(_shop);
            _priceCalculator = new BranchAndBoundPriceCalculator(_shop);
        }

        [Fact]
        public void AddSingleItemToShoppingCart()
        {
            var apple = new Item{Name = "Apple", Price = 0.5m};
            _shop.AddItem(new ShopItem { Item = apple, Quantity = 5 });
            _basket.Add(apple);
            Assert.Equal(4, _shop.GetStock(apple));
            Assert.Equal(1, _basket.GetItems()[apple]);
        }
        [Fact]
        public void CannotAddMoreThanShelfStock()
        {
            var apple = new Item { Name = "Apple", Price = 0.5m };
            _shop.AddItem(new ShopItem { Item = apple, Quantity = 2 });
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            Assert.Equal(0, _shop.GetStock(apple));
            Assert.Equal(2, _basket.GetItems()[apple]);
        }

        [Fact]
        public void GetPriceWithoutDiscount()
        {
            var apple = new Item { Name = "Apple", Price = 0.5m };
            _shop.AddItem(new ShopItem{Item = apple, Quantity = 5});
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);

            var receipt = _priceCalculator.GetReceipt(_basket);
            Assert.Equal(1.5m, receipt.Total);
        }

        [Fact]
        public void GetPriceWithDiscount()
        {
            var apple = new Item { Name = "Apple", Price = 0.5m };
            _shop.AddItem(new ShopItem { Item = apple, Quantity = 5 });
            _shop.AddDiscount(new Discount
            {
                Items = new Dictionary<Item, int> { { apple, 3} },
                Name = "Three for two on all apples",
                DiscountPrice = 1m
            });
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            var receipt = _priceCalculator.GetReceipt(_basket);
            Assert.Equal(1m, receipt.Total);
        }

        [Fact]
        public void GetPriceWithRepeatedDiscounts()
        {
            var apple = new Item { Name = "Apple", Price = 0.5m };
            _shop.AddItem(new ShopItem { Item = apple, Quantity = 5 });
            _shop.AddDiscount(new Discount
            {
                Items = new Dictionary<Item, int> { { apple, 2 } },
                Name = "Two for one on all apples",
                DiscountPrice = 0.5m
            });
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            var receipt = _priceCalculator.GetReceipt(_basket);
            Assert.Equal(1m, receipt.Total);
        }

        [Fact]
        public void GetPriceFindsCheapestDiscountType()
        {
            var apple = new Item { Name = "Apple", Price = 0.5m };
            _shop.AddItem(new ShopItem { Item = apple, Quantity = 5 });
            _shop.AddDiscount(new Discount
            {
                Items = new Dictionary<Item, int> {{apple, 2}},
                Name = "Two for one on all apples",
                DiscountPrice = 0.5m
            });
            _shop.AddDiscount(new Discount
            {
                Items = new Dictionary<Item, int> {{apple, 3}},
                Name = "Three for half of one on all apples",
                DiscountPrice = 0.25m
            });
            _shop.AddDiscount(new Discount
            {
                Items = new Dictionary<Item, int> {{apple, 4}},
                Name = "Four for two on all apples",
                DiscountPrice = 1m
            });
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            var receipt = _priceCalculator.GetReceipt(_basket);
            Assert.Equal(0.75m, receipt.Total);
        }

        [Fact]
        public void GetPriceFindsCheapestDiscountCombination()
        {
            var apple = new Item { Name = "Apple", Price = 0.5m };
            _shop.AddItem(new ShopItem { Item = apple, Quantity = 7 });
            _shop.AddDiscount(new Discount
            {
                Items = new Dictionary<Item, int> { { apple, 2 } },
                Name = "Discount A",
                DiscountPrice = 0.5m
            });
            _shop.AddDiscount(new Discount
            {
                Items = new Dictionary<Item, int> { { apple, 3 } },
                Name = "Discount B",
                DiscountPrice = 0.75m
            });

            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            
            var receipt = _priceCalculator.GetReceipt(_basket);
            Assert.Equal(1.75m, receipt.Total);
        }


        [Fact]
        public void GetListOfDiscountsUsed()
        {
            var apple = new Item { Name = "Apple", Price = 0.5m };
            _shop.AddItem(new ShopItem { Item = apple, Quantity = 7 });
            var discountA = new Discount
            {
                Items = new Dictionary<Item, int> {{apple, 2}},
                Name = "Discount A",
                DiscountPrice = 0.5m
            };
            var discountB = new Discount
            {
                Items = new Dictionary<Item, int> {{apple, 3}},
                Name = "Discount B",
                DiscountPrice = 0.75m
            };
            _shop.AddDiscount(discountA);
            _shop.AddDiscount(discountB);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);
            _basket.Add(apple);

            var receipt = _priceCalculator.GetReceipt(_basket);
            Assert.Contains(discountA, receipt.Discounts);
            Assert.Contains(discountB, receipt.Discounts);
        }
    }
}
