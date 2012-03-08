// Exporter class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Exporter
    {
        private readonly Backtester _backtester;

        public Exporter(Backtester backtester)
        {
            _backtester = backtester;
        }

        /// <summary>
        /// Exports the data
        /// </summary>
        public void ExportCSVData()
        {
            string ff = Data.FF; // Format modifier to print float numbers
            string df = Data.DF; // Format modifier to print date
            var sb = new StringBuilder();
            for (int bar = 0; bar < Data.DataSet.Bars; bar++)
            {
                sb.Append(Data.DataSet.Time[bar].ToString(df) + "\t");
                sb.Append(Data.DataSet.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.DataSet.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.High[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Volume[bar] + Environment.NewLine);
            }

            string fileName = Data.Symbol + (int)Data.DataSet.Period + ".csv";
            SaveData(fileName, sb);
        }

        /// <summary>
        /// Exports the data
        /// </summary>
        public void ExportDataOnly()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            string ff = Data.FF; // Format modifier to print float numbers
            string df = Data.DF; // Format modifier to print date
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + _backtester.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + Environment.NewLine);
            sb.Append("Number of bars: " + Data.DataSet.Bars + Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Date" + "\t");
            sb.Append("Hour" + "\t");
            sb.Append("Open" + "\t");
            sb.Append("High" + "\t");
            sb.Append("Low" + "\t");
            sb.Append("Close" + "\t");
            sb.Append("Volume" + Environment.NewLine);

            for (int bar = 0; bar < Data.DataSet.Bars; bar++)
            {
                sb.Append(Data.DataSet.Time[bar].ToString(df) + "\t");
                sb.Append(Data.DataSet.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.DataSet.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.High[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Volume[bar] + Environment.NewLine);
            }

            string fileName = _backtester.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.DataSet.Period.ToString();
            SaveData(fileName, sb);
        }

        /// <summary>
        /// Exports data and indicators values
        /// </summary>
        public void ExportIndicators()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            string ff = Data.FF; // Format modifier to print float numbers
            string df = Data.DF; // Format modifier to print date
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + _backtester.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + Environment.NewLine);
            sb.Append("Number of bars: " + Data.DataSet.Bars);

            sb.Append("\t\t\t\t\t\t\t\t");

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                string strSlotType = "";
                switch (_backtester.Strategy.Slot[slot].SlotType)
                {
                    case SlotTypes.Open:
                        strSlotType = "Opening point of the position";
                        break;
                    case SlotTypes.OpenFilter:
                        strSlotType = "Opening logic condition";
                        break;
                    case SlotTypes.Close:
                        strSlotType = "Closing point of the position";
                        break;
                    case SlotTypes.CloseFilter:
                        strSlotType = "Closing logic condition";
                        break;
                }

                sb.Append(strSlotType + "\t");
                for (int i = 0; i < _backtester.Strategy.Slot[slot].Component.Length; i++)
                    sb.Append("\t");
            }
            sb.Append(Environment.NewLine);


            // Names of the indicator components
            sb.Append("\t\t\t\t\t\t\t\t");

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                Indicator indicator = IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                                        _backtester.Strategy.Slot[slot].SlotType);

                sb.Append(indicator + "\t");
                for (int i = 0; i < _backtester.Strategy.Slot[slot].Component.Length; i++)
                    sb.Append("\t");
            }
            sb.Append(Environment.NewLine);

            // Data
            sb.Append("Date" + "\t");
            sb.Append("Hour" + "\t");
            sb.Append("Open" + "\t");
            sb.Append("High" + "\t");
            sb.Append("Low" + "\t");
            sb.Append("Close" + "\t");
            sb.Append("Volume" + "\t");

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                sb.Append("\t");
                foreach (IndicatorComp comp in _backtester.Strategy.Slot[slot].Component)
                    sb.Append(comp.CompName + "\t");
            }
            sb.Append(Environment.NewLine);

            for (int bar = 0; bar < Data.DataSet.Bars; bar++)
            {
                sb.Append(Data.DataSet.Time[bar].ToString(df) + "\t");
                sb.Append(Data.DataSet.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.DataSet.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.High[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Volume[bar] + "\t");

                for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
                {
                    sb.Append("\t");
                    foreach (IndicatorComp comp in _backtester.Strategy.Slot[slot].Component)
                        sb.Append(comp.Value[bar] + "\t");
                }
                sb.Append(Environment.NewLine);
            }

            string fileName = _backtester.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.DataSet.Period;
            SaveData(fileName, sb);
        }

        /// <summary>
        /// Exports the bar summary
        /// </summary>
        public void ExportBarSummary()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            string ff = Data.FF; // Format modifier to print float numbers
            string df = Data.DF; // Format modifier to print date
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + _backtester.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in pips" + Environment.NewLine);

            sb.Append("Bar Numb\t");
            sb.Append("Date\t");
            sb.Append("Hour\t");
            sb.Append("Open\t");
            sb.Append("High\t");
            sb.Append("Low\t");
            sb.Append("Close\t");
            sb.Append("Volume\t");
            sb.Append("Direction\t");
            sb.Append("Lots\t");
            sb.Append("Transaction\t");
            sb.Append("Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Spread\t");
            sb.Append("Rollover\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append("Interpolation" + Environment.NewLine);

            for (int bar = 0; bar < Data.DataSet.Bars; bar++)
            {
                sb.Append((bar + 1) + "\t");
                sb.Append(Data.DataSet.Time[bar].ToString(df) + "\t");
                sb.Append(Data.DataSet.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.DataSet.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.High[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.DataSet.Volume[bar] + "\t");
                if (_backtester.IsPos(bar))
                {
                    sb.Append(_backtester.SummaryDir(bar) + "\t");
                    sb.Append(_backtester.SummaryLots(bar) + "\t");
                    sb.Append(_backtester.SummaryTrans(bar) + "\t");
                    sb.Append(_backtester.SummaryPrice(bar).ToString(ff) + "\t");
                    sb.Append(_backtester.ProfitLoss(bar) + "\t");
                    sb.Append(_backtester.FloatingPL(bar) + "\t");
                }
                else
                {
                    sb.Append("\t\t\t\t\t\t");
                }
                sb.Append(_backtester.ChargedSpread(bar) + "\t");
                sb.Append(_backtester.ChargedRollOver(bar) + "\t");
                sb.Append(_backtester.Balance(bar) + "\t");
                sb.Append(_backtester.Equity(bar) + "\t");
                sb.Append(_backtester.BackTestEvalToString(bar) + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = _backtester.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.DataSet.Period.ToString();
            SaveData(fileName, sb);
        }

        /// <summary>
        /// Exports the positions info
        /// </summary>
        public void ExportPositions(bool showTransfers)
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            string ff = Data.FF; // Format modifier to print float numbers
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + _backtester.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in pips" + Environment.NewLine);

            sb.Append("Pos Numb\t");
            sb.Append("Bar Numb\t");
            sb.Append("Bar Opening Time\t");
            sb.Append("Direction\t");
            sb.Append("Lots\t");
            sb.Append("Transaction\t");
            sb.Append("Order Price\t");
            sb.Append("Average Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append(Environment.NewLine);

            for (int iPos = 0; iPos < _backtester.PositionsTotal; iPos++)
            {
                Position position = _backtester.PosFromNumb(iPos);
                int bar = _backtester.PosCoordinates[iPos].Bar;

                if (!showTransfers && position.Transaction == Transaction.Transfer)
                    continue;

                sb.Append((position.PosNumb + 1) + "\t");
                sb.Append((bar + 1) + "\t");
                sb.Append(Data.DataSet.Time[bar] + "\t");
                sb.Append(position.PosDir + "\t");
                sb.Append(position.PosLots + "\t");
                sb.Append(position.Transaction + "\t");
                sb.Append(position.FormOrdPrice.ToString(ff) + "\t");
                sb.Append(position.PosPrice.ToString(ff) + "\t");
                sb.Append(Math.Round(position.ProfitLoss) + "\t");
                sb.Append(Math.Round(position.FloatingPL) + "\t");
                sb.Append(Math.Round(position.Balance) + "\t");
                sb.Append(Math.Round(position.Equity) + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = _backtester.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.DataSet.Period;
            SaveData(fileName, sb);
        }

        /// <summary>
        /// Exports the positions info in currency
        /// </summary>
        public void ExportPositionsInMoney(bool showTransfers)
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            string ff = Data.FF; // Format modifier to print float numbers
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + _backtester.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in " + Configs.AccountCurrency +
                      Environment.NewLine);

            sb.Append("Pos Numb\t");
            sb.Append("Bar Numb\t");
            sb.Append("Bar Opening Time\t");
            sb.Append("Direction\t");
            sb.Append("Amount\t");
            sb.Append("Transaction\t");
            sb.Append("Order Price\t");
            sb.Append("Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append(Environment.NewLine);

            for (int pos = 0; pos < _backtester.PositionsTotal; pos++)
            {
                Position position = _backtester.PosFromNumb(pos);
                int bar = _backtester.PosCoordinates[pos].Bar;

                if (!showTransfers && position.Transaction == Transaction.Transfer)
                    continue;

                sb.Append((position.PosNumb + 1) + "\t");
                sb.Append((bar + 1) + "\t");
                sb.Append(Data.DataSet.Time[bar] + "\t");
                sb.Append(position.PosDir + "\t");
                sb.Append((position.PosDir == PosDirection.Long ? "" : "-") +
                          (position.PosLots * Data.DataSet.InstrProperties.LotSize) + "\t");
                sb.Append(position.Transaction + "\t");
                sb.Append(position.FormOrdPrice.ToString(ff) + "\t");
                sb.Append(position.PosPrice.ToString(ff) + "\t");
                sb.Append(position.MoneyProfitLoss.ToString("F2") + "\t");
                sb.Append(position.MoneyFloatingPL.ToString("F2") + "\t");
                sb.Append(position.MoneyBalance.ToString("F2") + "\t");
                sb.Append(position.MoneyEquity.ToString("F2") + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = _backtester.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.DataSet.Period;
            SaveData(fileName, sb);
        }

        /// <summary>
        /// Exports histogram data as CSV file
        /// </summary>
        /// <param name="s">string</param>
        public void ExportHistogramData(string s)
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + _backtester.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + Environment.NewLine);
            sb.Append("Histogram Data");
            sb.Append(Environment.NewLine);

            sb.Append("Result" + "\t");
            sb.Append("Count" + "\t");
            sb.Append("Total" + Environment.NewLine);

            sb.Append(s);

            string fileName = _backtester.Strategy.StrategyName + "_HistogramData";
            SaveData(fileName, sb);
        }

        /// <summary>
        /// Saves the data file
        /// </summary>
        private void SaveData(string fileName, StringBuilder data)
        {
            var sfdExport = new SaveFileDialog
                                {
                                    AddExtension = true,
                                    InitialDirectory = Data.ProgramDir,
                                    Title = Language.T("Export"),
                                    FileName = fileName
                                };
            if (fileName.EndsWith(".csv"))
            {
                sfdExport.InitialDirectory = Data.OfflineDataDir;
                sfdExport.Filter = "FSB data (*.csv)|*.csv|All files (*.*)|*.*";
            }
            else
                sfdExport.Filter =
                    "Excel file (*.xls)|*.xls|FSB data (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (sfdExport.ShowDialog() != DialogResult.OK) return;
            try
            {
                var sw = new StreamWriter(sfdExport.FileName);
                sw.Write(data.ToString());
                sw.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}