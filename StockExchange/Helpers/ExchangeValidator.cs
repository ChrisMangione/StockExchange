using StockExchange.Models;

namespace StockExchange.Helpers
{
    public static class ExchangeValidator
    {
        private static bool IsAllowedStockCode(string stockCode, string[] stockCodes)
        {
            return stockCodes.Contains(stockCode);
        }

        public static int ValidateOrderParams(OrderItem orderItem, string[] stockCodes)
        {
            if (orderItem.Volume <= 0)
                return ExchangeErrorCodes.InvalidVolume;
            
            if (orderItem.Price <= 0)
                return ExchangeErrorCodes.InvalidPrice;

            if (!IsAllowedStockCode(orderItem.StockCode, stockCodes))
                return ExchangeErrorCodes.InvalidStockCode;

            return ExchangeErrorCodes.NoError;

        }
    }
}
