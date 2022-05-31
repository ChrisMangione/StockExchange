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
    internal class OrderCrossedTests
    {
        [Test]
        public void CrossedOrder_OrderExecutedAndRemoved_ReceivedCrossedTradeEvents()
        {
            IExchange service = new ExchangeService();
            var consumer = new ExchangeMockConsumer(service);

            service.AddOrder("GOOG", BuySell.Buy, 100, 10, "test");
            service.AddOrder("GOOG", BuySell.Sell, 100, 11, "test");

            Assert.Multiple(() =>
            {
                Assert.That(consumer.TradeExecutedEvents.Count, Is.EqualTo(2));
            });
        }
    }
}
