// Strategy Optimizer - Report
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer
    /// </summary>
    public sealed partial class Optimizer
    {
        /// <summary>
        /// Prepares the report string.
        /// </summary>
        private void InitReport()
        {
            // Prepare report
            _sbReport = new StringBuilder();
            _sbReport.Append(
                "Net Balance" + "," +
                "Max Drawdown" + "," +
                "Gross Profit" + "," +
                "Gross Loss" + "," +
                "Executed Orders" + "," +
                "Traded Lots" + "," +
                "Time in Position" + "," +
                "Sent Orders" + "," +
                "Total Charged Spread" + "," +
                "Total Charged Rollover" + "," +
                "Win / Loss Ratio" + "," +
                "Equity Percent Drawdown" + ",");

            if (_backtester.Strategy.UsePermanentSL)
                _sbReport.Append("Permanent SL" + ",");
            if (_backtester.Strategy.UsePermanentTP)
                _sbReport.Append("Permanent TP" + ",");
            if (_backtester.Strategy.UseBreakEven)
                _sbReport.Append("Break Even" + ",");

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (_backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        _sbReport.Append(_backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Caption + ",");

            _sbReport.AppendLine();
        }

        /// <summary>
        ///  Fills a line to the report.
        /// </summary>
        private void FillInReport()
        {
            _sbReport.Append(
                _backtester.NetMoneyBalance.ToString("F2") + "," +
                _backtester.MaxMoneyDrawdown.ToString("F2") + "," +
                _backtester.GrossMoneyProfit.ToString("F2") + "," +
                _backtester.GrossMoneyLoss.ToString("F2") + "," +
                _backtester.ExecutedOrders + "," +
                _backtester.TradedLots + "," +
                _backtester.TimeInPosition + "," +
                _backtester.SentOrders + "," +
                _backtester.TotalChargedMoneySpread.ToString("F2") + "," +
                _backtester.TotalChargedMoneyRollOver.ToString("F2") + "," +
                _backtester.WinLossRatio.ToString("F2") + "," +
                _backtester.MoneyEquityPercentDrawdown.ToString("F2") + ",");

            if (_backtester.Strategy.UsePermanentSL)
                _sbReport.Append(_backtester.Strategy.PermanentSL + ",");
            if (_backtester.Strategy.UsePermanentTP)
                _sbReport.Append(_backtester.Strategy.PermanentTP + ",");
            if (_backtester.Strategy.UseBreakEven)
                _sbReport.Append(_backtester.Strategy.BreakEven + ",");

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (_backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        _sbReport.Append(_backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Value + ",");

            _sbReport.AppendLine();
        }

        /// <summary>
        /// Saves the report in a file.
        /// </summary>
        private void SaveReport()
        {
            string pathReport;
            string partilaPath = _backtester.Strategy.StrategyPath.Replace(".xml", "");
            int reportIndex = 0;
            do
            {
                reportIndex++;
                pathReport = partilaPath + "-Report-" + reportIndex + ".csv";
            } while (File.Exists(pathReport));

            try
            {
                using (var outfile = new StreamWriter(pathReport))
                {
                    outfile.Write(_sbReport.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}