using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ShoppingCart.Common
{
    public class Receipt
    {
        public List<Item> Items { get; set; }
        public List<Discount> Discounts { get; set; }
        public decimal Total { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----------------");
            sb.AppendLine("----- Items -----");
            Items.ForEach(i => sb.AppendLine($"{i.Name}: {i.Price}"));
            sb.AppendLine("--- Discounts ---");
            Discounts.ForEach(d => sb.AppendLine(d.Name));
            sb.AppendLine("----- Total -----");
            sb.AppendLine(Total.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine("-----------------");
            return sb.ToString();
        }
    }
}
