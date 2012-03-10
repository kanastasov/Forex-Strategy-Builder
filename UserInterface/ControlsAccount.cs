// Controls Account
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Controls Account: MenuAndStatusBar
    /// </summary>
    public partial class Controls
    {
        protected SmallBalanceChart BalanceChart { get; private set; }
        protected ToolStripComboBox ComboBoxInterpolationMethod { get; private set; }
        protected InfoPanel InfoPanelAccountStatistics { get; private set; }

        /// <summary>
        /// Initializes the controls in panel pnlOverview
        /// </summary>
        private void InitializeAccount()
        {
            var toolTip = new ToolTip();

            string[] methods = Enum.GetNames(typeof (InterpolationMethod));
            for (int i = 0; i < methods.Length; i++)
                methods[i] = Language.T(methods[i]);

            Graphics g = CreateGraphics();
            int maxWidth = 0;
            foreach (string method in methods)
                if ((int) g.MeasureString(method, Font).Width > maxWidth)
                    maxWidth = (int) g.MeasureString(method, Font).Width;
            g.Dispose();

            // ComboBox Interpolation Methods
            ComboBoxInterpolationMethod = new ToolStripComboBox
            {
                Name = "ComboBoxInterpolationMethod",
                AutoSize = false,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = maxWidth + (int) (18*Data.HorizontalDLU),
                ToolTipText = Language.T("Bar interpolation method.")
            };
            foreach (string method in methods)
                ComboBoxInterpolationMethod.Items.Add(method);
            ComboBoxInterpolationMethod.SelectedIndex = 0;
            ComboBoxInterpolationMethod.SelectedIndexChanged += SelectedIndexChanged;
            ToolStripAccount.Items.Add(ComboBoxInterpolationMethod);

            // Button Comparator
            var tsbtComparator = new ToolStripButton {Text = Language.T("Comparator"), Name = "Comparator"};
            tsbtComparator.Click += BtnToolsOnClick;
            tsbtComparator.ToolTipText = Language.T("Compare the interpolating methods.");
            ToolStripAccount.Items.Add(tsbtComparator);

            // Button Scanner
            var tsbtScanner = new ToolStripButton {Text = Language.T("Scanner"), Name = "Scanner"};
            tsbtScanner.Click += BtnToolsOnClick;
            tsbtScanner.ToolTipText = Language.T("Perform a deep intrabar scan.") + Environment.NewLine +
                                      Language.T("Quick scan") + " - F6.";
            ToolStripAccount.Items.Add(tsbtScanner);

            // Button Analyzer
            var tsbtAnalyzer = new ToolStripButton {Text = Language.T("Analyzer"), Name = "Analyzer"};
            tsbtAnalyzer.Click += BtnToolsOnClick;
            ToolStripAccount.Items.Add(tsbtAnalyzer);

            // Info Panel Account Statistics
            InfoPanelAccountStatistics = new InfoPanel {Parent = PanelAccount, Dock = DockStyle.Fill};

            new Splitter {Parent = PanelAccount, Dock = DockStyle.Bottom, BorderStyle = BorderStyle.None, Height = Gap};

            // Small Balance Chart
            BalanceChart = new SmallBalanceChart(Backtester.DataSet)
            {
                Parent = PanelAccount,
                Cursor = Cursors.Hand,
                Dock = DockStyle.Bottom,
                MinimumSize = new Size(100, 50),
                ShowDynamicInfo = true,
                IsContextButtonVisible = true
            };
            BalanceChart.PopUpContextMenu.Items.AddRange(GetBalanceChartContextMenuItems());
            BalanceChart.MouseMove += SmallBalanceChartMouseMove;
            BalanceChart.MouseLeave += SmallBalanceChartMouseLeave;
            BalanceChart.MouseUp += SmallBalanceChartMouseUp;
            toolTip.SetToolTip(BalanceChart, Language.T("Click to view the full chart.") +
                                             Environment.NewLine +
                                             Language.T("Right click to detach chart."));

            PanelAccount.Resize += PnlAccountResize;
        }

        private ToolStripItem[] GetBalanceChartContextMenuItems()
        {
            var menuStripShowFullBalanceChart = new ToolStripMenuItem
            {
                Image = Properties.Resources.balance_chart,
                Text = Language.T("Full Balance Chart") + "..."
            };
            menuStripShowFullBalanceChart.Click += ContextMenuShowFullBalanceChartClick;

            var menuStripDetachChart = new ToolStripMenuItem
            {
                Image = Properties.Resources.pushpin_detach,
                Text = Language.T("Detach Balance Chart") + "..."
            };
            menuStripDetachChart.Click += ContextMenuDetachChartClick;

            var itemCollection = new ToolStripItem[]
            {
                menuStripShowFullBalanceChart,
                menuStripDetachChart
            };

            return itemCollection;
        }

        private void ContextMenuShowFullBalanceChartClick(object sender, EventArgs e)
        {
            ShowFullBalanceChart();
        }

        private void ContextMenuDetachChartClick(object sender, EventArgs e)
        {
            DetachBalanceChart();
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        private void PnlAccountResize(object sender, EventArgs e)
        {
            BalanceChart.Height = 2*PanelAccount.ClientSize.Height/(Configs.ShowJournal ? 3 : 4);
        }

        /// <summary>
        /// Show the dynamic info on the status bar
        /// </summary>
        private void SmallBalanceChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = (SmallBalanceChart) sender;
            StatusLabelChartInfo = chart.CurrentBarInfo;
        }

        /// <summary>
        /// Deletes the dynamic info on the status bar
        /// </summary>
        private void SmallBalanceChartMouseLeave(object sender, EventArgs e)
        {
            StatusLabelChartInfo = string.Empty;
        }

        /// <summary>
        /// Shows the full account chart after clicking on it
        /// </summary>
        private void SmallBalanceChartMouseUp(object sender, MouseEventArgs e)
        {
            if(!Backtester.IsData || !Backtester.IsResult) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    ShowFullBalanceChart();
                    break;
                case MouseButtons.Right:
                    DetachBalanceChart();
                    break;
            }
        }

        protected virtual void DetachBalanceChart()
        {
        }

        /// <summary>
        /// Opens the corresponding tool
        /// </summary>
        protected virtual void BtnToolsOnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected override void MenuToolsOnClick(object sender, EventArgs e)
        {
        }
    }
}