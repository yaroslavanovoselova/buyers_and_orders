using System;
using System.Collections.Generic;
using System.Text;

namespace BuyersAndOrders
{
    /// <summary>
    /// Перечисление статусов заказа.
    /// </summary>
    [Flags]
    public enum OrderStatus
    {
        Default = 0,
        Processed = 1,
        Paid = 2,
        Shipped = 4,
        Executed = 8
    }
}
