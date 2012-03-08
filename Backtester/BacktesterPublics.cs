// Backtester - Publics
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Market.
    /// </summary>
    public partial class Backtester
    {
        /// <summary>
        /// Gets or sets the Interpolation Method
        /// </summary>
        public InterpolationMethod InterpolationMethod { get; set; }

        /// <summary>
        /// Interpolation Method string.
        /// </summary>
        public String InterpolationMethodToString()
        {
            string method;
            switch (InterpolationMethod)
            {
                case InterpolationMethod.Pessimistic:
                    method = Language.T("Pessimistic scenario");
                    break;
                case InterpolationMethod.Shortest:
                    method = Language.T("Shortest bar route");
                    break;
                case InterpolationMethod.Nearest:
                    method = Language.T("Nearest order first");
                    break;
                case InterpolationMethod.Optimistic:
                    method = Language.T("Optimistic scenario");
                    break;
                case InterpolationMethod.Random:
                    method = Language.T("Random execution");
                    break;
                default:
                    method = Language.T("Error");
                    break;
            }

            return method;
        }

        /// <summary>
        /// Interpolation Method string.
        /// </summary>
        public String InterpolationMethodShortToString()
        {
            string method;
            switch (InterpolationMethod)
            {
                case InterpolationMethod.Pessimistic:
                    method = Language.T("Pessimistic");
                    break;
                case InterpolationMethod.Shortest:
                    method = Language.T("Shortest");
                    break;
                case InterpolationMethod.Nearest:
                    method = Language.T("Nearest");
                    break;
                case InterpolationMethod.Optimistic:
                    method = Language.T("Optimistic");
                    break;
                case InterpolationMethod.Random:
                    method = Language.T("Random");
                    break;
                default:
                    method = Language.T("Error");
                    break;
            }

            return method;
        }

        /// <summary>
        /// Gets the position coordinates.
        /// </summary>
        public PositionCoordinates[] PosCoordinates { get { return _posCoord; } }

        /// <summary>
        /// Gets the total number of the positions.
        /// </summary>
        public int PositionsTotal { get { return _totalPositions; } }

        /// <summary>
        /// Number of the positions during de session.
        /// </summary>
        public int Positions(int bar)
        {
            return _session[bar].Positions;
        }

        /// <summary>
        /// Checks whether we have got a position. "Closed" is also a position.
        /// </summary>
        public bool IsPos(int bar)
        {
            PosDirection dir = _session[bar].Summary.PosDir;
            return dir == PosDirection.Long
                || dir == PosDirection.Short
                || dir == PosDirection.Closed;
        }

        /// <summary>
        /// Checks whether we have a position.
        /// </summary>
        private bool IsOpenPos(int bar)
        {
            bool isPosition =
                _session[bar].Summary.PosDir == PosDirection.Long ||
                _session[bar].Summary.PosDir == PosDirection.Short;

            return isPosition;
        }

        /// <summary>
        /// Last Position's number.
        /// </summary>
        public int SummaryPosNumb(int bar)
        {
            return _session[bar].Summary.PosNumb;
        }

        /// <summary>
        /// Last Position's order number.
        /// </summary>
        public int SummaryOrdNumb(int bar)
        {
            return _session[bar].Summary.FormOrdNumb;
        }

        /// <summary>
        /// Position direction at the end of the bar
        /// </summary>
        public PosDirection SummaryDir(int bar)
        {
            return _session[bar].Summary.PosDir;
        }

        /// <summary>
        /// Position lots at the end of the bar.
        /// </summary>
        public double SummaryLots(int bar)
        {
            return _session[bar].Summary.PosLots;
        }

        /// <summary>
        /// Position amount at the end of the bar.
        /// </summary>
        public int SummaryAmount(int bar)
        {
            return (int)Math.Round(_session[bar].Summary.PosLots * DataSet.InstrProperties.LotSize);
        }

        /// <summary>
        /// The last transaction for the bar.
        /// </summary>
        public Transaction SummaryTrans(int bar)
        {
            return _session[bar].Summary.Transaction;
        }

        /// <summary>
        /// Position price at the end of the bar.
        /// </summary>
        public double SummaryPrice(int bar)
        {
            return _session[bar].Summary.PosPrice;
        }

        /// <summary>
        /// Returns the position Order Price at the end of the bar.
        /// </summary>
        public double SummaryOrdPrice(int bar)
        {
            return _session[bar].Summary.FormOrdPrice;
        }

        /// <summary>
        /// Returns the Absolute Permanent SL
        /// </summary>
        public double SummaryAbsoluteSL(int bar)
        {
            return _session[bar].Summary.AbsoluteSL;
        }

        /// <summary>
        /// Returns the Absolute Permanent TP
        /// </summary>
        public double SummaryAbsoluteTP(int bar)
        {
            return _session[bar].Summary.AbsoluteTP;
        }

        /// <summary>
        /// Returns the Required Margin at the end of the bar
        /// </summary>
        public double SummaryRequiredMargin(int bar)
        {
            return _session[bar].Summary.RequiredMargin;
        }

        /// <summary>
        /// Returns the Free Margin at the end of the bar
        /// </summary>
        public double SummaryFreeMargin(int bar)
        {
            return _session[bar].Summary.FreeMargin;
        }

        /// <summary>
        /// Position icon at the end of the bar
        /// </summary>
        public PositionIcons SummaryPositionIcon(int bar)
        {
            return _session[bar].Summary.PositionIcon;
        }

        /// <summary>
        /// The number of the position
        /// </summary>
        public int PosNumb(int bar, int pos)
        {
            return _session[bar].Position[pos].PosNumb;
        }

        /// <summary>
        /// The position direction
        /// </summary>
        public PosDirection PosDir(int bar, int pos)
        {
            return _session[bar].Position[pos].PosDir;
        }

        /// <summary>
        /// The position lots
        /// </summary>
        public double PosLots(int bar, int pos)
        {
            return _session[bar].Position[pos].PosLots;
        }

        /// <summary>
        /// The position amount in currency
        /// </summary>
        public double PosAmount(int bar, int pos)
        {
            return _session[bar].Position[pos].PosLots * DataSet.InstrProperties.LotSize;
        }

        /// <summary>
        /// The position forming order number
        /// </summary>
        public int PosOrdNumb(int bar, int pos)
        {
            return _session[bar].Position[pos].FormOrdNumb;
        }

        /// <summary>
        /// The position forming order price
        /// </summary>
        public double PosOrdPrice(int bar, int pos)
        {
            return _session[bar].Position[pos].FormOrdPrice;
        }

        /// <summary>
        /// The position Required Margin
        /// </summary>
        public double PosRequiredMargin(int bar, int pos)
        {
            return _session[bar].Position[pos].RequiredMargin;
        }

        /// <summary>
        /// The position Free Margin
        /// </summary>
        public double PosFreeMargin(int bar, int pos)
        {
            return _session[bar].Position[pos].FreeMargin;
        }

        /// <summary>
        /// The position balance
        /// </summary>
        public double PosBalance(int bar, int pos)
        {
            return _session[bar].Position[pos].Balance;
        }

        /// <summary>
        /// The position equity
        /// </summary>
        public double PosEquity(int bar, int pos)
        {
            return _session[bar].Position[pos].Equity;
        }

        /// <summary>
        /// The position Profit Loss
        /// </summary>
        public double PosProfitLoss(int bar, int pos)
        {
            return _session[bar].Position[pos].ProfitLoss;
        }

        /// <summary>
        /// The position Floating P/L
        /// </summary>
        public double PosFloatingPL(int bar, int pos)
        {
            return _session[bar].Position[pos].FloatingPL;
        }

        /// <summary>
        /// The position Profit Loss in currency
        /// </summary>
        public double PosMoneyProfitLoss(int bar, int pos)
        {
            return _session[bar].Position[pos].MoneyProfitLoss;
        }

        /// <summary>
        /// The position Floating Profit Loss in currency
        /// </summary>
        public double PosMoneyFloatingPL(int bar, int pos)
        {
            return _session[bar].Position[pos].MoneyFloatingPL;
        }

        /// <summary>
        /// The position balance in currency
        /// </summary>
        public double PosMoneyBalance(int bar, int pos)
        {
            return _session[bar].Position[pos].MoneyBalance;
        }

        /// <summary>
        /// The position equity in currency
        /// </summary>
        public double PosMoneyEquity(int bar, int pos)
        {
            return _session[bar].Position[pos].MoneyEquity;
        }

        /// <summary>
        /// The position's corrected price
        /// </summary>
        public double PosPrice(int bar, int pos)
        {
            return _session[bar].Position[pos].PosPrice;
        }

        /// <summary>
        /// The position's Transaction
        /// </summary>
        public Transaction PosTransaction(int bar, int pos)
        {
            return _session[bar].Position[pos].Transaction;
        }

        /// <summary>
        /// The position's Icon
        /// </summary>
        public PositionIcons PosIcon(int bar, int pos)
        {
            return _session[bar].Position[pos].PositionIcon;
        }

        /// <summary>
        /// Returns the position's Profit Loss in pips.
        /// </summary>
        public int ProfitLoss(int bar)
        {
            return (int)Math.Round(_session[bar].Summary.ProfitLoss);
        }

        /// <summary>
        /// Returns the Floating Profit Loss at the end of the bar in pips
        /// </summary>
        public int FloatingPL(int bar)
        {
            return (int)Math.Round(_session[bar].Summary.FloatingPL);
        }

        /// <summary>
        /// Returns the account balance at the end of the bar in pips
        /// </summary>
        public int Balance(int bar)
        {
            return (int)Math.Round(_session[bar].Summary.Balance);
        }

        /// <summary>
        /// Returns the equity at the end of the bar in pips
        /// </summary>
        public int Equity(int bar)
        {
            return (int)Math.Round(_session[bar].Summary.Equity);
        }

        /// <summary>
        /// Returns the charged spread.
        /// </summary>
        public double ChargedSpread(int bar)
        {
            return _session[bar].Summary.Spread;
        }

        /// <summary>
        /// Returns the charged rollover.
        /// </summary>
        public double ChargedRollOver(int bar)
        {
            return _session[bar].Summary.Rollover;
        }

        /// <summary>
        /// Returns the bar end Profit Loss in currency.
        /// </summary>
        public double MoneyProfitLoss(int bar)
        {
            return _session[bar].Summary.MoneyProfitLoss;
        }

        /// <summary>
        /// Returns the bar end Floating Profit Loss in currency
        /// </summary>
        public double MoneyFloatingPL(int bar)
        {
            return _session[bar].Summary.MoneyFloatingPL;
        }

        /// <summary>
        /// Returns the account balance in currency
        /// </summary>
        public double MoneyBalance(int bar)
        {
            return _session[bar].Summary.MoneyBalance;
        }

        /// <summary>
        /// Returns the current bill in currency.
        /// </summary>
        public double MoneyEquity(int bar)
        {
            return _session[bar].Summary.MoneyEquity;
        }

        /// <summary>
        /// Returns the charged spread in currency.
        /// </summary>
        public double MoneyChargedSpread(int bar)
        {
            return _session[bar].Summary.MoneySpread;
        }

        /// <summary>
        /// Returns the charged rollover in currency.
        /// </summary>
        public double MoneyChargedRollOver(int bar)
        {
            return _session[bar].Summary.MoneyRollover;
        }

        /// <summary>
        /// Returns the backtest safety evaluation
        /// </summary>
        public string BackTestEvalToString(int bar)
        {
            return bar < Strategy.FirstBar || _session[bar].BacktestEval == BacktestEval.None
                       ? ""
                       : _session[bar].BacktestEval.ToString();
        }

        /// <summary>
        /// Returns the backtest safety evaluation
        /// </summary>
        public BacktestEval BackTestEval(int bar)
        {
            return _session[bar].BacktestEval;
        }

        /// <summary>
        /// Returns the position with the required number
        /// </summary>
        public Position PosFromNumb(int posNumber)
        {
            if (posNumber < 0) posNumber = 0;
            return _session[_posCoord[posNumber].Bar].Position[_posCoord[posNumber].Pos];
        }

        /// <summary>
        /// Gets the total number of the orders
        /// </summary>
        public int OrdersTotal { get { return SentOrders; } }

        /// <summary>
        /// Returns the number of orders for the designated bar
        /// </summary>
        public int Orders(int bar)
        {
            return _session[bar].Orders;
        }

        /// <summary>
        /// Returns the Order Number
        /// </summary>
        public int OrdNumb(int bar, int ord)
        {
            return _session[bar].Order[ord].OrdNumb;
        }

        /// <summary>
        /// Returns the order with the corresponding number
        /// </summary>
        public Order OrdFromNumb(int ordNumber)
        {
            if (ordNumber < 0) ordNumber = 0;
            return _session[_ordCoord[ordNumber].Bar].Order[_ordCoord[ordNumber].Ord];
        }

        /// <summary>
        ///  Way point
        /// </summary>
        public WayPoint WayPoint(int bar, int wayPointNumber)
        {
            return _session[bar].WayPoint[wayPointNumber];
        }

        /// <summary>
        ///  Bar's way points count.
        /// </summary>
        public int WayPoints(int bar)
        {
            return _session[bar].WayPoints;
        }

        /// <summary>
        /// Copies all sessions.
        /// </summary>
        public Session[] GetAllSessionsCopy()
        {
            var sessions = new Session[DataSet.Bars];
            for (int bar = 0; bar < DataSet.Bars; bar++)
                sessions[bar] = _session[bar].Copy();
            return sessions;
        }

        /// <summary>
        /// Copies Position Coordinates.
        /// </summary>
        public PositionCoordinates[] GetPosCoordinateCopy()
        {
            var posCoposinates = new PositionCoordinates[_posCoord.Length];
            for (int i = 0; i < _posCoord.Length; i++)
                posCoposinates[i] = _posCoord[i];
            return posCoposinates;
        }

        /// <summary>
        /// Copies Order Coordinates.
        /// </summary>
        public OrderCoordinates[] GetOrdCoordinateCopy()
        {
            var ordCoordinates = new OrderCoordinates[_ordCoord.Length];
            for (int i = 0; i < _ordCoord.Length; i++)
                ordCoordinates[i] = _ordCoord[i];
            return ordCoordinates;
        }
    }
}
