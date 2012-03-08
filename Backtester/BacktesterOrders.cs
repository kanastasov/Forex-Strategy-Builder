// Backtester - Orders
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Backtester Class
    /// </summary>
    public partial class Backtester
    {
        /// <summary>
        /// Sets a new order Buy Market.
        /// </summary>
        void OrdBuyMarket(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Buy;
            order.OrdType   = OrderType.Market;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        /// Sets a new order Buy Stop.
        /// </summary>
        void OrdBuyStop(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Buy;
            order.OrdType   = OrderType.Stop;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        /// Sets a new order Buy Limit.
        /// </summary>
        void OrdBuyLimit(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Buy;
            order.OrdType   = OrderType.Limit;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        /// Sets a new order Buy Stop Limit.
        /// </summary>
        void OrdBuyStopLimit(int bar, int orderIf, int toPos, double lots, double price1, double price2, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Buy;
            order.OrdType   = OrderType.StopLimit;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price1, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = Math.Round(price2, DataSet.InstrProperties.Digits);
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        /// Sets a new order Sell Market.
        /// </summary>
        void OrdSellMarket(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Sell;
            order.OrdType   = OrderType.Market;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        /// Sets a new order Sell Stop.
        /// </summary>
        void OrdSellStop(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Sell;
            order.OrdType   = OrderType.Stop;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        /// Sets a new order Sell Limit.
        /// </summary>
        void OrdSellLimit(int bar, int orderIf, int toPos, double lots, double price, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Sell;
            order.OrdType   = OrderType.Limit;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = 0;
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }

        /// <summary>
        /// Sets a new order Sell Stop Limit.
        /// </summary>
        void OrdSellStopLimit(int bar, int orderIf, int toPos, double lots, double price1, double price2, OrderSender sender, OrderOrigin origin, string note)
        {
            int sessionOrder = _session[bar].Orders;
            Order order = _session[bar].Order[sessionOrder] = new Order();

            order.OrdNumb   = SentOrders;
            order.OrdDir    = OrderDirection.Sell;
            order.OrdType   = OrderType.StopLimit;
            order.OrdCond   = orderIf > 0 ? OrderCondition.If : OrderCondition.Norm;
            order.OrdStatus = OrderStatus.Confirmed;
            order.OrdIF     = orderIf;
            order.OrdPos    = toPos;
            order.OrdLots   = lots;
            order.OrdPrice  = Math.Round(price1, DataSet.InstrProperties.Digits);
            order.OrdPrice2 = Math.Round(price2, DataSet.InstrProperties.Digits);
            order.OrdSender = sender;
            order.OrdOrigin = origin;
            order.OrdNote   = note;

            _ordCoord[SentOrders].Bar = bar;
            _ordCoord[SentOrders].Ord = sessionOrder;
            _session[bar].Orders++;
            SentOrders++;
        }
    }
}
