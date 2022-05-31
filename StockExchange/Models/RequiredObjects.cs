using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchange.Models
{
    public enum BuySell
    {
        Buy = 1,
        Sell = 2
    }

    public class OrderAddedEventArgs : EventArgs
    {
        public int OrderId { get; set; }
        public string StockCode { get; set; }
        public BuySell BuySell { get; set; }
        public int Volume { get; set; }
        public decimal Price { get; set; }
        public string UserReference { get; set; }

        public override bool Equals(object o)
        {
            var other = o as OrderAddedEventArgs;
            if (other == null) { return false; }
            return OrderId == other.OrderId
              && StockCode == other.StockCode
              && BuySell == other.BuySell
              && Volume == other.Volume
              && Price == other.Price
              && UserReference == other.UserReference;
        }
        public override int GetHashCode() { return OrderId; }
    }

    public class OrderRemovedEventArgs : EventArgs
    {
        public int OrderId { get; set; }
        public string UserReference { get; set; }
    }

    public class BestPriceChangedEventArgs : EventArgs
    {
        public string StockCode { get; set; }
        public int BestBuyVolume { get; set; }
        public decimal BestBuyPrice { get; set; }
        public int BestSellVolume { get; set; }
        public decimal BestSellPrice { get; set; }
    }

    public static class ExchangeErrorCodes
    {
        public const int NoError = 0;
        public const int InvalidStockCode = -1;
        public const int InvalidBuySell = -2;
        public const int InvalidVolume = -3;
        public const int InvalidPrice = -4;
        public const int UnknownOrder = -5;
    }

}