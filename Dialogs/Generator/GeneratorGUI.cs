// Strategy Generator - GUI
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    /// <summary>
    /// Strategy Generator
    /// </summary>
    public sealed partial class Generator : Form
    {
        private BackgroundWorker BgWorker { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnGenerate { get; set; }

        private CheckBox ChbGenerateNewStrategy { get; set; }
        private CheckBox ChbInitialOptimisation { get; set; }
        private CheckBox ChbPreservBreakEven { get; set; }
        private CheckBox ChbPreservPermSL { get; set; }
        private CheckBox ChbPreservPermTP { get; set; }
        private Color ColorText { get; set; }
        private InfoPanel InfpnlAccountStatistics { get; set; }
        private Label LblCalcStrInfo { get; set; }
        private Label LblCalcStrNumb { get; set; }
        private Label LblWorkingMinutes { get; set; }
        private NumericUpDown NUDWorkingMinutes { get; set; }
        private FancyPanel PnlCommon { get; set; }
        private FancyPanel PnlIndicators { get; set; }
        private FancyPanel PnlLimitations { get; set; }
        private FancyPanel PnlSettings { get; set; }
        private FancyPanel PnlTop10 { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private SmallBalanceChart BalanceChart { get; set; }
        private StrategyLayout StrategyField { get; set; }
        private readonly ToolTip _toolTip = new ToolTip();
        private ToolStrip TsGenerator { get; set; }
        private ToolStrip TsStrategy { get; set; }
        private Button BtnReset { get; set; }

        private CheckBox ChbAmbiguousBars { get; set; }
        private CheckBox ChbEquityPercent { get; set; }
        private CheckBox ChbHideFSB { get; set; }
        private CheckBox ChbMaxClosingLogicSlots { get; set; }
        private CheckBox ChbMaxDrawdown { get; set; }
        private CheckBox ChbMaxOpeningLogicSlots { get; set; }
        private CheckBox ChbMaxTrades { get; set; }
        private CheckBox ChbMinTrades { get; set; }
        private CheckBox ChbOOSPatternFilter { get; set; }
        private CheckBox ChbOutOfSample { get; set; }
        private CheckBox ChbSmoothBalanceLines { get; set; }

        private CheckBox ChbUseDefaultIndicatorValues { get; set; }
        private CheckBox ChbWinLossRatio { get; set; }
        private IndicatorsLayout IndicatorsField { get; set; }

        private NumericUpDown NUDAmbiguousBars { get; set; }
        private NumericUpDown NUDEquityPercent { get; set; }
        private NumericUpDown NUDMaxClosingLogicSlots { get; set; }
        private NumericUpDown NUDMaxDrawdown { get; set; }
        private NumericUpDown NUDMaxOpeningLogicSlots { get; set; }
        private NumericUpDown NUDMaxTrades { get; set; }
        private NumericUpDown NUDMinTrades { get; set; }
        private NumericUpDown NUDOOSPatternPercent { get; set; }
        private NumericUpDown NUDOutOfSample { get; set; }
        private NumericUpDown NUDSmoothBalanceCheckPoints { get; set; }
        private NumericUpDown NUDSmoothBalancePercent { get; set; }
        private NumericUpDown NUDWinLossRatio { get; set; }
        private Top10Layout Top10Field { get; set; }
        private ToolStripButton TsbtLinkAll { get; set; }
        private ToolStripButton TsbtLockAll { get; set; }
        private ToolStripButton TsbtOverview { get; set; }
        private ToolStripButton TsbtShowIndicators { get; set; }
        private ToolStripButton TsbtShowLimitations { get; set; }
        private ToolStripButton TsbtShowOptions { get; set; }
        private ToolStripButton TsbtShowSettings { get; set; }
        private ToolStripButton TsbtShowTop10 { get; set; }
        private ToolStripButton TsbtStrategyInfo { get; set; }
        private ToolStripButton TsbtStrategySize1 { get; set; }
        private ToolStripButton TsbtStrategySize2 { get; set; }
        private ToolStripButton TsbtUnlockAll { get; set; }
        private bool _isReset;
        private double _buttonWidthMultiplier = 1; // It's used in OnResize().
        private readonly Random _random = new Random();
        private readonly Backtester _backtester;

        /// <summary>
        /// Constructor
        /// </summary>
        public Generator(Backtester backtester)
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
            SetStrategyIndicators();
            StrategyOriginalDescription = _backtester.Strategy.Description;

            GeneratedDescription = string.Empty;
            _strategyBest = _backtester.Strategy.Clone();
            _bestBalance = _isOOS ? _backtester.Balance(_barOOS) : _backtester.NetBalance;
            _isGenerating = false;
            _isStartegyChanged = false;
            _indicatorBlackList = new List<string>();
            _barOOS = _backtester.DataSet.Bars - 1;

            ColorText = LayoutColors.ColorControlText;

            TsStrategy = new ToolStrip();
            TsGenerator = new ToolStrip();
            StrategyField = new StrategyLayout(_strategyBest);
            PnlCommon = new FancyPanel(Language.T("Common"));
            PnlLimitations = new FancyPanel(Language.T("Limitations"));
            PnlSettings = new FancyPanel(Language.T("Settings"));
            PnlTop10 = new FancyPanel(Language.T("Top 10"));
            PnlIndicators = new FancyPanel(Language.T("Indicators"));
            BalanceChart = new SmallBalanceChart(_backtester.DataSet);
            InfpnlAccountStatistics = new InfoPanel();
            ProgressBar = new ProgressBar();
            LblCalcStrInfo = new Label();
            LblCalcStrNumb = new Label();
            BtnAccept = new Button();
            BtnGenerate = new Button();
            BtnCancel = new Button();
            ChbGenerateNewStrategy = new CheckBox();
            ChbPreservPermSL = new CheckBox();
            ChbPreservPermTP = new CheckBox();
            ChbPreservBreakEven = new CheckBox();
            ChbInitialOptimisation = new CheckBox();
            NUDWorkingMinutes = new NumericUpDown();
            LblWorkingMinutes = new Label();

            MaximizeBox = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor = LayoutColors.ColorFormBack;
            AcceptButton = BtnGenerate;
            Text = Language.T("Strategy Generator") + " - " + _backtester.DataSet.Symbol + " " + _backtester.DataSet.PeriodString + ", " +
                   _backtester.DataSet.Bars + " " + Language.T("bars");
            FormClosing += GeneratorFormClosing;

            // Tool Strip Strategy
            TsStrategy.Parent = this;
            TsStrategy.Dock = DockStyle.None;
            TsStrategy.AutoSize = false;

            // Tool Strip Generator
            TsGenerator.Parent = this;
            TsGenerator.Dock = DockStyle.None;
            TsGenerator.AutoSize = false;

            // Creates a Strategy Layout
            StrategyField.Parent = this;
            StrategyField.ShowAddSlotButtons = false;
            StrategyField.ShowRemoveSlotButtons = false;
            StrategyField.ShowPadlockImg = true;
            StrategyField.SlotPropertiesTipText = Language.T("Lock or unlock the slot.");
            StrategyField.SlotToolTipText = Language.T("Lock, link, or unlock the slot.");

            PnlCommon.Parent = this;
            PnlLimitations.Parent = this;
            PnlSettings.Parent = this;
            PnlTop10.Parent = this;
            PnlIndicators.Parent = this;

            // Small Balance Chart
            BalanceChart.Parent = this;
            BalanceChart.BackColor = LayoutColors.ColorControlBack;
            BalanceChart.Visible = true;
            BalanceChart.Cursor = Cursors.Hand;
            BalanceChart.IsContextButtonVisible = true;
            BalanceChart.PopUpContextMenu.Items.AddRange(GetBalanceChartContextMenuItems());
            BalanceChart.Click += AccountAutputClick;
            BalanceChart.DoubleClick += AccountAutputClick;
            _toolTip.SetToolTip(BalanceChart, Language.T("Show account statistics."));
            BalanceChart.SetChartData(_backtester);

            // Info Panel Account Statistics
            InfpnlAccountStatistics.Parent = this;
            InfpnlAccountStatistics.Visible = false;
            InfpnlAccountStatistics.Cursor = Cursors.Hand;
            InfpnlAccountStatistics.IsContextButtonVisible = true;
            InfpnlAccountStatistics.PopUpContextMenu.Items.AddRange(GetInfoPanelContextMenuItems());
            InfpnlAccountStatistics.Click += AccountAutputClick;
            InfpnlAccountStatistics.DoubleClick += AccountAutputClick;
            _toolTip.SetToolTip(InfpnlAccountStatistics, Language.T("Show account chart."));

            // ProgressBar
            ProgressBar.Parent = this;
            ProgressBar.Minimum = 1;
            ProgressBar.Maximum = 100;
            ProgressBar.Step = 1;

            //Button Generate
            BtnGenerate.Parent = this;
            BtnGenerate.Name = "Generate";
            BtnGenerate.Text = Language.T("Generate");
            BtnGenerate.Click += BtnGenerateClick;
            BtnGenerate.UseVisualStyleBackColor = true;

            // Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Accept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.Enabled = false;
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;
            BtnAccept.Click += BtnAcceptOnClick;

            // Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancelOnClick;

            // BackgroundWorker
            BgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            BgWorker.DoWork += BgWorkerDoWork;
            BgWorker.ProgressChanged += BgWorkerProgressChanged;
            BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            SetButtonsStrategy();
            SetButtonsGenerator();
            SetPanelCommon();
            SetPanelLimitations();
            SetPanelSettings();
            SetPanelTop10();
            SetPanelIndicators();
            LoadOptions();
            SetStrategyDescriptionButton();

            ChbHideFSB.CheckedChanged += HideFSBClick;
        }

        public string StrategyOriginalDescription { get; private set; }
        public Strategy Strategy { get { return _backtester.Strategy; } }

        public Form ParrentForm { private get; set; }

        /// <summary>
        /// Gets the strategy description
        /// </summary>
        public string GeneratedDescription { get; private set; }

        /// <summary>
        /// Whether the strategy was modified or entirely generated
        /// </summary>
        public bool IsStrategyModified { get; private set; }

        /// <summary>
        /// Loads and parses the generator's options.
        /// </summary>
        private void LoadOptions()
        {
            if (string.IsNullOrEmpty(Configs.GeneratorOptions))
                return;

            string[] options = Configs.GeneratorOptions.Split(';');
            var i = 0;
            try
            {
                ChbGenerateNewStrategy.Checked = bool.Parse(options[i++]);
                ChbPreservPermSL.Checked = bool.Parse(options[i++]);
                ChbPreservPermTP.Checked = bool.Parse(options[i++]);
                ChbPreservBreakEven.Checked = bool.Parse(options[i++]);
                ChbInitialOptimisation.Checked = bool.Parse(options[i++]);
                ChbMaxOpeningLogicSlots.Checked = bool.Parse(options[i++]);
                NUDMaxOpeningLogicSlots.Value = Math.Min(int.Parse(options[i++]), Strategy.MaxOpenFilters);
                ChbMaxClosingLogicSlots.Checked = bool.Parse(options[i++]);
                NUDMaxClosingLogicSlots.Value = Math.Min(int.Parse(options[i++]), Strategy.MaxCloseFilters);
                ChbOutOfSample.Checked = bool.Parse(options[i++]);
                NUDOutOfSample.Value = int.Parse(options[i++]);
                NUDWorkingMinutes.Value = int.Parse(options[i++]);
                ChbAmbiguousBars.Checked = bool.Parse(options[i++]);
                NUDAmbiguousBars.Value = int.Parse(options[i++]);
                ChbMaxDrawdown.Checked = bool.Parse(options[i++]);
                NUDMaxDrawdown.Value = int.Parse(options[i++]);
                ChbMinTrades.Checked = bool.Parse(options[i++]);
                NUDMinTrades.Value = int.Parse(options[i++]);
                ChbMaxTrades.Checked = bool.Parse(options[i++]);
                NUDMaxTrades.Value = int.Parse(options[i++]);
                ChbWinLossRatio.Checked = bool.Parse(options[i++]);
                NUDWinLossRatio.Value = int.Parse(options[i++])/100M;
                ChbEquityPercent.Checked = bool.Parse(options[i++]);
                NUDEquityPercent.Value = int.Parse(options[i++]);
                ChbOOSPatternFilter.Checked = bool.Parse(options[i++]);
                NUDOOSPatternPercent.Value = int.Parse(options[i++]);
                ChbSmoothBalanceLines.Checked = bool.Parse(options[i++]);
                NUDSmoothBalancePercent.Value = int.Parse(options[i++]);
                NUDSmoothBalanceCheckPoints.Value = int.Parse(options[i++]);
                ChbUseDefaultIndicatorValues.Checked = bool.Parse(options[i++]);
                ChbHideFSB.Checked = bool.Parse(options[i]);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Saves the generator's options.
        /// </summary>
        private void SaveOptions()
        {
            string options =
                ChbGenerateNewStrategy.Checked + ";" +
                ChbPreservPermSL.Checked + ";" +
                ChbPreservPermTP.Checked + ";" +
                ChbPreservBreakEven.Checked + ";" +
                ChbInitialOptimisation.Checked + ";" +
                ChbMaxOpeningLogicSlots.Checked + ";" +
                NUDMaxOpeningLogicSlots.Value + ";" +
                ChbMaxClosingLogicSlots.Checked + ";" +
                NUDMaxClosingLogicSlots.Value + ";" +
                ChbOutOfSample.Checked + ";" +
                NUDOutOfSample.Value + ";" +
                NUDWorkingMinutes.Value + ";" +
                ChbAmbiguousBars.Checked + ";" +
                NUDAmbiguousBars.Value + ";" +
                ChbMaxDrawdown.Checked + ";" +
                NUDMaxDrawdown.Value + ";" +
                ChbMinTrades.Checked + ";" +
                NUDMinTrades.Value + ";" +
                ChbMaxTrades.Checked + ";" +
                NUDMaxTrades.Value + ";" +
                ChbWinLossRatio.Checked + ";" +
                ((int) (NUDWinLossRatio.Value*100M)) + ";" +
                ChbEquityPercent.Checked + ";" +
                NUDEquityPercent.Value + ";" +
                ChbOOSPatternFilter.Checked + ";" +
                NUDOOSPatternPercent.Value + ";" +
                ChbSmoothBalanceLines.Checked + ";" +
                NUDSmoothBalancePercent.Value + ";" +
                NUDSmoothBalanceCheckPoints.Value + ";" +
                ChbUseDefaultIndicatorValues.Checked + ";" +
                ChbHideFSB.Checked;

            Configs.GeneratorOptions = options;
        }

        /// <summary>
        /// Sets controls in panel Common
        /// </summary>
        private void SetPanelCommon()
        {
            // chbGenerateNewStrategy
            ChbGenerateNewStrategy.Parent = PnlCommon;
            ChbGenerateNewStrategy.Text = Language.T("Generate a new strategy at every start");
            ChbGenerateNewStrategy.AutoSize = true;
            ChbGenerateNewStrategy.Checked = true;
            ChbGenerateNewStrategy.ForeColor = LayoutColors.ColorControlText;
            ChbGenerateNewStrategy.BackColor = Color.Transparent;

            // chbPreservPermSL
            ChbPreservPermSL.Parent = PnlCommon;
            ChbPreservPermSL.Text = Language.T("Do not change the Permanent Stop Loss");
            ChbPreservPermSL.AutoSize = true;
            ChbPreservPermSL.Checked = true;
            ChbPreservPermSL.ForeColor = LayoutColors.ColorControlText;
            ChbPreservPermSL.BackColor = Color.Transparent;

            // chbPreservPermTP
            ChbPreservPermTP.Parent = PnlCommon;
            ChbPreservPermTP.Text = Language.T("Do not change the Permanent Take Profit");
            ChbPreservPermTP.AutoSize = true;
            ChbPreservPermTP.Checked = true;
            ChbPreservPermTP.ForeColor = LayoutColors.ColorControlText;
            ChbPreservPermTP.BackColor = Color.Transparent;

            // chbPreservbreakEven
            ChbPreservBreakEven.Parent = PnlCommon;
            ChbPreservBreakEven.Text = Language.T("Do not change the Break Even");
            ChbPreservBreakEven.AutoSize = true;
            ChbPreservBreakEven.Checked = true;
            ChbPreservBreakEven.ForeColor = LayoutColors.ColorControlText;
            ChbPreservBreakEven.BackColor = Color.Transparent;

            // chbPseudoOpt
            ChbInitialOptimisation.Parent = PnlCommon;
            ChbInitialOptimisation.Text = Language.T("Perform an initial optimization");
            ChbInitialOptimisation.AutoSize = true;
            ChbInitialOptimisation.Checked = true;
            ChbInitialOptimisation.ForeColor = LayoutColors.ColorControlText;
            ChbInitialOptimisation.BackColor = Color.Transparent;

            ChbMaxOpeningLogicSlots = new CheckBox
                                          {
                                              Parent = PnlCommon,
                                              ForeColor = ColorText,
                                              BackColor = Color.Transparent,
                                              Text = Language.T("Maximum number of opening logic slots"),
                                              Checked = true,
                                              AutoSize = true
                                          };

            NUDMaxOpeningLogicSlots = new NumericUpDown {Parent = PnlCommon, TextAlign = HorizontalAlignment.Center};
            NUDMaxOpeningLogicSlots.BeginInit();
            NUDMaxOpeningLogicSlots.Minimum = 0;
            NUDMaxOpeningLogicSlots.Maximum = Strategy.MaxOpenFilters;
            NUDMaxOpeningLogicSlots.Increment = 1;
            NUDMaxOpeningLogicSlots.Value = 2;
            NUDMaxOpeningLogicSlots.EndInit();

            ChbMaxClosingLogicSlots = new CheckBox
                                          {
                                              Parent = PnlCommon,
                                              ForeColor = ColorText,
                                              BackColor = Color.Transparent,
                                              Text = Language.T("Maximum number of closing logic slots"),
                                              Checked = true,
                                              AutoSize = true
                                          };

            NUDMaxClosingLogicSlots = new NumericUpDown {Parent = PnlCommon, TextAlign = HorizontalAlignment.Center};
            NUDMaxClosingLogicSlots.BeginInit();
            NUDMaxClosingLogicSlots.Minimum = 0;
            NUDMaxClosingLogicSlots.Maximum = Strategy.MaxCloseFilters;
            NUDMaxClosingLogicSlots.Increment = 1;
            NUDMaxClosingLogicSlots.Value = 1;
            NUDMaxClosingLogicSlots.EndInit();

            //lblNumUpDown
            LblWorkingMinutes.Parent = PnlCommon;
            LblWorkingMinutes.ForeColor = LayoutColors.ColorControlText;
            LblWorkingMinutes.BackColor = Color.Transparent;
            LblWorkingMinutes.Text = Language.T("Working time");
            LblWorkingMinutes.AutoSize = true;
            LblWorkingMinutes.TextAlign = ContentAlignment.MiddleRight;

            // numUpDownWorkingTime
            NUDWorkingMinutes.Parent = PnlCommon;
            NUDWorkingMinutes.Value = 5;
            NUDWorkingMinutes.Minimum = 0;
            NUDWorkingMinutes.Maximum = 10000;
            NUDWorkingMinutes.TextAlign = HorizontalAlignment.Center;
            _toolTip.SetToolTip(NUDWorkingMinutes, Language.T("Set the number of minutes for the Generator to work.") +
                                                  Environment.NewLine + "0 - " + Language.T("No limits").ToLower() + ".");

            // Label Calculated Strategies Caption
            LblCalcStrInfo.Parent = PnlCommon;
            LblCalcStrInfo.AutoSize = true;
            LblCalcStrInfo.ForeColor = LayoutColors.ColorControlText;
            LblCalcStrInfo.BackColor = Color.Transparent;
            LblCalcStrInfo.Text = Language.T("Calculations");

            // Label Calculated Strategies Number
            LblCalcStrNumb.Parent = PnlCommon;
            LblCalcStrNumb.BorderStyle = BorderStyle.FixedSingle;
            LblCalcStrNumb.ForeColor = LayoutColors.ColorControlText;
            LblCalcStrNumb.BackColor = LayoutColors.ColorControlBack;
            LblCalcStrNumb.TextAlign = ContentAlignment.MiddleCenter;
            LblCalcStrNumb.Text = "0";
        }

        /// <summary>
        /// Sets controls in panel Limitations
        /// </summary>
        private void SetPanelLimitations()
        {
            ChbAmbiguousBars = new CheckBox
                                   {
                                       Parent = PnlLimitations,
                                       ForeColor = ColorText,
                                       BackColor = Color.Transparent,
                                       Text = Language.T("Maximum number of ambiguous bars"),
                                       Checked = true,
                                       AutoSize = true
                                   };

            NUDAmbiguousBars = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDAmbiguousBars.BeginInit();
            NUDAmbiguousBars.Minimum = 0;
            NUDAmbiguousBars.Maximum = 100;
            NUDAmbiguousBars.Increment = 1;
            NUDAmbiguousBars.Value = 10;
            NUDAmbiguousBars.EndInit();

            ChbMaxDrawdown = new CheckBox
                                 {
                                     Parent = PnlLimitations,
                                     ForeColor = ColorText,
                                     BackColor = Color.Transparent,
                                     Text = Language.T("Maximum equity drawdown") + " [" +
                                            (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]"),
                                     Checked = false,
                                     AutoSize = true
                                 };

            NUDMaxDrawdown = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDMaxDrawdown.BeginInit();
            NUDMaxDrawdown.Minimum = 0;
            NUDMaxDrawdown.Maximum = Configs.InitialAccount;
            NUDMaxDrawdown.Increment = 10;
            NUDMaxDrawdown.Value = (decimal) Math.Round(Configs.InitialAccount/4.0);
            NUDMaxDrawdown.EndInit();

            ChbEquityPercent = new CheckBox
                                   {
                                       Parent = PnlLimitations,
                                       ForeColor = ColorText,
                                       BackColor = Color.Transparent,
                                       Text =
                                           Language.T("Maximum equity drawdown") + " [% " + Configs.AccountCurrency +
                                           "]",
                                       Checked = true,
                                       AutoSize = true
                                   };

            NUDEquityPercent = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDEquityPercent.BeginInit();
            NUDEquityPercent.Minimum = 1;
            NUDEquityPercent.Maximum = 100;
            NUDEquityPercent.Increment = 1;
            NUDEquityPercent.Value = 25;
            NUDEquityPercent.EndInit();

            ChbMinTrades = new CheckBox
                               {
                                   Parent = PnlLimitations,
                                   ForeColor = ColorText,
                                   BackColor = Color.Transparent,
                                   Text = Language.T("Minimum number of trades"),
                                   Checked = true,
                                   AutoSize = true
                               };

            NUDMinTrades = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDMinTrades.BeginInit();
            NUDMinTrades.Minimum = 10;
            NUDMinTrades.Maximum = 1000;
            NUDMinTrades.Increment = 10;
            NUDMinTrades.Value = 100;
            NUDMinTrades.EndInit();

            ChbMaxTrades = new CheckBox
                               {
                                   Parent = PnlLimitations,
                                   ForeColor = ColorText,
                                   BackColor = Color.Transparent,
                                   Text = Language.T("Maximum number of trades"),
                                   Checked = false,
                                   AutoSize = true
                               };

            NUDMaxTrades = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDMaxTrades.BeginInit();
            NUDMaxTrades.Minimum = 10;
            NUDMaxTrades.Maximum = 10000;
            NUDMaxTrades.Increment = 10;
            NUDMaxTrades.Value = 1000;
            NUDMaxTrades.EndInit();

            ChbWinLossRatio = new CheckBox
                                  {
                                      Parent = PnlLimitations,
                                      ForeColor = ColorText,
                                      BackColor = Color.Transparent,
                                      Text = Language.T("Minimum win / loss trades ratio"),
                                      Checked = false,
                                      AutoSize = true
                                  };

            NUDWinLossRatio = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDWinLossRatio.BeginInit();
            NUDWinLossRatio.Minimum = 0.10M;
            NUDWinLossRatio.Maximum = 1;
            NUDWinLossRatio.Increment = 0.01M;
            NUDWinLossRatio.Value = 0.30M;
            NUDWinLossRatio.DecimalPlaces = 2;
            NUDWinLossRatio.EndInit();

            ChbOOSPatternFilter = new CheckBox
                                      {
                                          Parent = PnlLimitations,
                                          ForeColor = ColorText,
                                          BackColor = Color.Transparent,
                                          Text = Language.T("Filter bad OOS performance"),
                                          Checked = false,
                                          AutoSize = true
                                      };

            NUDOOSPatternPercent = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDOOSPatternPercent.BeginInit();
            NUDOOSPatternPercent.Minimum = 1;
            NUDOOSPatternPercent.Maximum = 50;
            NUDOOSPatternPercent.Value = 20;
            NUDOOSPatternPercent.EndInit();
            _toolTip.SetToolTip(NUDOOSPatternPercent, Language.T("Deviation percent."));

            ChbSmoothBalanceLines = new CheckBox
                                        {
                                            Parent = PnlLimitations,
                                            ForeColor = ColorText,
                                            BackColor = Color.Transparent,
                                            Text = Language.T("Filter non-linear balance pattern"),
                                            Checked = false,
                                            AutoSize = true
                                        };

            NUDSmoothBalancePercent = new NumericUpDown
                                          {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDSmoothBalancePercent.BeginInit();
            NUDSmoothBalancePercent.Minimum = 1;
            NUDSmoothBalancePercent.Maximum = 50;
            NUDSmoothBalancePercent.Value = 20;
            NUDSmoothBalancePercent.EndInit();
            _toolTip.SetToolTip(NUDSmoothBalancePercent, Language.T("Deviation percent."));

            NUDSmoothBalanceCheckPoints = new NumericUpDown
                                              {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDSmoothBalanceCheckPoints.BeginInit();
            NUDSmoothBalanceCheckPoints.Minimum = 1;
            NUDSmoothBalanceCheckPoints.Maximum = 50;
            NUDSmoothBalanceCheckPoints.Value = 1;
            NUDSmoothBalanceCheckPoints.EndInit();
            _toolTip.SetToolTip(NUDSmoothBalanceCheckPoints, Language.T("Check points count."));
        }

        /// <summary>
        /// Sets controls in panel Settings
        /// </summary>
        private void SetPanelSettings()
        {
            ChbOutOfSample = new CheckBox
                                 {
                                     Parent = PnlSettings,
                                     ForeColor = ColorText,
                                     BackColor = Color.Transparent,
                                     Text = Language.T("Out of sample testing, percent of OOS bars"),
                                     Checked = false,
                                     AutoSize = true
                                 };
            ChbOutOfSample.CheckedChanged += ChbOutOfSampleCheckedChanged;

            NUDOutOfSample = new NumericUpDown {Parent = PnlSettings, TextAlign = HorizontalAlignment.Center};
            NUDOutOfSample.BeginInit();
            NUDOutOfSample.Minimum = 10;
            NUDOutOfSample.Maximum = 60;
            NUDOutOfSample.Increment = 1;
            NUDOutOfSample.Value = 30;
            NUDOutOfSample.EndInit();
            NUDOutOfSample.ValueChanged += NudOutOfSampleValueChanged;

            ChbUseDefaultIndicatorValues = new CheckBox
                                               {
                                                   Parent = PnlSettings,
                                                   ForeColor = ColorText,
                                                   BackColor = Color.Transparent,
                                                   Text = Language.T("Only use default numeric indicator values"),
                                                   Checked = false,
                                                   AutoSize = true
                                               };

            ChbHideFSB = new CheckBox
                             {
                                 Parent = PnlSettings,
                                 ForeColor = ColorText,
                                 BackColor = Color.Transparent,
                                 Text = Language.T("Hide FSB when Generator starts"),
                                 Checked = true,
                                 AutoSize = true,
                                 Cursor = Cursors.Default
                             };

            BtnReset = new Button
                           {
                               Parent = PnlSettings,
                               UseVisualStyleBackColor = true,
                               Text = Language.T("Reset all parameters and settings")
                           };
            BtnReset.Click += BtnResetClick;
        }

        /// <summary>
        /// Sets controls in panel Top 10
        /// </summary>
        private void SetPanelTop10()
        {
            Top10Field = new Top10Layout(10) {Parent = PnlTop10};
        }

        /// <summary>
        /// Sets controls in panel Indicators
        /// </summary>
        private void SetPanelIndicators()
        {
            IndicatorsField = new IndicatorsLayout {Parent = PnlIndicators};
        }

        /// <summary>
        /// Sets tool strip buttons
        /// </summary>
        private void SetButtonsStrategy()
        {
            TsbtLockAll = new ToolStripButton
                              {
                                  Name = "tsbtLockAll",
                                  Image = Resources.padlock_img,
                                  ToolTipText = Language.T("Lock all slots.")
                              };
            TsbtLockAll.Click += ChangeSlotStatus;
            TsStrategy.Items.Add(TsbtLockAll);

            TsbtUnlockAll = new ToolStripButton
                                {
                                    Name = "tsbtUnlockAll",
                                    Image = Resources.open_padlock_img,
                                    ToolTipText = Language.T("Unlock all slots.")
                                };
            TsbtUnlockAll.Click += ChangeSlotStatus;
            TsStrategy.Items.Add(TsbtUnlockAll);

            TsbtLinkAll = new ToolStripButton
                              {
                                  Name = "tsbtLinkAll",
                                  Image = Resources.linked,
                                  ToolTipText = Language.T("Link all slots.")
                              };
            TsbtLinkAll.Click += ChangeSlotStatus;
            TsStrategy.Items.Add(TsbtLinkAll);

            TsStrategy.Items.Add(new ToolStripSeparator());

            // Button Overview
            TsbtOverview = new ToolStripButton
                               {
                                   Name = "Overview",
                                   Text = Language.T("Overview"),
                                   ToolTipText = Language.T("See the strategy overview.")
                               };
            TsbtOverview.Click += ShowOverview;
            TsStrategy.Items.Add(TsbtOverview);

            // Button tsbtStrategySize1
            TsbtStrategySize1 = new ToolStripButton
                                    {
                                        DisplayStyle = ToolStripItemDisplayStyle.Image,
                                        Image = Resources.slot_size_max,
                                        Tag = 1,
                                        ToolTipText = Language.T("Show detailed info in the slots."),
                                        Alignment = ToolStripItemAlignment.Right
                                    };
            TsbtStrategySize1.Click += BtnSlotSizeClick;
            TsStrategy.Items.Add(TsbtStrategySize1);

            // Button tsbtStrategySize2
            TsbtStrategySize2 = new ToolStripButton
                                    {
                                        DisplayStyle = ToolStripItemDisplayStyle.Image,
                                        Image = Resources.slot_size_min,
                                        Tag = 2,
                                        ToolTipText = Language.T("Show minimum info in the slots."),
                                        Alignment = ToolStripItemAlignment.Right
                                    };
            TsbtStrategySize2.Click += BtnSlotSizeClick;
            TsStrategy.Items.Add(TsbtStrategySize2);

            // Button tsbtStrategyInfo
            TsbtStrategyInfo = new ToolStripButton
                                   {
                                       DisplayStyle = ToolStripItemDisplayStyle.Image,
                                       Image = Resources.str_info_infook,
                                       ToolTipText = Language.T("Show the strategy description."),
                                       Alignment = ToolStripItemAlignment.Right
                                   };
            TsbtStrategyInfo.Click += BtnStrategyDescriptionClick;
            TsStrategy.Items.Add(TsbtStrategyInfo);
        }

        /// <summary>
        /// Sets tool strip buttons
        /// </summary>
        private void SetButtonsGenerator()
        {
            // Button Options
            TsbtShowOptions = new ToolStripButton {Name = "tsbtShowOptions", Text = Language.T("Common"), Enabled = false};
            TsbtShowOptions.Click += ChangeGeneratorPanel;
            TsGenerator.Items.Add(TsbtShowOptions);

            // Button Limitations
            TsbtShowLimitations = new ToolStripButton {Name = "tsbtShowLimitations", Text = Language.T("Limitations")};
            TsbtShowLimitations.Click += ChangeGeneratorPanel;
            TsGenerator.Items.Add(TsbtShowLimitations);

            // Button Settings
            TsbtShowSettings = new ToolStripButton {Name = "tsbtShowSettings", Text = Language.T("Settings")};
            TsbtShowSettings.Click += ChangeGeneratorPanel;
            TsGenerator.Items.Add(TsbtShowSettings);

            // Button Top10
            TsbtShowTop10 = new ToolStripButton {Name = "tsbtShowTop10", Text = Language.T("Top 10")};
            TsbtShowTop10.Click += ChangeGeneratorPanel;
            TsGenerator.Items.Add(TsbtShowTop10);

            // Button Indicators
            TsbtShowIndicators = new ToolStripButton {Name = "tsbtIndicators", Text = Language.T("Indicators")};
            TsbtShowIndicators.Click += ChangeGeneratorPanel;
            TsGenerator.Items.Add(TsbtShowIndicators);
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ParrentForm.Visible = !ChbHideFSB.Checked;

            // Find correct size
            int maxCheckBoxWidth = 250;
            foreach (Control control in PnlLimitations.Controls)
            {
                if (maxCheckBoxWidth < control.Width)
                    maxCheckBoxWidth = control.Width;
            }
            foreach (Control control in PnlCommon.Controls)
            {
                if (maxCheckBoxWidth < control.Width)
                    maxCheckBoxWidth = control.Width;
            }

            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            const int nudWidth = 55;
            PnlLimitations.Width = 3*buttonWidth + 2*btnHrzSpace;
            int iBorderWidth = (PnlLimitations.Width - PnlLimitations.ClientSize.Width)/2;

            if (maxCheckBoxWidth + 3*btnHrzSpace + nudWidth + 4 > PnlLimitations.ClientSize.Width)
                _buttonWidthMultiplier = ((maxCheckBoxWidth + nudWidth + 3*btnHrzSpace + 2*iBorderWidth + 4)/3.0)/
                                        buttonWidth;

            ClientSize = new Size(2*((int) (3*buttonWidth*_buttonWidthMultiplier) + 2*btnHrzSpace) + 3*btnHrzSpace, 528);

            OnResize(e);

            RebuildStrategyLayout(_strategyBest);
            RefreshAccountStatisticas();
            Top10AddStrategy();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60*_buttonWidthMultiplier);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int border = btnHrzSpace;
            int rightSideWidth = 3*buttonWidth + 2*btnHrzSpace;
            int rightSideLocation = ClientSize.Width - rightSideWidth - btnHrzSpace;
            int leftSideWidth = ClientSize.Width - 3*buttonWidth - 5*btnHrzSpace;
            const int nudWidth = 55;
            const int optionsHeight = 228;

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Generate
            BtnGenerate.Size = new Size(buttonWidth, buttonHeight);
            BtnGenerate.Location = new Point(BtnAccept.Left - buttonWidth - btnHrzSpace,
                                             ClientSize.Height - buttonHeight - btnVertSpace);

            // Progress Bar
            ProgressBar.Size = new Size(ClientSize.Width - leftSideWidth - 3*border, (int) (Data.VerticalDLU*9));
            ProgressBar.Location = new Point(leftSideWidth + 2*border, BtnAccept.Top - ProgressBar.Height - btnVertSpace);

            // Tool Strip Strategy
            TsStrategy.Width = leftSideWidth + border;
            TsStrategy.Location = Point.Empty;

            // Tool Strip Strategy
            TsGenerator.Width = ClientSize.Width - leftSideWidth - border;
            TsGenerator.Location = new Point(TsStrategy.Right + border, 0);

            // Panel Common
            PnlCommon.Size = new Size(rightSideWidth, optionsHeight);
            PnlCommon.Location = new Point(rightSideLocation, TsStrategy.Bottom + border);

            // Panel pnlLimitations
            PnlLimitations.Size = new Size(rightSideWidth, optionsHeight);
            PnlLimitations.Location = new Point(rightSideLocation, TsStrategy.Bottom + border);

            // Panel Settings
            PnlSettings.Size = new Size(rightSideWidth, optionsHeight);
            PnlSettings.Location = new Point(rightSideLocation, TsStrategy.Bottom + border);

            // Panel Top 10
            PnlTop10.Size = new Size(rightSideWidth, optionsHeight);
            PnlTop10.Location = new Point(rightSideLocation, TsStrategy.Bottom + border);

            // Panel Indicators
            PnlIndicators.Size = new Size(rightSideWidth, optionsHeight);
            PnlIndicators.Location = new Point(rightSideLocation, TsStrategy.Bottom + border);

            // Panel StrategyLayout
            StrategyField.Size = new Size(leftSideWidth, ClientSize.Height - border - TsStrategy.Bottom - border);
            StrategyField.Location = new Point(border, TsStrategy.Bottom + border);

            // Panel Balance Chart
            BalanceChart.Size = new Size(ClientSize.Width - leftSideWidth - 3*border,
                                              ProgressBar.Top - 3*border - PnlCommon.Bottom);
            BalanceChart.Location = new Point(StrategyField.Right + border, PnlCommon.Bottom + border);

            // Account Statistics
            InfpnlAccountStatistics.Size = new Size(ClientSize.Width - leftSideWidth - 3*border,
                                                    ProgressBar.Top - 3*border - PnlCommon.Bottom);
            InfpnlAccountStatistics.Location = new Point(StrategyField.Right + border, PnlCommon.Bottom + border);

            //chbGenerateNewStrategy
            ChbGenerateNewStrategy.Location = new Point(border + 2, 26);

            //chbPreservPermSL
            ChbPreservPermSL.Location = new Point(border + 2, ChbGenerateNewStrategy.Bottom + border + 4);

            //chbPreservPermTP
            ChbPreservPermTP.Location = new Point(border + 2, ChbPreservPermSL.Bottom + border + 4);

            //chbPreservbreakEven
            ChbPreservBreakEven.Location = new Point(border + 2, ChbPreservPermTP.Bottom + border + 4);

            // ChbInitialOptimisation
            ChbInitialOptimisation.Location = new Point(border + 2, ChbPreservBreakEven.Bottom + border + 4);

            // ChbMaxOpeningLogicSlots
            ChbMaxOpeningLogicSlots.Location = new Point(border + 2, ChbInitialOptimisation.Bottom + border + 4);

            // NUDMaxOpeningLogicSlots
            NUDMaxOpeningLogicSlots.Width = nudWidth;
            NUDMaxOpeningLogicSlots.Location = new Point(NUDAmbiguousBars.Left, ChbMaxOpeningLogicSlots.Top - 1);

            // ChbMaxClosingLogicSlots
            ChbMaxClosingLogicSlots.Location = new Point(border + 2, ChbMaxOpeningLogicSlots.Bottom + border + 4);

            // NUDMaxClosingLogicSlots
            NUDMaxClosingLogicSlots.Width = nudWidth;
            NUDMaxClosingLogicSlots.Location = new Point(NUDAmbiguousBars.Left, ChbMaxClosingLogicSlots.Top - 1);

            // Labels Strategy Calculations
            LblCalcStrInfo.Location = new Point(border - 1, PnlCommon.Height - NUDMaxOpeningLogicSlots.Height - border);
            LblCalcStrNumb.Size = new Size(nudWidth, NUDMaxOpeningLogicSlots.Height - 1);
            LblCalcStrNumb.Location = new Point(LblCalcStrInfo.Right + border, LblCalcStrInfo.Top - 3);

            //Working Minutes
            NUDWorkingMinutes.Width = nudWidth;
            NUDWorkingMinutes.Location = new Point(NUDMaxOpeningLogicSlots.Right - nudWidth, LblCalcStrInfo.Top - 2);
            LblWorkingMinutes.Location = new Point(NUDWorkingMinutes.Left - LblWorkingMinutes.Width - 3,
                                                   LblCalcStrInfo.Top);

            // ChbAmbiguousBars
            ChbAmbiguousBars.Location = new Point(border + 2, 25);

            // NUDAmbiguousBars
            NUDAmbiguousBars.Width = nudWidth;
            NUDAmbiguousBars.Location = new Point(PnlLimitations.ClientSize.Width - nudWidth - border - 2,
                                                  ChbAmbiguousBars.Top - 1);

            // MaxDrawdown
            ChbMaxDrawdown.Location = new Point(border + 2, ChbAmbiguousBars.Bottom + border + 4);
            NUDMaxDrawdown.Width = nudWidth;
            NUDMaxDrawdown.Location = new Point(NUDAmbiguousBars.Left, ChbMaxDrawdown.Top - 1);

            // MaxDrawdown %
            ChbEquityPercent.Location = new Point(border + 2, NUDMaxDrawdown.Bottom + border + 4);
            NUDEquityPercent.Width = nudWidth;
            NUDEquityPercent.Location = new Point(NUDAmbiguousBars.Left, ChbEquityPercent.Top - 1);

            // MinTrades
            ChbMinTrades.Location = new Point(border + 2, ChbEquityPercent.Bottom + border + 4);
            NUDMinTrades.Width = nudWidth;
            NUDMinTrades.Location = new Point(NUDAmbiguousBars.Left, ChbMinTrades.Top - 1);

            // MaxTrades
            ChbMaxTrades.Location = new Point(border + 2, ChbMinTrades.Bottom + border + 4);
            NUDMaxTrades.Width = nudWidth;
            NUDMaxTrades.Location = new Point(NUDAmbiguousBars.Left, ChbMaxTrades.Top - 1);

            // WinLossRatios
            ChbWinLossRatio.Location = new Point(border + 2, ChbMaxTrades.Bottom + border + 4);
            NUDWinLossRatio.Width = nudWidth;
            NUDWinLossRatio.Location = new Point(NUDAmbiguousBars.Left, ChbWinLossRatio.Top - 1);

            // OOS Pattern Filter
            ChbOOSPatternFilter.Location = new Point(border + 2, ChbWinLossRatio.Bottom + border + 4);
            NUDOOSPatternPercent.Width = nudWidth;
            NUDOOSPatternPercent.Location = new Point(NUDAmbiguousBars.Left, ChbOOSPatternFilter.Top - 1);

            // Balance lines pattern
            ChbSmoothBalanceLines.Location = new Point(border + 2, ChbOOSPatternFilter.Bottom + border + 4);
            NUDSmoothBalancePercent.Width = nudWidth;
            NUDSmoothBalancePercent.Location = new Point(NUDAmbiguousBars.Left, ChbSmoothBalanceLines.Top - 1);
            NUDSmoothBalanceCheckPoints.Width = nudWidth;
            NUDSmoothBalanceCheckPoints.Location = new Point(NUDSmoothBalancePercent.Left - nudWidth - border,
                                                             ChbSmoothBalanceLines.Top - 1);

            // chbOutOfSample
            ChbOutOfSample.Location = new Point(border + 2, 25);
            NUDOutOfSample.Width = nudWidth;
            NUDOutOfSample.Location = new Point(NUDAmbiguousBars.Left, ChbOutOfSample.Top - 1);

            // Use default indicator values
            ChbUseDefaultIndicatorValues.Location = new Point(border + 2, ChbOutOfSample.Bottom + border + 4);

            // Hide FSB when generator starts
            ChbHideFSB.Location = new Point(border + 2, ChbUseDefaultIndicatorValues.Bottom + border + 4);

            // Button Reset
            BtnReset.Width = PnlSettings.ClientSize.Width - 2*(border + 2);
            BtnReset.Location = new Point(border + 2, PnlSettings.Height - BtnReset.Height - border - 2);

            // Top 10 Layout
            Top10Field.Size = new Size(PnlTop10.Width - 2*2, PnlTop10.Height - (int) PnlTop10.CaptionHeight - 2);
            Top10Field.Location = new Point(2, (int) PnlTop10.CaptionHeight);

            // Indicators Layout
            IndicatorsField.Size = new Size(PnlIndicators.Width - 2*2,
                                             PnlIndicators.Height - (int) PnlIndicators.CaptionHeight - 2);
            IndicatorsField.Location = new Point(2, (int) PnlIndicators.CaptionHeight);
        }

        private void BtnAcceptOnClick(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void BtnCancelOnClick(object sender, EventArgs eventArgs)
        {
            Close();
        }

        /// <summary>
        /// Check whether the strategy have been changed
        /// </summary>
        private void GeneratorFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isReset)
                SaveOptions();

            if (_isGenerating)
            {
                // Cancel the asynchronous operation.
                BgWorker.CancelAsync();
                e.Cancel = true;
                return;
            }

            if (DialogResult == DialogResult.Cancel && _isStartegyChanged)
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept the generated strategy?"),
                                                  Data.ProgramName, MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Question);

                switch (dr)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
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

            if (!_isReset)
                IndicatorsField.SetConfigFile();

            _backtester.Strategy = ClearStrategySlotsStatus(_backtester.Strategy);
            ParrentForm.Visible = true;
        }

        /// <summary>
        /// Refreshes the balance chart
        /// </summary>
        private void RefreshSmallBalanceChart()
        {
            if (BalanceChart.InvokeRequired)
            {
                Invoke(new DelegateRefreshBalanceChart(RefreshSmallBalanceChart), new object[] {});
            }
            else
            {
                BalanceChart.SetChartData(_backtester);
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
            }
        }

        /// <summary>
        /// Refreshes the AccountStatistics
        /// </summary>
        private void RefreshAccountStatisticas()
        {
            if (InfpnlAccountStatistics.InvokeRequired)
            {
                Invoke(new DelegateRefreshAccountStatisticas(RefreshAccountStatisticas), new object[] {});
            }
            else
            {
                InfpnlAccountStatistics.Update(
                    _backtester.AccountStatsParam,
                    _backtester.AccountStatsValue,
                    _backtester.AccountStatsFlags,
                    Language.T("Account Statistics"));
            }
        }

        /// <summary>
        /// Creates a new strategy layout according to the given strategy.
        /// </summary>
        private void RebuildStrategyLayout(Strategy strategy)
        {
            if (StrategyField.InvokeRequired)
            {
                Invoke(new DelegateRebuildStrategyLayout(RebuildStrategyLayout), new object[] {strategy});
            }
            else
            {
                StrategyField.RebuildStrategyControls(strategy);
                StrategyField.PanelProperties.Click += PnlPropertiesClick;
                StrategyField.PanelProperties.DoubleClick += PnlPropertiesClick;
                for (int slot = 0; slot < strategy.Slots; slot++)
                {
                    StrategyField.SlotPanelsList[slot].Click += PnlSlotClick;
                    StrategyField.SlotPanelsList[slot].DoubleClick += PnlSlotClick;
                }
            }
        }

        /// <summary>
        /// Sets the lblCalcStrNumb.Text
        /// </summary>
        private void SetLabelCyclesText(string text)
        {
            if (LblCalcStrNumb.InvokeRequired)
            {
                BeginInvoke(new SetCyclesCallback(SetLabelCyclesText), new object[] {text});
            }
            else
            {
                LblCalcStrNumb.Text = text;
            }
        }

        private ToolStripItem[] GetBalanceChartContextMenuItems()
        {
            var mi1 = new ToolStripMenuItem
            {
                Image = Resources.info_panel,
                Text = Language.T("Account Statistics")
            };
            mi1.Click += AccountAutputClick;

            var itemCollection = new ToolStripItem[]
            {
                mi1
            };

            return itemCollection;
        }

        private ToolStripItem[] GetInfoPanelContextMenuItems()
        {
            var mi1 = new ToolStripMenuItem
            {
                Image = Resources.chart_balance_equity,
                Text = Language.T("Account Chart")
            };
            mi1.Click += AccountAutputClick;

            var itemCollection = new ToolStripItem[]
            {
                mi1
            };

            return itemCollection;
        }

        /// <summary>
        /// Composes an informative error message. It presumes that the reason for the error is a custom indicator. Ohhh!!
        /// </summary>
        private string GenerateCalculationErrorMessage(string exceptionMessage)
        {
            string text = "<h1>Error: " + exceptionMessage + "</h1>";
            string customIndicators = "";
            int customIndCount = 0;

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                string indName = _backtester.Strategy.Slot[slot].IndicatorName;
                Indicator indicator = IndicatorStore.ConstructIndicator(indName, _backtester.DataSet, _backtester.Strategy.Slot[slot].SlotType);
                if (indicator.CustomIndicator)
                {
                    customIndCount++;
                    _indicatorBlackList.Add(indName);
                    customIndicators += "<li>" + _backtester.Strategy.Slot[slot].IndicatorName + "</li>" + Environment.NewLine;
                }
            }

            if (customIndCount > 0)
            {
                string plural = (customIndCount > 1 ? "s" : "");

                text +=
                    "<p>" +
                    "An error occurred when calculating the strategy." + " " +
                    "The error can be a result of the following custom indicator" + plural + ":" +
                    "</p>" +
                    "<ul>" +
                    customIndicators +
                    "</ul>" +
                    "<p>" +
                    "Please report this error to the author of the indicator" + plural + "!<br />" +
                    "You may remove this indicator" + plural + " from the Custom Indicators folder." +
                    "</p>";
            }
            else
            {
                text +=
                    "<p>" +
                    "Please report this error in the support forum!" +
                    "</p>";
            }

            return text;
        }

        /// <summary>
        /// Report Indicator Error
        /// </summary>
        private void ReportIndicatorError(string text, string caption)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateReportIndicatorError(ReportIndicatorError), new object[] {text, caption});
            }
            else
            {
                var msgBox = new FancyMessageBox(text, caption) {BoxWidth = 450, BoxHeight = 250};
                msgBox.Show();
            }
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        private void NudOutOfSampleValueChanged(object sender, EventArgs e)
        {
            SetOOS();
            BalanceChart.OOSBar = _barOOS;

            if (_isOOS)
            {
                BalanceChart.SetChartData(_backtester);
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
            }
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        private void ChbOutOfSampleCheckedChanged(object sender, EventArgs e)
        {
            SetOOS();

            BalanceChart.IsOOS = _isOOS;
            BalanceChart.OOSBar = _barOOS;

            BalanceChart.SetChartData(_backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        private void SetOOS()
        {
            _isOOS = ChbOutOfSample.Checked;
            _barOOS = _backtester.DataSet.Bars - _backtester.DataSet.Bars*(int) NUDOutOfSample.Value/100 - 1;
            _targetBalanceRatio = 1 + (int) NUDOutOfSample.Value/100.0F;
        }

        /// <summary>
        /// Generates a description
        /// </summary>
        private string GenerateDescription()
        {
            // Description
            if (_lockedEntryFilters == 0 && _lockedExitFilters == 0 &&
                _lockedEntrySlot == null && _lockedExitSlot == null &&
                _strategyBest.PropertiesStatus == StrategySlotStatus.Open)
            {
                IsStrategyModified = false;
                GeneratedDescription = Language.T("Automatically generated on") + " ";
            }
            else
            {
                IsStrategyModified = true;
                GeneratedDescription = Language.T("Modified by the strategy generator on") + " ";
            }

            GeneratedDescription += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ".";

            if (_isOOS)
            {
                GeneratedDescription += Environment.NewLine + Language.T("Out of sample testing, percent of OOS bars") +
                                        ": " + NUDOutOfSample.Value.ToString(CultureInfo.InvariantCulture) + "%";
                GeneratedDescription += Environment.NewLine + Language.T("Balance") + ": " +
                                        (Configs.AccountInMoney
                                             ? _backtester.MoneyBalance(_barOOS).ToString("F2") + " " + Configs.AccountCurrency
                                             : _backtester.Balance(_barOOS).ToString(CultureInfo.InvariantCulture) + " " + Language.T("pips"));
                GeneratedDescription += " (" + _backtester.DataSet.Time[_barOOS].ToShortDateString() + " " +
                                        _backtester.DataSet.Time[_barOOS].ToShortTimeString() + "  " + Language.T("Bar") + ": " +
                                        _barOOS.ToString(CultureInfo.InvariantCulture) + ")";
            }

            return GeneratedDescription;
        }

        /// <summary>
        /// Toggles FSB visibility.
        /// </summary>
        private void HideFSBClick(object sender, EventArgs e)
        {
            ParrentForm.Visible = !ChbHideFSB.Checked;
        }

        /// <summary>
        /// Toggles panels.
        /// </summary>
        private void ChangeGeneratorPanel(object sender, EventArgs e)
        {
            var button = (ToolStripButton) sender;
            string name = button.Name;

            if (name == "tsbtShowOptions")
            {
                PnlCommon.Visible = true;
                PnlLimitations.Visible = false;
                PnlSettings.Visible = false;
                PnlTop10.Visible = false;
                PnlIndicators.Visible = false;

                TsbtShowOptions.Enabled = false;
                TsbtShowLimitations.Enabled = true;
                TsbtShowSettings.Enabled = true;
                TsbtShowTop10.Enabled = true;
                TsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowLimitations")
            {
                PnlCommon.Visible = false;
                PnlLimitations.Visible = true;
                PnlSettings.Visible = false;
                PnlTop10.Visible = false;
                PnlIndicators.Visible = false;

                TsbtShowOptions.Enabled = true;
                TsbtShowLimitations.Enabled = false;
                TsbtShowSettings.Enabled = true;
                TsbtShowTop10.Enabled = true;
                TsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowSettings")
            {
                PnlCommon.Visible = false;
                PnlLimitations.Visible = false;
                PnlSettings.Visible = true;
                PnlTop10.Visible = false;
                PnlIndicators.Visible = false;

                TsbtShowOptions.Enabled = true;
                TsbtShowLimitations.Enabled = true;
                TsbtShowSettings.Enabled = false;
                TsbtShowTop10.Enabled = true;
                TsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowTop10")
            {
                PnlCommon.Visible = false;
                PnlLimitations.Visible = false;
                PnlSettings.Visible = false;
                PnlTop10.Visible = true;
                PnlIndicators.Visible = false;

                TsbtShowOptions.Enabled = true;
                TsbtShowLimitations.Enabled = true;
                TsbtShowSettings.Enabled = true;
                TsbtShowTop10.Enabled = false;
                TsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtIndicators")
            {
                PnlCommon.Visible = false;
                PnlLimitations.Visible = false;
                PnlSettings.Visible = false;
                PnlTop10.Visible = false;
                PnlIndicators.Visible = true;

                TsbtShowOptions.Enabled = true;
                TsbtShowLimitations.Enabled = true;
                TsbtShowSettings.Enabled = true;
                TsbtShowTop10.Enabled = true;
                TsbtShowIndicators.Enabled = false;
            }
        }

        /// <summary>
        /// Shows strategy overview.
        /// </summary>
        private void ShowOverview(object sender, EventArgs e)
        {
            if (GeneratedDescription != string.Empty)
                _backtester.Strategy.Description = GeneratedDescription;

            var so = new Browser(Language.T("Strategy Overview"), StrategyFormatOverview.FormatOverview(_backtester, IsStrDescriptionRelevant()));
            so.Show();
        }

        /// <summary>
        /// Lock, unlock, link all slots.
        /// </summary>
        private void ChangeSlotStatus(object sender, EventArgs e)
        {
            var button = (ToolStripButton) sender;
            string name = button.Name;

            if (name == "tsbtLockAll")
            {
                _strategyBest.PropertiesStatus = StrategySlotStatus.Locked;
                for (int slot = 0; slot < _strategyBest.Slots; slot++)
                    _strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Locked;
            }
            else if (name == "tsbtUnlockAll")
            {
                _strategyBest.PropertiesStatus = StrategySlotStatus.Open;
                for (int slot = 0; slot < _strategyBest.Slots; slot++)
                    _strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Open;
            }
            else if (name == "tsbtLinkAll")
            {
                _strategyBest.PropertiesStatus = StrategySlotStatus.Open;
                for (int slot = 0; slot < _strategyBest.Slots; slot++)
                    _strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Linked;
            }

            StrategyField.RepaintStrategyControls(_strategyBest);
        }

        /// <summary>
        /// Lock, link, or unlock the strategy slot.
        /// </summary>
        private void PnlSlotClick(object sender, EventArgs e)
        {
            if (_isGenerating)
                return;

            var slot = (int) ((Panel) sender).Tag;

            if (_strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Open)
                _strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Locked;
            else if (_strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Locked)
                _strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Linked;
            else if (_strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                _strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Open;

            StrategyField.RepaintStrategyControls(_strategyBest);
        }

        /// <summary>
        /// Lock, link, or unlock the strategy properties slot.
        /// </summary>
        private void PnlPropertiesClick(object sender, EventArgs e)
        {
            if (_isGenerating)
                return;

            if (_strategyBest.PropertiesStatus == StrategySlotStatus.Open)
                _strategyBest.PropertiesStatus = StrategySlotStatus.Locked;
            else if (_strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                _strategyBest.PropertiesStatus = StrategySlotStatus.Open;

            StrategyField.RepaintStrategyControls(_strategyBest);
        }

        /// <summary>
        /// Changes the slot size
        /// </summary>
        private void BtnSlotSizeClick(object sender, EventArgs e)
        {
            var tag = (int) ((ToolStripButton) sender).Tag;

            if (tag == 1)
            {
                if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.min ||
                    StrategyField.SlotMinMidMax == SlotSizeMinMidMax.mid)
                {
                    TsbtStrategySize1.Image = Resources.slot_size_mid;
                    TsbtStrategySize1.ToolTipText = Language.T("Show regular info in the slots.");
                    TsbtStrategySize2.Image = Resources.slot_size_min;
                    TsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.max;
                }
                else if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    TsbtStrategySize1.Image = Resources.slot_size_max;
                    TsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    TsbtStrategySize2.Image = Resources.slot_size_min;
                    TsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.mid;
                }
            }
            else
            {
                if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.min)
                {
                    TsbtStrategySize1.Image = Resources.slot_size_max;
                    TsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    TsbtStrategySize2.Image = Resources.slot_size_min;
                    TsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.mid;
                }
                else if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.mid ||
                         StrategyField.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    TsbtStrategySize1.Image = Resources.slot_size_max;
                    TsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    TsbtStrategySize2.Image = Resources.slot_size_mid;
                    TsbtStrategySize2.ToolTipText = Language.T("Show regular info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.min;
                }
            }

            StrategyField.RearangeStrategyControls();
        }

        /// <summary>
        /// View and edit the strategy description
        /// </summary>
        private void BtnStrategyDescriptionClick(object sender, EventArgs e)
        {
            if (GeneratedDescription != string.Empty)
                _backtester.Strategy.Description = GeneratedDescription;
            var si = new StrategyDescription(_backtester);
            si.ShowDialog();
            GeneratedDescription = _backtester.Strategy.Description;
        }

        /// <summary>
        /// Sets the strategy description button icon
        /// </summary>
        private void SetStrategyDescriptionButton()
        {
            if (GeneratedDescription != string.Empty)
                _backtester.Strategy.Description = GeneratedDescription;

            if (_backtester.Strategy.Description == "")
                TsbtStrategyInfo.Image = Resources.str_info_noinfo;
            else
            {
                TsbtStrategyInfo.Image = IsStrDescriptionRelevant() ? Resources.str_info_infook : Resources.str_info_warning;
            }
        }

        /// <summary>
        /// Clears the slots status of the given strategy.
        /// </summary>
        private Strategy ClearStrategySlotsStatus(Strategy strategy)
        {
            Strategy tempStrategy = strategy.Clone();
            tempStrategy.PropertiesStatus = StrategySlotStatus.Open;
            foreach (IndicatorSlot slot in tempStrategy.Slot)
                slot.SlotStatus = StrategySlotStatus.Open;

            return tempStrategy;
        }

        /// <summary>
        /// Adds a strategy to Top 10 list.
        /// </summary>
        private void Top10AddStrategy()
        {
            if (Top10Field.InvokeRequired)
            {
                Invoke(new DelegateTop10AddStrategy(Top10AddStrategy), new object[] {});
            }
            else
            {
                var top10Slot = new Top10Slot {Width = 290, Height = 65};
                top10Slot.InitSlot(_backtester);
                top10Slot.Click += Top10SlotClick;
                top10Slot.DoubleClick += Top10SlotClick;
                var top10StrategyInfo = new Top10StrategyInfo
                                            {
                                                Balance = Configs.AccountInMoney
                                                              ? (int) Math.Round(_backtester.NetMoneyBalance)
                                                              : _backtester.NetBalance,
                                                Top10Slot = top10Slot,
                                                TheStrategy = _backtester.Strategy.Clone()
                                            };
                Top10Field.AddStrategyInfo(top10StrategyInfo);
            }
        }

        /// <summary>
        /// Loads a strategy from the clicked Top 10 slot.
        /// </summary>
        private void Top10SlotClick(object sender, EventArgs e)
        {
            if (_isGenerating)
                return;

            var top10Slot = (Top10Slot) sender;

            if (top10Slot.IsSelected)
                return;

            Top10Field.ClearSelectionOfSelectedSlot();

            top10Slot.IsSelected = true;
            top10Slot.Invalidate();

            _backtester.Strategy = Top10Field.GetStrategy(top10Slot.Balance);
            _bestBalance = 0;
            CalculateTheResult(true);
        }

        /// <summary>
        /// Toggles the account chart and statistics.
        /// </summary>
        private void AccountAutputClick(object sender, EventArgs e)
        {
            bool isChartVisible = BalanceChart.Visible;
            BalanceChart.Visible = !isChartVisible;
            InfpnlAccountStatistics.Visible = isChartVisible;
        }

        /// <summary>
        /// Resets Generator
        /// </summary>
        private void BtnResetClick(object sender, EventArgs e)
        {
            Configs.GeneratorOptions = "";
            Configs.BannedIndicators = "";
            _isReset = true;
            Close();
        }

        #region Nested type: DelegateRebuildStrategyLayout

        private delegate void DelegateRebuildStrategyLayout(Strategy strategy);

        #endregion

        #region Nested type: DelegateRefreshAccountStatisticas

        private delegate void DelegateRefreshAccountStatisticas();

        #endregion

        #region Nested type: DelegateRefreshBalanceChart

        private delegate void DelegateRefreshBalanceChart();

        #endregion

        #region Nested type: DelegateReportIndicatorError

        private delegate void DelegateReportIndicatorError(string text, string caption);

        #endregion

        #region Nested type: DelegateTop10AddStrategy

        private delegate void DelegateTop10AddStrategy();

        #endregion

        #region Nested type: SetCyclesCallback

        private delegate void SetCyclesCallback(string text);

        #endregion
    }
}