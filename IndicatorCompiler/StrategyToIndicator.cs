// Export Strategy to Indicator Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    public static class StrategyToIndicator
    {
        private static Backtester _backtester;

        public static void ExportStrategyToIndicator(Backtester backtester)
        {
            _backtester = backtester;
            var sbLong = new StringBuilder();
            var sbShort = new StringBuilder();

            for (int bar = _backtester.Strategy.FirstBar; bar < _backtester.DataSet.Bars; bar++)
                for (int pos = 0; pos < _backtester.Positions(bar); pos++)
                {
                    if (_backtester.PosDir(bar, pos) == PosDirection.Long)
                        sbLong.AppendLine("				\"" + _backtester.DataSet.Time[bar] + "\",");

                    if (_backtester.PosDir(bar, pos) == PosDirection.Short)
                        sbShort.AppendLine("				\"" + _backtester.DataSet.Time[bar] + "\",");
                }

            string strategy = Resources.StrategyToIndicator;
            strategy = strategy.Replace("#MODIFIED#", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#INSTRUMENT#", _backtester.DataSet.Symbol);
            strategy = strategy.Replace("#BASEPERIOD#", Data.DataPeriodToString(_backtester.DataSet.Period));
            strategy = strategy.Replace("#STARTDATE#", _backtester.DataSet.Time[_backtester.Strategy.FirstBar].ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#ENDDATE#", _backtester.DataSet.Time[_backtester.DataSet.Bars - 1].ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#PERIODMINUTES#", ((int) _backtester.DataSet.Period).ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#LISTLONG#", sbLong.ToString());
            strategy = strategy.Replace("#LISTSHORT#", sbShort.ToString());

            var savedlg = new SaveFileDialog
                              {
                                  InitialDirectory = Data.SourceFolder,
                                  AddExtension = true,
                                  Title = Language.T("Custom Indicators"),
                                  Filter = Language.T("Custom Indicators") + " (*.cs)|*.cs"
                              };

            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                strategy = strategy.Replace("#INDICATORNAME#", Path.GetFileNameWithoutExtension(savedlg.FileName));
                var sw = new StreamWriter(savedlg.FileName);
                try
                {
                    sw.Write(strategy);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Language.T("Custom Indicators"));
                }
                finally
                {
                    sw.Close();
                }
            }
        }
    }
}