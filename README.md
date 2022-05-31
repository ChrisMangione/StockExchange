# StockExchange

How would I change the IExchange Interface

- RemoveOrder only accepts 'orderId' which is limiting to the data structure. Make it also require stock code. This also means I cannot implement asynchronous add or remove per stock code
- If RemoveOrder can only have one parameter consider making orderId a string with concatenated stock code
- Make Add and Remove asynchronous for each stock code. Will have to implement locking if exchange is adding or removing. The exchange, when editing the order book, has to be synchronous. Some thought should go into this as an exchange should be first in first served.
- Think about the events, might have to also make asynchronous fire and forget without interrupting execution flow
- Add a TradeExecuted event to the interface
