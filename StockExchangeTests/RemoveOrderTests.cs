using NUnit.Framework;
using StockExchange;
using StockExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchangeTests
{
    [TestFixture]
    internal class RemoveOrderTests
    {
        [Test]
        public void RemoveOrder_NonExistingOrder_CorrectEventAndErrorCode()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);

            var result1 = service.RemoveOrder(1);
            var result2 = service.RemoveOrder(2);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(ExchangeErrorCodes.UnknownOrder, result1);
                Assert.AreEqual(ExchangeErrorCodes.UnknownOrder, result2);
                Assert.IsEmpty(consumer.BestPriceChangedEvents);
                Assert.IsEmpty(consumer.OrderAddedEvents);
                Assert.IsEmpty(consumer.OrderRemovedEvents);
            });
        }

        [Test]
        public void RemoveOrder_ExistingOrder_CorrectEvent()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);
            var orderItem = new OrderItem(1, "GOOG", BuySell.Buy, 10, 100, "test");

            service.AddOrder(orderItem.StockCode, orderItem.BuySell, orderItem.Volume, orderItem.Price, orderItem.UserReference);
            var result = service.RemoveOrder(consumer.OrderAddedEvents[0].OrderId);

            Assert.Multiple(() =>
            {
                Assert.That(consumer.BestPriceChangedEvents.Count(), Is.EqualTo(2));
                Assert.That(consumer.OrderAddedEvents.Count(), Is.EqualTo(1));
                Assert.That(consumer.OrderRemovedEvents.Count(), Is.EqualTo(1));
            });
        }

        [Test]
        public void RemoveOrder_LevelHasMultipleOrders_CorrectEvent()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);
            var orderItem1 = new OrderItem(1, "GOOG", BuySell.Buy, 10, 100, "test");
            var orderItem2 = new OrderItem(2, "GOOG", BuySell.Buy, 10, 100, "test");
            var orderItem3 = new OrderItem(3, "GOOG", BuySell.Buy, 10, 100, "test");

            service.AddOrder(orderItem1.StockCode, orderItem1.BuySell, orderItem1.Volume, orderItem1.Price, orderItem1.UserReference);
            service.AddOrder(orderItem2.StockCode, orderItem2.BuySell, orderItem2.Volume, orderItem2.Price, orderItem2.UserReference);
            service.AddOrder(orderItem3.StockCode, orderItem3.BuySell, orderItem3.Volume, orderItem3.Price, orderItem3.UserReference);
            var result = service.RemoveOrder(consumer.OrderAddedEvents[0].OrderId);

            Assert.Multiple(() =>
            {
                Assert.That(consumer.BestPriceChangedEvents.Count(), Is.EqualTo(4));
                Assert.That(consumer.OrderAddedEvents.Count(), Is.EqualTo(3));
                Assert.That(consumer.OrderRemovedEvents.Count(), Is.EqualTo(1));
            });
        }

        [Test]
        public void RemoveOrder_MultipleLevels_CorrectEvent()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);
            var allItems = new List<OrderItem>()
            {
                new OrderItem(1, "GOOG", BuySell.Sell, 10, 100, "test"),
                new OrderItem(2, "GOOG", BuySell.Sell, 11, 100, "test"),
                new OrderItem(3, "GOOG", BuySell.Sell, 12, 100, "test"),
                new OrderItem(4, "GOOG", BuySell.Buy, 9, 100, "test"),
                new OrderItem(5, "GOOG", BuySell.Buy, 8, 100, "test"),
                new OrderItem(6, "GOOG", BuySell.Buy, 7, 100, "test"),
            };

            foreach (var item in allItems)
                service.AddOrder(item.StockCode, item.BuySell, item.Volume, item.Price, item.UserReference);

            var result = service.RemoveOrder(consumer.OrderAddedEvents[0].OrderId);

            Assert.Multiple(() =>
            {
                Assert.That(consumer.BestPriceChangedEvents.Count(), Is.EqualTo(3));
                Assert.That(consumer.OrderAddedEvents.Count(), Is.EqualTo(6));
                Assert.That(consumer.OrderRemovedEvents.Count(), Is.EqualTo(1));
            });
        }

        [Test]
        public void RemoveOrder_MultipleLevels_EventArgsExpected()
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

            var result = service.RemoveOrder(consumer.OrderAddedEvents[0].OrderId);

            Assert.Multiple(() =>
            {
                Assert.That(consumer.BestPriceChangedEvents.Count(), Is.EqualTo(3));
                Assert.That(consumer.OrderAddedEvents.Count(), Is.EqualTo(6));
                CollectionAssert.AreEquivalent(consumer.OrderAddedEvents, allItems);
                Assert.That(consumer.OrderRemovedEvents.Count(), Is.EqualTo(1));
            });
        }
    }
}
