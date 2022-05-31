using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchange.Models
{
    public class TradeExecutedEventArgs
    {
        public TradeStatus Status { get; set; }
        public int OrderId { get; set; }

        // Have to include volume for case of partial fills
        public int Volume { get; set; }
        public decimal Price { get; set; }
    }

    public enum TradeStatus
    {
        Filled = 1,
        PartialFill = 2,
        Pulled = 3,
    }
}
