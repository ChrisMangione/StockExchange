using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockExchange.Models;
using StockExchange.Helpers;

namespace StockExchange
{
    public class ExchangeService : OrderBook, IExchange, ITradeExecutor
    {
        public event EventHandler<OrderAddedEventArgs> OrderAdded;
        public event EventHandler<OrderRemovedEventArgs> OrderRemoved;
        public event EventHandler<BestPriceChangedEventArgs> BestPriceChanged;

        // Event triggered when cross
        public event EventHandler<TradeExecutedEventArgs> TradeExecuted;

        // Possible to add more Stock Codes
        public ExchangeService(string[] stockCodes) : base(stockCodes)
        {
        }

        public ExchangeService() : this (new string[] { "AAPL", "MSFT", "GOOG" })
        {
        } 

        public int AddOrder(string stockCode, BuySell buySell, int volume, decimal price, string userReference)
        {
            int orderId = OrderMap.Any() ? OrderMap.Last().OrderId + 1 : 1;
            var orderItem = new OrderItem(orderId, stockCode, buySell, price, volume, userReference);

            var errorCode = ExchangeValidator.ValidateOrderParams(orderItem, base._allowedStockCodes);
            if (errorCode != ExchangeErrorCodes.NoError)
                return errorCode;

            base.AddToOrderBook(orderItem);

            OnOrderAdded(orderItem);

            return ExchangeErrorCodes.NoError;
        }

        public int RemoveOrder(int orderId)
        {
            var hasItem = OrderMap.TryGetValue(orderId, out OrderItem orderItem);
            if (!hasItem)
                return ExchangeErrorCodes.UnknownOrder;

            base.RemoveFromOrderBook(orderItem);

            OnOrderRemoved(orderId, orderItem.UserReference);

            return ExchangeErrorCodes.NoError;
        }

        private void OnOrderAdded(OrderItem orderItem)
        {
            var args = new OrderAddedEventArgs()
            {
                OrderId = orderItem.OrderId,
                Volume = orderItem.Volume,
                BuySell = orderItem.BuySell,
                Price = orderItem.Price,
                StockCode = orderItem.StockCode,
                UserReference = orderItem.UserReference
            };
            OrderAdded?.Invoke(this, args);
        }

        private void OnOrderRemoved(int orderId, string userReference)
        {
            var args = new OrderRemovedEventArgs()
            {
                OrderId = orderId,
                UserReference = userReference
            };
            OrderRemoved?.Invoke(this, args);
        }

        protected override void OnLevelChanged(BestPriceChangedEventArgs args)
        {
            BestPriceChanged?.Invoke(this, args);
        }

        protected override void OnTradeExecuted(TradeExecutedEventArgs args)
        {
            TradeExecuted?.Invoke(this, args);
        }

    }
}
