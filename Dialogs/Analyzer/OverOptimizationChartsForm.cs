// OverOptimizationChartsForm Form
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    internal sealed class OverOptimizationChartsForm : Form
    {
        private int _currentChartNumber;

        /// <summary>
        /// Constructor
        /// </summary>
        public OverOptimizationChartsForm(OverOptimizationDataTable[] tableReport)
        {
            TableReport = tableReport;

            Text = Language.T("Over-optimization Report");
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.Sizable;
            AcceptButton = BtnClose;

            // Button Next Chart
            BtnNextCharts = new Button {Parent = this, Text = Language.T("Next Chart")};
            BtnNextCharts.Click += BtnNextChartsClick;
            BtnNextCharts.DoubleClick += BtnNextChartsClick;
            BtnNextCharts.MouseWheel += ChartMouseWheel;
            BtnNextCharts.KeyUp += NextChartsKeyUp;
            BtnNextCharts.UseVisualStyleBackColor = true;

            // Button Close
            BtnClose = new Button
                           {
                               Parent = this,
                               Text = Language.T("Close"),
                               DialogResult = DialogResult.Cancel,
                               UseVisualStyleBackColor = true
                           };

            CurrentChart = new OverOptimizationCharts {Parent = this};
            CurrentChart.InitChart(tableReport[_currentChartNumber]);
        }

        private Button BtnClose { get; set; }
        private Button BtnNextCharts { get; set; }

        private OverOptimizationDataTable[] TableReport { get; set; }

        private OverOptimizationCharts CurrentChart { get; set; }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(500, 400);
            MinimumSize = new Size(300, 200);
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
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

            // Next Chart
            BtnNextCharts.Size = new Size((int) (1.5*buttonWidth), buttonHeight);
            BtnNextCharts.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Chart
            CurrentChart.Size = new Size(ClientSize.Width - 2*space, BtnClose.Top - space - btnVertSpace);
            CurrentChart.Location = new Point(space, space);
        }

        /// <summary>
        /// Opens next chart.
        /// </summary>
        private void BtnNextChartsClick(object sender, EventArgs e)
        {
            ShowNextChart();
        }

        /// <summary>
        /// Shows a chart on mouse wheel.
        /// </summary>
        private void ChartMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                ShowNextChart();
            else if (e.Delta < 0)
                ShowPreviousChart();
        }

        /// <summary>
        /// Shows a chart on keyup.
        /// </summary>
        private void NextChartsKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    ShowNextChart();
                    break;
                case Keys.PageUp:
                    ShowPreviousChart();
                    break;
            }
        }

        /// <summary>
        /// Shows next chart.
        /// </summary>
        private void ShowNextChart()
        {
            _currentChartNumber++;
            if (_currentChartNumber >= TableReport.Length)
                _currentChartNumber = 0;

            CurrentChart.Parent = this;
            CurrentChart.InitChart(TableReport[_currentChartNumber]);
            CurrentChart.Invalidate();
        }

        /// <summary>
        /// Shows previous chart.
        /// </summary>
        private void ShowPreviousChart()
        {
            _currentChartNumber--;
            if (_currentChartNumber < 0)
                _currentChartNumber = TableReport.Length - 1;

            CurrentChart.Parent = this;
            CurrentChart.InitChart(TableReport[_currentChartNumber]);
            CurrentChart.Invalidate();
        }
    }
}