// Forex Strategy Builder - Scanner
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Forex_Strategy_Builder.Market;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// The Scanner
    /// </summary>
    public sealed class Scanner : Form
    {
        // Controls
        private SmallBalanceChart BalanceChart { get; set; }
        private Panel InfoPanel { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private Label LblProgress { get; set; }
        private Button BtnClose { get; set; }
        private CheckBox ChbAutoscan { get; set; }
        private CheckBox ChbTickScan { get; set; }
        private BackgroundWorker BgWorker { get; set; }

        private readonly Color _colorText;
        private readonly Font _fontInfo;
        private readonly int _infoRowHeight;
        private readonly bool _isTickDataFile;
        private bool _isLoadingNow;
        private int _progressPercent;
        private string _warningMessage;
        private Backtester Backtester { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Scanner()
        {
            InfoPanel = new Panel();
            BalanceChart = new SmallBalanceChart();
            ProgressBar = new ProgressBar();
            LblProgress = new Label();
            ChbAutoscan = new CheckBox();
            ChbTickScan = new CheckBox();
            BtnClose = new Button();

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnClose;
            Text = Language.T("Intrabar Scanner");
            BackColor = LayoutColors.ColorFormBack;
            FormClosing += ScannerFormClosing;

            _colorText = LayoutColors.ColorControlText;
            _fontInfo = new Font(Font.FontFamily, 9);
            _infoRowHeight = Math.Max(_fontInfo.Height, 18);
            _isTickDataFile = CheckTickDataFile();

            // pnlInfo
            InfoPanel.Parent = this;
            InfoPanel.BackColor = LayoutColors.ColorControlBack;
            InfoPanel.Paint += PnlInfoPaint;

            // Small Balance Chart
            BalanceChart.Parent = this;

            // ProgressBar
            ProgressBar.Parent = this;

            // Label Progress
            LblProgress.Parent = this;
            LblProgress.ForeColor = LayoutColors.ColorControlText;
            LblProgress.AutoSize = true;

            // Automatic Scan checkbox.
            ChbAutoscan.Parent = this;
            ChbAutoscan.ForeColor = _colorText;
            ChbAutoscan.BackColor = Color.Transparent;
            ChbAutoscan.Text = Language.T("Automatic Scan");
            ChbAutoscan.Checked = Configs.Autoscan;
            ChbAutoscan.AutoSize = true;
            ChbAutoscan.CheckedChanged += ChbAutoscanCheckedChanged;

            // Tick Scan checkbox.
            ChbTickScan.Parent = this;
            ChbTickScan.ForeColor = _colorText;
            ChbTickScan.BackColor = Color.Transparent;
            ChbTickScan.Text = Language.T("Use Ticks");
            ChbTickScan.Checked = Configs.UseTickData && _isTickDataFile;
            ChbTickScan.AutoSize = true;
            ChbTickScan.Visible = _isTickDataFile;
            ChbTickScan.CheckedChanged += ChbTickScanCheckedChanged;

            //Button Close
            BtnClose.Parent = this;
            BtnClose.Name = "Close";
            BtnClose.Text = Language.T("Close");
            BtnClose.DialogResult = DialogResult.OK;
            BtnClose.UseVisualStyleBackColor = true;

            // BackGroundWorker
            BgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            BgWorker.DoWork += BgWorkerDoWork;
            BgWorker.ProgressChanged += BgWorkerProgressChanged;
            BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            _isLoadingNow = false;

            if (!_isTickDataFile)
                Configs.UseTickData = false;
        }

        /// <summary>
        /// Sets scanner compact mode.
        /// </summary>
        public bool CompactMode { private get; set; }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (CompactMode)
            {
                InfoPanel.Visible = false;
                BalanceChart.Visible = false;
                LblProgress.Visible = true;
                ChbAutoscan.Visible = false;

                Width = 300;
                Height = 95;
                TopMost = true;

                StartLoading();
            }
            else
            {
                LblProgress.Visible = false;
                ChbAutoscan.Visible = true;
                BalanceChart.SetChartData(Backtester);
                Width = 460;
                Height = 540;
                if (!_isTickDataFile)
                    Height -= _infoRowHeight;
            }
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;

            if (CompactMode)
            {
                //Button Close
                BtnClose.Size = new Size(buttonWidth, buttonHeight);
                BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                               ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                ProgressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDLU*9));
                ProgressBar.Location = new Point(space, btnVertSpace);

                // Label Progress
                LblProgress.Location = new Point(space, BtnClose.Top + 5);
            }
            else
            {
                //Button Close
                BtnClose.Size = new Size(buttonWidth, buttonHeight);
                BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                               ClientSize.Height - buttonHeight - btnVertSpace);

                // ProgressBar
                ProgressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDLU*9));
                ProgressBar.Location = new Point(space, BtnClose.Top - ProgressBar.Height - btnVertSpace);

                // Panel Info
                int pnlInfoHeight = _isTickDataFile ? _infoRowHeight*11 + 2 : _infoRowHeight*10 + 2;
                InfoPanel.Size = new Size(ClientSize.Width - 2*space, pnlInfoHeight);
                InfoPanel.Location = new Point(space, space);

                // Panel balance chart
                BalanceChart.Size = new Size(ClientSize.Width - 2*space, ProgressBar.Top - InfoPanel.Bottom - 2*space);
                BalanceChart.Location = new Point(space, InfoPanel.Bottom + space);

                // Label Progress
                LblProgress.Location = new Point(space, BtnClose.Top + 5);

                // Auto scan checkbox
                ChbAutoscan.Location = new Point(space, BtnClose.Top + 5);

                // TickScan checkbox
                ChbTickScan.Location = new Point(ChbAutoscan.Right + space, BtnClose.Top + 5);
            }
        }

        /// <summary>
        /// Loads data and recalculates.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (CompactMode)
                return;

            if (!Data.DataSet.IsIntrabarData)
            {
                StartLoading();
            }
            else
            {
                Backtester.Calculate(Backtester.Strategy, Data.DataSet);
                ShowScanningResult();
                ProgressBar.Value = 100;
                BtnClose.Focus();
            }
        }

        /// <summary>
        /// Stops the background worker.
        /// </summary>
        private void ScannerFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isLoadingNow) return;
            // Cancels the asynchronous operation.
            BgWorker.CancelAsync();
            e.Cancel = true;
        }

        /// <summary>
        /// Repaint the panel Info
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            // +------------------------------------------------------+
            // |                   Data                               |
            // |------------------- ----------------------------------+
            // | Period  | Bars  | From | Until | Cover |  %  | Label |
            // |------------------------------------------------------+
            //xp0       xp1     xp2    xp3     xp4     xp5   xp6     xp7

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (!Data.IsData || !Data.IsResult) return;

            var pnl = (Panel) sender;
            const int border = 2;
            const int xp0 = border;
            const int xp1 = 80;
            const int xp2 = 140;
            const int xp3 = 200;
            const int xp4 = 260;
            const int xp5 = 320;
            const int xp6 = 370;
            int xp7 = pnl.ClientSize.Width - border;

            var size = new Size(xp7 - xp0, _infoRowHeight);

            var sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near};

            // Caption background
            var pntStart = new PointF(0, 0);
            SizeF szfCaption = new Size(pnl.ClientSize.Width - 0, 2*_infoRowHeight);
            var rectfCaption = new RectangleF(pntStart, szfCaption);
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            var stringFormatCaption = new StringFormat
                                          {
                                              LineAlignment = StringAlignment.Center,
                                              Trimming = StringTrimming.EllipsisCharacter,
                                              FormatFlags = StringFormatFlags.NoWrap,
                                              Alignment = StringAlignment.Near
                                          };
            string stringCaptionText = Language.T("Intrabar Data");
            float captionWidth = Math.Min(InfoPanel.ClientSize.Width, xp7 - xp0);
            float captionTextWidth = g.MeasureString(stringCaptionText, _fontInfo).Width;
            float captionTextX = Math.Max((captionWidth - captionTextWidth)/2f, 0);
            var pfCaptionText = new PointF(captionTextX, 0);
            var sfCaptionText = new SizeF(captionWidth - captionTextX, _infoRowHeight);
            rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);

            Brush brush = new SolidBrush(LayoutColors.ColorCaptionText);
            // First caption row
            g.DrawString(stringCaptionText, _fontInfo, brush, rectfCaption, stringFormatCaption);

            // Second title row
            g.DrawString(Language.T("Period"), _fontInfo, brush, (xp1 + xp0)/2f, _infoRowHeight, sf);
            g.DrawString(Language.T("Bars"), _fontInfo, brush, (xp2 + xp1)/2f, _infoRowHeight, sf);
            g.DrawString(Language.T("From"), _fontInfo, brush, (xp3 + xp2)/2f, _infoRowHeight, sf);
            g.DrawString(Language.T("Until"), _fontInfo, brush, (xp4 + xp3)/2f, _infoRowHeight, sf);
            g.DrawString(Language.T("Coverage"), _fontInfo, brush, (xp5 + xp4)/2f, _infoRowHeight, sf);
            g.DrawString("%", _fontInfo, brush, (xp6 + xp5)/2f, _infoRowHeight, sf);
            g.DrawString(Language.T("Label"), _fontInfo, brush, (xp7 + xp6)/2f, _infoRowHeight, sf);

            brush = new SolidBrush(LayoutColors.ColorControlText);
            int allPeriods = Enum.GetValues(typeof (DataPeriods)).Length;
            for (int period = 0; period <= allPeriods; period++)
            {
                int y = (period + 2)*_infoRowHeight;
                var point = new Point(xp0, y);

                if (Math.Abs(period%2f - 0) > 0.0001)
                    g.FillRectangle(new SolidBrush(LayoutColors.ColorEvenRowBack), new Rectangle(point, size));
            }

            // Tick statistics
            if (_isTickDataFile)
            {
                g.DrawString(Language.T("Tick"), _fontInfo, brush, (xp1 + xp0)/2, 2*_infoRowHeight, sf);
                if (Data.DataSet.IsTickData && Configs.UseTickData)
                {
                    int firstBarWithTicks = -1;
                    int lastBarWithTicks = -1;
                    int tickBars = 0;
                    for (int b = 0; b < Data.DataSet.Bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.DataSet.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.DataSet.TickData[b] != null)
                        {
                            lastBarWithTicks = b;
                            tickBars++;
                        }
                    }
                    double percentage = 100d*tickBars/Data.DataSet.Bars;

                    int y = 2*_infoRowHeight;
                    string ticks = (Data.DataSet.Ticks > 999999) 
                        ? (Data.DataSet.Ticks / 1000).ToString(CultureInfo.InvariantCulture) + "K" 
                        : Data.DataSet.Ticks.ToString(CultureInfo.InvariantCulture);
                    g.DrawString(ticks, _fontInfo, brush, (xp2 + xp1)/2, y, sf);
                    g.DrawString((firstBarWithTicks + 1).ToString(CultureInfo.InvariantCulture), _fontInfo, brush, (xp3 + xp2)/2, y, sf);
                    g.DrawString((lastBarWithTicks + 1).ToString(CultureInfo.InvariantCulture), _fontInfo, brush, (xp4 + xp3)/2, y, sf);
                    g.DrawString(tickBars.ToString(CultureInfo.InvariantCulture), _fontInfo, brush, (xp5 + xp4)/2, y, sf);
                    g.DrawString(percentage.ToString("F2"), _fontInfo, brush, (xp6 + xp5)/2, y, sf);

                    var rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    ColorMagic.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.min1], 60);
                    rectf = new RectangleF(xp6 + 10, y + 7, xp7 - xp6 - 20, 3);
                    ColorMagic.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.day], 60);
                }
            }

            for (int prd = 0; prd < allPeriods; prd++)
            {
                int startY = _isTickDataFile ? 3 : 2;
                int y = (prd + startY)*_infoRowHeight;

                var period = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(prd);
                int intraBars = Data.DataSet.IntraBars == null || !Data.DataSet.IsIntrabarData ? 0 : Data.DataSet.IntraBars[prd];
                int fromBar = 0;
                int untilBar = 0;
                int coveredBars = 0;
                double percentage = 0;

                bool isMultyAreas = false;
                if (intraBars > 0)
                {
                    bool isFromBarFound = false;
                    bool isUntilBarFound = false;
                    untilBar = Data.DataSet.Bars;
                    for (int bar = 0; bar < Data.DataSet.Bars; bar++)
                    {
                        if (!isFromBarFound && Data.DataSet.IntraBarsPeriods[bar] == period)
                        {
                            fromBar = bar;
                            isFromBarFound = true;
                        }
                        if (isFromBarFound && !isUntilBarFound &&
                            (Data.DataSet.IntraBarsPeriods[bar] != period || bar == Data.DataSet.Bars - 1))
                        {
                            if (bar < Data.DataSet.Bars - 1)
                            {
                                isUntilBarFound = true;
                                untilBar = bar;
                            }
                            else
                            {
                                untilBar = Data.DataSet.Bars;
                            }
                            coveredBars = untilBar - fromBar;
                        }
                        if (isFromBarFound && isUntilBarFound && Data.DataSet.IntraBarsPeriods[bar] == period)
                        {
                            isMultyAreas = true;
                            coveredBars++;
                        }
                    }
                    if (isFromBarFound)
                    {
                        percentage = 100d*coveredBars/Data.DataSet.Bars;
                        fromBar++;
                    }
                    else
                    {
                        fromBar = 0;
                        untilBar = 0;
                        coveredBars = 0;
                        percentage = 0;
                    }
                }
                else if (period == Data.DataSet.Period)
                {
                    intraBars = Data.DataSet.Bars;
                    fromBar = 1;
                    untilBar = Data.DataSet.Bars;
                    coveredBars = Data.DataSet.Bars;
                    percentage = 100;
                }

                g.DrawString(Data.DataPeriodToString(period), _fontInfo, brush, (xp1 + xp0)/2, y, sf);

                if (coveredBars > 0 || period == Data.DataSet.Period)
                {
                    g.DrawString(intraBars.ToString(CultureInfo.InvariantCulture), _fontInfo, brush, (xp2 + xp1)/2, y, sf);
                    g.DrawString(fromBar.ToString(CultureInfo.InvariantCulture), _fontInfo, brush, (xp3 + xp2)/2, y, sf);
                    g.DrawString(untilBar.ToString(CultureInfo.InvariantCulture), _fontInfo, brush, (xp4 + xp3)/2, y, sf);
                    g.DrawString(coveredBars.ToString(CultureInfo.InvariantCulture) + (isMultyAreas ? "*" : ""), _fontInfo, brush, (xp5 + xp4)/2, y, sf);
                    g.DrawString(percentage.ToString("F2"), _fontInfo, brush, (xp6 + xp5)/2, y, sf);

                    var rectf = new RectangleF(xp6 + 10, y + 4, xp7 - xp6 - 20, 9);
                    ColorMagic.GradientPaint(g, rectf, Data.PeriodColor[period], 60);
                }
            }

            var penLine = new Pen(LayoutColors.ColorJournalLines);
            g.DrawLine(penLine, xp1, 2*_infoRowHeight, xp1, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp2, 2*_infoRowHeight, xp2, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp3, 2*_infoRowHeight, xp3, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp4, 2*_infoRowHeight, xp4, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp5, 2*_infoRowHeight, xp5, pnl.ClientSize.Height);
            g.DrawLine(penLine, xp6, 2*_infoRowHeight, xp6, pnl.ClientSize.Height);

            // Border
            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    border);
            g.DrawLine(penBorder, 1, 2*_infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, 2*_infoRowHeight, pnl.ClientSize.Width - border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - border + 1);
        }

        /// <summary>
        /// Starts intrabar data loading.
        /// </summary>
        private void StartLoading()
        {
            if (_isLoadingNow)
            {
                // Cancel the asynchronous operation.
                BgWorker.CancelAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            ProgressBar.Value = 0;
            _warningMessage = string.Empty;
            _isLoadingNow = true;
            _progressPercent = 0;
            LblProgress.Visible = true;
            ChbAutoscan.Visible = false;
            ChbTickScan.Visible = false;

            BtnClose.Text = Language.T("Cancel");

            // Start the bgWorker
            BgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;

            LoadData(worker);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 200)
                ProgressBar.Style = ProgressBarStyle.Marquee;
            else
                ProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Data.DataSet.IsIntrabarData || Configs.UseTickData && Data.DataSet.IsTickData || Data.DataSet.Period == DataPeriods.min1)
                Backtester.Calculate(Backtester.Strategy, Data.DataSet);

            if (!CompactMode)
                ShowScanningResult();
            CompleteScanning();

            if (_warningMessage != string.Empty && Configs.CheckData)
                MessageBox.Show(_warningMessage + Environment.NewLine + Environment.NewLine +
                                Language.T("The data is probably incomplete and the scanning may not be reliable!") +
                                Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\"."),
                                Language.T("Scanner"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            if (CompactMode)
                Close();
        }

        /// <summary>
        /// Updates the chart and info panel.
        /// </summary>
        private void ShowScanningResult()
        {
            BalanceChart.SetChartData(Backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            InfoPanel.Invalidate();

            ChbAutoscan.Visible = true;
            ChbTickScan.Visible = Configs.UseTickData || _isTickDataFile;
        }

        /// <summary>
        /// Resets controls after loading data.
        /// </summary>
        private void CompleteScanning()
        {
            ProgressBar.Style = ProgressBarStyle.Blocks;

            LblProgress.Text = string.Empty;
            LblProgress.Visible = false;

            BtnClose.Text = Language.T("Close");
            Cursor = Cursors.Default;
            _isLoadingNow = false;
            BtnClose.Focus();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData(BackgroundWorker worker)
        {
            int periodsToLoad = 0;
            int allPeriods = Enum.GetValues(typeof (DataPeriods)).Length;
            Data.DataSet.IntraBars = new int[allPeriods];
            Data.DataSet.IntraBarData = new Bar[Data.DataSet.Bars][];
            Data.DataSet.IntraBarBars = new int[Data.DataSet.Bars];
            Data.DataSet.IntraBarsPeriods = new DataPeriods[Data.DataSet.Bars];
            Data.DataSet.LoadedIntraBarPeriods = 0;

            for (int bar = 0; bar < Data.DataSet.Bars; bar++)
            {
                Data.DataSet.IntraBarsPeriods[bar] = Data.DataSet.Period;
                Data.DataSet.IntraBarBars[bar] = 0;
            }

            // Counts how many periods to load
            for (int prd = 0; prd < allPeriods; prd++)
            {
                var period = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(prd);
                if (period < Data.DataSet.Period)
                {
                    periodsToLoad++;
                }
            }

            // Load the intrabar data (Starts from 1 Min)
            for (int prd = 0; prd < allPeriods && _isLoadingNow; prd++)
            {
                if (worker.CancellationPending) break;

                int loadedBars = 0;
                var period = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(prd);

                SetLabelProgressText(Language.T("Loading:") + " " + Data.DataPeriodToString(period) + "...");

                if (period < Data.DataSet.Period)
                {
                    loadedBars = LoadIntrabarData(period);
                    if (loadedBars > 0)
                    {
                        Data.DataSet.IsIntrabarData = true;
                        Data.DataSet.LoadedIntraBarPeriods++;
                    }
                }
                else if (period == Data.DataSet.Period)
                {
                    loadedBars = Data.DataSet.Bars;
                    Data.DataSet.LoadedIntraBarPeriods++;
                }

                Data.DataSet.IntraBars[prd] = loadedBars;

                // Report progress as a percentage of the total task.
                int percentComplete = periodsToLoad > 0 ? 100*(prd + 1)/periodsToLoad : 100;
                percentComplete = percentComplete > 100 ? 100 : percentComplete;
                if (percentComplete > _progressPercent)
                {
                    _progressPercent = percentComplete;
                    worker.ReportProgress(percentComplete);
                }
            }

            CheckIntrabarData();
            RepairIntrabarData();

            if (Configs.UseTickData)
            {
                SetLabelProgressText(Language.T("Loading:") + " " + Language.T("Ticks") + "...");
                worker.ReportProgress(200);
                try
                {
                    LoadTickData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Loads the Intrabar data.
        /// </summary>
        private int LoadIntrabarData(DataPeriods period)
        {
            var instrument = new Instrument(Data.DataSet.InstrProperties.Clone(), (int) period)
                                 {
                                     DataDir = Data.OfflineDataDir,
                                     MaxBars = Configs.MaxIntraBars
                                 };


            // Loads the data
            int loadingResult = instrument.LoadData();
            int loadedIntrabars = instrument.Bars;

            if (loadingResult == 0 && loadedIntrabars > 0)
            {
                if (Data.DataSet.Period != DataPeriods.week)
                {
                    if (instrument.DaysOff > 5)
                        _warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                                           Data.DataPeriodToString(period) + " - " + Language.T("Maximum days off:") +
                                           " " + instrument.DaysOff;
                    if (Data.DataStats.Update - instrument.Update > new TimeSpan(24, 0, 0))
                        _warningMessage += Environment.NewLine + Language.T("Data for:") + " " + Data.Symbol + " " +
                                           Data.DataPeriodToString(period) + " - " + Language.T("Updated on:") + " " +
                                           instrument.Update.ToString(CultureInfo.InvariantCulture);
                }

                int startBigBar;
                for (startBigBar = 0; startBigBar < Data.DataSet.Bars; startBigBar++)
                    if (Data.DataSet.Time[startBigBar] >= instrument.Time(0))
                        break;

                int stopBigBar;
                for (stopBigBar = startBigBar; stopBigBar < Data.DataSet.Bars; stopBigBar++)
                    if (Data.DataSet.IntraBarsPeriods[stopBigBar] != Data.DataSet.Period)
                        break;

                // Seek for a place to put the intrabars.
                int lastIntraBar = 0;
                for (int bar = startBigBar; bar < stopBigBar; bar++)
                {
                    Data.DataSet.IntraBarData[bar] = new Bar[(int)Data.DataSet.Period / (int)period];
                    DateTime endTime = Data.DataSet.Time[bar] + new TimeSpan(0, (int)Data.DataSet.Period, 0);
                    int indexBar = 0;
                    for (int intrabar = lastIntraBar;
                         intrabar < loadedIntrabars && instrument.Time(intrabar) < endTime;
                         intrabar++)
                    {
                        if (instrument.Time(intrabar) >= Data.DataSet.Time[bar])
                        {
                            Data.DataSet.IntraBarData[bar][indexBar].Time = instrument.Time(intrabar);
                            Data.DataSet.IntraBarData[bar][indexBar].Open = instrument.Open(intrabar);
                            Data.DataSet.IntraBarData[bar][indexBar].High = instrument.High(intrabar);
                            Data.DataSet.IntraBarData[bar][indexBar].Low = instrument.Low(intrabar);
                            Data.DataSet.IntraBarData[bar][indexBar].Close = instrument.Close(intrabar);
                            Data.DataSet.IntraBarsPeriods[bar] = period;
                            Data.DataSet.IntraBarBars[bar]++;
                            indexBar++;
                            lastIntraBar = intrabar;
                        }
                    }
                }
            }

            return loadedIntrabars;
        }

        /// <summary>
        /// Checks the intrabar data.
        /// </summary>
        private void CheckIntrabarData()
        {
            int inraBarDataStarts = 0;
            for (int bar = 0; bar < Data.DataSet.Bars; bar++)
            {
                if (inraBarDataStarts == 0 && Data.DataSet.IntraBarsPeriods[bar] != Data.DataSet.Period)
                    inraBarDataStarts = bar;

                if (inraBarDataStarts > 0 && Data.DataSet.IntraBarsPeriods[bar] == Data.DataSet.Period)
                {
                    inraBarDataStarts = 0;
                    _warningMessage += Environment.NewLine +
                                       Language.T("There is no intrabar data from bar No:") + " " +
                                       (bar + 1) + " - " + Data.DataSet.Time[bar];
                }
            }
        }

        /// <summary>
        /// Repairs the intrabar data.
        /// </summary>
        private void RepairIntrabarData()
        {
            for (int bar = 0; bar < Data.DataSet.Bars; bar++)
            {
                if (Data.DataSet.IntraBarsPeriods[bar] != Data.DataSet.Period)
                {
                    // We have intrabar data here

                    // Repair the Opening prices
                    double price = Data.DataSet.Open[bar];
                    int b = 0;
                    Data.DataSet.IntraBarData[bar][b].Open = Data.DataSet.Open[bar];
                    if (price > Data.DataSet.IntraBarData[bar][b].High &&
                        price > Data.DataSet.IntraBarData[bar][b].Low)
                    {
                        // Adjust the High price
                        Data.DataSet.IntraBarData[bar][b].High = price;
                    }
                    else if (price < Data.DataSet.IntraBarData[bar][b].High &&
                             price < Data.DataSet.IntraBarData[bar][b].Low)
                    {
                        // Adjust the Low price
                        Data.DataSet.IntraBarData[bar][b].Low = price;
                    }

                    // Repair the Closing prices
                    price = Data.DataSet.Close[bar];
                    b = Data.DataSet.IntraBarBars[bar] - 1;
                    Data.DataSet.IntraBarData[bar][b].Close = Data.DataSet.Close[bar];
                    if (price > Data.DataSet.IntraBarData[bar][b].High &&
                        price > Data.DataSet.IntraBarData[bar][b].Low)
                    {
                        // Adjust the High price
                        Data.DataSet.IntraBarData[bar][b].High = price;
                    }
                    else if (price < Data.DataSet.IntraBarData[bar][b].High &&
                             price < Data.DataSet.IntraBarData[bar][b].Low)
                    {
                        // Adjust the Low price
                        Data.DataSet.IntraBarData[bar][b].Low = price;
                    }

                    int minIntrabar = -1; // Contains the min price
                    int maxIntrabar = -1; // Contains the max price
                    double minPrice = double.MaxValue;
                    double maxPrice = double.MinValue;

                    for (b = 0; b < Data.DataSet.IntraBarBars[bar]; b++)
                    {
                        // Find min and max
                        if (Data.DataSet.IntraBarData[bar][b].Low < minPrice)
                        {
                            // Min found
                            minPrice = Data.DataSet.IntraBarData[bar][b].Low;
                            minIntrabar = b;
                        }
                        if (Data.DataSet.IntraBarData[bar][b].High > maxPrice)
                        {
                            // Max found
                            maxPrice = Data.DataSet.IntraBarData[bar][b].High;
                            maxIntrabar = b;
                        }
                        if (b > 0)
                        {
                            // Repair the Opening prices
                            price = Data.DataSet.IntraBarData[bar][b - 1].Close;
                            Data.DataSet.IntraBarData[bar][b].Open = price;
                            if (price > Data.DataSet.IntraBarData[bar][b].High &&
                                price > Data.DataSet.IntraBarData[bar][b].Low)
                            {
                                // Adjust the High price
                                Data.DataSet.IntraBarData[bar][b].High = price;
                            }
                            else if (price < Data.DataSet.IntraBarData[bar][b].High &&
                                     price < Data.DataSet.IntraBarData[bar][b].Low)
                            {
                                // Adjust the Low price
                                Data.DataSet.IntraBarData[bar][b].Low = price;
                            }
                        }
                    }

                    if (minPrice > Data.DataSet.Low[bar]) // Repair the Bottom
                        Data.DataSet.IntraBarData[bar][minIntrabar].Low = Data.DataSet.Low[bar];
                    if (maxPrice < Data.DataSet.High[bar]) // Repair the Top
                        Data.DataSet.IntraBarData[bar][maxIntrabar].High = Data.DataSet.High[bar];
                }
            }
        }

        /// <summary>
        /// Loads available tick data.
        /// </summary>
        private void LoadTickData()
        {
            var fileStream = new FileStream(Data.OfflineDataDir + Data.Symbol + "0.bin", FileMode.Open);
            var binaryReader = new BinaryReader(fileStream);
            Data.DataSet.TickData = new double[Data.DataSet.Bars][];
            int bar = 0;

            long totalVolume = 0;

            long pos = 0;
            long length = binaryReader.BaseStream.Length;
            while (pos < length)
            {
                DateTime time = DateTime.FromBinary(binaryReader.ReadInt64());
                pos += sizeof (Int64);

                int volume = binaryReader.ReadInt32();
                pos += sizeof (Int32);

                int count = binaryReader.ReadInt32();
                pos += sizeof (Int32);

                var bidTicks = new double[count];
                for (int i = 0; i < count; i++)
                    bidTicks[i] = binaryReader.ReadDouble();
                pos += count*sizeof (Double);

                while (bar < Data.DataSet.Bars - 1 && Data.DataSet.Time[bar] < time)
                {
                    if (time < Data.DataSet.Time[bar + 1])
                        break;
                    bar++;
                }

                if (time == Data.DataSet.Time[bar])
                {
                    Data.DataSet.TickData[bar] = bidTicks;
                }
                else if ((bar < Data.DataSet.Bars - 1 && time > Data.DataSet.Time[bar] && time < Data.DataSet.Time[bar + 1]) ||
                         bar == Data.DataSet.Bars - 1)
                {
                    if (Data.DataSet.TickData[bar] == null &&
                        (Math.Abs(Data.DataSet.Open[bar] - bidTicks[0]) < 10 * Data.DataSet.InstrProperties.Pip))
                        Data.DataSet.TickData[bar] = bidTicks;
                    else
                        AddTickData(bar, bidTicks);
                }

                totalVolume += volume;
            }

            binaryReader.Close();
            fileStream.Close();

            Data.DataSet.IsTickData = false;
            var barsWithTicks = 0;
            for (var b = 0; b < Data.DataSet.Bars; b++)
                if (Data.DataSet.TickData[b] != null)
                    barsWithTicks++;

            if (barsWithTicks > 0)
            {
                Data.DataSet.Ticks = totalVolume;
                Data.DataSet.IsTickData = true;
            }
        }

        /// <summary>
        /// Determines whether a tick data file exists.
        /// </summary>
        private bool CheckTickDataFile()
        {
            return File.Exists(Data.OfflineDataDir + Data.Symbol + "0.bin");
        }

        /// <summary>
        /// Adds tick data to Data
        /// </summary>
        private void AddTickData(int bar, double[] bidTicks)
        {
            if (Data.DataSet.TickData[bar] == null) return;
            int oldLenght = Data.DataSet.TickData[bar].Length;
            int ticksAdd = bidTicks.Length;
            Array.Resize(ref Data.DataSet.TickData[bar], oldLenght + ticksAdd);
            Array.Copy(bidTicks, 0, Data.DataSet.TickData[bar], oldLenght, ticksAdd);
        }

        /// <summary>
        /// Export tick data to a .CSV file.
        /// </summary>
        private void ExportTickToCSV()
        {
            using (var sw = new StreamWriter(Data.OfflineDataDir + Data.Symbol + "0.csv"))
            {
                for (var bar = 0; bar < Data.DataSet.Bars; bar++)
                {
                    if (Data.DataSet.TickData[bar] == null)
                    {
                        sw.WriteLine((bar + 1).ToString(CultureInfo.InvariantCulture) + "\t" +
                                     Data.DataSet.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t" +
                                     Data.DataSet.Time[bar].DayOfWeek);
                    }
                    else
                    {
                        sw.Write((bar + 1) + "\t" + Data.DataSet.Time[bar].ToString("yyyy-MM-dd HH:mm") + "\t");
                        foreach (var tick in Data.DataSet.TickData[bar])
                            sw.Write(tick.ToString("F5") + "\t");
                        sw.WriteLine();
                    }
                }
                sw.Close();
            }
        }

        /// <summary>
        /// Sets the lblProgress.Text.
        /// </summary>
        private void SetLabelProgressText(string text)
        {
            if (LblProgress.InvokeRequired)
            {
                Invoke(new SetLabelProgressCallback(SetLabelProgressText), new object[] {text});
            }
            else
            {
                LblProgress.Text = text;
            }
        }

        /// <summary>
        /// Auto scan checkbox.
        /// </summary>
        private void ChbAutoscanCheckedChanged(object sender, EventArgs e)
        {
            Configs.Autoscan = ChbAutoscan.Checked;
        }

        /// <summary>
        /// Tick scan checkbox.
        /// </summary>
        private void ChbTickScanCheckedChanged(object sender, EventArgs e)
        {
            Configs.UseTickData = ChbTickScan.Checked;
            StartLoading();
        }

        #region Nested type: SetLabelProgressCallback

        private delegate void SetLabelProgressCallback(string text);

        #endregion
    }
}
