using NUnit.Framework;
using StockExchange;
using StockExchange.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockExchangeTests
{
    [TestFixture]
    public class AddOrderTests
    {

        [Test]
        public void AddOrder_AllSameVolumeAndPrice_CorrectEventCount()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);

            service.AddOrder("GOOG", BuySell.Buy, 100, 10, "test");
            service.AddOrder("GOOG", BuySell.Buy, 100, 10, "test");
            service.AddOrder("GOOG", BuySell.Buy, 100, 10, "test");

            Assert.Multiple(() =>
            {
                Assert.That(consumer.OrderAddedEvents.Count, Is.EqualTo(3));
                Assert.That(consumer.BestPriceChangedEvents.Count, Is.EqualTo(3));
            });
        }

        [Test]
        public void AddOrders_BestVolumeChangedTriggered_CorrectEventCount()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);

            service.AddOrder("GOOG", BuySell.Buy, 300, 10, "test");
            service.AddOrder("GOOG", BuySell.Buy, 200, 10, "test");
            service.AddOrder("GOOG", BuySell.Buy, 100, 10, "test");
            service.AddOrder("GOOG", BuySell.Buy, 200, 10, "test");

            Assert.Multiple(() =>
            {
                Assert.That(consumer.BestPriceChangedEvents.Count, Is.EqualTo(4));
                Assert.That(consumer.BestPriceChangedEvents.Max(o => o.BestBuyPrice), Is.EqualTo(10));
                Assert.That(consumer.BestPriceChangedEvents.Sum(o => o.BestBuyVolume), Is.EqualTo(2200));
                Assert.That(consumer.BestPriceChangedEvents.Sum(o => o.BestSellVolume), Is.EqualTo(0));
                Assert.That(consumer.BestPriceChangedEvents.Sum(o => o.BestSellPrice), Is.EqualTo(0));
            });
        }

        [Test]
        public void AddOrders_BestPriceChangedTriggered_CorrectEventCount()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);

            service.AddOrder("GOOG", BuySell.Buy, 100, 10, "test");
            service.AddOrder("GOOG", BuySell.Buy, 100, 9, "test");
            service.AddOrder("GOOG", BuySell.Buy, 100, 8, "test");
            service.AddOrder("GOOG", BuySell.Buy, 100, 9, "test");

            Assert.Multiple(() =>
            {
                Assert.That(consumer.BestPriceChangedEvents.Count, Is.EqualTo(1));
                Assert.That(consumer.BestPriceChangedEvents.Sum(o => o.BestBuyPrice), Is.EqualTo(10));
                Assert.That(consumer.BestPriceChangedEvents.Sum(o => o.BestBuyVolume), Is.EqualTo(100));
                Assert.That(consumer.BestPriceChangedEvents.Sum(o => o.BestSellPrice), Is.EqualTo(0));
                Assert.That(consumer.BestPriceChangedEvents.Sum(o => o.BestSellVolume), Is.EqualTo(0));
            });
        }


        [Test]
        public void AddOrders_MultipleLevels_ExpectedEventArgs()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);
            var allItems = new List<OrderAddedEventArgs>()
            {
                new OrderAddedEventArgs { OrderId = 1, StockCode = "GOOG", BuySell = BuySell.Sell, Price = 10, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 2, StockCode = "GOOG", BuySell = BuySell.Sell, Price = 11, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 3, StockCode = "GOOG", BuySell = BuySell.Sell, Price = 12, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 4, StockCode = "GOOG", BuySell = BuySell.Buy, Price = 9, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 5, StockCode = "GOOG", BuySell = BuySell.Buy, Price = 8, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 6, StockCode = "GOOG", BuySell = BuySell.Buy, Price = 7, Volume = 100, UserReference = "test" },
            };

            foreach (var item in allItems)
                service.AddOrder(item.StockCode, item.BuySell, item.Volume, item.Price, item.UserReference);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEquivalent(consumer.OrderAddedEvents, allItems);
                Assert.That(consumer.BestPriceChangedEvents.Count(), Is.EqualTo(2));
                Assert.That(consumer.OrderAddedEvents.Count(), Is.EqualTo(6));
                Assert.That(consumer.OrderRemovedEvents.Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public void AddOrders_MultipleTickers_ExpectedEventArgs()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);
            var allItems = new List<OrderAddedEventArgs>()
            {
                new OrderAddedEventArgs { OrderId = 1, StockCode = "GOOG", BuySell = BuySell.Sell, Price = 10, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 2, StockCode = "GOOG", BuySell = BuySell.Sell, Price = 11, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 3, StockCode = "AAPL", BuySell = BuySell.Sell, Price = 12, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 4, StockCode = "AAPL", BuySell = BuySell.Buy, Price = 9, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 5, StockCode = "GOOG", BuySell = BuySell.Buy, Price = 8, Volume = 100, UserReference = "test" },
                new OrderAddedEventArgs { OrderId = 6, StockCode = "GOOG", BuySell = BuySell.Buy, Price = 7, Volume = 100, UserReference = "test" },
            };

            foreach (var item in allItems)
                service.AddOrder(item.StockCode, item.BuySell, item.Volume, item.Price, item.UserReference);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEquivalent(consumer.OrderAddedEvents, allItems);
                Assert.That(consumer.BestPriceChangedEvents.Count(), Is.EqualTo(4));
                Assert.That(consumer.OrderAddedEvents.Count(), Is.EqualTo(6));
                Assert.That(consumer.OrderRemovedEvents.Count(), Is.EqualTo(0));
            });
        }
    }
}