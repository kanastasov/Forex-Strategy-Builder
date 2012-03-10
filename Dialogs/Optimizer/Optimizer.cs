// Strategy Optimizer
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer
    /// </summary>
    public sealed partial class Optimizer : Form
    {
        private const int Border = 2;
        private const int OptionsVersion = 2;
        private readonly Random _rand = new Random();
        private readonly ToolTip _toolTip = new ToolTip();
        private int[] _aiChecked; // An array of the checked parameters
        private int _barOOS;
        private int _checkedParams; // Count of the checked parameters
        private Color _colorText;
        private int _computedCycles; // Currently completed cycles
        private int _cycles; // Count of the cycles
        private Font _fontIndicator;
        private Font _fontParamValueBold;
        private Font _fontParamValueRegular;
        private int _formHeight;
        private bool _isOOS;
        private bool _isOptimizing; // It is true when the optimizer is running
        private bool _isReset;
        private bool _isStartegyChanged;
        private int _parameters; // Count of the NumericParameters
        private int _progressPercent; // Reached progress in %
        private int _protections; // Count of permanent protections
        private StringBuilder _sbReport;
        private OptimizerButtons _lastSelectButton = OptimizerButtons.SelectRandom;
        private int _lastSetStepButtonValue = 5;
        private readonly Backtester _backtester;

        /// <summary>
        /// Constructor
        /// </summary>
        public Optimizer(Backtester backtester)
        {
            _backtester = new Backtester
            {
                Strategy = backtester.Strategy.Clone(),
                DataSet = backtester.DataSet,
                DataStats = backtester.DataStats,
                IsData = true
            };
            _backtester.Calculate();
            _backtester.CalculateAccountStats();
            _backtester.IsResult = true;
            _barOOS = _backtester.DataSet.Bars - 1;

            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            AcceptButton = BtnAccept;
            CancelButton = BtnCancel;
            Text = Language.T("Optimizer");
            FormClosing += OptimizerFormClosing;

            InitializeControls();
            SetupButtons();
            SetPanelLimitations();
            SetPanelSettings();
            LoadOptions();

            ChbHideFSB.CheckedChanged += HideFSBClick;
        }

        private ToolStrip TsOptimizerButtons { get; set; }
        private ToolStripButton[] AOptimizerButtons { get; set; }

        private Panel PnlParamsBase { get; set; }
        private Panel PnlCaptions { get; set; }
        private Panel PnlParamsBase2 { get; set; }
        private Panel PnlParams { get; set; }
        private FancyPanel PnlLimitations { get; set; }
        private FancyPanel PnlSettings { get; set; }

        private CheckBox[] AchbxParameterName { get; set; }
        private Label[] AlblInitialValue { get; set; }
        private Label[] AlblParameterValue { get; set; }
        private NumericUpDown[] AnudParameterMin { get; set; }
        private NumericUpDown[] AnudParameterMax { get; set; }
        private NumericUpDown[] AnudParameterStep { get; set; }
        private SmallBalanceChart BalanceChart { get; set; }
        private Label[] AlblIndicatorName { get; set; }
        private Label LblNoParams { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private Button BtnOptimize { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Parameter[] AParameter { get; set; }
        private VScrollBar ScrollBar { get; set; }
        private BackgroundWorker BgWorker { get; set; }

        private CheckBox ChbAmbiguousBars { get; set; }
        private NumericUpDown NUDAmbiguousBars { get; set; }
        private CheckBox ChbMaxDrawdown { get; set; }
        private NumericUpDown NUDMaxDrawdown { get; set; }
        private CheckBox ChbMinTrades { get; set; }
        private NumericUpDown NUDMinTrades { get; set; }
        private CheckBox ChbMaxTrades { get; set; }
        private NumericUpDown NUDMaxTrades { get; set; }
        private CheckBox ChbWinLossRatio { get; set; }
        private NumericUpDown NUDWinLossRatio { get; set; }
        private CheckBox ChbEquityPercent { get; set; }
        private NumericUpDown NUDEquityPercent { get; set; }
        private CheckBox ChbOOSPatternFilter { get; set; }
        private NumericUpDown NUDOOSPatternPercent { get; set; }
        private CheckBox ChbSmoothBalanceLines { get; set; }
        private NumericUpDown NUDSmoothBalancePercent { get; set; }
        private NumericUpDown NUDSmoothBalanceCheckPoints { get; set; }
        private CheckBox ChbOptimizerWritesReport { get; set; }

        private CheckBox ChbHideFSB { get; set; }
        private Button BtnResetSettings { get; set; }

        private CheckBox ChbOutOfSample { get; set; }
        private NumericUpDown NUDOutOfSample { get; set; }

        private Form FormFSB { get; set; }

        public Strategy Strategy
        {
            get { return _backtester.Strategy; }
        }

        public Form SetParrentForm
        {
            set { FormFSB = value; }
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FormFSB.Visible = !ChbHideFSB.Checked;

            SetIndicatorParams();
            SelectParameters(_lastSelectButton);

            Width = 555;
            Height = Math.Max(_formHeight, 570);
            MinimumSize = new Size(555, 570);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int buttonWidth = (ClientSize.Width - 4*btnHrzSpace)/3;
            int space = btnHrzSpace;

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Optimize
            BtnOptimize.Size = new Size(buttonWidth, buttonHeight);
            BtnOptimize.Location = new Point(BtnAccept.Left - buttonWidth - btnHrzSpace,
                                             ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            ProgressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDLU*9));
            ProgressBar.Location = new Point(space, BtnCancel.Top - ProgressBar.Height - btnVertSpace);

            // Panel Preview
            BalanceChart.Size = new Size(ClientSize.Width - 2*space, 200);
            BalanceChart.Location = new Point(space, ProgressBar.Top - space - BalanceChart.Height);

            // Panel Parameters Base
            PnlParamsBase.Size = new Size(ClientSize.Width - 2*space,
                                          BalanceChart.Top - 2*space - TsOptimizerButtons.Bottom);
            PnlParamsBase.Location = new Point(space, TsOptimizerButtons.Bottom + space);

            // Panel Parameters Base 2
            PnlParamsBase2.Size = new Size(PnlParamsBase.Width - 2*Border,
                                           PnlParamsBase.Height - PnlCaptions.Height - Border);
            PnlParamsBase2.Location = new Point(Border, PnlCaptions.Height);

            // Panel Parameters
            PnlParams.Width = PnlParamsBase2.ClientSize.Width - ScrollBar.Width;

            // No Parameters
            LblNoParams.Location = new Point(5, 5);

            // Panel Limitations
            PnlLimitations.Size = PnlParamsBase.Size;
            PnlLimitations.Location = PnlParamsBase.Location;

            const int nudWidth = 55;

            // chbAmbiguousBars
            ChbAmbiguousBars.Location = new Point(Border + 5, 27);

            // nudAmbiguousBars
            NUDAmbiguousBars.Width = nudWidth;
            NUDAmbiguousBars.Location = new Point(PnlLimitations.ClientSize.Width - nudWidth - Border - 5,
                                                  ChbAmbiguousBars.Top - 1);

            // MaxDrawdown
            ChbMaxDrawdown.Location = new Point(Border + 5, ChbAmbiguousBars.Bottom + Border + 4);
            NUDMaxDrawdown.Width = nudWidth;
            NUDMaxDrawdown.Location = new Point(NUDAmbiguousBars.Left, ChbMaxDrawdown.Top - 1);

            // MaxDrawdown %
            ChbEquityPercent.Location = new Point(Border + 5, NUDMaxDrawdown.Bottom + Border + 4);
            NUDEquityPercent.Width = nudWidth;
            NUDEquityPercent.Location = new Point(NUDAmbiguousBars.Left, ChbEquityPercent.Top - 1);

            // MinTrades
            ChbMinTrades.Location = new Point(Border + 5, ChbEquityPercent.Bottom + Border + 4);
            NUDMinTrades.Width = nudWidth;
            NUDMinTrades.Location = new Point(NUDAmbiguousBars.Left, ChbMinTrades.Top - 1);

            // MaxTrades
            ChbMaxTrades.Location = new Point(Border + 5, ChbMinTrades.Bottom + Border + 4);
            NUDMaxTrades.Width = nudWidth;
            NUDMaxTrades.Location = new Point(NUDAmbiguousBars.Left, ChbMaxTrades.Top - 1);

            // WinLossRatios
            ChbWinLossRatio.Location = new Point(Border + 5, ChbMaxTrades.Bottom + Border + 4);
            NUDWinLossRatio.Width = nudWidth;
            NUDWinLossRatio.Location = new Point(NUDAmbiguousBars.Left, ChbWinLossRatio.Top - 1);

            // OOS Pattern Filter
            ChbOOSPatternFilter.Location = new Point(Border + 5, ChbWinLossRatio.Bottom + Border + 4);
            NUDOOSPatternPercent.Width = nudWidth;
            NUDOOSPatternPercent.Location = new Point(NUDAmbiguousBars.Left, ChbOOSPatternFilter.Top - 1);

            // Balance lines pattern
            ChbSmoothBalanceLines.Location = new Point(Border + 5, ChbOOSPatternFilter.Bottom + Border + 4);
            NUDSmoothBalancePercent.Width = nudWidth;
            NUDSmoothBalancePercent.Location = new Point(NUDAmbiguousBars.Left, ChbSmoothBalanceLines.Top - 1);
            NUDSmoothBalanceCheckPoints.Width = nudWidth;
            NUDSmoothBalanceCheckPoints.Location = new Point(NUDSmoothBalancePercent.Left - nudWidth - Border,
                                                             ChbSmoothBalanceLines.Top - 1);

            // Panel Settings
            PnlSettings.Size = PnlParamsBase.Size;
            PnlSettings.Location = PnlParamsBase.Location;

            // Out Of Sample
            ChbOutOfSample.Location = new Point(Border + 5, 27);
            NUDOutOfSample.Width = nudWidth;
            NUDOutOfSample.Location = new Point(PnlSettings.ClientSize.Width - nudWidth - Border - 5,
                                                ChbOutOfSample.Top - 1);

            // chbOptimizerWritesReport
            ChbOptimizerWritesReport.Location = new Point(Border + 5, ChbOutOfSample.Bottom + Border + 4);

            // Hide FSB when generator starts
            ChbHideFSB.Location = new Point(Border + 5, ChbOptimizerWritesReport.Bottom + Border + 4);

            // Button Reset
            BtnResetSettings.Width = PnlSettings.ClientSize.Width - 2*(Border + 5);
            BtnResetSettings.Location = new Point(Border + 5, PnlSettings.Height - BtnResetSettings.Height - Border - 2);

            // pnlCaptions
            PnlCaptions.Height = 20;
            PnlCaptions.Invalidate();
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        private void OptimizerFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isReset)
                SaveOptions();

            if (_isOptimizing)
            {
                // Cancel the asynchronous operation.
                BgWorker.CancelAsync();
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.Cancel && _isStartegyChanged)
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept changes to the strategy?"),
                                                  Language.T("Optimizer"), MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Question);

                switch (dr)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        DialogResult = DialogResult.OK;
                        break;
                    case DialogResult.No:
                        DialogResult = DialogResult.Cancel;
                        break;
                }
            }
            else if (DialogResult == DialogResult.OK && !_isStartegyChanged)
            {
                DialogResult = DialogResult.Cancel;
            }

            FormFSB.Visible = true;
        }
    }
}