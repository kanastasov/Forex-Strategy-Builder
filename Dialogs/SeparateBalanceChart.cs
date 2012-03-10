// SeparateBalanceChart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder.Dialogs
{
    public sealed class SeparateBalanceChart : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SeparateBalanceChart(Backtester backtester)
        {
            Text = Language.T("Balance / Equity Chart");
            Icon = Data.Icon;
            AcceptButton = BtnClose;

            // Button Close
            BtnClose = new Button
                           {
                               Parent = this,
                               Text = Language.T("Close"),
                               DialogResult = DialogResult.Cancel,
                               UseVisualStyleBackColor = true
                           };

            // Balance chart
            BalanceChart = new SmallBalanceChart(backtester.DataSet) {Parent = this, ShowDynamicInfo = true};
            BalanceChart.MouseMove += BalanceChartMouseMove;
            BalanceChart.MouseLeave += BalanceChartMouseLeave;
            BalanceChart.SetChartData(backtester);
            BalanceChart.InitChart();

            // Label Dynamic Info
            LblDynInfo = new Label
                             {Parent = this, ForeColor = LayoutColors.ColorControlText, BackColor = Color.Transparent};
        }

        private Button BtnClose { get; set; }
        private SmallBalanceChart BalanceChart { get; set; }
        private Label LblDynInfo { get; set; }

        /// <summary>
        /// Perform initializing.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(500, 400);
            MinimumSize = new Size(300, 250);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;

            // Button Close
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // Balance Chart
            BalanceChart.Size = new Size(ClientSize.Width - 2*space, BtnClose.Top - space - btnVertSpace);
            BalanceChart.Location = new Point(space, space);

            // Label dynamic info.
            LblDynInfo.Width = BtnClose.Left - 2*space;
            LblDynInfo.Location = new Point(space, BtnClose.Top + 6);
        }

        /// <summary>
        /// Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Show the dynamic info on the status bar.
        /// </summary>
        private void BalanceChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = (SmallBalanceChart) sender;
            LblDynInfo.Text = chart.CurrentBarInfo;
        }

        /// <summary>
        /// Deletes the dynamic info on the status bar.
        /// </summary>
        private void BalanceChartMouseLeave(object sender, EventArgs e)
        {
            LblDynInfo.Text = string.Empty;
        }
    }
}