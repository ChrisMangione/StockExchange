using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockExchange.Models;

namespace StockExchange
{
    public interface ITradeExecutor
    {
        public event EventHandler<TradeExecutedEventArgs> TradeExecuted;
    }
}
