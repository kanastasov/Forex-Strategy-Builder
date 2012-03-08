// Backtester class - Statistics
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester
    /// </summary>
    public partial class Backtester
    {
        double _maxBalance;
        double _minBalance;
        double _maxDrawdown;
        double _maxEquity;
        double _minEquity;
        double _maxEquityDrawdown;
        double _grossProfit;
        double _grossLoss;
        double _minMoneyBalance;
        int[] _balanceDrawdown;
        int[] _equityDrawdown;

        int    _barsInPosition;
        int    _totalTrades;

        DateTime _maxBalanceDate;
        DateTime _minBalanceDate;
        DateTime _maxMoneyBalanceDate;
        DateTime _minMoneyBalanceDate;
        double   _maxMoneyDrawdownPercent;
        DateTime _maxDrawdownDate;
        DateTime _maxMoneyDrawdownDate;

        public Backtester()
        {
            AccountStatsFlags = new bool[0];
            AccountStatsValue = new string[0];
            AccountStatsParam = new string[0];
            AdditionalStatsValueShort = new string[0];
            AdditionalStatsValueLong = new string[0];
            AdditionalStatsValueTotal = new string[0];
            AdditionalStatsParamName = new string[0];
        }

        /// <summary>
        /// Gets the account balance in pips
        /// </summary>
        public int NetBalance { get { return Balance(DataSet.Bars - 1); } }

        /// <summary>
        /// Gets the max balance in pips
        /// </summary>
        public int MaxBalance { get { return (int)Math.Round(_maxBalance); } }

        /// <summary>
        /// Gets the min balance in pips
        /// </summary>
        public int MinBalance { get { return (int)Math.Round(_minBalance); } }

        /// <summary>
        /// Gets the account balance in currency
        /// </summary>
        public double NetMoneyBalance { get { return MoneyBalance(DataSet.Bars - 1); } }

        /// <summary>
        /// Gets the max balance in currency
        /// </summary>
        public double MaxMoneyBalance { get; private set; }

        /// <summary>
        /// Gets the min balance in currency
        /// </summary>
        public double MinMoneyBalance { get { return _minMoneyBalance; } }

        /// <summary>
        /// Gets the max equity
        /// </summary>
        public int MaxEquity { get { return (int)Math.Round(_maxEquity); } }

        /// <summary>
        /// Gets the min equity in pips
        /// </summary>
        public int MinEquity { get { return (int)Math.Round(_minEquity); } }

        /// <summary>
        /// Gets the max Equity in currency
        /// </summary>
        public double MaxMoneyEquity { get; private set; }

        /// <summary>
        /// Gets the min Equity in currency
        /// </summary>
        public double MinMoneyEquity { get; private set; }

        /// <summary>
        /// Gets the maximum drawdown in the account bill
        /// </summary>
        public int MaxDrawdown { get { return (int)Math.Round(_maxDrawdown); } }

        /// <summary>
        /// Gets the maximum equity drawdown in the account bill
        /// </summary>
        public int MaxEquityDrawdown { get { return (int)Math.Round(_maxEquityDrawdown); } }

        /// <summary>
        /// Gets the maximum money drawdown
        /// </summary>
        public double MaxMoneyDrawdown { get; private set; }

        /// <summary>
        /// Gets the maximum money equity drawdown
        /// </summary>
        public double MaxMoneyEquityDrawdown { get; private set; }

        /// <summary>
        /// The total earned pips
        /// </summary>
        public int GrossProfit { get { return (int)Math.Round(_grossProfit); } }

        /// <summary>
        /// The total earned money
        /// </summary>
        public double GrossMoneyProfit { get; private set; }

        /// <summary>
        /// The total lost pips
        /// </summary>
        public int GrossLoss { get { return (int)Math.Round(_grossLoss); } }

        /// <summary>
        /// The total lost money
        /// </summary>
        public double GrossMoneyLoss { get; private set; }

        /// <summary>
        /// Gets the count of executed orders
        /// </summary>
        public int ExecutedOrders { get; private set; }

        /// <summary>
        /// Gets the count of lots have been traded
        /// </summary>
        public double TradedLots { get; private set; }

        /// <summary>
        /// Gets the time in position in percents
        /// </summary>
        public int TimeInPosition { get { return (int)Math.Round(100f * _barsInPosition / (DataSet.Bars - Strategy.FirstBar)); } }

        /// <summary>
        /// Gets the count of sent orders
        /// </summary>
        public int SentOrders { get; private set; }

        /// <summary>
        /// Gets the Charged Spread
        /// </summary>
        public double TotalChargedSpread { get; private set; }

        /// <summary>
        /// Gets the Charged Spread in currency
        /// </summary>
        public double TotalChargedMoneySpread { get; private set; }

        /// <summary>
        /// Gets the Charged RollOver
        /// </summary>
        public double TotalChargedRollOver { get; private set; }

        /// <summary>
        /// Gets the Charged RollOver in currency
        /// </summary>
        public double TotalChargedMoneyRollOver { get; private set; }

        /// <summary>
        /// Gets the Charged Slippage
        /// </summary>
        public double TotalChargedSlippage { get; private set; }

        /// <summary>
        /// Gets the Charged Slippage in currency
        /// </summary>
        public double TotalChargedMoneySlippage { get; private set; }

        /// <summary>
        /// Gets the Charged Commission
        /// </summary>
        public double TotalChargedCommission { get; private set; }

        /// <summary>
        /// Gets the Charged Commission in currency
        /// </summary>
        public double TotalChargedMoneyCommission { get; private set; }

        /// <summary>
        /// Winning Trades
        /// </summary>
        public int WinningTrades { get; private set; }

        /// <summary>
        /// Losing Trades
        /// </summary>
        public int LosingTrades { get; private set; }

        /// <summary>
        /// Win / Loss ratio
        /// </summary>
        public double WinLossRatio { get; private set; }

        /// <summary>
        /// Money Equity Percent Drawdown
        /// </summary>
        public double MoneyEquityPercentDrawdown { get; private set; }

        /// <summary>
        /// Equity Percent Drawdown
        /// </summary>
        public double EquityPercentDrawdown { get; private set; }

        /// <summary>
        /// Returns the ambiguous calculated bars
        /// </summary>
        public int AmbiguousBars { get; private set; }

        /// <summary>
        /// Was the intrabar scanning performed
        /// </summary>
        public bool IsScanPerformed { get; private set; }

        /// <summary>
        /// Margin Call Bar
        /// </summary>
        public int MarginCallBar { get; private set; }

        /// <summary>
        /// Gets the number of days tested.
        /// </summary>
        public int TestedDays { get; private set; }

        /// <summary>
        /// Gets the profit per tested day.
        /// </summary>
        public int ProfitPerDay { get { return TestedDays > 0 ? Balance(DataSet.Bars - 1) / TestedDays : 0; } }

        /// <summary>
        /// Gets the profit per tested day in currency.
        /// </summary>
        public double MoneyProfitPerDay { get { return TestedDays > 0 ? (MoneyBalance(DataSet.Bars - 1) - Configs.InitialAccount) / TestedDays : 0; } }

        /// <summary>
        /// Gets the account stats parameters
        /// </summary>
        public string[] AccountStatsParam { get; private set; }

        /// <summary>
        /// Gets the account stats values
        /// </summary>
        public string[] AccountStatsValue { get; private set; }

        /// <summary>
        /// Gets the account stats flags
        /// </summary>
        public bool[] AccountStatsFlags { get; private set; }

        /// <summary>
        /// Returns the Balance Drawdown in pips
        /// </summary>
        public int BalanceDrawdown(int bar)
        {
            return _balanceDrawdown[bar];
        }

        /// <summary>
        /// Returns the Equity Drawdown in pips
        /// </summary>
        public int EquityDrawdown(int bar)
        {
            return _equityDrawdown[bar];
        }

        /// <summary>
        /// Returns the Balance Drawdown in currency
        /// </summary>
        public double MoneyBalanceDrawdown(int bar)
        {
            return _balanceDrawdown[bar] * DataSet.InstrProperties.Point * DataSet.InstrProperties.LotSize / AccountExchangeRate(DataSet.Close[bar]);
        }

        /// <summary>
        /// Returns the Equity Drawdown in currency.
        /// </summary>
        public double MoneyEquityDrawdown(int bar)
        {
            return _equityDrawdown[bar] * DataSet.InstrProperties.Point * DataSet.InstrProperties.LotSize / AccountExchangeRate(DataSet.Close[bar]);
        }

        /// <summary>
        /// Calculates the account statistics.
        /// </summary>
        public void CalculateAccountStats()
        {
            _maxBalance = 0;
            _minBalance = 0;
            _maxEquity  = 0;
            _minEquity  = 0;
            _maxEquityDrawdown = 0;
            _maxDrawdown       = 0;

            MaxMoneyBalance = Configs.InitialAccount;
            _minMoneyBalance = Configs.InitialAccount;
            MaxMoneyEquity  = Configs.InitialAccount;
            MinMoneyEquity  = Configs.InitialAccount;
            MaxMoneyEquityDrawdown = 0;
            MaxMoneyDrawdown       = 0;

            _barsInPosition    = 0;
            _grossProfit       = 0;
            _grossLoss         = 0;
            GrossMoneyProfit  = 0;
            GrossMoneyLoss    = 0;
            TotalChargedSpread     = 0;
            TotalChargedRollOver   = 0;
            TotalChargedCommission = 0;
            TotalChargedSlippage   = 0;
            TotalChargedMoneySpread     = 0;
            TotalChargedMoneyRollOver   = 0;
            TotalChargedMoneyCommission = 0;
            TotalChargedMoneySlippage   = 0;
            AmbiguousBars     = 0;
            _balanceDrawdown  = new int[DataSet.Bars];
            _equityDrawdown   = new int[DataSet.Bars];

            _maxBalanceDate       = DataSet.Time[0];
            _minBalanceDate       = DataSet.Time[0];
            _maxMoneyBalanceDate  = DataSet.Time[0];
            _minMoneyBalanceDate  = DataSet.Time[0];
            _maxDrawdownDate      = DataSet.Time[0];
            _maxMoneyDrawdownDate = DataSet.Time[0];

            EquityPercentDrawdown      = 100;
            _maxMoneyDrawdownPercent    = 0;
            MoneyEquityPercentDrawdown = 0;
            WinLossRatio               = 0;

            WinningTrades = 0;
            LosingTrades  = 0;
            _totalTrades   = 0;
            TestedDays    = 0;

            for (int bar = Strategy.FirstBar; bar < DataSet.Bars; bar++)
            {
                // Balance
                double balance = _session[bar].Summary.Balance;
                if (balance > _maxBalance)
                {
                    _maxBalance = balance;
                    _maxBalanceDate = DataSet.Time[bar];
                }
                if (balance < _minBalance)
                {
                    _minBalance = balance;
                    _minBalanceDate = DataSet.Time[bar];
                }

                // Money Balance
                double moneyBalance = _session[bar].Summary.MoneyBalance;
                if (moneyBalance > MaxMoneyBalance)
                {
                    MaxMoneyBalance = moneyBalance;
                    _maxMoneyBalanceDate = DataSet.Time[bar];
                }
                if (moneyBalance < _minMoneyBalance)
                {
                    _minMoneyBalance = moneyBalance;
                    _minMoneyBalanceDate = DataSet.Time[bar];
                }

                // Equity
                double equity = _session[bar].Summary.Equity;
                if (equity > _maxEquity) _maxEquity = equity;
                if (equity < _minEquity) _minEquity = equity;

                // Money Equity
                double moneyEquity = _session[bar].Summary.MoneyEquity;
                if (moneyEquity > MaxMoneyEquity) MaxMoneyEquity = moneyEquity;
                if (moneyEquity < MinMoneyEquity) MinMoneyEquity = moneyEquity;

                // Maximum Drawdown
                if (_maxBalance - balance > _maxDrawdown)
                {
                    _maxDrawdown = _maxBalance - balance;
                    _maxDrawdownDate = DataSet.Time[bar];
                }

                // Maximum Equity Drawdown
                if (_maxEquity - equity > _maxEquityDrawdown)
                {
                    _maxEquityDrawdown = _maxEquity - equity;

                    // In percents
                    if (_maxEquity > 0)
                        EquityPercentDrawdown = 100 * (_maxEquityDrawdown / _maxEquity);
                }

                // Maximum Money Drawdown
                if (MaxMoneyBalance - MoneyBalance(bar) > MaxMoneyDrawdown)
                {
                    MaxMoneyDrawdown        = MaxMoneyBalance - MoneyBalance(bar);
                    _maxMoneyDrawdownPercent = 100 * (MaxMoneyDrawdown / MaxMoneyBalance);
                    _maxMoneyDrawdownDate    = DataSet.Time[bar];
                }

                // Maximum Money Equity Drawdown
                if (MaxMoneyEquity - MoneyEquity(bar) > MaxMoneyEquityDrawdown)
                {
                    MaxMoneyEquityDrawdown = MaxMoneyEquity - MoneyEquity(bar);

                    // Maximum Money Equity Drawdown in percents
                    if (100 * MaxMoneyEquityDrawdown / MaxMoneyEquity > MoneyEquityPercentDrawdown)
                        MoneyEquityPercentDrawdown = 100 * (MaxMoneyEquityDrawdown / MaxMoneyEquity);
                }

                // Drawdown
                _balanceDrawdown[bar] = (int)Math.Round((_maxBalance - balance));
                _equityDrawdown[bar]  = (int)Math.Round((_maxEquity  - equity));

                // DS.Bars in position
                if (_session[bar].Positions > 0)
                    _barsInPosition++;

                // Bar interpolation evaluation
                if (_session[bar].BacktestEval == BacktestEval.Ambiguous)
                {
                    AmbiguousBars++;
                }

                // Margin Call bar
                if (!Configs.TradeUntilMarginCall && MarginCallBar == 0 && _session[bar].Summary.FreeMargin < 0)
                    MarginCallBar = bar;
            }

            for (int pos = 0; pos < PositionsTotal; pos++)
            {   // Charged fees
                Position position = PosFromNumb(pos);
                TotalChargedSpread          += position.Spread;
                TotalChargedRollOver        += position.Rollover;
                TotalChargedCommission      += position.Commission;
                TotalChargedSlippage        += position.Slippage;
                TotalChargedMoneySpread     += position.MoneySpread;
                TotalChargedMoneyRollOver   += position.MoneyRollover;
                TotalChargedMoneyCommission += position.MoneyCommission;
                TotalChargedMoneySlippage   += position.MoneySlippage;

                // Winning losing trades.
                if (position.Transaction == Transaction.Close  ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                {
                    if (position.ProfitLoss > 0)
                    {
                        _grossProfit      += position.ProfitLoss;
                        GrossMoneyProfit += position.MoneyProfitLoss;
                        WinningTrades++;
                    }
                    else if (position.ProfitLoss < 0)
                    {
                        _grossLoss      += position.ProfitLoss;
                        GrossMoneyLoss += position.MoneyProfitLoss;
                        LosingTrades++;
                    }
                    _totalTrades++;
                }
            }

            WinLossRatio = WinningTrades / Math.Max((LosingTrades + WinningTrades), 1.0);

            ExecutedOrders = 0;
            TradedLots = 0;
            for (int ord = 0; ord < SentOrders; ord++)
            {
                if (OrdFromNumb(ord).OrdStatus == OrderStatus.Executed)
                {
                    ExecutedOrders++;
                    TradedLots += OrdFromNumb(ord).OrdLots;
                }
            }

            TestedDays = (DataSet.Time[DataSet.Bars - 1] - DataSet.Time[Strategy.FirstBar]).Days;
            if (TestedDays < 1)
                TestedDays = 1;

            if (Configs.AccountInMoney)
                GenerateAccountStatsInMoney();
            else
                GenerateAccountStats();

            if (Configs.AdditionalStatistics)
            {
                CalculateAdditionalStats();

                if (Configs.AccountInMoney)
                    SetAdditionalMoneyStats();
                else
                    SetAdditionalStats();
            }
        }

        /// <summary>
        /// Generate the Account Statistics in currency.
        /// </summary>
        void GenerateAccountStatsInMoney()
        {
            AccountStatsParam = new[]
            {
                Language.T("Intrabar scanning"),
                Language.T("Interpolation method"),
                Language.T("Ambiguous bars"),
                Language.T("Profit per day"),
                Language.T("Tested bars"),
                Language.T("Initial account"),
                Language.T("Account balance"),
                Language.T("Minimum account"),
                Language.T("Maximum account"),
                Language.T("Maximum drawdown"),
                Language.T("Max equity drawdown"),
                Language.T("Max equity drawdown"),
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Sent orders"),
                Language.T("Executed orders"),
                Language.T("Traded lots"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Time in position"),
                Language.T("Charged spread"),
                Language.T("Charged rollover"),
                Language.T("Charged commission"),
                Language.T("Charged slippage"),
                Language.T("Total charges"),
                Language.T("Balance without charges"),
                Language.T("Account exchange rate")
            };

            string unit = " " + Configs.AccountCurrency;

            AccountStatsValue = new string[28];
            AccountStatsValue[0]  = IsScanPerformed ? Language.T("Accomplished") : Language.T("Not accomplished");
            AccountStatsValue[1]  = InterpolationMethodShortToString();
            AccountStatsValue[2]  = AmbiguousBars.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[3]  = MoneyProfitPerDay.ToString("F2") + unit;
            AccountStatsValue[4]  = (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[5]  = Configs.InitialAccount.ToString("F2") + unit;
            AccountStatsValue[6]  = NetMoneyBalance.ToString("F2")  + unit;
            AccountStatsValue[7]  = MinMoneyBalance.ToString("F2")  + unit;
            AccountStatsValue[8]  = MaxMoneyBalance.ToString("F2")  + unit;
            AccountStatsValue[9]  = MaxMoneyDrawdown.ToString("F2") + unit;
            AccountStatsValue[10] = MaxMoneyEquityDrawdown.ToString("F2") + unit;
            AccountStatsValue[11] = MoneyEquityPercentDrawdown.ToString("F2") + " %";
            AccountStatsValue[12] = GrossMoneyProfit.ToString("F2") + unit;
            AccountStatsValue[13] = GrossMoneyLoss.ToString("F2")   + unit;
            AccountStatsValue[14] = SentOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[15] = ExecutedOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[16] = TradedLots.ToString("F2");
            AccountStatsValue[17] = WinningTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[18] = LosingTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[19] = WinLossRatio.ToString("F2");
            AccountStatsValue[20] = TimeInPosition + " %";
            AccountStatsValue[21] = TotalChargedMoneySpread.ToString("F2")     + unit;
            AccountStatsValue[22] = TotalChargedMoneyRollOver.ToString("F2")   + unit;
            AccountStatsValue[23] = TotalChargedMoneyCommission.ToString("F2") + unit;
            AccountStatsValue[24] = TotalChargedMoneySlippage.ToString("F2")   + unit;
            AccountStatsValue[25] = (TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission + TotalChargedMoneySlippage).ToString("F2") + unit;
            AccountStatsValue[26] = (NetMoneyBalance + TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission + TotalChargedMoneySlippage).ToString("F2") + unit;

            if (DataSet.InstrProperties.PriceIn == Configs.AccountCurrency)
                AccountStatsValue[27] = Language.T("Not used");
            else if (DataSet.InstrProperties.InstrType == InstrumetType.Forex && DataSet.Symbol.StartsWith(Configs.AccountCurrency))
                AccountStatsValue[27] = Language.T("Deal price");
            else if (Configs.AccountCurrency == "USD")
                AccountStatsValue[27] = DataSet.InstrProperties.RateToUSD.ToString("F4");
            else if (Configs.AccountCurrency == "EUR")
                AccountStatsValue[27] = DataSet.InstrProperties.RateToEUR.ToString("F4");

            AccountStatsFlags = new bool[28];
            AccountStatsFlags[0] = AmbiguousBars > 0 && !IsScanPerformed;
            AccountStatsFlags[1] = InterpolationMethod != InterpolationMethod.Pessimistic;
            AccountStatsFlags[2] = AmbiguousBars > 0;
            AccountStatsFlags[6] = NetMoneyBalance < Configs.InitialAccount;
            AccountStatsFlags[9] = MaxDrawdown > Configs.InitialAccount / 2;
        }

        /// <summary>
        /// Generate the Account Statistics in pips.
        /// </summary>
        void GenerateAccountStats()
        {
            AccountStatsParam = new[]
            {
                Language.T("Intrabar scanning"),
                Language.T("Interpolation method"),
                Language.T("Ambiguous bars"),
                Language.T("Profit per day"),
                Language.T("Tested bars"),
                Language.T("Account balance"),
                Language.T("Minimum account"),
                Language.T("Maximum account"),
                Language.T("Maximum drawdown"),
                Language.T("Max equity drawdown"),
                Language.T("Max equity drawdown"),
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Sent orders"),
                Language.T("Executed orders"),
                Language.T("Traded lots"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Time in position"),
                Language.T("Charged spread"),
                Language.T("Charged rollover"),
                Language.T("Charged commission"),
                Language.T("Charged slippage"),
                Language.T("Total charges"),
                Language.T("Balance without charges")
            };

            string unit = " " + Language.T("pips");
            AccountStatsValue = new string[26];
            AccountStatsValue[0]  = IsScanPerformed ? Language.T("Accomplished") : Language.T("Not accomplished");
            AccountStatsValue[1]  = InterpolationMethodShortToString();
            AccountStatsValue[2]  = AmbiguousBars.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[3]  = ProfitPerDay + unit;
            AccountStatsValue[4]  = (DataSet.Bars - Strategy.FirstBar).ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[5]  = NetBalance  + unit;
            AccountStatsValue[6]  = MinBalance  + unit;
            AccountStatsValue[7]  = MaxBalance  + unit;
            AccountStatsValue[8]  = MaxDrawdown + unit;
            AccountStatsValue[9]  = MaxEquityDrawdown + unit;
            AccountStatsValue[10] = EquityPercentDrawdown.ToString("F2") + " %";
            AccountStatsValue[11] = GrossProfit + unit;
            AccountStatsValue[12] = GrossLoss   + unit;
            AccountStatsValue[13] = SentOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[14] = ExecutedOrders.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[15] = TradedLots.ToString("F2");
            AccountStatsValue[16] = WinningTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[17] = LosingTrades.ToString(CultureInfo.InvariantCulture);
            AccountStatsValue[18] = ((float)WinningTrades/(WinningTrades + LosingTrades)).ToString("F2");
            AccountStatsValue[19] = TimeInPosition + " %";
            AccountStatsValue[20] = Math.Round(TotalChargedSpread) + unit;
            AccountStatsValue[21] = Math.Round(TotalChargedRollOver) + unit;
            AccountStatsValue[22] = Math.Round(TotalChargedCommission) + unit;
            AccountStatsValue[23] = TotalChargedSlippage.ToString("F2") + unit;
            AccountStatsValue[24] = Math.Round(TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage) + unit;
            AccountStatsValue[25] = Math.Round(NetBalance + TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage) + unit;

            AccountStatsFlags = new bool[26];
            AccountStatsFlags[0] = AmbiguousBars > 0 && !IsScanPerformed;
            AccountStatsFlags[1] = InterpolationMethod != InterpolationMethod.Pessimistic;
            AccountStatsFlags[2] = AmbiguousBars > 0;
            AccountStatsFlags[5] = NetBalance < 0;
            AccountStatsFlags[8] = MaxDrawdown > 500;
        }
    }
}
