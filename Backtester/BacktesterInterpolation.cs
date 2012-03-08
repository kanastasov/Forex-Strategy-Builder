// Backtester - Interpolation
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester
    /// </summary>
    public partial class Backtester
    {
        /// <summary>
        /// Arranges the orders inside the bar.
        /// </summary>
        private void BarInterpolation(int bar)
        {
            BacktestEval eval;
            double open = DataSet.Open[bar];
            double high = DataSet.High[bar];
            double low = DataSet.Low[bar];
            double close = DataSet.Close[bar];
            double current = open;

            int reachedIntrabar = 0;
            int tradedIntrabar = -1;
            int reachedTick = 0;

            int lastPosBreakEven = -1;
            int lastPosActivatedBE = -1;

            do
            {
                Order orderHigher = null;
                Order orderLower = null;
                double priceHigher = high;
                double priceLower = low;
                bool isHigherPrice = false;
                bool isLowerPrice = false;
                bool isTopReachable = true;
                bool isBottomReachable = true;
                bool isClosingAmbiguity = false;
                bool isScanningResult = false;

                // Break Even
                int currentPosNumber = _session[bar].Summary.PosNumb;
                if (Strategy.UseBreakEven && lastPosBreakEven != currentPosNumber && IsOpenPos(bar))
                {
                    if (lastPosActivatedBE != currentPosNumber && !_session[bar].Summary.IsBreakEvenActivated)
                    {
                        SetBreakEvenActivation(bar);
                        lastPosActivatedBE = currentPosNumber;
                    }

                    if (SetBreakEvenExit(bar, current, lastPosBreakEven))
                        lastPosBreakEven = currentPosNumber;
                }

                // Setting the parameters
                for (int ord = 0; ord < _session[bar].Orders; ord++)
                {
                    if (!CheckOrd(bar, ord)) continue;

                    Order order = _session[bar].Order[ord];
                    var prices = new[] {order.OrdPrice, order.OrdPrice2};
                    foreach (double price in prices)
                    {
                        if (high + _micron <= price || price <= low - _micron)
                            continue;

                        if (isTopReachable)
                            isTopReachable = current > price + _micron;

                        if (isBottomReachable)
                            isBottomReachable = current < price - _micron;

                        if (price > current - _micron && price < priceHigher + _micron)
                        {
                            // New nearer Upper price
                            isHigherPrice = true;
                            priceHigher = price;
                            orderHigher = order;
                            isTopReachable = false;
                        }
                        else if (price < current && price > priceLower - _micron)
                        {
                            // New nearer Lower price
                            isLowerPrice = true;
                            priceLower = price;
                            orderLower = order;
                            isBottomReachable = false;
                        }
                    }
                }

                // Evaluate the bar
                if (!isLowerPrice && !isHigherPrice)
                {
                    // No more orders
                    eval = BacktestEval.None;
                }
                else if (isLowerPrice && isHigherPrice)
                {
                    // There are a higher and a lower order
                    eval = BacktestEval.Ambiguous;
                }
                else if (isHigherPrice && priceHigher - current < _micron)
                {
                    // There is an order at the current price
                    eval = BacktestEval.Correct;
                }
                else if (isLowerPrice && current - priceLower < _micron)
                {
                    // There is an order at the current price
                    eval = BacktestEval.Correct;
                }
                else
                {
                    // Check for a Closing Ambiguity
                    if (_session[bar].IsBottomReached && _session[bar].IsTopReached &&
                        current > close - _micron && close > priceLower)
                        isClosingAmbiguity = true;

                    else if (_session[bar].IsBottomReached && _session[bar].IsTopReached &&
                             current < close + _micron && close < priceHigher)
                        isClosingAmbiguity = true;

                    else if (_session[bar].IsTopReached && isHigherPrice &&
                             current > close - _micron)
                        isClosingAmbiguity = true;

                    else if (_session[bar].IsBottomReached && isLowerPrice &&
                             current < close + _micron)
                        isClosingAmbiguity = true;

                    eval = isClosingAmbiguity
                               ? BacktestEval.Ambiguous
                               : BacktestEval.Correct;
                }


                if (_isScanning && Configs.UseTickData && DataSet.IsTickData && DataSet.TickData[bar] != null)
                {
                    isScanningResult = TickScanning(bar, eval,
                                                    ref current, ref reachedTick,
                                                    isTopReachable, isBottomReachable,
                                                    isHigherPrice, isLowerPrice,
                                                    priceHigher, priceLower,
                                                    orderHigher, orderLower,
                                                    isClosingAmbiguity);
                }

                if (_isScanning && !isScanningResult && DataSet.IntraBarsPeriods[bar] != DataSet.Period)
                {
                    isScanningResult = IntrabarScanning(bar, eval, ref current,
                                                        ref reachedIntrabar, ref tradedIntrabar,
                                                        isTopReachable, isBottomReachable,
                                                        isHigherPrice, isLowerPrice,
                                                        priceHigher, priceLower,
                                                        orderHigher, orderLower,
                                                        isClosingAmbiguity);
                }

                if (isScanningResult) continue;

                switch (InterpolationMethod)
                {
                        // Calls a method
                    case InterpolationMethod.Optimistic:
                    case InterpolationMethod.Pessimistic:
                        OptimisticPessimisticMethod(bar, eval, ref current,
                                                    isTopReachable, isBottomReachable,
                                                    isHigherPrice, isLowerPrice,
                                                    priceHigher, priceLower,
                                                    orderHigher, orderLower,
                                                    isClosingAmbiguity);
                        break;
                    case InterpolationMethod.Shortest:
                        ShortestMethod(bar, eval, ref current,
                                       isTopReachable, isBottomReachable,
                                       isHigherPrice, isLowerPrice,
                                       priceHigher, priceLower,
                                       orderHigher, orderLower,
                                       isClosingAmbiguity);
                        break;
                    case InterpolationMethod.Nearest:
                        NearestMethod(bar, eval, ref current,
                                      isTopReachable, isBottomReachable,
                                      isHigherPrice, isLowerPrice,
                                      priceHigher, priceLower,
                                      orderHigher, orderLower,
                                      isClosingAmbiguity);
                        break;
                    case InterpolationMethod.Random:
                        RandomMethod(bar, eval, ref current,
                                     isTopReachable, isBottomReachable,
                                     isHigherPrice, isLowerPrice,
                                     priceHigher, priceLower,
                                     orderHigher, orderLower,
                                     isClosingAmbiguity);
                        break;
                }
            } while (!(eval == BacktestEval.None && _session[bar].IsTopReached && _session[bar].IsBottomReached));
        }

        /// <summary>
        /// Tick Scanning
        /// </summary>
        private bool TickScanning(int bar, BacktestEval eval,
                                         ref double current, ref int reachedTick,
                                         bool isTopReachable, bool isBottomReachable,
                                         bool isHigherPrice, bool isLowerPrice,
                                         double priceHigher, double priceLower,
                                         Order orderHigher, Order orderLower,
                                         bool isClosingAmbiguity)
        {
            double high = DataSet.High[bar];
            double low = DataSet.Low[bar];
            double close = DataSet.Close[bar];
            bool isScanningResult = false;

            if (eval == BacktestEval.None)
            {
                // There isn't any orders
                if (!_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    int tickCount = DataSet.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (DataSet.TickData[bar][tick] + _micron > high)
                        {
                            // Top found
                            current = high;
                            _session[bar].SetWayPoint(high, WayPointType.High);
                            _session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                        if (DataSet.TickData[bar][tick] - _micron < low)
                        {
                            // Bottom found
                            current = low;
                            _session[bar].SetWayPoint(low, WayPointType.Low);
                            _session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!_session[bar].IsTopReached)
                {
                    // Whether hit the Top
                    int tickCount = DataSet.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (DataSet.TickData[bar][tick] + _micron > high)
                        {
                            // Top found
                            current = high;
                            _session[bar].SetWayPoint(high, WayPointType.High);
                            _session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!_session[bar].IsBottomReached)
                {
                    // Whether hit the Bottom
                    int tickCount = DataSet.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (DataSet.TickData[bar][tick] - _micron < low)
                        {
                            // Bottom found
                            current = low;
                            _session[bar].SetWayPoint(low, WayPointType.Low);
                            _session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }

            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top / bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!_session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the Bottom
                    int tickCount = DataSet.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (DataSet.TickData[bar][tick] + _micron > thePrice)
                        {
                            // The order is reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                        if (DataSet.TickData[bar][tick] - _micron < low)
                        {
                            // Bottom is reached
                            current = low;
                            _session[bar].SetWayPoint(low, WayPointType.Low);
                            _session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!_session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the Top
                    int tickCount = DataSet.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (DataSet.TickData[bar][tick] + _micron > high)
                        {
                            // The Top is reached
                            current = high;
                            _session[bar].SetWayPoint(high, WayPointType.High);
                            _session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                        if (DataSet.TickData[bar][tick] - _micron < thePrice)
                        {
                            // The order is reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Execute the order
                    double priceOld = DataSet.TickData[bar][reachedTick];
                    int tickCount = DataSet.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (priceOld - _micron < thePrice && DataSet.TickData[bar][tick] + _micron > thePrice ||
                            priceOld + _micron > thePrice && DataSet.TickData[bar][tick] - _micron < thePrice)
                        {
                            // Order reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the the first reached order
                    int tickCount = DataSet.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (DataSet.TickData[bar][tick] + _micron > priceHigher)
                        {
                            // Upper order is reached
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                        if (DataSet.TickData[bar][tick] - _micron < priceLower)
                        {
                            // Lower order is reached
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Execute or exit the bar
                    Order theOrder = null;
                    double thePrice = 0.0;
                    if (isHigherPrice)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else if (isLowerPrice)
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }

                    bool executeOrder = false;
                    if (isHigherPrice)
                    {
                        int tickCount = DataSet.TickData[bar].Length;
                        for (int tick = reachedTick; tick < tickCount; tick++)
                        {
                            reachedTick = tick;
                            if (DataSet.TickData[bar][tick] + _micron > thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        int tickCount = DataSet.TickData[bar].Length;
                        for (int tick = reachedTick; tick < tickCount; tick++)
                        {
                            reachedTick = tick;
                            if (DataSet.TickData[bar][tick] - _micron < thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }

                    if (executeOrder)
                    {
                        // Execute the order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                    }
                    else
                    {
                        // Exit the bar
                        current = close;
                        theOrder.OrdStatus = OrderStatus.Cancelled;
                        _session[bar].BacktestEval = BacktestEval.Correct;
                    }

                    isScanningResult = true;
                }
            }

            return isScanningResult;
        }

        /// <summary>
        /// Intrabar Scanning
        /// </summary>
        private bool IntrabarScanning(int bar, BacktestEval eval, ref double current,
                                             ref int reachedIntrabar, ref int tradedIntrabar,
                                             bool isTopReachable, bool isBottomReachable,
                                             bool isHigherPrice, bool isLowerPrice,
                                             double priceHigher, double priceLower,
                                             Order orderHigher, Order orderLower,
                                             bool isClosingAmbiguity)
        {
            double high = DataSet.High[bar];
            double low = DataSet.Low[bar];
            double close = DataSet.Close[bar];
            bool isScanningResult = false;

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (DataSet.IntraBarData[bar][intraBar].High + _micron > high)
                        {
                            // Top found
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (DataSet.IntraBarData[bar][intraBar].Low - _micron < low)
                        {
                            // Bottom found
                            if (isScanningResult)
                            {
                                // Top and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {
                            // Hit the Top
                            current = high;
                            _session[bar].SetWayPoint(high, WayPointType.High);
                            _session[bar].IsTopReached = true;
                        }
                        else
                        {
                            // Hit the Bottom
                            current = low;
                            _session[bar].SetWayPoint(low, WayPointType.Low);
                            _session[bar].IsBottomReached = true;
                        }
                    }
                }
                else if (!_session[bar].IsTopReached)
                {
                    // Whether hit the Top
                    for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (DataSet.IntraBarData[bar][intraBar].High + _micron > high)
                        {
                            // Top found
                            current = high;
                            _session[bar].SetWayPoint(high, WayPointType.High);
                            _session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!_session[bar].IsBottomReached)
                {
                    // Whether hit the Bottom
                    for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (DataSet.IntraBarData[bar][intraBar].Low - _micron < low)
                        {
                            // Bottom found
                            current = low;
                            _session[bar].SetWayPoint(low, WayPointType.Low);
                            _session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }

            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top / bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!_session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (DataSet.IntraBarData[bar][intraBar].High + _micron > thePrice)
                        {
                            // The order is reached
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (DataSet.IntraBarData[bar][intraBar].Low - _micron < low)
                        {
                            // Bottom is reached
                            if (isScanningResult)
                            {
                                // The Order and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {
                            // Execute
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                return false;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            tradedIntrabar = reachedIntrabar;
                        }
                        else
                        {
                            // Hit the Bottom
                            current = low;
                            _session[bar].SetWayPoint(low, WayPointType.Low);
                            _session[bar].IsBottomReached = true;
                        }
                    }
                }
                else if (!_session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the Top
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (DataSet.IntraBarData[bar][intraBar].High + _micron > high)
                        {
                            // The Top is reached
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (DataSet.IntraBarData[bar][intraBar].Low - _micron < thePrice)
                        {
                            // The order is reached
                            if (isScanningResult)
                            {
                                // The Top and the order are into the same intrabar
                                isScanningResult = false;
                                break;
                            }

                            // The order is reachable downwards
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {
                            // Hit the Top
                            current = high;
                            _session[bar].SetWayPoint(high, WayPointType.High);
                            _session[bar].IsTopReached = true;
                        }
                        else
                        {
                            // Execute
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                return false;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            tradedIntrabar = reachedIntrabar;
                        }
                    }
                }
                else
                {
                    // Execute the order
                    for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (DataSet.IntraBarData[bar][intraBar].High + _micron > thePrice &&
                            DataSet.IntraBarData[bar][intraBar].Low - _micron < thePrice)
                        {
                            // Order reached
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                return false;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            isScanningResult = true;
                            tradedIntrabar = reachedIntrabar;
                            break;
                        }
                    }
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the the first reached order
                    bool executeUpper = false;
                    for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (DataSet.IntraBarData[bar][intraBar].High + _micron > priceHigher)
                        {
                            // Upper order is reached
                            executeUpper = true;
                            isScanningResult = true;
                        }

                        if (DataSet.IntraBarData[bar][intraBar].Low - _micron < priceLower)
                        {
                            // Lower order is reached
                            if (isScanningResult)
                            {
                                // Top and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        Order theOrder;
                        double thePrice;
                        if (executeUpper)
                        {
                            theOrder = orderHigher;
                            thePrice = priceHigher;
                        }
                        else
                        {
                            theOrder = orderLower;
                            thePrice = priceLower;
                        }

                        if (tradedIntrabar == reachedIntrabar)
                        {
                            return false;
                        }
                        eval = BacktestEval.Correct;
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                        tradedIntrabar = reachedIntrabar;
                    }
                }
                else
                {
                    // Execute or exit the bar
                    Order theOrder = null;
                    double thePrice = 0;
                    if (isHigherPrice)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else if (isLowerPrice)
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }

                    bool executeOrder = false;
                    if (isHigherPrice)
                    {
                        for (int intraBar = reachedIntrabar; intraBar < DataSet.IntraBarBars[bar]; intraBar++)
                        {
                            reachedIntrabar = intraBar;

                            if (DataSet.IntraBarData[bar][intraBar].High + _micron > thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        for (int b = reachedIntrabar; b < DataSet.IntraBarBars[bar]; b++)
                        {
                            reachedIntrabar = b;

                            if (DataSet.IntraBarData[bar][b].Low - _micron < thePrice)
                            {
                                // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }

                    if (executeOrder)
                    {
                        // Execute the order
                        if (tradedIntrabar == reachedIntrabar)
                        {
                            return false;
                        }
                        current = thePrice;
                        eval = BacktestEval.Correct;
                        ExecOrd(bar, theOrder, thePrice, eval);
                        tradedIntrabar = reachedIntrabar;
                    }
                    else
                    {
                        // Exit the bar
                        current = close;
                        theOrder.OrdStatus = OrderStatus.Cancelled;
                        _session[bar].BacktestEval = BacktestEval.Correct;
                    }
                    isScanningResult = true;
                }
            }

            return isScanningResult;
        }

        /// <summary>
        /// Random Execution Method
        /// </summary>
        private void RandomMethod(int bar, BacktestEval eval, ref double current,
                                         bool isTopReachable, bool isBottomReachable,
                                         bool isHigherPrice, bool isLowerPrice,
                                         double priceHigher, double priceLower,
                                         Order orderHigher, Order orderLower,
                                         bool isClosingAmbiguity)
        {
            double high = DataSet.High[bar];
            double low = DataSet.Low[bar];
            double close = DataSet.Close[bar];
            var random = new Random();

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    var upRange = (int) Math.Round((high - current)/DataSet.InstrProperties.Point);
                    var downRange = (int) Math.Round((current - low)/DataSet.InstrProperties.Point);
                    upRange = upRange < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange = 1;
                        downRange = 1;
                    }
                    int rand = random.Next(upRange + downRange);
                    bool isHitHigh;

                    if (upRange > downRange)
                        isHitHigh = rand > upRange;
                    else
                        isHitHigh = rand < downRange;

                    if (isHitHigh)
                    {
                        // Hit the Top
                        current = high;
                        _session[bar].SetWayPoint(high, WayPointType.High);
                        _session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        _session[bar].SetWayPoint(low, WayPointType.Low);
                        _session[bar].IsBottomReached = true;
                    }
                }
                else if (!_session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    _session[bar].SetWayPoint(high, WayPointType.High);
                    _session[bar].IsTopReached = true;
                }
                else if (!_session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    _session[bar].SetWayPoint(low, WayPointType.Low);
                    _session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top/bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!_session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    var iUpRange = (int) Math.Round((thePrice - current)/DataSet.InstrProperties.Point);
                    var iDnRange = (int) Math.Round((current - low)/DataSet.InstrProperties.Point);
                    iUpRange = iUpRange < 0 ? 0 : iUpRange;
                    iDnRange = iDnRange < 0 ? 0 : iDnRange;
                    if (iDnRange + iDnRange == 0)
                    {
                        iUpRange = 1;
                        iDnRange = 1;
                    }
                    int rand = random.Next(iUpRange + iDnRange);
                    bool executeUpper;

                    if (iUpRange > iDnRange)
                        executeUpper = rand > iUpRange;
                    else
                        executeUpper = rand < iDnRange;

                    if (executeUpper)
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        _session[bar].SetWayPoint(low, WayPointType.Low);
                        _session[bar].IsBottomReached = true;
                    }
                }
                else if (!_session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the Top
                    var upRange = (int) Math.Round((high - current)/DataSet.InstrProperties.Point);
                    var downRange = (int) Math.Round((current - thePrice)/DataSet.InstrProperties.Point);
                    upRange = upRange < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange = 1;
                        downRange = 1;
                    }
                    int rand = random.Next(upRange + downRange);
                    bool executeUpper;

                    if (upRange > downRange)
                        executeUpper = rand > upRange;
                    else
                        executeUpper = rand < downRange;

                    if (executeUpper)
                    {
                        // Hit the Top
                        current = high;
                        _session[bar].SetWayPoint(high, WayPointType.High);
                        _session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the randomly chosen order
                    var upRange = (int) Math.Round((priceHigher - current)/DataSet.InstrProperties.Point);
                    var downRange = (int) Math.Round((current - priceLower)/DataSet.InstrProperties.Point);
                    upRange = upRange < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange = 1;
                        downRange = 1;
                    }
                    int rand = random.Next(upRange + downRange);
                    bool executeUpper;

                    if (upRange > downRange)
                        executeUpper = rand > upRange;
                    else
                        executeUpper = rand < downRange;

                    Order theOrder;
                    double thePrice;
                    if (executeUpper)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Execute or exit the bar
                    if (isHigherPrice)
                    {
                        var upRange = (int) Math.Round((priceHigher - current)/DataSet.InstrProperties.Point);
                        var downRange = (int) Math.Round((close - current)/DataSet.InstrProperties.Point);
                        upRange = upRange < 0 ? 0 : upRange;
                        downRange = downRange < 0 ? 0 : downRange;
                        if (downRange + downRange == 0)
                        {
                            upRange = 1;
                            downRange = 0;
                        }
                        int rand = random.Next(upRange + downRange);

                        if (rand > upRange)
                        {
                            // Execute the order
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                            _session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        var upRange = (int) Math.Round((current - close)/DataSet.InstrProperties.Point);
                        var downRange = (int) Math.Round((current - priceLower)/DataSet.InstrProperties.Point);
                        upRange = upRange < 0 ? 0 : upRange;
                        downRange = downRange < 0 ? 0 : downRange;
                        if (downRange + downRange == 0)
                        {
                            upRange = 0;
                            downRange = 1;
                        }
                        int rand = random.Next(upRange + downRange);

                        if (rand > downRange)
                        {
                            // Execute the order
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                            _session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Nearest order first Method
        /// </summary>
        private void NearestMethod(int bar, BacktestEval eval, ref double current,
                                          bool isTopReachable, bool isBottomReachable,
                                          bool isHigherPrice, bool isLowerPrice,
                                          double priceHigher, double priceLower,
                                          Order orderHigher, Order orderLower,
                                          bool isClosingAmbiguity)
        {
            double open = DataSet.Open[bar];
            double high = DataSet.High[bar];
            double low = DataSet.Low[bar];
            double close = DataSet.Close[bar];

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    if (close < open)
                    {
                        // Hit the Top
                        current = high;
                        _session[bar].SetWayPoint(high, WayPointType.High);
                        _session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        _session[bar].SetWayPoint(low, WayPointType.Low);
                        _session[bar].IsBottomReached = true;
                    }
                }
                else if (!_session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    _session[bar].SetWayPoint(high, WayPointType.High);
                    _session[bar].IsTopReached = true;
                }
                else if (!_session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    _session[bar].SetWayPoint(low, WayPointType.Low);
                    _session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top/bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!_session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    double upRange = thePrice - current;
                    double downRange = current - low;

                    if (upRange < downRange)
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        _session[bar].SetWayPoint(low, WayPointType.Low);
                        _session[bar].IsBottomReached = true;
                    }
                }
                else if (!_session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the bottom
                    double upRange = high - current;
                    double downRange = current - thePrice;

                    if (upRange < downRange)
                    {
                        // Hit the Top
                        current = high;
                        _session[bar].SetWayPoint(high, WayPointType.High);
                        _session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the nearest order
                    double upRange = priceHigher - current;
                    double downRange = current - priceLower;

                    Order theOrder;
                    double thePrice;
                    if (upRange < downRange)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Exit the bar
                    double orderRange = isHigherPrice ? priceHigher - current : current - priceLower;
                    double closeRange = Math.Abs(current - close);

                    if (orderRange < closeRange)
                    {
                        // Execute the order
                        Order theOrder;
                        double thePrice;
                        if (isHigherPrice)
                        {
                            theOrder = orderHigher;
                            thePrice = priceHigher;
                        }
                        else
                        {
                            theOrder = orderLower;
                            thePrice = priceLower;
                        }
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Cancel the order, go to Close
                        current = close;
                        _session[bar].BacktestEval = BacktestEval.Ambiguous;
                        if (isHigherPrice)
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                        else if (isLowerPrice)
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                    }
                }
            }
        }

        /// <summary>
        /// Shortest route inside the bar Method
        /// </summary>
        private void ShortestMethod(int bar, BacktestEval eval, ref double current,
                                           bool isTopReachable, bool isBottomReachable,
                                           bool isHigherPrice, bool isLowerPrice,
                                           double priceHigher, double priceLower,
                                           Order orderHigher, Order orderLower,
                                           bool isClosingAmbiguity)
        {
            double open = DataSet.Open[bar];
            double high = DataSet.High[bar];
            double low = DataSet.Low[bar];
            double close = DataSet.Close[bar];

            bool isGoUpward;
            if (!_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                isGoUpward = open > close;
            else if (_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                isGoUpward = false;
            else if (!_session[bar].IsTopReached && _session[bar].IsBottomReached)
                isGoUpward = true;
            else
                isGoUpward = open > close;

            if (isLowerPrice && current - priceLower < _micron)
                isGoUpward = false;
            if (isHigherPrice && priceHigher - current < _micron)
                isGoUpward = true;

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    if (isGoUpward)
                    {
                        // Hit the Top
                        current = high;
                        _session[bar].SetWayPoint(high, WayPointType.High);
                        _session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        _session[bar].SetWayPoint(low, WayPointType.Low);
                        _session[bar].IsBottomReached = true;
                    }
                }
                else if (!_session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    _session[bar].SetWayPoint(high, WayPointType.High);
                    _session[bar].IsTopReached = true;
                }
                else if (!_session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    _session[bar].SetWayPoint(low, WayPointType.Low);
                    _session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the top/bottom or execute
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!_session[bar].IsBottomReached && isBottomReachable && !isGoUpward)
                {
                    // Hit the Bottom
                    current = low;
                    _session[bar].SetWayPoint(low, WayPointType.Low);
                    _session[bar].IsBottomReached = true;
                }
                else if (!_session[bar].IsTopReached && isTopReachable && isGoUpward)
                {
                    // Hit the Top
                    current = high;
                    _session[bar].SetWayPoint(high, WayPointType.High);
                    _session[bar].IsTopReached = true;
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute the nearest order
                    Order theOrder;
                    double thePrice;

                    if (isGoUpward)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Exit the bar
                    current = close;
                    _session[bar].BacktestEval = BacktestEval.Ambiguous;
                    if (isHigherPrice)
                        orderHigher.OrdStatus = OrderStatus.Cancelled;
                    else if (isLowerPrice)
                        orderLower.OrdStatus = OrderStatus.Cancelled;
                }
            }
        }

        /// <summary>
        /// Optimistic / Pessimistic Method
        /// </summary>
        private void OptimisticPessimisticMethod(int bar, BacktestEval eval, ref double current,
                                                        bool isTopReachable, bool isBottomReachable,
                                                        bool isHigherPrice, bool isLowerPrice,
                                                        double priceHigher, double priceLower,
                                                        Order orderHigher, Order orderLower,
                                                        bool isClosingAmbiguity)
        {
            double open = DataSet.Open[bar];
            double high = DataSet.High[bar];
            double low = DataSet.Low[bar];
            double close = DataSet.Close[bar];

            bool isOptimistic = InterpolationMethod == InterpolationMethod.Optimistic;

            if (eval == BacktestEval.None)
            {
                // There is no more orders
                if (!_session[bar].IsTopReached && !_session[bar].IsBottomReached)
                {
                    // Neither the top nor the bottom was reached
                    if (close < open)
                    {
                        // Hit the Top
                        current = high;
                        _session[bar].SetWayPoint(high, WayPointType.High);
                        _session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        _session[bar].SetWayPoint(low, WayPointType.Low);
                        _session[bar].IsBottomReached = true;
                    }
                }
                else if (!_session[bar].IsTopReached)
                {
                    // Hit the Top
                    current = high;
                    _session[bar].SetWayPoint(high, WayPointType.High);
                    _session[bar].IsTopReached = true;
                }
                else if (!_session[bar].IsBottomReached)
                {
                    // Hit the Bottom
                    current = low;
                    _session[bar].SetWayPoint(low, WayPointType.Low);
                    _session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {
                // Hit the order or the top/bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!_session[bar].IsBottomReached && isBottomReachable)
                {
                    // The order or the bottom
                    bool goUpward;

                    if (current < low + _micron)
                        goUpward = false;
                    else if (thePrice - current < _micron)
                        goUpward = true;
                    else if (theOrder.OrdDir == OrderDirection.Buy)
                        goUpward = !isOptimistic;
                    else if (theOrder.OrdDir == OrderDirection.Sell)
                        goUpward = isOptimistic;
                    else
                        goUpward = thePrice - current < current - low;

                    if (goUpward)
                    {
                        // Execute order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        _session[bar].SetWayPoint(low, WayPointType.Low);
                        _session[bar].IsBottomReached = true;
                    }
                }
                else if (!_session[bar].IsTopReached && isTopReachable)
                {
                    // The order or the top
                    bool goUpward;

                    if (current > high - _micron)
                        goUpward = true;
                    else if (current - thePrice < _micron)
                        goUpward = false;
                    else if (theOrder.OrdDir == OrderDirection.Buy)
                        goUpward = !isOptimistic;
                    else if (theOrder.OrdDir == OrderDirection.Sell)
                        goUpward = isOptimistic;
                    else
                        goUpward = high - current < current - thePrice;

                    if (goUpward)
                    {
                        // Hit the Top
                        current = high;
                        _session[bar].SetWayPoint(high, WayPointType.High);
                        _session[bar].IsTopReached = true;
                    }
                    else
                    {
                        // Execute order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {
                    // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {
                // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {
                    // Execute one of both orders
                    bool executeUpper;

                    if (priceHigher - current < _micron)
                        executeUpper = true;
                    else if (current - priceLower < _micron)
                        executeUpper = false;
                    else if (_session[bar].Summary.PosDir == PosDirection.Long)
                        executeUpper = isOptimistic;
                    else if (_session[bar].Summary.PosDir == PosDirection.Short)
                        executeUpper = !isOptimistic;
                    else
                    {
                        if (orderHigher.OrdDir == OrderDirection.Buy && orderLower.OrdDir == OrderDirection.Buy)
                            executeUpper = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Sell && orderLower.OrdDir == OrderDirection.Sell)
                            executeUpper = isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Buy && orderLower.OrdDir == OrderDirection.Sell)
                        {
                            if (current < close)
                                executeUpper = isOptimistic;
                            else
                                executeUpper = !isOptimistic;

                            if (Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                                executeUpper = !executeUpper;
                        }
                        else
                        {
                            if (current < close)
                                executeUpper = !isOptimistic;
                            else
                                executeUpper = isOptimistic;

                            if (Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                                executeUpper = !executeUpper;
                        }
                    }

                    Order theOrder;
                    double thePrice;
                    if (executeUpper)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;

                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {
                    // Execute or exit the bar
                    if (isHigherPrice)
                    {
                        bool toExecute = false;
                        if (_session[bar].Summary.PosDir == PosDirection.Long)
                            toExecute = isOptimistic;
                        else if (_session[bar].Summary.PosDir == PosDirection.Short)
                            toExecute = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Buy)
                            toExecute = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Sell)
                            toExecute = isOptimistic;

                        if (toExecute)
                        {
                            // Execute
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                            _session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                    else if (isLowerPrice)
                    {
                        // The priceLower or Exit the bar
                        bool toExecute = false;

                        if (_session[bar].Summary.PosDir == PosDirection.Long)
                            toExecute = !isOptimistic;
                        else if (_session[bar].Summary.PosDir == PosDirection.Short)
                            toExecute = isOptimistic;
                        else if (orderLower.OrdDir == OrderDirection.Buy)
                            toExecute = isOptimistic;
                        else if (orderLower.OrdDir == OrderDirection.Sell)
                            toExecute = !isOptimistic;

                        if (toExecute)
                        {
                            // Execute
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, eval);
                        }
                        else
                        {
                            // Exit the bar
                            current = close;
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                            _session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                }
            }
        }
    }
}