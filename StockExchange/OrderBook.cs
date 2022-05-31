using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockExchange.Models;
using StockExchange.Helpers;

namespace StockExchange
{
    public abstract class OrderBook 
    {
        // Dictionaries by StockCode.
        // Using Sorted Dictionaries where the price is the key and the value is the level (with volume and orderId queue)
        private Dictionary<string, SortedDictionary<decimal, Level>> _bids { get; set; } = new Dictionary<string, SortedDictionary<decimal, Level>>();
        private Dictionary<string, SortedDictionary<decimal, Level>> _asks { get; set; } = new Dictionary<string, SortedDictionary<decimal, Level>>();

        protected OrderMap OrderMap { get; set; } = new OrderMap();

        protected string[] _allowedStockCodes { get; private set; }

        public OrderBook(string[] tickers)
        {
            _allowedStockCodes = tickers;
            foreach (var ticker in tickers)
            {
                _bids.Add(ticker, new SortedDictionary<decimal, Level>(new ReverseComparer<decimal>()));
                _asks.Add(ticker, new SortedDictionary<decimal, Level>());
            }
        }

        protected void AddToOrderBook(OrderItem orderItem)
        {
            OrderMap.Add(orderItem);

            bool isCross = false;
            if (orderItem.BuySell == BuySell.Sell)
                isCross = AddLevel(_asks, orderItem);
            else if (orderItem.BuySell == BuySell.Buy)
                isCross = AddLevel(_bids, orderItem);

            if (isCross)
                TradesCrossed(orderItem);
        }

        private bool AddLevel(Dictionary<string, SortedDictionary<decimal, Level>> level, OrderItem orderItem)
        {
            bool hasLevel = level[orderItem.StockCode].TryGetValue(orderItem.Price, out var value);
            if (hasLevel)
            {
                level[orderItem.StockCode][orderItem.Price].Volume += orderItem.Volume;
                var topLevel = level[orderItem.StockCode].First();
                topLevel.Value.OrderIdQueue.Add(orderItem.OrderId);
                if (topLevel.Key == orderItem.Price)
                    return BetterPrice(orderItem.StockCode);
                return false;
            }

            level[orderItem.StockCode].Add(orderItem.Price,
                new Level { Volume = orderItem.Volume, OrderIdQueue = new SortedSet<int>(new[] { orderItem.OrderId }) });

            var topLevelQueue = level[orderItem.StockCode].First().Value.OrderIdQueue.First();
            if (topLevelQueue == orderItem.OrderId)
                return BetterPrice(orderItem.StockCode);
            return false;
        }

        protected void RemoveFromOrderBook(OrderItem orderItem)
        {
            OrderMap.Remove(orderItem.OrderId);

            if (orderItem.BuySell == BuySell.Sell)
                RemoveLevel(_asks, orderItem);
            else if (orderItem.BuySell == BuySell.Buy)
                RemoveLevel(_bids, orderItem);
        }

        private void RemoveLevel(Dictionary<string, SortedDictionary<decimal, Level>> level, OrderItem orderItem)
        {
            bool hasLevel = level[orderItem.StockCode].TryGetValue(orderItem.Price, out var value);
            var topLevel = level[orderItem.StockCode].First();
            if (hasLevel && value.Volume > orderItem.Volume)
            {
                level[orderItem.StockCode][orderItem.Price].Volume -= orderItem.Volume;
                level[orderItem.StockCode][orderItem.Price].OrderIdQueue.Remove(orderItem.OrderId);
                if (topLevel.Key == orderItem.Price)
                    BetterPrice(orderItem.StockCode);
                return;
            }

            level[orderItem.StockCode].Remove(orderItem.Price);
            if (topLevel.Key == orderItem.Price)
                BetterPrice(orderItem.StockCode);
        }

        private bool BetterPrice(string stockCode)
        {
            var topBid = _bids[stockCode].FirstOrDefault(new KeyValuePair<decimal, Level>(0, new Level { Volume = 0 }));
            var topAsk = _asks[stockCode].FirstOrDefault(new KeyValuePair<decimal, Level>(0, new Level { Volume = 0 }));

            var args = new BestPriceChangedEventArgs()
            {
                StockCode = stockCode,
                BestBuyPrice = topBid.Key,
                BestBuyVolume = topBid.Value.Volume,
                BestSellPrice = topAsk.Key,
                BestSellVolume = topAsk.Value.Volume,
            };
            OnLevelChanged(args);
            return topBid.Key >= topAsk.Key;
        }

        protected abstract void OnLevelChanged(BestPriceChangedEventArgs args);

        protected abstract void OnTradeExecuted(TradeExecutedEventArgs args);

        /// <summary>
        /// Have the best bid and asks now crossed?
        /// Trigger trade by adding and removing from orderbook and level
        /// Would have to think about priority also, lower order ids at topBid and TopAsk level have priority
        /// </summary>
        private void TradesCrossed(OrderItem orderItem)
        {
            Dictionary<string, SortedDictionary<decimal, Level>> orderSide = orderItem.BuySell == BuySell.Sell ? _asks : _bids;
            Dictionary<string, SortedDictionary<decimal, Level>> opposingSide = orderItem.BuySell == BuySell.Sell ? _asks : _bids;

            var tradinglevel = opposingSide[orderItem.StockCode][orderItem.Price];

            // Partial Fill of current order item
            if (orderItem.Volume >= tradinglevel.Volume)
            {
                // Remove from opposingSide
                // Adjust volume
                // Amend levels
            }
            // Partial Fill of opposing order
            else if (orderItem.Volume >= tradinglevel.Volume)
            {
                // Remove from orderside
                // Adjust volume
                // Amend levels
            }
            // Exact Fill
            else
            {
                // Remove both
                // Amend levels
            }

            // Two triggers for both sides that were executed
            // Empty because not implemented
            OnTradeExecuted(new TradeExecutedEventArgs());
            OnTradeExecuted(new TradeExecutedEventArgs());
        }
    }
}
