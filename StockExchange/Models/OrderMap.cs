using System.Collections.ObjectModel;

namespace StockExchange.Models
{
    public class OrderMap : KeyedCollection<int, OrderItem>
    {
        protected override int GetKeyForItem(OrderItem item)
        {
            return item.OrderId;
        }
    }

    public class OrderItem
    {
        public OrderItem(int orderId, string stockCode, BuySell buySell, decimal price, int volume, string userReference)
        {
            OrderId = orderId;
            StockCode = stockCode;
            BuySell = buySell;
            Price = price;
            Volume = volume;
            UserReference = userReference;
        }
        public int OrderId { get; set; }
        public string StockCode { get; set; }
        public BuySell BuySell { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
        public string UserReference { get; set; }
    }
}