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
        private static Backtester Backtester { get; set; }

        public static void ExportStrategyToIndicator()
        {
            var sbLong = new StringBuilder();
            var sbShort = new StringBuilder();

            for (int bar = Backtester.Strategy.FirstBar; bar < Data.DataSet.Bars; bar++)
                for (int pos = 0; pos < Backtester.Positions(bar); pos++)
                {
                    if (Backtester.PosDir(bar, pos) == PosDirection.Long)
                        sbLong.AppendLine("				\"" + Data.DataSet.Time[bar] + "\",");

                    if (Backtester.PosDir(bar, pos) == PosDirection.Short)
                        sbShort.AppendLine("				\"" + Data.DataSet.Time[bar] + "\",");
                }

            string strategy = Resources.StrategyToIndicator;
            strategy = strategy.Replace("#MODIFIED#", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#INSTRUMENT#", Data.Symbol);
            strategy = strategy.Replace("#BASEPERIOD#", Data.DataPeriodToString(Data.DataSet.Period));
            strategy = strategy.Replace("#STARTDATE#", Data.DataSet.Time[Backtester.Strategy.FirstBar].ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#ENDDATE#", Data.DataSet.Time[Data.DataSet.Bars - 1].ToString(CultureInfo.InvariantCulture));
            strategy = strategy.Replace("#PERIODMINUTES#", ((int) Data.DataSet.Period).ToString(CultureInfo.InvariantCulture));
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