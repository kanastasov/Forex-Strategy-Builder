// MicroBalanceChartImage class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public class MicroBalanceChartImage
    {
        private readonly Backtester _backtester;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="width">Chart Width</param>
        /// <param name="height">Chart Height</param>
        /// <param name="backtester">Current Backtester </param>
        public MicroBalanceChartImage(int width, int height, Backtester backtester)
        {
            _backtester = backtester;
            InitChart(width, height);
        }

        public Bitmap Chart { get; private set; }

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        private void InitChart(int width, int height)
        {
            Chart = new Bitmap(width, height);

            if (!_backtester.IsData || !_backtester.IsResult || _backtester.DataSet.Bars <= _backtester.Strategy.FirstBar) return;

            const int border = 1;
            const int space = 2;

            int maximum;
            int minimum;

            int firstBar = _backtester.Strategy.FirstBar;
            int bars = _backtester.DataSet.Bars;
            int chartBars = _backtester.DataSet.Bars - firstBar;
            int maxBalance = Configs.AccountInMoney ? (int) _backtester.MaxMoneyBalance : _backtester.MaxBalance;
            int minBalance = Configs.AccountInMoney ? (int) _backtester.MinMoneyBalance : _backtester.MinBalance;
            int maxEquity = Configs.AccountInMoney ? (int) _backtester.MaxMoneyEquity : _backtester.MaxEquity;
            int minEquity = Configs.AccountInMoney ? (int) _backtester.MinMoneyEquity : _backtester.MinEquity;

            if (Configs.AdditionalStatistics)
            {
                int maxLongBalance = Configs.AccountInMoney ? (int) _backtester.MaxLongMoneyBalance : _backtester.MaxLongBalance;
                int minLongBalance = Configs.AccountInMoney ? (int) _backtester.MinLongMoneyBalance : _backtester.MinLongBalance;
                int maxShortBalance = Configs.AccountInMoney ? (int) _backtester.MaxShortMoneyBalance : _backtester.MaxShortBalance;
                int minShortBalance = Configs.AccountInMoney ? (int) _backtester.MinShortMoneyBalance : _backtester.MinShortBalance;
                int maxLSBalance = Math.Max(maxLongBalance, maxShortBalance);
                int minLSBalance = Math.Min(minLongBalance, minShortBalance);

                maximum = Math.Max(Math.Max(maxBalance, maxEquity), maxLSBalance) + 1;
                minimum = Math.Min(Math.Min(minBalance, minEquity), minLSBalance) - 1;
            }
            else
            {
                maximum = Math.Max(maxBalance, maxEquity) + 1;
                minimum = Math.Min(minBalance, minEquity) - 1;
            }

            const int yTop = border + space;
            int yBottom = height - border - space;
            const int xLeft = border;
            int xRight = width - border - space;
            float xScale = (xRight - xLeft)/(float) chartBars;
            float yScale = (yBottom - yTop)/(float) (maximum - minimum);

            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            var balancePoints = new PointF[chartBars];
            var equityPoints = new PointF[chartBars];
            var longBalancePoints = new PointF[chartBars];
            var shortBalancePoints = new PointF[chartBars];

            int index = 0;
            for (int bar = firstBar; bar < bars; bar++)
            {
                balancePoints[index].X = xLeft + index*xScale;
                equityPoints[index].X = xLeft + index*xScale;
                if (Configs.AccountInMoney)
                {
                    balancePoints[index].Y = (float) (yBottom - (_backtester.MoneyBalance(bar) - minimum)*yScale);
                    equityPoints[index].Y = (float) (yBottom - (_backtester.MoneyEquity(bar) - minimum)*yScale);
                }
                else
                {
                    balancePoints[index].Y = yBottom - (_backtester.Balance(bar) - minimum)*yScale;
                    equityPoints[index].Y = yBottom - (_backtester.Equity(bar) - minimum)*yScale;
                }

                if (Configs.AdditionalStatistics)
                {
                    longBalancePoints[index].X = xLeft + index*xScale;
                    shortBalancePoints[index].X = xLeft + index*xScale;
                    if (Configs.AccountInMoney)
                    {
                        longBalancePoints[index].Y = (float) (yBottom - (_backtester.LongMoneyBalance(bar) - minimum)*yScale);
                        shortBalancePoints[index].Y = (float) (yBottom - (_backtester.ShortMoneyBalance(bar) - minimum)*yScale);
                    }
                    else
                    {
                        longBalancePoints[index].Y = yBottom - (_backtester.LongBalance(bar) - minimum)*yScale;
                        shortBalancePoints[index].Y = yBottom - (_backtester.ShortBalance(bar) - minimum)*yScale;
                    }
                }

                index++;
            }

            Graphics g = Graphics.FromImage(Chart);

            // Paints the background by gradient
            var rectField = new RectangleF(1, 1, width - 2, height - 2);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorChartBack), rectField);

            // Border
            g.DrawRectangle(penBorder, 0, 0, width - 1, height - 1);

            // Equity line
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), equityPoints);

            // Draw Long and Short balance
            if (Configs.AdditionalStatistics)
            {
                g.DrawLines(new Pen(Color.Red), shortBalancePoints);
                g.DrawLines(new Pen(Color.Green), longBalancePoints);
            }

            // Draw the balance line
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), balancePoints);
        }
    }
}