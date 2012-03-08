// Strategy Analyzer - OverOptimization Calculations
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    public sealed partial class OverOptimization
    {
        private int _computedCycles; // Currently completed cycles.
        private int _countStratParams;
        private int _cycles; // Count of the cycles.
        private int _deviationSteps;

        private string _listParametersName;

        private string _pathReportFile;

        private int _progressPercent; // Reached progress in %.
        private OverOptimizationDataTable _tableParameters;
        private OverOptimizationDataTable[] _tableReport;
        private int _tablesCount; // The count of data tables.

        private Backtester Backtester { get; set; }


        /// <summary>
        /// Counts the numeric parameters of the strategy.
        /// </summary>
        private void CountStrategyParams()
        {
            _countStratParams = 0;
            for (int slot = 0; slot < Backtester.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        _countStratParams++;
        }

        /// <summary>
        /// Sets table with parameter values.
        /// </summary>
        private void SetParametersValues(int percentDeviation, int countParam)
        {
            _listParametersName = "Index" + Configs.ColumnSeparator + "Parameter name" + Environment.NewLine;
            _countStratParams = 0;
            _cycles = 0;
            _deviationSteps = 2*percentDeviation + 1;

            _tableParameters = new OverOptimizationDataTable(percentDeviation, countParam)
                                   {Name = "Values of the Parameters"};

            for (int slot = 0; slot < Backtester.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled && _countStratParams < countParam)
                    {
                        NumericParam currentParam = Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam];
                        double minVal = currentParam.Min;
                        double maxVal = currentParam.Max;
                        int point = currentParam.Point;
                        double originalValue = currentParam.Value;
                        double deltaStep = originalValue/100.0;

                        for (int p = 0; p < _deviationSteps; p++)
                        {
                            int index = percentDeviation - p;
                            double value = originalValue + index*deltaStep;
                            value = Math.Round(value, point);

                            if (index == 0)
                                value = originalValue;
                            if (value < minVal)
                                value = minVal;
                            if (value > maxVal)
                                value = maxVal;

                            _tableParameters.SetData(index, _countStratParams, value);
                            _cycles++;
                        }

                        _listParametersName += (_countStratParams + 1) + Configs.ColumnSeparator + currentParam.Caption +
                                               Environment.NewLine;
                        _countStratParams++;
                    }

            for (int prm = _countStratParams; prm < countParam; prm++)
                _listParametersName += (prm + 1) + Environment.NewLine;
            _listParametersName += Environment.NewLine;
        }

        /// <summary>
        /// Calculates Data Tables.
        /// </summary>
        private void CalculateStatsTables(int percentDeviation, int countParam)
        {
            string unit = " " + Configs.AccountCurrency;

            var tableNames = new[]
                                 {
                                     Language.T("Account balance") + unit,
                                     Language.T("Profit per day") + unit,
                                     Language.T("Maximum drawdown") + unit,
                                     Language.T("Gross profit") + unit,
                                     Language.T("Gross loss") + unit,
                                     Language.T("Executed orders"),
                                     Language.T("Traded lots"),
                                     Language.T("Time in position") + " %",
                                     Language.T("Sent orders"),
                                     Language.T("Charged spread") + unit,
                                     Language.T("Charged rollover") + unit,
                                     Language.T("Winning trades"),
                                     Language.T("Losing trades"),
                                     Language.T("Win/loss ratio"),
                                     Language.T("Max equity drawdown") + " %"
                                 };

            _tablesCount = tableNames.Length;
            _tableReport = new OverOptimizationDataTable[_tablesCount];

            for (int t = 0; t < tableNames.Length; t++)
            {
                _tableReport[t] = new OverOptimizationDataTable(percentDeviation, countParam) {Name = tableNames[t]};
            }

            int parNumber = 0;
            bool isBGWorkCanceled = false;
            for (int slot = 0; slot < Backtester.Strategy.Slots && !isBGWorkCanceled; slot++)
                for (int numParam = 0; numParam < 6 && !isBGWorkCanceled; numParam++)
                    if (Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled && parNumber < countParam)
                    {
                        for (int index = percentDeviation; index >= -percentDeviation && !isBGWorkCanceled; index--)
                        {
                            isBGWorkCanceled = _bgWorker.CancellationPending;
                            Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Value = _tableParameters.GetData(
                                index, parNumber);

                            CalculateIndicator(slot);
                            //Backtester.Calculate();
                            //Backtester.CalculateAccountStats();

                            var statValues = new[]
                                                 {
                                                     Backtester.NetMoneyBalance,
                                                     Backtester.MoneyProfitPerDay,
                                                     Backtester.MaxMoneyDrawdown,
                                                     Backtester.GrossMoneyProfit,
                                                     Backtester.GrossMoneyLoss,
                                                     Backtester.ExecutedOrders,
                                                     Backtester.TradedLots,
                                                     Backtester.TimeInPosition,
                                                     Backtester.SentOrders,
                                                     Backtester.TotalChargedMoneySpread,
                                                     Backtester.TotalChargedMoneyRollOver,
                                                     Backtester.WinningTrades,
                                                     Backtester.LosingTrades,
                                                     Backtester.WinLossRatio,
                                                     Backtester.MoneyEquityPercentDrawdown
                                                 };

                            for (int tn = 0; tn < _tablesCount; tn++)
                                _tableReport[tn].SetData(index, parNumber, statValues[tn]);

                            // Report progress as a percentage of the total task.
                            _computedCycles++;
                            int percentComplete = Math.Min(100*_computedCycles/_cycles, 100);
                            if (percentComplete > _progressPercent)
                            {
                                _progressPercent = percentComplete;
                                _bgWorker.ReportProgress(percentComplete);
                            }
                        }

                        // Set default value
                        Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Value = _tableParameters.GetData(0,
                                                                                                              parNumber);
                        CalculateIndicator(slot);
                        parNumber++;
                    }
        }

        /// <summary>
        /// Calculates the indicator in the designated slot.
        /// </summary>
        private void CalculateIndicator(int slot)
        {
            IndicatorParam ip = Backtester.Strategy.Slot[slot].IndParam;

            Indicator indicator = IndicatorStore.ConstructIndicator(ip.IndicatorName, ip.SlotType);

            // List parameters
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index = ip.ListParam[i].Index;
                indicator.IndParam.ListParam[i].Text = ip.ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ip.ListParam[i].Enabled;
            }

            // Numeric parameters
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value = ip.NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = ip.NumParam[i].Enabled;
            }

            // Check parameters
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = ip.CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            indicator.Calculate(ip.SlotType);

            // Sets Backtester.Strategy
            Backtester.Strategy.Slot[slot].IndicatorName = indicator.IndicatorName;
            Backtester.Strategy.Slot[slot].IndParam = indicator.IndParam;
            Backtester.Strategy.Slot[slot].Component = indicator.Component;
            Backtester.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Backtester.Strategy.Slot[slot].SpecValue = indicator.SpecialValues;
            Backtester.Strategy.Slot[slot].MinValue = indicator.SeparatedChartMinValue;
            Backtester.Strategy.Slot[slot].MaxValue = indicator.SeparatedChartMaxValue;
            Backtester.Strategy.Slot[slot].IsDefined = true;

            // Searches the indicators' components to determine the Data.FirstBar 
            Backtester.Strategy.SetFirstBar();
        }

        /// <summary>
        /// Generates the Over-optimization report.
        /// </summary>
        private string GenerateReport()
        {
            string report = _listParametersName + _tableParameters.DataToString();
            foreach (OverOptimizationDataTable table in _tableReport)
                report += table.DataToString();

            return report;
        }

        /// <summary>
        /// Saves the report in a file.
        /// </summary>
        private void SaveReport(string report)
        {
            string pathReport;
            string partilPath = Backtester.Strategy.StrategyPath.Replace(".xml", "");
            int reportIndex = 0;
            do
            {
                reportIndex++;
                pathReport = partilPath + "-Over-optimization_Report-" + reportIndex + ".csv";
            } while (File.Exists(pathReport));

            try
            {
                using (var outfile = new StreamWriter(pathReport))
                {
                    outfile.Write(report);
                    _pathReportFile = pathReport;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}