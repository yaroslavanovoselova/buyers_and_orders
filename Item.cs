using System;
using System.Collections.Generic;
using System.Text;

namespace BuyersAndOrders
{
    public class Item
    {
        /// <summary>
        /// Название товара.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Артикул товара.
        /// </summary>
        public string ArticleNumber { get; set; }

        /// <summary>
        /// Оставшееся количество товара.
        /// </summary>
        public int? AmountLeft { get; set; }

        /// <summary>
        /// Стомость товара.
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Конструктор товара.
        /// </summary>
        /// <param name="name"> Название. </param>
        /// <param name="articleNumber"> Артикул. </param>
        /// <param name="amount"> Оставшееся количество. </param>
        /// <param name="price"> Стоимость. </param>
        public Item(string name, string articleNumber, int? amount, double? price)
        {
            Name = name;
            ArticleNumber = articleNumber;
            AmountLeft = amount;
            Price = price;
        }
    }
}
