﻿//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs.Generator
{
    public class Top10Slot : Panel
    {
        private const int Border = 1;
        private const int Space = 4;

        private Bitmap chart;
        private int drawdown;
        private double profitPerDay;
        private double winLoss;

        public bool IsSelected { get; set; }
        public int Balance { get; private set; }
        public string CustomSortingOption { get; set; }
        public float CustomSortingValue { get; set; }

        /// <summary>
        ///     Sets the chart parameters
        /// </summary>
        public void InitSlot()
        {
            int chartHeight = ClientSize.Height - 2*(Border + Space) + 1;
            var cahartWidth = (int) (1.5*chartHeight);
            var microChart = new MicroBalanceChartImage(cahartWidth, chartHeight);

            chart = microChart.Chart;
            Balance = Configs.AccountInMoney ? (int) Math.Round(Backtester.NetMoneyBalance) : Backtester.NetBalance;
            profitPerDay = Configs.AccountInMoney ? Backtester.MoneyProfitPerDay : Backtester.ProfitPerDay;
            drawdown = Configs.AccountInMoney ? (int) Math.Round(Backtester.MaxMoneyDrawdown) : Backtester.MaxDrawdown;
            winLoss = Backtester.WinLossRatio;
        }

        /// <summary>
        ///     Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);
            var penGlow = new Pen(LayoutColors.ColorWarningRowBack, 3);
            Brush brushFore = new SolidBrush(LayoutColors.ColorChartFore);

            // Paints the background by gradient
            var rectField = new RectangleF(1, 1, ClientSize.Width - 2, ClientSize.Height - 2);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Border
            g.DrawRectangle(penBorder, 1, 1, ClientSize.Width - 2, ClientSize.Height - 2);

            // Glow
            if (IsSelected)
                g.DrawRectangle(penGlow, 3, 3, ClientSize.Width - 6, ClientSize.Height - 6);

            // Draws the chart image
            g.DrawImage(chart, new Point(Border + Space, Border + Space));

            // Draws the stats
            int textLeft = Border + Space + chart.Width + Space;

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
                    " " + (Configs.AccountInMoney ? profitPerDay.ToString("F2") : profitPerDay.ToString("F0")),
                    " " + drawdown.ToString(CultureInfo.InvariantCulture),
                    " " + winLoss.ToString("F2")
                };

            // Modified display when custom sorting is used
            if (CustomSortingOption != String.Empty)
            {
                paramNames[3] = CustomSortingOption;
                paramValues[3] = " " + CustomSortingValue.ToString("F2");
            }

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

            string unit = (Configs.AccountInMoney ? " " + Configs.AccountCurrency : " " + Language.T("points"));
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
        ///     Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            Invalidate();
        }
    }
}