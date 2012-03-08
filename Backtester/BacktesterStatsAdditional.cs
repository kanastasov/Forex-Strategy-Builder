// Backtester - Additional Statistics
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;
using Forex_Strategy_Builder.Interfaces;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester - Additional statistics
    /// </summary>
    public partial class Backtester
    {
        private double[] _longBalance;
        private double[] _shortBalance;
        private double[] _longMoneyBalance;
        private double[] _shortMoneyBalance;
        private double _maxLongBalance;
        private double _minLongBalance;
        private double _maxShortBalance;
        private double _minShortBalance;

        private DateTime _maxLongBalanceDate;
        private DateTime _minLongBalanceDate;
        private DateTime _maxShortBalanceDate;
        private DateTime _minShortBalanceDate;
        private DateTime _maxLongMoneyBalanceDate;
        private DateTime _minLongMoneyBalanceDate;
        private DateTime _maxShortMoneyBalanceDate;
        private DateTime _minShortMoneyBalanceDate;

        private double _grossLongProfit;
        private double _grossLongLoss;
        private double _grossShortProfit;
        private double _grossShortLoss;
        private double _grossLongMoneyProfit;
        private double _grossLongMoneyLoss;
        private double _grossShortMoneyProfit;
        private double _grossShortMoneyLoss;

        private double _maxLongDrawdown;
        private double _maxShortDrawdown;
        private double _maxLongMoneyDrawdown;
        private double _maxShortMoneyDrawdown;
        private double _maxLongMoneyDrawdownPercent;
        private double _maxShortMoneyDrawdownPercent;
        private DateTime _maxLongDrawdownDate;
        private DateTime _maxShortDrawdownDate;
        private DateTime _maxLongMoneyDrawdownDate;
        private DateTime _maxShortMoneyDrawdownDate;

        private int _barsWithLongPos;
        private int _barsWithShortPos;
        private int _barsWithPos;

        private int _winningLongTrades;
        private int _winningShortTrades;
        private int _losingLongTrades;
        private int _losingShortTrades;
        private int _totalLongTrades;
        private int _totalShortTrades;

        private double _maxLongWin;
        private double _maxShortWin;
        private double _maxLongMoneyWin;
        private double _maxShortMoneyWin;
        private double _maxLongLoss;
        private double _maxShortLoss;
        private double _maxLongMoneyLoss;
        private double _maxShortMoneyLoss;

        private double _ahpr;
        private double _ahprLong;
        private double _ahprShort;
        private double _ghpr;
        private double _ghprLong;
        private double _ghprShort;
        private double _sharpeRatio;
        private double _sharpeRatioLong;
        private double _sharpeRatioShort;

        /// <summary>
        /// Gets the Additional Stats Parameter Name.
        /// </summary>
        public string[] AdditionalStatsParamName { get; private set; }

        /// <summary>
        /// Gets the Additional Stats Value Long + Short.
        /// </summary>
        public string[] AdditionalStatsValueTotal { get; private set; }

        /// <summary>
        /// Gets the Additional Stats Value Long.
        /// </summary>
        public string[] AdditionalStatsValueLong { get; private set; }

        /// <summary>
        /// Gets the Additional Stats Value Short.
        /// </summary>
        public string[] AdditionalStatsValueShort { get; private set; }

        /// <summary>
        /// Gets the long balance in pips.
        /// </summary>
        public int NetLongBalance
        {
            get { return (int)Math.Round(_longBalance[DataSet.Bars - 1]); }
        }

        /// <summary>
        /// Gets the short balance in pips.
        /// </summary>
        public int NetShortBalance
        {
            get { return (int)Math.Round(_shortBalance[DataSet.Bars - 1]); }
        }

        /// <summary>
        /// Gets the max long balance in pips.
        /// </summary>
        public int MaxLongBalance
        {
            get { return (int) Math.Round(_maxLongBalance); }
        }

        /// <summary>
        /// Gets the max short balance in pips.
        /// </summary>
        public int MaxShortBalance
        {
            get { return (int) Math.Round(_maxShortBalance); }
        }

        /// <summary>
        /// Gets the min long balance in pips.
        /// </summary>
        public int MinLongBalance
        {
            get { return (int) Math.Round(_minLongBalance); }
        }

        /// <summary>
        /// Gets the min short balance in pips.
        /// </summary>
        public int MinShortBalance
        {
            get { return (int) Math.Round(_minShortBalance); }
        }

        /// <summary>
        /// Gets the long balance in money
        /// </summary>
        public double NetLongMoneyBalance
        {
            get { return _longMoneyBalance[DataSet.Bars - 1]; }
        }

        /// <summary>
        /// Gets the short balance in money.
        /// </summary>
        public double NetShortMoneyBalance
        {
            get { return _shortMoneyBalance[DataSet.Bars - 1]; }
        }

        /// <summary>
        /// Gets the max long balance in money.
        /// </summary>
        public double MaxLongMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the max short balance in money.
        /// </summary>
        public double MaxShortMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the min long balance in money.
        /// </summary>
        public double MinLongMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the min short balance in money.
        /// </summary>
        public double MinShortMoneyBalance { get; private set; }

        /// <summary>
        /// Returns the long balance at the end of the bar in pips.
        /// </summary>
        public int LongBalance(int bar)
        {
            return (int) Math.Round(_longBalance[bar]);
        }

        /// <summary>
        /// Returns the short balance at the end of the bar in pips.
        /// </summary>
        public int ShortBalance(int bar)
        {
            return (int) Math.Round(_shortBalance[bar]);
        }

        /// <summary>
        /// Returns the long balance at the end of the bar in money.
        /// </summary>
        public double LongMoneyBalance(int bar)
        {
            return _longMoneyBalance[bar];
        }

        /// <summary>
        /// Returns the short balance at the end of the bar in money.
        /// </summary>
        public double ShortMoneyBalance(int bar)
        {
            return _shortMoneyBalance[bar];
        }

        /// <summary>
        /// Calculates the values of the stats parameters.
        /// </summary>
        private void CalculateAdditionalStats()
        {
            _longBalance = new double[DataSet.Bars];
            _shortBalance = new double[DataSet.Bars];
            _longMoneyBalance = new double[DataSet.Bars];
            _shortMoneyBalance = new double[DataSet.Bars];

            MaxLongMoneyBalance = Configs.InitialAccount;
            MinLongMoneyBalance = Configs.InitialAccount;
            MaxShortMoneyBalance = Configs.InitialAccount;
            MinShortMoneyBalance = Configs.InitialAccount;
            _maxLongBalance = 0;
            _minLongBalance = 0;
            _maxShortBalance = 0;
            _minShortBalance = 0;

            _maxLongBalanceDate = DataSet.Time[0];
            _minLongBalanceDate = DataSet.Time[0];
            _maxShortBalanceDate = DataSet.Time[0];
            _minShortBalanceDate = DataSet.Time[0];
            _maxLongMoneyBalanceDate = DataSet.Time[0];
            _minLongMoneyBalanceDate = DataSet.Time[0];
            _maxShortMoneyBalanceDate = DataSet.Time[0];
            _minShortMoneyBalanceDate = DataSet.Time[0];
            _maxLongDrawdownDate = DataSet.Time[0];
            _maxShortDrawdownDate = DataSet.Time[0];
            _maxLongMoneyDrawdownDate = DataSet.Time[0];
            _maxShortMoneyDrawdownDate = DataSet.Time[0];

            _grossLongProfit = 0;
            _grossLongLoss = 0;
            _grossShortProfit = 0;
            _grossShortLoss = 0;
            _grossLongMoneyProfit = 0;
            _grossLongMoneyLoss = 0;
            _grossShortMoneyProfit = 0;
            _grossShortMoneyLoss = 0;

            _maxLongDrawdown = 0;
            _maxShortDrawdown = 0;
            _maxLongMoneyDrawdown = 0;
            _maxShortMoneyDrawdown = 0;
            _maxShortDrawdown = 0;
            _maxLongMoneyDrawdown = 0;
            _maxShortMoneyDrawdown = 0;
            _maxLongMoneyDrawdownPercent = 0;
            _maxShortMoneyDrawdownPercent = 0;

            _barsWithPos = 0;
            _barsWithLongPos = 0;
            _barsWithShortPos = 0;

            _winningLongTrades = 0;
            _winningShortTrades = 0;
            _losingLongTrades = 0;
            _losingShortTrades = 0;

            _totalLongTrades = 0;
            _totalShortTrades = 0;

            _maxLongWin = 0;
            _maxShortWin = 0;
            _maxLongMoneyWin = 0;
            _maxShortMoneyWin = 0;
            _maxLongLoss = 0;
            _maxShortLoss = 0;
            _maxLongMoneyLoss = 0;
            _maxShortMoneyLoss = 0;

            for (int bar = 0; bar < Strategy.FirstBar; bar++)
            {
                _longBalance[bar] = 0;
                _shortBalance[bar] = 0;
                _longMoneyBalance[bar] = Configs.InitialAccount;
                _shortMoneyBalance[bar] = Configs.InitialAccount;
            }

            for (int bar = Strategy.FirstBar; bar < DataSet.Bars; bar++)
            {
                _longBalance[bar] = _longBalance[bar - 1];
                _shortBalance[bar] = _shortBalance[bar - 1];
                _longMoneyBalance[bar] = _longMoneyBalance[bar - 1];
                _shortMoneyBalance[bar] = _shortMoneyBalance[bar - 1];

                bool isLong = false;
                bool isShort = false;
                for (int pos = 0; pos < Positions(bar); pos++)
                {
                    if (PosDir(bar, pos) == PosDirection.Long)
                        isLong = true;

                    if (PosDir(bar, pos) == PosDirection.Short)
                        isShort = true;

                    double positionProfitLoss = PosProfitLoss(bar, pos);
                    double positionMoneyProfitLoss = PosMoneyProfitLoss(bar, pos);

                    if (PosTransaction(bar, pos) == Transaction.Close ||
                        PosTransaction(bar, pos) == Transaction.Reduce ||
                        PosTransaction(bar, pos) == Transaction.Reverse)
                    {
                        if (OrdFromNumb(PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Sell)
                        {
                            // Closing long position
                            _longBalance[bar] += positionProfitLoss;
                            _longMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                _grossLongProfit += positionProfitLoss;
                                _grossLongMoneyProfit += positionMoneyProfitLoss;
                                _winningLongTrades++;
                                if (positionProfitLoss > _maxLongWin)
                                    _maxLongWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > _maxLongMoneyWin)
                                    _maxLongMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                _grossLongLoss += positionProfitLoss;
                                _grossLongMoneyLoss += positionMoneyProfitLoss;
                                _losingLongTrades++;
                                if (positionProfitLoss < _maxLongLoss)
                                    _maxLongLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < _maxLongMoneyLoss)
                                    _maxLongMoneyLoss = positionMoneyProfitLoss;
                            }

                            _totalLongTrades++;
                        }
                        if (OrdFromNumb(PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Buy)
                        {
                            // Closing short position
                            _shortBalance[bar] += positionProfitLoss;
                            _shortMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                _grossShortProfit += positionProfitLoss;
                                _grossShortMoneyProfit += positionMoneyProfitLoss;
                                _winningShortTrades++;
                                if (positionProfitLoss > _maxShortWin)
                                    _maxShortWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > _maxShortMoneyWin)
                                    _maxShortMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                _grossShortLoss += positionProfitLoss;
                                _grossShortMoneyLoss += positionMoneyProfitLoss;
                                _losingShortTrades++;
                                if (positionProfitLoss < _maxShortLoss)
                                    _maxShortLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < _maxShortMoneyLoss)
                                    _maxShortMoneyLoss = positionMoneyProfitLoss;
                            }

                            _totalShortTrades++;
                        }
                    }
                }

                _barsWithPos += (isLong || isShort) ? 1 : 0;
                _barsWithLongPos += isLong ? 1 : 0;
                _barsWithShortPos += isShort ? 1 : 0;

                if (_maxLongBalance < _longBalance[bar])
                {
                    _maxLongBalance = _longBalance[bar];
                    _maxLongBalanceDate = DataSet.Time[bar];
                }
                if (_minLongBalance > _longBalance[bar])
                {
                    _minLongBalance = _longBalance[bar];
                    _minLongBalanceDate = DataSet.Time[bar];
                }
                if (_maxShortBalance < _shortBalance[bar])
                {
                    _maxShortBalance = _shortBalance[bar];
                    _maxShortBalanceDate = DataSet.Time[bar];
                }
                if (_minShortBalance > _shortBalance[bar])
                {
                    _minShortBalance = _shortBalance[bar];
                    _minShortBalanceDate = DataSet.Time[bar];
                }
                if (MaxLongMoneyBalance < _longMoneyBalance[bar])
                {
                    MaxLongMoneyBalance = _longMoneyBalance[bar];
                    _maxLongMoneyBalanceDate = DataSet.Time[bar];
                }
                if (MinLongMoneyBalance > _longMoneyBalance[bar])
                {
                    MinLongMoneyBalance = _longMoneyBalance[bar];
                    _minLongMoneyBalanceDate = DataSet.Time[bar];
                }
                if (MaxShortMoneyBalance < _shortMoneyBalance[bar])
                {
                    MaxShortMoneyBalance = _shortMoneyBalance[bar];
                    _maxShortMoneyBalanceDate = DataSet.Time[bar];
                }
                if (MinShortMoneyBalance > _shortMoneyBalance[bar])
                {
                    MinShortMoneyBalance = _shortMoneyBalance[bar];
                    _minShortMoneyBalanceDate = DataSet.Time[bar];
                }

                // Maximum Drawdown
                if (_maxLongBalance - _longBalance[bar] > _maxLongDrawdown)
                {
                    _maxLongDrawdown = _maxLongBalance - _longBalance[bar];
                    _maxLongDrawdownDate = DataSet.Time[bar];
                }

                if (MaxLongMoneyBalance - _longMoneyBalance[bar] > _maxLongMoneyDrawdown)
                {
                    _maxLongMoneyDrawdown = MaxLongMoneyBalance - _longMoneyBalance[bar];
                    _maxLongMoneyDrawdownPercent = 100*_maxLongMoneyDrawdown/MaxLongMoneyBalance;
                    _maxLongMoneyDrawdownDate = DataSet.Time[bar];
                }

                if (_maxShortBalance - _shortBalance[bar] > _maxShortDrawdown)
                {
                    _maxShortDrawdown = _maxShortBalance - _shortBalance[bar];
                    _maxShortDrawdownDate = DataSet.Time[bar];
                }

                if (MaxShortMoneyBalance - _shortMoneyBalance[bar] > _maxShortMoneyDrawdown)
                {
                    _maxShortMoneyDrawdown = MaxShortMoneyBalance - _shortMoneyBalance[bar];
                    _maxShortMoneyDrawdownPercent = 100*_maxShortMoneyDrawdown/MaxShortMoneyBalance;
                    _maxShortMoneyDrawdownDate = DataSet.Time[bar];
                }
            }

            // Holding period returns
            _ahpr = 0;
            _ahprLong = 0;
            _ahprShort = 0;

            var hpr = new double[_totalTrades];
            var hprLong = new double[_totalLongTrades];
            var hprShort = new double[_totalShortTrades];

            double totalHPR = 0;
            double totalHPRLong = 0;
            double totalHPRShort = 0;

            double startBalance = Configs.InitialAccount;
            double startBalanceLong = Configs.InitialAccount;
            double startBalanceShort = Configs.InitialAccount;

            int count = 0;
            int countL = 0;
            int countS = 0;

            for (int pos = 0; pos < PositionsTotal; pos++)
            {
                // Charged fees
                Position position = PosFromNumb(pos);
                // Winning losing trades.
                if (position.Transaction == Transaction.Close ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                {
                    if (OrdFromNumb(position.FormOrdNumb).OrdDir == OrderDirection.Sell)
                    {
                        // Closing long position
                        hprLong[countL] = 1 + position.MoneyProfitLoss/startBalanceLong;
                        totalHPRLong += hprLong[countL];
                        countL++;
                        startBalanceLong += position.MoneyProfitLoss;
                    }
                    if (OrdFromNumb(position.FormOrdNumb).OrdDir == OrderDirection.Buy)
                    {
                        // Closing short position
                        hprShort[countS] = 1 + position.MoneyProfitLoss/startBalanceShort;
                        totalHPRShort += hprShort[countS];
                        countS++;
                        startBalanceShort += position.MoneyProfitLoss;
                    }
                    hpr[count] = 1 + position.MoneyProfitLoss/startBalance;
                    totalHPR += hpr[count];
                    count++;
                    startBalance += position.MoneyProfitLoss;
                }
            }

            double averageHPR = totalHPR/_totalTrades;
            double averageHPRLong = totalHPRLong/_totalLongTrades;
            double averageHPRShort = totalHPRShort/_totalShortTrades;

            _ahpr = 100*(averageHPR - 1);
            _ahprLong = 100*(averageHPRLong - 1);
            _ahprShort = 100*(averageHPRShort - 1);

            _ghpr = 100*(Math.Pow((NetMoneyBalance/Configs.InitialAccount), (1f/_totalTrades)) - 1);
            _ghprLong = 100*(Math.Pow((NetLongMoneyBalance/Configs.InitialAccount), (1f/_totalLongTrades)) - 1);
            _ghprShort = 100*(Math.Pow((NetShortMoneyBalance/Configs.InitialAccount), (1f/_totalShortTrades)) - 1);

            // Sharpe Ratio
            _sharpeRatio = 0;
            _sharpeRatioLong = 0;
            _sharpeRatioShort = 0;

            double sumPow = 0;
            double sumPowLong = 0;
            double sumPowShort = 0;

            for (int i = 0; i < _totalTrades; i++)
                sumPow += Math.Pow((hpr[i] - averageHPR), 2);
            for (int i = 0; i < _totalLongTrades; i++)
                sumPowLong += Math.Pow((hprLong[i] - averageHPRLong), 2);
            for (int i = 0; i < _totalShortTrades; i++)
                sumPowShort += Math.Pow((hprShort[i] - averageHPRShort), 2);

            double stDev = Math.Sqrt(sumPow/(_totalTrades - 1));
            double stDevLong = Math.Sqrt(sumPowLong/(_totalLongTrades - 1));
            double stDevShort = Math.Sqrt(sumPowShort/(_totalShortTrades - 1));

            _sharpeRatio = (averageHPR - 1)/stDev;
            _sharpeRatioLong = (averageHPRLong - 1)/stDevLong;
            _sharpeRatioShort = (averageHPRShort - 1)/stDevShort;
        }

        /// <summary>
        /// Sets the additional stats in pips.
        /// </summary>
        private void SetAdditionalStats()
        {
            string unit = " " + Language.T("pips");

            AdditionalStatsParamName = new[]
            {
                Language.T("Initial account"),
                Language.T("Account balance"),
                Language.T("Net profit"),
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Profit factor"),
                Language.T("Annualized profit"),
                Language.T("Minimum account"),
                Language.T("Minimum account date"),
                Language.T("Maximum account"),
                Language.T("Maximum account date"),
                Language.T("Absolute drawdown"),
                Language.T("Maximum drawdown"),
                Language.T("Maximum drawdown date"),
                Language.T("Historical bars"),
                Language.T("Tested bars"),
                Language.T("Bars with trades"),
                Language.T("Bars with trades") + " %",
                Language.T("Number of trades"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Maximum profit"),
                Language.T("Average profit"),
                Language.T("Maximum loss"),
                Language.T("Average loss"),
                Language.T("Expected payoff")
            };

            int totalWinTrades = _winningLongTrades + _winningShortTrades;
            int totalLossTrades = _losingLongTrades + _losingShortTrades;
            int totalTrades = totalWinTrades + totalLossTrades;

            AdditionalStatsValueTotal = new[]
            {
                "0" + unit,
                NetBalance + unit,
                NetBalance + unit,
                Math.Round(_grossProfit) + unit,
                Math.Round(_grossLoss) + unit,
                (Math.Abs(_grossLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossProfit/_grossLoss).ToString("F2")),
                Math.Round(((365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*NetBalance)) + unit,
                MinBalance + unit,
                _minBalanceDate.ToShortDateString(),
                MaxBalance + unit,
                _maxBalanceDate.ToShortDateString(),
                Math.Abs(MinBalance) + unit,
                Math.Round(_maxDrawdown) + unit,
                _maxDrawdownDate.ToShortDateString(),
                DataSet.Bars.ToString(CultureInfo.InvariantCulture),
                (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithPos/(DataSet.Bars - Strategy.FirstBar)).ToString("F2") + "%",
                totalTrades.ToString(CultureInfo.InvariantCulture),
                totalWinTrades.ToString(CultureInfo.InvariantCulture),
                totalLossTrades.ToString(CultureInfo.InvariantCulture),
                (1f*totalWinTrades/(totalWinTrades + totalLossTrades)).ToString("F2"),
                Math.Round(Math.Max(_maxLongWin, _maxShortWin)) + unit,
                Math.Round(_grossProfit/totalWinTrades) + unit,
                Math.Round(Math.Min(_maxLongLoss, _maxShortLoss)) + unit,
                Math.Round(_grossLoss/totalLossTrades) + unit,
                (1f*NetBalance/totalTrades).ToString("F2") + unit
            };

            AdditionalStatsValueLong = new[]
            {
                "0" + unit,
                NetLongBalance + unit,
                NetLongBalance + unit,
                Math.Round(_grossLongProfit) + unit,
                Math.Round(_grossLongLoss) + unit,
                (Math.Abs(_grossLongLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossLongProfit/_grossLongLoss).ToString("F2")),
                Math.Round(((365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*NetLongBalance)) + unit,
                MinLongBalance + unit,
                _minLongBalanceDate.ToShortDateString(),
                MaxLongBalance + unit,
                _maxLongBalanceDate.ToShortDateString(),
                Math.Round(Math.Abs(_minLongBalance)) + unit,
                Math.Round(_maxLongDrawdown) + unit,
                _maxLongDrawdownDate.ToShortDateString(),
                DataSet.Bars.ToString(CultureInfo.InvariantCulture),
                (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithLongPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithLongPos/(DataSet.Bars - Strategy.FirstBar)).ToString("F2") + "%",
                _totalLongTrades.ToString(CultureInfo.InvariantCulture),
                _winningLongTrades.ToString(CultureInfo.InvariantCulture),
                _losingLongTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningLongTrades/(_winningLongTrades + _losingLongTrades)).ToString("F2"),
                Math.Round(_maxLongWin) + unit,
                Math.Round(_grossLongProfit/_winningLongTrades) + unit,
                Math.Round(_maxLongLoss) + unit,
                Math.Round(_grossLongLoss/_losingLongTrades) + unit,
                (1f*NetLongBalance/(_winningLongTrades + _losingLongTrades)).ToString("F2") + unit
            };

            AdditionalStatsValueShort = new[]
            {
                "0" + unit,
                NetShortBalance + unit,
                NetShortBalance + unit,
                Math.Round(_grossShortProfit) + unit,
                Math.Round(_grossShortLoss) + unit,
                (Math.Abs(_grossShortLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossShortProfit/_grossShortLoss).ToString("F2")),
                Math.Round(((365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*NetShortBalance)) + unit,
                MinShortBalance + unit,
                _minShortBalanceDate.ToShortDateString(),
                MaxShortBalance + unit,
                _maxShortBalanceDate.ToShortDateString(),
                Math.Round(Math.Abs(_minShortBalance)) + unit,
                Math.Round(_maxShortDrawdown) + unit,
                _maxShortDrawdownDate.ToShortDateString(),
                DataSet.Bars.ToString(CultureInfo.InvariantCulture),
                (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithShortPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithShortPos/(DataSet.Bars - Strategy.FirstBar)).ToString("F2") + "%",
                _totalShortTrades.ToString(CultureInfo.InvariantCulture),
                _winningShortTrades.ToString(CultureInfo.InvariantCulture),
                _losingShortTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningShortTrades/(_winningShortTrades + _losingShortTrades)).ToString("F2"),
                Math.Round(_maxShortWin) + unit,
                Math.Round(_grossShortProfit/_winningShortTrades) + unit,
                Math.Round(_maxShortLoss) + unit,
                Math.Round(_grossShortLoss/_losingShortTrades) + unit,
                (1f*NetShortBalance/(_winningShortTrades + _losingShortTrades)).ToString("F2") + unit
            };
        }

        /// <summary>
        /// Sets the additional stats in Money.
        /// </summary>
        private void SetAdditionalMoneyStats()
        {
            string unit = " " + Configs.AccountCurrency;

            AdditionalStatsParamName = new[]
            {
                Language.T("Initial account"),
                Language.T("Account balance"),
                Language.T("Net profit"),
                Language.T("Net profit") + " %",
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Profit factor"),
                Language.T("Annualized profit"),
                Language.T("Annualized profit") + " %",
                Language.T("Minimum account"),
                Language.T("Minimum account date"),
                Language.T("Maximum account"),
                Language.T("Maximum account date"),
                Language.T("Absolute drawdown"),
                Language.T("Maximum drawdown"),
                Language.T("Maximum drawdown") + " %",
                Language.T("Maximum drawdown date"),
                Language.T("Historical bars"),
                Language.T("Tested bars"),
                Language.T("Bars with trades"),
                Language.T("Bars with trades") + " %",
                Language.T("Number of trades"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Maximum profit"),
                Language.T("Average profit"),
                Language.T("Maximum loss"),
                Language.T("Average loss"),
                Language.T("Expected payoff"),
                Language.T("Average holding period returns"),
                Language.T("Geometric holding period returns"),
                Language.T("Sharpe ratio")
            };

            int totalWinTrades = _winningLongTrades + _winningShortTrades;
            int totalLossTrades = _losingLongTrades + _losingShortTrades;
            int totalTrades = totalWinTrades + totalLossTrades;

            AdditionalStatsValueTotal = new[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetMoneyBalance.ToString("F2") + unit,
                (NetMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100*((NetMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                GrossMoneyProfit.ToString("F2") + unit,
                GrossMoneyLoss.ToString("F2") + unit,
                (Math.Abs(GrossMoneyLoss - 0) < 0.00001 ? "N/A" : Math.Abs(GrossMoneyProfit/GrossMoneyLoss).ToString("F2")),
                ((365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*(NetMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100*(365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*(NetMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                MinMoneyBalance.ToString("F2") + unit,
                _minMoneyBalanceDate.ToShortDateString(),
                MaxMoneyBalance.ToString("F2") + unit,
                _maxMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinMoneyBalance).ToString("F2") + unit,
                MaxMoneyDrawdown.ToString("F2") + unit,
                _maxMoneyDrawdownPercent.ToString("F2") + "%",
                _maxMoneyDrawdownDate.ToShortDateString(),
                DataSet.Bars.ToString(CultureInfo.InvariantCulture),
                (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithPos/(DataSet.Bars - Strategy.FirstBar)).ToString("F2") + "%",
                totalTrades.ToString(CultureInfo.InvariantCulture),
                totalWinTrades.ToString(CultureInfo.InvariantCulture),
                totalLossTrades.ToString(CultureInfo.InvariantCulture),
                (1f*totalWinTrades/(totalWinTrades + totalLossTrades)).ToString("F2"),
                Math.Max(_maxLongMoneyWin, _maxShortMoneyWin).ToString("F2") + unit,
                (GrossMoneyProfit/totalWinTrades).ToString("F2") + unit,
                Math.Min(_maxLongMoneyLoss, _maxShortMoneyLoss).ToString("F2") + unit,
                (GrossMoneyLoss/totalLossTrades).ToString("F2") + unit,
                (1f*(NetMoneyBalance - Configs.InitialAccount)/totalTrades).ToString("F2") + unit,
                _ahpr.ToString("F2") + "%",
                _ghpr.ToString("F2") + "%",
                _sharpeRatio.ToString("F2")
            };

            AdditionalStatsValueLong = new[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetLongMoneyBalance.ToString("F2") + unit,
                (NetLongMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100*((NetLongMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                _grossLongMoneyProfit.ToString("F2") + unit,
                _grossLongMoneyLoss.ToString("F2") + unit,
                (Math.Abs(_grossLongMoneyLoss - 0) < 0.00001 ? "N/A" : Math.Abs(_grossLongMoneyProfit/_grossLongMoneyLoss).ToString("F2")),
                ((365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*
                (NetLongMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100*(365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*
                (NetLongMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                MinLongMoneyBalance.ToString("F2") + unit,
                _minLongMoneyBalanceDate.ToShortDateString(),
                MaxLongMoneyBalance.ToString("F2") + unit,
                _maxLongMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinLongMoneyBalance).ToString("F2") + unit,
                _maxLongMoneyDrawdown.ToString("F2") + unit,
                _maxLongMoneyDrawdownPercent.ToString("F2") + "%",
                _maxLongMoneyDrawdownDate.ToShortDateString(),
                DataSet.Bars.ToString(CultureInfo.InvariantCulture),
                (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithLongPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithLongPos/(DataSet.Bars - Strategy.FirstBar)).ToString("F2") + "%",
                _totalLongTrades.ToString(CultureInfo.InvariantCulture),
                _winningLongTrades.ToString(CultureInfo.InvariantCulture),
                _losingLongTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningLongTrades/(_winningLongTrades + _losingLongTrades)).ToString("F2"),
                _maxLongMoneyWin.ToString("F2") + unit,
                (_grossLongMoneyProfit/_winningLongTrades).ToString("F2") + unit,
                _maxLongMoneyLoss.ToString("F2") + unit,
                (_grossLongMoneyLoss/_losingLongTrades).ToString("F2") + unit,
                (1f*(NetLongMoneyBalance - Configs.InitialAccount)/
                (_winningLongTrades + _losingLongTrades)).ToString("F2") + unit,
                _ahprLong.ToString("F2") + "%",
                _ghprLong.ToString("F2") + "%",
                _sharpeRatioLong.ToString("F2")
            };

            AdditionalStatsValueShort = new[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetShortMoneyBalance.ToString("F2") + unit,
                (NetShortMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100*((NetShortMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount)).ToString("F2") + "%",
                _grossShortMoneyProfit.ToString("F2") + unit,
                _grossShortMoneyLoss.ToString("F2") + unit,
                (Math.Abs(_grossShortMoneyLoss - 0) < 0.0001 ? "N/A" : Math.Abs(_grossShortMoneyProfit/_grossShortMoneyLoss).ToString("F2")),
                ((365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*(NetShortMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100*(365f/DataSet.Time[DataSet.Bars - 1].Subtract(DataSet.Time[0]).Days)*(NetShortMoneyBalance - Configs.InitialAccount)/Configs.InitialAccount).ToString("F2") + "%",
                MinShortMoneyBalance.ToString("F2") + unit,
                _minShortMoneyBalanceDate.ToShortDateString(),
                MaxShortMoneyBalance.ToString("F2") + unit,
                _maxShortMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinShortMoneyBalance).ToString("F2") + unit,
                _maxShortMoneyDrawdown.ToString("F2") + unit,
                _maxShortMoneyDrawdownPercent.ToString("F2") + "%",
                _maxShortMoneyDrawdownDate.ToShortDateString(),
                DataSet.Bars.ToString(CultureInfo.InvariantCulture),
                (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture),
                _barsWithShortPos.ToString(CultureInfo.InvariantCulture),
                (100f*_barsWithShortPos/(DataSet.Bars - Strategy.FirstBar)).ToString("F2") + "%",
                _totalShortTrades.ToString(CultureInfo.InvariantCulture),
                _winningShortTrades.ToString(CultureInfo.InvariantCulture),
                _losingShortTrades.ToString(CultureInfo.InvariantCulture),
                (1f*_winningShortTrades/(_winningShortTrades + _losingShortTrades)).ToString("F2"),
                _maxShortMoneyWin.ToString("F2") + unit,
                (_grossShortMoneyProfit/_winningShortTrades).ToString("F2") + unit,
                _maxShortMoneyLoss.ToString("F2") + unit,
                (_grossShortMoneyLoss/_losingShortTrades).ToString("F2") + unit,
                (1f*(NetShortMoneyBalance - Configs.InitialAccount)/(_winningShortTrades + _losingShortTrades)).ToString("F2") + unit,
                _ahprShort.ToString("F2") + "%",
                _ghprShort.ToString("F2") + "%",
                _sharpeRatioShort.ToString("F2")
            };
        }
    }
}