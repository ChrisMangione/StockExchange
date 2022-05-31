using StockExchange;
using StockExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchangeTests
{
    internal class ExchangeMockConsumer 
    {
        public List<OrderAddedEventArgs> OrderAddedEvents { get; set; } = new List<OrderAddedEventArgs>();
        public List<OrderRemovedEventArgs> OrderRemovedEvents { get; set; } = new List<OrderRemovedEventArgs>();
        public List<BestPriceChangedEventArgs> BestPriceChangedEvents { get; set; } = new List<BestPriceChangedEventArgs>();
        public List<TradeExecutedEventArgs> TradeExecutedEvents { get; set; } = new List<TradeExecutedEventArgs>();

        public ExchangeMockConsumer(ITradeExecutor exchange)
        {
            exchange.TradeExecuted += OnTradeExecutedReceived;
        }

        public ExchangeMockConsumer(IExchange exchange) : this(exchange as ITradeExecutor)
        {
            exchange.OrderAdded += OnAddedReceived;
            exchange.OrderRemoved += OnRemovedReceived;
            exchange.BestPriceChanged += OnChangedReceived;
        }

        private void OnChangedReceived(object? sender, BestPriceChangedEventArgs e)
        {
            BestPriceChangedEvents.Add(e);
        }

        private void OnRemovedReceived(object? sender, OrderRemovedEventArgs e)
        {
            OrderRemovedEvents.Add(e);
        }

        private void OnAddedReceived(object? sender, OrderAddedEventArgs e)
        {
            OrderAddedEvents.Add(e);
        }

        private void OnTradeExecutedReceived(object? sender, TradeExecutedEventArgs e)
        {
            TradeExecutedEvents.Add(e);
        }

    }
}
