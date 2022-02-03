using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuyersAndOrders
{
    public class Order
    {
        /// <summary>
        /// Заказанный товары.
        /// </summary>
        public List<Item> OrderedItems { get; set; }

        /// <summary>
        /// Количестьво заказов сделанных клиентом.
        /// </summary>
        public static uint AmountOfOrders { get; set; } = 0;

        /// <summary>
        /// Уникальный номер заказа.
        /// </summary>
        public uint OrderNumber { get; set; }

        /// <summary>
        /// Дата и время заказа.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Статус заказа.
        /// </summary>
        public OrderStatus Status;

        /// <summary>
        /// Стоимость всего заказа.
        /// </summary>
        public double OrderPrice { get => OrderedItems.Select(item => (double)item.Price * (double)item.AmountLeft).Sum(); }
        
        /// <summary>
        /// Представление статуса в виде строки.
        /// </summary>
        public string StringStatus
        {
            get
            {
                var strStatus = "";
                if ((Status & OrderStatus.Processed) == OrderStatus.Processed)
                    strStatus += "Обработан -> ";
                if ((Status & OrderStatus.Paid) == OrderStatus.Paid)
                    strStatus += "Оплачен -> ";
                if ((Status & OrderStatus.Shipped) == OrderStatus.Shipped)
                    strStatus += "Отгружен -> ";
                if ((Status & OrderStatus.Executed) == OrderStatus.Executed)
                    strStatus += "Исполнен";
                char[] charsToTrim = { ' ', '-', '>' };
                return strStatus.Trim(charsToTrim);
            }
        }

        /// <summary>
        /// Заказчик.
        /// </summary>
        [JsonIgnore]
        public Client OrderingClient { get; set; }

        /// <summary>
        /// Логин заказчика.
        /// </summary>
        [JsonIgnore]
        public string OrderingClientLogin { get => OrderingClient.Login; }

        /// <summary>
        /// Конструктор заказа.
        /// </summary>
        /// <param name="currentClient"> Заказчик. </param>
        public Order(Client currentClient)
        {
            OrderingClient = currentClient;
            Status = OrderStatus.Default;
            OrderNumber = ++AmountOfOrders;
            OrderedItems = new List<Item>();
        }
    }
}
