using System;
using System.Collections.Generic;
using System.Linq;
using ShoppingCart.Common;

namespace ShoppingCart.PricingService
{
    public class BranchAndBoundPriceCalculator : IPriceCalculator
    {
        private readonly IShop _shop;
        private Receipt _receipt;
        private List<Discount> _currentDiscounts;
        private decimal _currentBest;

        public BranchAndBoundPriceCalculator(IShop shop)
        {
            _shop = shop;
            _currentDiscounts = new List<Discount>();
        }

        private decimal GetTotal(Basket basket)
        {
            return BranchAndBound(basket.GetItems(), new List<Discount>(), new List<Discount>());
        }

        public Receipt GetReceipt(Basket basket)
        {
            _receipt = new Receipt
            {
                Items = GetBasketItems(basket),
                Total = GetTotal(basket),
                Discounts = _currentDiscounts
            };
            return _receipt;
        }

        private static List<Item> GetBasketItems(Basket basket)
        {
            return basket.GetItems().Keys.ToList();
        }

        private decimal BranchAndBound(IDictionary<Item, int> basketItems, List<Discount> appliedDiscounts,  List<Discount> removedDiscounts)
        {
            var discounts = GetAvailableDiscountList(basketItems);
            discounts.RemoveAll(removedDiscounts.Contains);
            
            if (!discounts.Any())
            {
                var pathTotal = GetPathTotal(basketItems, appliedDiscounts);
                if (_currentBest == 0m || pathTotal < _currentBest)
                {
                    _currentBest = pathTotal;
                    _currentDiscounts = appliedDiscounts;
                }
                return pathTotal;
            }

            if (_currentBest > 0m && appliedDiscounts.Sum(d => d.DiscountPrice) > _currentBest)
            {
                return GetPathTotal(basketItems, appliedDiscounts);
            }

            var topDiscount = discounts.First();
            
            var itemsWithDiscountApplied = ApplyDiscount(CopyDictionary(basketItems), topDiscount);

            var priceWithDiscount = BranchAndBound(itemsWithDiscountApplied, appliedDiscounts.Append(topDiscount).ToList(), removedDiscounts);
            var priceWithoutDiscount = BranchAndBound(basketItems, appliedDiscounts, removedDiscounts.Append(topDiscount).ToList());

            return priceWithDiscount <= priceWithoutDiscount ? priceWithDiscount : priceWithoutDiscount;
        }

        private static decimal GetPathTotal(IDictionary<Item, int> basketItems, List<Discount> appliedDiscounts)
        {
            return appliedDiscounts.Sum(d => d.DiscountPrice) + ApplyUnitPrice(basketItems);
        }

        private static Dictionary<Item, int> ApplyDiscount(Dictionary<Item, int> items, Discount discount)
        {
            foreach (var requisite in discount.Items)
            {
                items[requisite.Key] -= requisite.Value;
                if (items[requisite.Key] == 0)
                {
                    items.Remove(requisite.Key);
                }
            }

            return items;
        }

        private static bool IsDiscountApplicable(Discount discount, IDictionary<Item, int> basketItems)
        {
            return discount.Items.All(d => IsApplicable(d, basketItems));
        }

        private List<Discount> GetAvailableDiscountList(IDictionary<Item, int> basketItems)
        {
            return _shop.Discounts.Where(d => IsDiscountApplicable(d, basketItems)).ToList();
        }

        private static bool IsApplicable(KeyValuePair<Item, int> requisite, IDictionary<Item, int> basketItems)
        {
            return basketItems.ContainsKey(requisite.Key) && basketItems[requisite.Key] >= requisite.Value;
        }

        private static decimal ApplyUnitPrice(IDictionary<Item, int> items)
        {
            return items.Sum(i => GetItemPrice(i));
        }

        private static decimal GetItemPrice(KeyValuePair<Item, int> item)
        {
            return item.Key.Price * item.Value;
        }

        private static Dictionary<Item, int> CopyDictionary(IDictionary<Item, int> basketItems)
        {
            return basketItems.ToDictionary(basketItem => basketItem.Key, basketItem => basketItem.Value);
        }
    }
}