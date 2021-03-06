using StockExchange.Models;

namespace StockExchange
{
    public interface IExchange
    {
        /// <summary>
        /// Add the given order information to the exchange.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This should trigger OrderAdded if the order was successfully added,
        /// and BestPriceChanged if the best price or volume for that stock 
        /// changed.
        /// </para>
        /// </remarks>
        /// <param name="stockCode">The stock being bought or sold</param>
        /// <param name="buySell">Is the order to sell or buy the stock</param>
        /// <param name="volume">How many shares are to be bought or sold</param>
        /// <param name="price">What price is being offered or accepted for each 
        /// share</param>
        /// <param name="userReference">Identifies the order to the orderer.</param>
        /// <returns>Returns 0 if everything works, or an error code if something 
        /// goes wrong.</returns>
        int AddOrder(
            string stockCode,
            BuySell buySell, int volume, decimal price,
            string userReference);

        /// <summary>
        /// Remove a given order from the exchange
        /// </summary>
        /// <param name="orderId">The order id as generated by AddOrder()</param>
        /// <returns></returns>
        int RemoveOrder(int orderId);

        /// <summary>
        /// Triggered when an order is successfully addd to the exchange
        /// </summary>
        event EventHandler<OrderAddedEventArgs> OrderAdded;

        /// <summary>
        /// Triggered when an order is successfully removed from the exchange
        /// </summary>
        event EventHandler<OrderRemovedEventArgs> OrderRemoved;

        /// <summary>
        /// Triggered when the best price or volume for a stock changes.
        /// </summary>
        event EventHandler<BestPriceChangedEventArgs> BestPriceChanged;
    }

}
