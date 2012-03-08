// Top 10 Strategy Classes
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    public class Top10Slot : Panel
    {
        private const int Border = 1;
        private const int Space = 4;

        private Bitmap _chart;
        private int _drawdown;
        private double _profitPerDay;
        private double _winLoss;

        public bool IsSelected { get; set; }
        public int Balance { get; private set; }

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        public void InitSlot(Backtester backtester)
        {
            int chartHeight = ClientSize.Height - 2*(Border + Space) + 1;
            var cahartWidth = (int) (1.5*chartHeight);
            var microChart = new MicroBalanceChartImage(cahartWidth, chartHeight, backtester);

            _chart = microChart.Chart;
            Balance = Configs.AccountInMoney ? (int) Math.Round(backtester.NetMoneyBalance) : backtester.NetBalance;
            _profitPerDay = Configs.AccountInMoney ? backtester.MoneyProfitPerDay : backtester.ProfitPerDay;
            _drawdown = Configs.AccountInMoney ? (int) Math.Round(backtester.MaxMoneyDrawdown) : backtester.MaxDrawdown;
            _winLoss = backtester.WinLossRatio;
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
            var penGlow = new Pen(LayoutColors.ColorWarningRowBack, 3);
            Brush brushFore = new SolidBrush(LayoutColors.ColorChartFore);

            // Paints the background by gradient
            var rectField = new RectangleF(1, 1, ClientSize.Width - 2, ClientSize.Height - 2);
            ColorMagic.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Border
            g.DrawRectangle(penBorder, 1, 1, ClientSize.Width - 2, ClientSize.Height - 2);

            // Glow
            if (IsSelected)
                g.DrawRectangle(penGlow, 3, 3, ClientSize.Width - 6, ClientSize.Height - 6);

            // Draws the chart image
            g.DrawImage(_chart, new Point(Border + Space, Border + Space));

            // Draws the stats
            int textLeft = Border + Space + _chart.Width + Space;

            var paramNames = new[]
                                 {
                                     Language.T("Account balance"),
                                     Language.T("Profit per day"),
                                     Language.T("Maximum drawdown"),
                                     Language.T("Win/loss ratio")
                                 };
            var paramValues = new[]
                                  {
                                      " " + Balance.ToString(CultureInfo.InvariantCulture),
                                      " " + (Configs.AccountInMoney ? _profitPerDay.ToString("F2") : _profitPerDay.ToString("F0")),
                                      " " + _drawdown.ToString(CultureInfo.InvariantCulture),
                                      " " + _winLoss.ToString("F2")
                                  };

            int maxParamNameLenght = 0;
            foreach (string parameter in paramNames)
            {
                float nameWidth = g.MeasureString(parameter, Font).Width;
                if (nameWidth > maxParamNameLenght)
                    maxParamNameLenght = (int) nameWidth;
            }
            int valLeft = textLeft + maxParamNameLenght;

            int maxParamValueLenght = 0;
            foreach (string val in paramValues)
            {
                float valWidth = g.MeasureString(val, Font).Width;
                if (valWidth > maxParamValueLenght)
                    maxParamValueLenght = (int) valWidth;
            }
            int unitLeft = valLeft + maxParamValueLenght;

            string unit = (Configs.AccountInMoney ? " " + Configs.AccountCurrency : " " + Language.T("pips"));
            var unitWidth = (int) g.MeasureString(unit, Font).Width;

            for (int i = 0; i < 4; i++)
            {
                int vPos = Border + Space + i*Font.Height;
                g.DrawString(paramNames[i], Font, brushFore, textLeft, vPos);
                g.DrawString(paramValues[i], Font, brushFore, valLeft, vPos);
                if (i < 3 && unitLeft + unitWidth < ClientSize.Width - 4)
                    g.DrawString(unit, Font, brushFore, unitLeft, vPos);
            }
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            Invalidate();
        }
    }
}