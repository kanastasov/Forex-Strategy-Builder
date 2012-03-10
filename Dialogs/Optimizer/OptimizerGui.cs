// Strategy Optimizer - GUI
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer - GUI
    /// </summary>
    public sealed partial class Optimizer
    {
        /// <summary>
        /// Initializes main controls of optimizer.
        /// </summary>
        private void InitializeControls()
        {
            PnlParamsBase = new Panel();
            PnlParamsBase2 = new Panel();
            PnlCaptions = new Panel();
            PnlParams = new Panel();
            PnlLimitations = new FancyPanel(Language.T("Limitations"));
            PnlSettings = new FancyPanel(Language.T("Settings"));
            ScrollBar = new VScrollBar();
            BalanceChart = new SmallBalanceChart(_backtester.DataSet);
            ProgressBar = new ProgressBar();
            BtnOptimize = new Button();
            BtnAccept = new Button();
            BtnCancel = new Button();

            LblNoParams = new Label();

            _fontIndicator = new Font(Font.FontFamily, 11);
            _colorText = LayoutColors.ColorControlText;

            // Panel Parameters Base
            PnlParamsBase.Parent = this;
            PnlParamsBase.BackColor = LayoutColors.ColorControlBack;
            PnlParamsBase.Paint += PnlParamsBasePaint;

            // Panel Captions
            PnlCaptions.Parent = PnlParamsBase;
            PnlCaptions.Dock = DockStyle.Top;
            PnlCaptions.BackColor = LayoutColors.ColorCaptionBack;
            PnlCaptions.ForeColor = LayoutColors.ColorCaptionText;
            PnlCaptions.Paint += PnlCaptionsPaint;

            // Panel Parameters Base 2
            PnlParamsBase2.Parent = PnlParamsBase;
            PnlParamsBase2.BackColor = LayoutColors.ColorControlBack;
            PnlParamsBase2.Resize += PnlParamsBase2Resize;

            // ScrollBar
            ScrollBar.Parent = PnlParamsBase2;
            ScrollBar.Dock = DockStyle.Right;
            ScrollBar.TabStop = true;
            ScrollBar.ValueChanged += ScrollBarValueChanged;
            ScrollBar.MouseWheel += ScrollBarMouseWheel;

            // Panel Parameters
            PnlParams.Parent = PnlParamsBase2;
            PnlParams.BackColor = LayoutColors.ColorControlBack;

            // Label No Parameters
            LblNoParams.Parent = PnlParams;
            LblNoParams.Text = Language.T("There are no parameters suitable for optimization.");
            LblNoParams.AutoSize = true;
            LblNoParams.Visible = false;

            // Panel Limitations
            PnlLimitations.Parent = this;
            PnlLimitations.Visible = false;

            // Panel Settings
            PnlSettings.Parent = this;
            PnlSettings.Visible = false;

            // Small Balance Chart
            BalanceChart.Parent = this;
            BalanceChart.BackColor = LayoutColors.ColorControlBack;
            BalanceChart.SetChartData(_backtester);

            // ProgressBar
            ProgressBar.Parent = this;
            ProgressBar.Minimum = 1;
            ProgressBar.Maximum = 100;
            ProgressBar.Step = 1;

            // Button Optimize
            BtnOptimize.Parent = this;
            BtnOptimize.Name = "btnOptimize";
            BtnOptimize.Text = Language.T("Optimize");
            BtnOptimize.TabIndex = 0;
            BtnOptimize.Click += BtnOptimizeClick;
            BtnOptimize.UseVisualStyleBackColor = true;

            // Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "btnAccept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.TabIndex = 1;
            BtnAccept.Enabled = false;
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;

            // Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.TabIndex = 2;
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            // BackGroundWorker
            BgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            BgWorker.DoWork += BgWorkerDoWork;
            BgWorker.ProgressChanged += BgWorkerProgressChanged;
            BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
        }

        /// <summary>
        /// Sets controls in panel Limitations
        /// </summary>
        private void SetPanelLimitations()
        {
            ChbAmbiguousBars = new CheckBox
                                   {
                                       Parent = PnlLimitations,
                                       ForeColor = _colorText,
                                       BackColor = Color.Transparent,
                                       Text = Language.T("Maximum number of ambiguous bars"),
                                       Checked = false,
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
                                     ForeColor = _colorText,
                                     BackColor = Color.Transparent,
                                     Checked = false,
                                     Text = Language.T("Maximum equity drawdown") + " [" +
                                            (Configs.AccountInMoney
                                                 ? Configs.AccountCurrency + "]"
                                                 : Language.T("pips") + "]"),
                                     AutoSize = true
                                 };

            NUDMaxDrawdown = new NumericUpDown {Parent = PnlLimitations, TextAlign = HorizontalAlignment.Center};
            NUDMaxDrawdown.BeginInit();
            NUDMaxDrawdown.Minimum = 0;
            NUDMaxDrawdown.Maximum = Configs.InitialAccount;
            NUDMaxDrawdown.Increment = 10;
            NUDMaxDrawdown.Value = Configs.InitialAccount/4M;
            NUDMaxDrawdown.EndInit();

            ChbEquityPercent = new CheckBox
                                   {
                                       Parent = PnlLimitations,
                                       ForeColor = _colorText,
                                       BackColor = Color.Transparent,
                                       Text = Language.T("Maximum equity drawdown") + " [% " + Configs.AccountCurrency +"]",
                                       Checked = false,
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
                                   ForeColor = _colorText,
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
                                   ForeColor = _colorText,
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
                                      ForeColor = _colorText,
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
                                          ForeColor = _colorText,
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
                                            ForeColor = _colorText,
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
                                     ForeColor = _colorText,
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

            ChbOptimizerWritesReport = new CheckBox
                                           {
                                               Parent = PnlSettings,
                                               ForeColor = _colorText,
                                               BackColor = Color.Transparent,
                                               Text = Language.T("Optimizer writes a report for each optimized strategy"),
                                               Checked = false,
                                               AutoSize = true
                                           };

            ChbHideFSB = new CheckBox
                             {
                                 Parent = PnlSettings,
                                 ForeColor = _colorText,
                                 BackColor = Color.Transparent,
                                 Text = Language.T("Hide FSB when Optimizer starts"),
                                 Checked = true,
                                 AutoSize = true
                             };

            BtnResetSettings = new Button
                                   {
                                       Parent = PnlSettings,
                                       UseVisualStyleBackColor = true,
                                       Text = Language.T("Reset all parameters and settings")
                                   };
            BtnResetSettings.Click += BtnResetClick;
        }

        /// <summary>
        /// Sets ups the chart's buttons.
        /// </summary>
        private void SetupButtons()
        {
            TsOptimizerButtons = new ToolStrip {Parent = this};

            AOptimizerButtons = new ToolStripButton[Enum.GetValues(typeof (OptimizerButtons)).Length];
            for (int i = 0; i < AOptimizerButtons.Length; i++)
            {
                AOptimizerButtons[i] = new ToolStripButton {Tag = (OptimizerButtons) i};
                AOptimizerButtons[i].Click += ButtonsClick;
                AOptimizerButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Text;
                TsOptimizerButtons.Items.Add(AOptimizerButtons[i]);
                if (i == (int)OptimizerButtons.SelectRandom ||
                    i == (int)OptimizerButtons.SetStep20 ||
                    i == (int)OptimizerButtons.ResetStrategy)
                    TsOptimizerButtons.Items.Add(new ToolStripSeparator());
            }

            // Select All
            AOptimizerButtons[(int) OptimizerButtons.SelectAll].Image = Resources.optimizer_select_all;
            AOptimizerButtons[(int) OptimizerButtons.SelectAll].DisplayStyle = ToolStripItemDisplayStyle.Image;
            AOptimizerButtons[(int) OptimizerButtons.SelectAll].ToolTipText = Language.T("Select all parameters.");

            // Select None
            AOptimizerButtons[(int) OptimizerButtons.SelectNone].Image = Resources.optimizer_select_none;
            AOptimizerButtons[(int) OptimizerButtons.SelectNone].DisplayStyle = ToolStripItemDisplayStyle.Image;
            AOptimizerButtons[(int) OptimizerButtons.SelectNone].ToolTipText = Language.T("Select none of the parameters.");

            // Select Random
            AOptimizerButtons[(int) OptimizerButtons.SelectRandom].Image = Resources.optimizer_select_random;
            AOptimizerButtons[(int) OptimizerButtons.SelectRandom].DisplayStyle = ToolStripItemDisplayStyle.Image;
            AOptimizerButtons[(int) OptimizerButtons.SelectRandom].ToolTipText = Language.T("Select a random number of parameters.");

            // Set step 5
            AOptimizerButtons[(int) OptimizerButtons.SetStep5].Text = "±5";
            AOptimizerButtons[(int) OptimizerButtons.SetStep5].ToolTipText = Language.T("Set Min / Max ± # steps.").Replace("#", "5");

            // Set step 10
            AOptimizerButtons[(int) OptimizerButtons.SetStep10].Text = "±10";
            AOptimizerButtons[(int) OptimizerButtons.SetStep10].ToolTipText = Language.T("Set Min / Max ± # steps.").Replace("#", "10");

            // Set step 15
            AOptimizerButtons[(int) OptimizerButtons.SetStep15].Text = "±15";
            AOptimizerButtons[(int) OptimizerButtons.SetStep15].ToolTipText = Language.T("Set Min / Max ± # steps.").Replace("#", "15");

            // Set step 20
            AOptimizerButtons[(int) OptimizerButtons.SetStep20].Text = "±20";
            AOptimizerButtons[(int) OptimizerButtons.SetStep20].ToolTipText = Language.T("Set Min / Max ± # steps.").Replace("#", "20");

            // Reset Strategy
            AOptimizerButtons[(int) OptimizerButtons.ResetStrategy].Image = Resources.refresh;
            AOptimizerButtons[(int) OptimizerButtons.ResetStrategy].DisplayStyle = ToolStripItemDisplayStyle.Image;
            AOptimizerButtons[(int) OptimizerButtons.ResetStrategy].ToolTipText = Language.T("Reset strategy parameters.");

            // Show Parameters
            AOptimizerButtons[(int) OptimizerButtons.ShowParams].Text = Language.T("Parameters");
            AOptimizerButtons[(int) OptimizerButtons.ShowParams].ToolTipText = Language.T("Show indicator parameters.");
            AOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = false;

            // Show Limitations
            AOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Text = Language.T("Limitations");
            AOptimizerButtons[(int) OptimizerButtons.ShowLimitations].ToolTipText =
                Language.T("Show strategy limitations.");

            // Show Settings
            AOptimizerButtons[(int) OptimizerButtons.ShowSettings].Text = Language.T("Settings");
            AOptimizerButtons[(int) OptimizerButtons.ShowSettings].ToolTipText = Language.T("Show optimizer settings.");
        }

        /// <summary>
        /// Loads and parses the optimizer's options.
        /// </summary>
        private void LoadOptions()
        {
            if (string.IsNullOrEmpty(Configs.OptimizerOptions))
                return;

            string[] options = Configs.OptimizerOptions.Split(';');
            int i = 0;

            try
            {
                if (int.Parse(options[i++]) < OptionsVersion) return;
                ChbOutOfSample.Checked = bool.Parse(options[i++]);
                NUDOutOfSample.Value = int.Parse(options[i++]);
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
                ChbOptimizerWritesReport.Checked = bool.Parse(options[i++]);
                ChbHideFSB.Checked = bool.Parse(options[i++]);
                _formHeight = int.Parse(options[i++]);
                _lastSelectButton = (OptimizerButtons) Enum.Parse(typeof (OptimizerButtons), options[i++]);
                _lastSetStepButtonValue = int.Parse(options[i]);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Optimizer.LoadOptions: " + exception.Message);
            }
        }

        /// <summary>
        /// Saves the generator's options.
        /// </summary>
        private void SaveOptions()
        {
            string options =
                OptionsVersion + ";" +
                ChbOutOfSample.Checked + ";" +
                NUDOutOfSample.Value + ";" +
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
                ChbOptimizerWritesReport.Checked + ";" +
                ChbHideFSB.Checked + ";" +
                Height + ";" +
                _lastSelectButton + ";" +
                _lastSetStepButtonValue;

            Configs.OptimizerOptions = options;
        }

        /// <summary>
        /// Paints pnlCaptions
        /// </summary>
        private void PnlCaptionsPaint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            ColorMagic.GradientPaint(g, pnl.ClientRectangle, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            Brush brush = new SolidBrush(pnl.ForeColor);
            var stringFormat = new StringFormat {Alignment = StringAlignment.Center};

            g.DrawString(Language.T("Parameter"), Font, brush, 25, 3);
            g.DrawString(Language.T("Initial"), Font, brush, 190, 3, stringFormat);
            g.DrawString(Language.T("Value"), Font, brush, 250, 3, stringFormat);
            g.DrawString(Language.T("Minimum"), Font, brush, 293, 3);
            g.DrawString(Language.T("Maximum"), Font, brush, 367, 3);
            g.DrawString(Language.T("Step"), Font, brush, 452, 3);
        }

        /// <summary>
        /// Paints PnlParamsBase
        /// </summary>
        private void PnlParamsBasePaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);

            g.DrawLine(penBorder, 1, 0, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - Border + 1, 0, pnl.ClientSize.Width - Border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - Border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - Border + 1);
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        private void NudOutOfSampleValueChanged(object sender, EventArgs e)
        {
            _isOOS = ChbOutOfSample.Checked;
            _barOOS = _backtester.DataSet.Bars - _backtester.DataSet.Bars*(int) NUDOutOfSample.Value/100 - 1;

            BalanceChart.OOSBar = _barOOS;

            if (!_isOOS) return;
            BalanceChart.SetChartData(_backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        private void ChbOutOfSampleCheckedChanged(object sender, EventArgs e)
        {
            _isOOS = ChbOutOfSample.Checked;
            _barOOS = _backtester.DataSet.Bars - _backtester.DataSet.Bars*(int) NUDOutOfSample.Value/100 - 1;

            BalanceChart.IsOOS = _isOOS;
            BalanceChart.OOSBar = _barOOS;

            BalanceChart.SetChartData(_backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
        }

        /// <summary>
        /// Changes chart's settings after a button click.
        /// </summary>
        private void ButtonsClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;
            var button = (OptimizerButtons) btn.Tag;

            switch (button)
            {
                case OptimizerButtons.ShowParams:
                    PnlParamsBase.Visible = true;
                    PnlLimitations.Visible = false;
                    PnlSettings.Visible = false;
                    if (_isOptimizing == false)
                        for (int i = 0; i <= (int) OptimizerButtons.SetStep15; i++)
                            AOptimizerButtons[i].Enabled = true;
                    AOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = false;
                    AOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Enabled = true;
                    AOptimizerButtons[(int) OptimizerButtons.ShowSettings].Enabled = true;
                    break;
                case OptimizerButtons.ShowLimitations:
                    PnlParamsBase.Visible = false;
                    PnlLimitations.Visible = true;
                    PnlSettings.Visible = false;
                    for (int i = 0; i <= (int) OptimizerButtons.SetStep15; i++)
                        AOptimizerButtons[i].Enabled = false;
                    AOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = true;
                    AOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Enabled = false;
                    AOptimizerButtons[(int) OptimizerButtons.ShowSettings].Enabled = true;
                    break;
                case OptimizerButtons.ShowSettings:
                    PnlParamsBase.Visible = false;
                    PnlLimitations.Visible = false;
                    PnlSettings.Visible = true;
                    for (int i = 0; i <= (int) OptimizerButtons.SetStep15; i++)
                        AOptimizerButtons[i].Enabled = false;
                    AOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = true;
                    AOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Enabled = true;
                    AOptimizerButtons[(int) OptimizerButtons.ShowSettings].Enabled = false;
                    break;
            }

            if (_isOptimizing)
                return;

            switch (button)
            {
                case OptimizerButtons.SelectAll:
                    SelectParameters(OptimizerButtons.SelectAll);
                    _lastSelectButton = OptimizerButtons.SelectAll;
                    break;
                case OptimizerButtons.SelectNone:
                    SelectParameters(OptimizerButtons.SelectNone);
                    _lastSelectButton = OptimizerButtons.SelectNone;
                    break;
                case OptimizerButtons.SelectRandom:
                    SelectParameters(OptimizerButtons.SelectRandom);
                    _lastSelectButton = OptimizerButtons.SelectRandom;
                    break;
                case OptimizerButtons.SetStep5:
                    SetParamsMinMax(5);
                    _lastSetStepButtonValue = 5;
                    break;
                case OptimizerButtons.SetStep10:
                    SetParamsMinMax(10);
                    _lastSetStepButtonValue = 10;
                    break;
                case OptimizerButtons.SetStep15:
                    SetParamsMinMax(15);
                    _lastSetStepButtonValue = 15;
                    break;
                case OptimizerButtons.SetStep20:
                    SetParamsMinMax(20);
                    _lastSetStepButtonValue = 20;
                    break;
                case OptimizerButtons.ResetStrategy:
                    ResetStrategyParameters();
                    break;
            }
        }

        /// <summary>
        /// Check Box checked changed
        /// </summary>
        private void OptimizerCheckedChanged(object sender, EventArgs e)
        {
            BtnOptimize.Focus();
        }

        /// <summary>
        /// Arranges the controls into the pnlParams
        /// </summary>
        private void PnlParamsBase2Resize(object sender, EventArgs e)
        {
            if (PnlParams.Height > PnlParamsBase2.Height)
            {
                ScrollBar.Maximum = PnlParams.Height - PnlParamsBase2.Height + 40;
                ScrollBar.Value = 0;
                ScrollBar.SmallChange = 20;
                ScrollBar.LargeChange = 40;
                ScrollBar.Visible = true;
            }
            else
            {
                ScrollBar.Visible = false;
                ScrollBar.Minimum = 0;
                ScrollBar.Maximum = 0;
                ScrollBar.Value = 0;
            }

            PnlParams.Location = new Point(0, -ScrollBar.Value);
        }

        /// <summary>
        /// Invalidate the Panel Parameters
        /// </summary>
        private void ScrollBarValueChanged(object sender, EventArgs e)
        {
            PnlParams.Location = new Point(0, -ScrollBar.Value);
        }

        /// <summary>
        /// Shift the pnlParams viewpoint
        /// </summary>
        private void ScrollBarMouseWheel(object sender, MouseEventArgs e)
        {
            if (!ScrollBar.Visible) return;
            int newValue = ScrollBar.Value - e.Delta/120;

            if (newValue < ScrollBar.Minimum)
                ScrollBar.Value = ScrollBar.Minimum;
            else if (newValue > ScrollBar.Maximum)
                ScrollBar.Value = ScrollBar.Maximum;
            else
                ScrollBar.Value = newValue;
        }

        /// <summary>
        /// Toggles FSB visibility.
        /// </summary>
        private void HideFSBClick(object sender, EventArgs e)
        {
            FormFSB.Visible = !ChbHideFSB.Checked;
        }

        /// <summary>
        /// Resets Generator
        /// </summary>
        private void BtnResetClick(object sender, EventArgs e)
        {
            Configs.OptimizerOptions = "";
            _isReset = true;
            Close();
        }
    }
}