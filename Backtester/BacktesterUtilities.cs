// Backtester - Utilities
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder
// Website http://forexsb.com
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    public partial class Backtester
    {

        /// <summary>
        /// Calculates the required margin.
        /// </summary>
        public double RequiredMargin(double lots, int bar)
        {
            double amount = lots * DataSet.InstrProperties.LotSize;
            double exchangeRate = DataSet.Close[bar] / AccountExchangeRate(DataSet.Close[bar]);
            double requiredMargin = amount * exchangeRate / Configs.Leverage;

            return requiredMargin;
        }

        /// <summary>
        /// Calculates the trading size in normalized lots.
        /// </summary>
        public double TradingSize(double size, int bar)
        {
            if (Strategy.UseAccountPercentEntry)
            {
                double maxMargin = _session[bar].Summary.MoneyEquity * size / 100.0;
                double exchangeRate = DataSet.Close[bar] / AccountExchangeRate(DataSet.Close[bar]);
                size = maxMargin * Configs.Leverage / (exchangeRate * DataSet.InstrProperties.LotSize);
            }

            return NormalizeEntryLots(size);
        }

        /// <summary>
        /// Normalizes an entry order's size.
        /// </summary>
        public double NormalizeEntryLots(double lots)
        {
            const double minlot = 0.01;
            double maxlot = Strategy.MaxOpenLots;
            const double lotstep = 0.01;

            if (lots <= 0)
                return (0);

            var steps = (int)Math.Round((lots - minlot) / lotstep);
            lots = minlot + steps * lotstep;

            if (lots <= minlot)
                return (minlot);

            if (lots >= maxlot)
                return (maxlot);

            return lots;
        }

        /// <summary>
        /// Account Exchange Rate.
        /// </summary>
        public double AccountExchangeRate(double price)
        {
            double exchangeRate = 0;

            if (DataSet.InstrProperties.PriceIn == Configs.AccountCurrency)
                exchangeRate = 1;
            else if (DataSet.InstrProperties.InstrType == InstrumetType.Forex && DataSet.Symbol.StartsWith(Configs.AccountCurrency))
                exchangeRate = price;
            else if (Configs.AccountCurrency == "USD")
                exchangeRate = DataSet.InstrProperties.RateToUSD;
            else if (Configs.AccountCurrency == "EUR")
                exchangeRate = DataSet.InstrProperties.RateToEUR;

            return exchangeRate;
        }

        /// <summary>
        /// Calculates the commission in pips.
        /// </summary>
        private double Commission(double lots, double price, bool isPosClosing)
        {
            double commission = 0;

            if (DataSet.InstrProperties.Commission < 0.00001)
                return 0;

            if (DataSet.InstrProperties.CommissionTime == CommissionTime.open && isPosClosing)
                return 0; // Commission is not applied to the position closing

            if (DataSet.InstrProperties.CommissionType == CommissionType.pips)
                commission = DataSet.InstrProperties.Commission;

            else if (DataSet.InstrProperties.CommissionType == CommissionType.percents)
            {
                commission = (price / DataSet.InstrProperties.Point) * (DataSet.InstrProperties.Commission / 100);
                return commission;
            }

            else if (DataSet.InstrProperties.CommissionType == CommissionType.money)
                commission = DataSet.InstrProperties.Commission / (DataSet.InstrProperties.Point * DataSet.InstrProperties.LotSize);

            if (DataSet.InstrProperties.CommissionScope == CommissionScope.lot)
                commission *= lots; // Commission per lot

            return commission;
        }

        /// <summary>
        /// Calculates the commission in currency.
        /// </summary>
        public double CommissionInMoney(double lots, double price, bool isPosClosing)
        {
            double commission = 0;

            if (DataSet.InstrProperties.Commission < 0.00001)
                return 0;

            if (DataSet.InstrProperties.CommissionTime == CommissionTime.open && isPosClosing)
                return 0; // Commission is not applied to the position closing

            if (DataSet.InstrProperties.CommissionType == CommissionType.pips)
                commission = DataSet.InstrProperties.Commission * DataSet.InstrProperties.Point * DataSet.InstrProperties.LotSize / AccountExchangeRate(price);

            else if (DataSet.InstrProperties.CommissionType == CommissionType.percents)
            {
                commission = lots * DataSet.InstrProperties.LotSize * price * (DataSet.InstrProperties.Commission / 100) / AccountExchangeRate(price);
                return commission;
            }

            else if (DataSet.InstrProperties.CommissionType == CommissionType.money)
                commission = DataSet.InstrProperties.Commission / AccountExchangeRate(price);

            if (DataSet.InstrProperties.CommissionScope == CommissionScope.lot)
                commission *= lots; // Commission per lot

            return commission;
        }

        /// <summary>
        /// Calculates the rollover fee in currency.
        /// </summary>
        public double RolloverInMoney(PosDirection posDir, double lots, int daysRollover, double price)
        {
            double point = DataSet.InstrProperties.Point;
            int lotSize = DataSet.InstrProperties.LotSize;
            double swapLongPips = 0; // Swap long in pips
            double swapShortPips = 0; // Swap short in pips
            if (DataSet.InstrProperties.SwapType == CommissionType.pips)
            {
                swapLongPips = DataSet.InstrProperties.SwapLong;
                swapShortPips = DataSet.InstrProperties.SwapShort;
            }
            else if (DataSet.InstrProperties.SwapType == CommissionType.percents)
            {
                swapLongPips = (price / point) * (0.01 * DataSet.InstrProperties.SwapLong / 365);
                swapShortPips = (price / point) * (0.01 * DataSet.InstrProperties.SwapShort / 365);
            }
            else if (DataSet.InstrProperties.SwapType == CommissionType.money)
            {
                swapLongPips = DataSet.InstrProperties.SwapLong / (point * lotSize);
                swapShortPips = DataSet.InstrProperties.SwapShort / (point * lotSize);
            }

            double rollover = lots * lotSize * (posDir == PosDirection.Long ? swapLongPips : -swapShortPips) * point * daysRollover / AccountExchangeRate(price);

            return rollover;
        }

        /// <summary>
        /// Converts pips to money.
        /// </summary>
        public double PipsToMoney(double pips, int bar)
        {
            return pips * DataSet.InstrProperties.Point * DataSet.InstrProperties.LotSize / AccountExchangeRate(DataSet.Close[bar]);
        }

        /// <summary>
        /// Converts money to pips.
        /// </summary>
        public double MoneyToPips(double money, int bar)
        {
            return money * AccountExchangeRate(DataSet.Close[bar]) / (DataSet.InstrProperties.Point * DataSet.InstrProperties.LotSize);
        }
    }
}
