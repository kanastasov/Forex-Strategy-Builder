// Forex Strategy Builder - Comparator
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Media;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    internal sealed class Comparator : Form
    {
        private readonly BackgroundWorker _bgWorker;
        private readonly Brush _brushRandArea;
        private readonly int _countMethods;
        private readonly bool _isTradeUntilMC = Configs.TradeUntilMarginCall;
        private readonly Pen _penBalance;
        private readonly Pen _penNearest;
        private readonly Pen _penOptimistic;
        private readonly Pen _penPessimistic;
        private readonly Pen _penRandBands;
        private readonly Pen _penRandom;
        private readonly Pen _penShortest;
        private float[] _afBalance;
        private float[] _afMaxRandom;
        private float[,] _afMethods;
        private float[] _afMinRandom;
        private float[,] _afRandoms;
        private int _checkedMethods;
        private bool _isPaintChart;
        private bool _isRandom;
        private bool _isWorking; // It is true when the comparator is running
        private int _lines;
        private float _maximum;
        private float _maximumRandom;
        private float _minimum;
        private float _minimumRandom;

        /// <summary>
        /// Initialize the form and controls
        /// </summary>
        public Comparator()
        {
            PnlOptions = new Panel();
            PnlChart = new Panel();
            ProgressBar = new ProgressBar();
            LblAverageBalance = new Label();
            NumRandom = new NumericUpDown();
            LblRandomCycles = new Label();
            BtnCalculate = new Button();
            BtnClose = new Button();

            Text = Language.T("Comparator");
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = Data.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            FormClosing += ActionsFormClosing;

            _isPaintChart = false;

            //Button Calculate
            BtnCalculate.Parent = this;
            BtnCalculate.Name = "btnCalculate";
            BtnCalculate.Text = Language.T("Calculate");
            BtnCalculate.Click += BtnCalculateClick;
            BtnCalculate.UseVisualStyleBackColor = true;

            //Button Close
            BtnClose.Parent = this;
            BtnClose.Name = "btnClose";
            BtnClose.Text = Language.T("Close");
            BtnClose.DialogResult = DialogResult.OK;
            BtnClose.UseVisualStyleBackColor = true;

            // ProgressBar
            ProgressBar.Parent = this;
            ProgressBar.Minimum = 1;
            ProgressBar.Maximum = 100;
            ProgressBar.Step = 1;

            PnlChart.Parent = this;
            PnlChart.ForeColor = LayoutColors.ColorControlText;
            PnlChart.Paint += PnlChartPaint;

            PnlOptions.Parent = this;
            PnlOptions.ForeColor = LayoutColors.ColorControlText;
            PnlOptions.Paint += PnlOptionsPaint;

            _countMethods = Enum.GetValues(typeof (InterpolationMethod)).Length;
            AchboxMethods = new CheckBox[_countMethods];
            for (int i = 0; i < _countMethods; i++)
            {
                AchboxMethods[i] = new CheckBox
                                       {
                                           Parent = PnlOptions,
                                           Text = Language.T(Enum.GetNames(typeof (InterpolationMethod))[i]),
                                           Tag = Enum.GetValues(typeof (InterpolationMethod)).GetValue(i),
                                           Checked = true,
                                           BackColor = Color.Transparent,
                                           AutoSize = true
                                       };
                AchboxMethods[i].CheckedChanged += ComparatorCheckedChanged;
            }

            // Label Average Balance
            LblAverageBalance.Parent = PnlOptions;
            LblAverageBalance.AutoSize = true;
            LblAverageBalance.Text = Language.T("Average balance");
            LblAverageBalance.ForeColor = LayoutColors.ColorControlText;
            LblAverageBalance.BackColor = Color.Transparent;
            LblAverageBalance.TextAlign = ContentAlignment.MiddleLeft;

            // NumUpDown random cycles
            NumRandom.BeginInit();
            NumRandom.Parent = this;
            NumRandom.Value = 25;
            NumRandom.Minimum = 3;
            NumRandom.Maximum = 100;
            NumRandom.TextAlign = HorizontalAlignment.Center;
            NumRandom.EndInit();

            // Label Random Cycles
            LblRandomCycles.Parent = this;
            LblRandomCycles.AutoSize = true;
            LblRandomCycles.ForeColor = LayoutColors.ColorControlText;
            LblRandomCycles.BackColor = Color.Transparent;
            LblRandomCycles.Text = Language.T("Random iterations");
            LblRandomCycles.TextAlign = ContentAlignment.MiddleLeft;

            // Colors
            _penOptimistic = new Pen(LayoutColors.ComparatorChartOptimisticLine);
            _penPessimistic = new Pen(LayoutColors.ComparatorChartPessimisticLine);
            _penShortest = new Pen(LayoutColors.ComparatorChartShortestLine);
            _penNearest = new Pen(LayoutColors.ComparatorChartNearestLine);
            _penRandom = new Pen(LayoutColors.ComparatorChartRandomLine);
            _penRandBands = new Pen(LayoutColors.ComparatorChartRandomBands);
            _brushRandArea = new SolidBrush(LayoutColors.ComparatorChartRandomArea);
            _penBalance = new Pen(LayoutColors.ComparatorChartBalanceLine) {Width = 2};

            // BackGroundWorker
            _bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _bgWorker.DoWork += BgWorkerDoWork;
            _bgWorker.ProgressChanged += BgWorkerProgressChanged;
            _bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            Configs.TradeUntilMarginCall = false;
        }

        private Backtester Backtester { get; set; }

        private Panel PnlOptions { get; set; }
        private CheckBox[] AchboxMethods { get; set; }
        private Label LblAverageBalance { get; set; }
        private Panel PnlChart { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private NumericUpDown NumRandom { get; set; }
        private Label LblRandomCycles { get; set; }
        private Button BtnCalculate { get; set; }
        private Button BtnClose { get; set; }

        /// <summary>
        /// Resizes the form
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Width = (int) (Data.HorizontalDLU*290);
            Height = (int) (Data.VerticalDLU*260);
        }

        /// <summary>
        /// Arrange the controls
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;
            int controlZoneH = buttonHeight + 2*btnVertSpace;
            int controlZoneY = ClientSize.Height - controlZoneH;
            int buttonY = controlZoneY + btnVertSpace;

            PnlOptions.Size = new Size(ClientSize.Width - 2*space, 90);
            PnlOptions.Location = new Point(space, space);

            int positionX = (PnlOptions.ClientSize.Width - 10)/3;
            const int positionY = 27;
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (num < _countMethods)
                        AchboxMethods[num].Location = new Point(j*positionX + 40, i*30 + positionY);
                    else
                        LblAverageBalance.Location = new Point(j*positionX + 40, i*30 + positionY + 1);

                    num++;
                }
            }

            NumRandom.Size = new Size(50, buttonHeight);
            NumRandom.Location = new Point(btnHrzSpace, controlZoneY + (controlZoneH - NumRandom.Height)/2);
            LblRandomCycles.Location = new Point(NumRandom.Right + 5,
                                                 controlZoneY + (controlZoneH - LblRandomCycles.Height)/2);

            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, buttonY);

            BtnCalculate.Size = new Size(buttonWidth, buttonHeight);
            BtnCalculate.Location = new Point(BtnClose.Left - buttonWidth - btnHrzSpace, buttonY);

            ProgressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDLU*9));
            ProgressBar.Location = new Point(space, BtnClose.Top - ProgressBar.Height - btnVertSpace);
            PnlChart.Size = new Size(ClientSize.Width - 2*space, ProgressBar.Top - PnlOptions.Bottom - 2*space);
            PnlChart.Location = new Point(space, PnlOptions.Bottom + space);
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isWorking)
            {
                // Cancel the asynchronous operation.
                _bgWorker.CancelAsync();
                e.Cancel = true;
            }

            Configs.TradeUntilMarginCall = _isTradeUntilMC;
        }

        /// <summary>
        /// A check boxes status
        /// </summary>
        private void ComparatorCheckedChanged(object sender, EventArgs e)
        {
            var chbox = (CheckBox) sender;
            var interpMethod = (InterpolationMethod) chbox.Tag;

            if (interpMethod == InterpolationMethod.Random)
            {
                NumRandom.Enabled = chbox.Checked;
            }
        }

        /// <summary>
        /// Calculate
        /// </summary>
        private void BtnCalculateClick(object sender, EventArgs e)
        {
            if (_isWorking)
            {
                // Cancel the asynchronous operation.
                _bgWorker.CancelAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            ProgressBar.Value = 1;
            _isWorking = true;
            BtnClose.Enabled = false;
            BtnCalculate.Text = Language.T("Stop");

            for (int m = 0; m < _countMethods; m++)
            {
                AchboxMethods[m].Enabled = false;
            }
            NumRandom.Enabled = false;

            // Start the bgWorker
            _bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;

            ProgressBar.Value = 1;

            // Optimize all Parameters
            if (Calculate(worker) == 0)
            {
                _isPaintChart = true;
                PnlChart.Invalidate();
            }
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _isWorking = false;
            BtnClose.Enabled = true;
            BtnCalculate.Text = Language.T("Calculate");

            for (int m = 0; m < _countMethods; m++)
            {
                AchboxMethods[m].Enabled = true;
            }
            NumRandom.Enabled = true;

            Cursor = Cursors.Default;
            BtnClose.Focus();
        }

        /// <summary>
        /// Calculates the balance lines
        /// </summary>
        private int Calculate(BackgroundWorker worker)
        {
            // Determine the number of lines
            // For each method per line
            // The random line shows the averaged values
            // Also we have two border lines for the random method
            // Plus the average balance line

            _isRandom = false;
            _minimum = float.MaxValue;
            _maximum = float.MinValue;
            _minimumRandom = float.MaxValue;
            _maximumRandom = float.MinValue;
            var randomLines = (int) NumRandom.Value;

            _checkedMethods = 0;
            _lines = 1;
            for (int m = 0; m < _countMethods; m++)
                if (AchboxMethods[m].Checked)
                {
                    _checkedMethods++;
                    _lines++;
                    if ((InterpolationMethod) AchboxMethods[m].Tag == InterpolationMethod.Random)
                        _isRandom = true;
                }

            if (_checkedMethods == 0 && Configs.PlaySounds)
            {
                SystemSounds.Hand.Play();
                return -1;
            }

            _afBalance = new float[Data.DataSet.Bars - Backtester.Strategy.FirstBar];
            _afMethods = new float[_countMethods,Data.DataSet.Bars - Backtester.Strategy.FirstBar];
            if (_isRandom)
            {
                _afRandoms = new float[randomLines,Data.DataSet.Bars - Backtester.Strategy.FirstBar];
                _afMinRandom = new float[Data.DataSet.Bars - Backtester.Strategy.FirstBar];
                _afMaxRandom = new float[Data.DataSet.Bars - Backtester.Strategy.FirstBar];
            }

            // Progress parameters
            int computedCycles = 0;
            int cycles = _lines + (_isRandom ? randomLines : 0);
            int highestPercentageReached = 0;
            int percentComplete;

            // Calculates the lines
            for (int m = 0; m < _countMethods; m++)
            {
                if (worker.CancellationPending) return -1;
                if (!AchboxMethods[m].Checked) continue;

                var method = (InterpolationMethod) AchboxMethods[m].Tag;

                if (method == InterpolationMethod.Random)
                {
                    for (int r = 0; r < randomLines; r++)
                    {
                        if (worker.CancellationPending) return -1;

                        Backtester.InterpolationMethod = method;
                        //Backtester.Calculate();

                        if (Configs.AccountInMoney)
                            for (int iBar = 0; iBar < Data.DataSet.Bars - Backtester.Strategy.FirstBar; iBar++)
                                _afRandoms[r, iBar] = (float) Backtester.MoneyBalance(iBar + Backtester.Strategy.FirstBar);
                        else
                            for (int iBar = 0; iBar < Data.DataSet.Bars - Backtester.Strategy.FirstBar; iBar++)
                                _afRandoms[r, iBar] = Backtester.Balance(iBar + Backtester.Strategy.FirstBar);


                        // Report progress as a percentage of the total task.
                        computedCycles++;
                        percentComplete = 100*computedCycles/cycles;
                        percentComplete = percentComplete > 100 ? 100 : percentComplete;
                        if (percentComplete > highestPercentageReached)
                        {
                            highestPercentageReached = percentComplete;
                            worker.ReportProgress(percentComplete);
                        }
                    }

                    for (int iBar = 0; iBar < Data.DataSet.Bars - Backtester.Strategy.FirstBar; iBar++)
                    {
                        float randomSum = 0;
                        float minRandom = float.MaxValue;
                        float maxRandom = float.MinValue;
                        for (int r = 0; r < randomLines; r++)
                        {
                            float value = _afRandoms[r, iBar];
                            randomSum += value;
                            minRandom = value < minRandom ? value : minRandom;
                            maxRandom = value > maxRandom ? value : maxRandom;
                        }
                        _afMethods[m, iBar] = randomSum/randomLines;
                        _afMinRandom[iBar] = minRandom;
                        _afMaxRandom[iBar] = maxRandom;
                        _minimumRandom = minRandom < _minimumRandom ? minRandom : _minimumRandom;
                        _maximumRandom = maxRandom > _maximumRandom ? maxRandom : _maximumRandom;
                    }

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    percentComplete = 100*computedCycles/cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
                else
                {
                    Backtester.InterpolationMethod = method;
                    //Backtester.Calculate();

                    if (Configs.AccountInMoney)
                        for (int iBar = 0; iBar < Data.DataSet.Bars - Backtester.Strategy.FirstBar; iBar++)
                            _afMethods[m, iBar] = (float) Backtester.MoneyBalance(iBar + Backtester.Strategy.FirstBar);
                    else
                        for (int iBar = 0; iBar < Data.DataSet.Bars - Backtester.Strategy.FirstBar; iBar++)
                            _afMethods[m, iBar] = Backtester.Balance(iBar + Backtester.Strategy.FirstBar);

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    percentComplete = 100*computedCycles/cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
            }

            // Calculates the average balance, Min and Max
            for (int bar = 0; bar < Data.DataSet.Bars - Backtester.Strategy.FirstBar; bar++)
            {
                float sum = 0;
                for (int m = 0; m < _countMethods; m++)
                {
                    if (!AchboxMethods[m].Checked) continue;

                    float value = _afMethods[m, bar];
                    sum += value;
                    if (value < _minimum)
                        _minimum = value;
                    if (value > _maximum)
                        _maximum = value;
                }
                _afBalance[bar] = sum/_checkedMethods;
            }

            // Report progress as a percentage of the total task.
            computedCycles++;
            percentComplete = 100*computedCycles/cycles;
            percentComplete = percentComplete > 100 ? 100 : percentComplete;
            if (percentComplete > highestPercentageReached)
            {
                worker.ReportProgress(percentComplete);
            }

            return 0;
        }

        /// <summary>
        /// Paints panel pnlOptions
        /// </summary>
        private void PnlOptionsPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            const int border = 2;

            // Chart Title
            string str = Language.T("Interpolation Methods");
            var font = new Font(Font.FontFamily, 9);
            var fCaptionHeight = (float) Math.Max(font.Height, 18);
            var rectfCaption = new RectangleF(0, 0, pnl.ClientSize.Width, fCaptionHeight);
            var stringFormatCaption = new StringFormat
                                          {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(str, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Paint the panel background
            var rectClient = new RectangleF(border, fCaptionHeight, pnl.ClientSize.Width - 2*border,
                                            pnl.Height - fCaptionHeight - border);
            ColorMagic.GradientPaint(g, rectClient, LayoutColors.ColorControlBack, LayoutColors.DepthControl);

            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, fCaptionHeight, pnl.ClientSize.Width - border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - border + 1);

            int positionX = (PnlOptions.ClientSize.Width - 10)/3;
            const int positionY = 35;
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (num < _countMethods)
                    {
                        var pt1 = new Point(j*positionX + 10, i*30 + positionY);
                        var pt2 = new Point(j*positionX + 30, i*30 + positionY);
                        var pen = new Pen(Color.Red);
                        switch ((InterpolationMethod) AchboxMethods[num].Tag)
                        {
                            case InterpolationMethod.Pessimistic:
                                pen = new Pen(LayoutColors.ComparatorChartPessimisticLine);
                                break;
                            case InterpolationMethod.Shortest:
                                pen = new Pen(LayoutColors.ComparatorChartShortestLine);
                                break;
                            case InterpolationMethod.Nearest:
                                pen = new Pen(LayoutColors.ComparatorChartNearestLine);
                                break;
                            case InterpolationMethod.Optimistic:
                                pen = new Pen(LayoutColors.ComparatorChartOptimisticLine);
                                break;
                            case InterpolationMethod.Random:
                                var pntRnd1 = new Point(j*positionX + 10, i*30 + positionY - 6);
                                var pntRnd2 = new Point(j*positionX + 30, i*30 + positionY - 6);
                                var pntRnd3 = new Point(j*positionX + 10, i*30 + positionY + 6);
                                var pntRnd4 = new Point(j*positionX + 30, i*30 + positionY + 6);
                                var penRnd = new Pen(LayoutColors.ComparatorChartRandomBands, 2);
                                Brush brushRnd = new SolidBrush(LayoutColors.ComparatorChartRandomArea);
                                g.FillRectangle(brushRnd,
                                                new Rectangle(pntRnd1.X, pntRnd1.Y, pntRnd2.X - pntRnd1.X,
                                                              pntRnd4.Y - pntRnd2.Y));
                                g.DrawLine(penRnd, pntRnd1, pntRnd2);
                                g.DrawLine(penRnd, pntRnd3, pntRnd4);
                                pen = new Pen(LayoutColors.ComparatorChartRandomLine);
                                break;
                        }
                        pen.Width = 2;
                        g.DrawLine(pen, pt1, pt2);
                    }
                    else
                    {
                        var pt1 = new Point(j*positionX + 10, i*30 + positionY);
                        var pt2 = new Point(j*positionX + 30, i*30 + positionY);
                        var pen = new Pen(LayoutColors.ComparatorChartBalanceLine) {Width = 3};
                        g.DrawLine(pen, pt1, pt2);
                    }

                    num++;
                }
            }
        }

        /// <summary>
        /// Paints the charts
        /// </summary>
        private void PnlChartPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            const int space = 5;
            const int border = 2;

            // Chart Title
            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";
            string str = Language.T("Balance Chart") + unit;
            var font = new Font(Font.FontFamily, 9);
            var fCaptionHeight = (float) Math.Max(font.Height, 18);
            var rectfCaption = new RectangleF(0, 0, pnl.ClientSize.Width, fCaptionHeight);
            var stringFormatCaption = new StringFormat
                                          {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(str, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Paint the panel background
            var rectClient = new RectangleF(border, fCaptionHeight, pnl.ClientSize.Width - 2*border,
                                            pnl.Height - fCaptionHeight - border);
            ColorMagic.GradientPaint(g, rectClient, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, fCaptionHeight, pnl.ClientSize.Width - border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - border + 1);

            if (!_isPaintChart)
            {
                if (Backtester.AmbiguousBars == 0)
                {
                    string sNote = Language.T("The Comparator is useful when the backtest shows ambiguous bars!");
                    var rectfNote = new RectangleF(0, 30, pnl.ClientSize.Width, Font.Height);
                    g.DrawString(sNote, Font, new SolidBrush(LayoutColors.ColorChartFore), rectfNote,
                                 stringFormatCaption);
                }
                return;
            }

            int bars = Data.DataSet.Bars - Backtester.Strategy.FirstBar;
            int max = (int) Math.Max(_maximum, _maximumRandom) + 1;
            int min = (int) Math.Min(_minimum, _minimumRandom) - 1;
            min = (int) Math.Floor(min/10f)*10;
            int yTop = (int) fCaptionHeight + 2*space + 1;
            int yBottom = (pnl.ClientSize.Height - 2*space - border);
            var labelWidth =
                (int)
                Math.Max(g.MeasureString(min.ToString(CultureInfo.InvariantCulture), Font).Width,
                         g.MeasureString(max.ToString(CultureInfo.InvariantCulture), Font).Width);
            labelWidth = Math.Max(labelWidth, 30);
            int xRight = pnl.ClientSize.Width - border - space - labelWidth;

            //
            // Grid
            //
            int cntLabels = Math.Max((yBottom - yTop)/20, 1);
            var delta = (float) Math.Max(Math.Round((max - min)/(float) cntLabels), 10);
            int step = (int) Math.Ceiling(delta/10)*10;
            cntLabels = (int) Math.Ceiling((max - min)/(float) step);
            max = min + cntLabels*step;
            float scaleY = (yBottom - yTop)/(cntLabels*(float) step);
            Brush brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            var penGrid = new Pen(LayoutColors.ColorChartGrid)
                              {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            // Price labels
            for (int label = min; label <= max; label += step)
            {
                var labelY = (int) (yBottom - (label - min)*scaleY);
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, brushFore, xRight,
                             labelY - Font.Height/2 - 1);
                g.DrawLine(penGrid, border + space, labelY, xRight, labelY);
            }

            float fScaleX = (xRight - 2*space - border)/(float) bars;

            if (_isRandom)
            {
                // Draws the random area and Min Max lines
                var apntMinRandom = new PointF[bars];
                var apntMaxRandom = new PointF[bars];
                for (int bar = 0; bar < bars; bar++)
                {
                    apntMinRandom[bar].X = border + space + bar*fScaleX;
                    apntMinRandom[bar].Y = yBottom - (_afMinRandom[bar] - min)*scaleY;
                    apntMaxRandom[bar].X = border + space + bar*fScaleX;
                    apntMaxRandom[bar].Y = yBottom - (_afMaxRandom[bar] - min)*scaleY;
                }
                apntMinRandom[0].Y = apntMaxRandom[0].Y;
                var path = new GraphicsPath();
                path.AddLines(apntMinRandom);
                path.AddLine(apntMinRandom[bars - 1], apntMaxRandom[bars - 1]);
                path.AddLines(apntMaxRandom);
                var region = new Region(path);
                g.FillRegion(_brushRandArea, region);
                g.DrawLines(_penRandBands, apntMinRandom);
                g.DrawLines(_penRandBands, apntMaxRandom);
            }

            // Draws the lines
            for (int m = 0; m < _countMethods; m++)
            {
                if (!AchboxMethods[m].Checked) continue;

                var apntLines = new PointF[bars];
                for (int bar = 0; bar < bars; bar++)
                {
                    apntLines[bar].X = border + space + bar*fScaleX;
                    apntLines[bar].Y = yBottom - (_afMethods[m, bar] - min)*scaleY;
                }

                var pen = new Pen(LayoutColors.ColorSignalRed);
                switch ((InterpolationMethod) AchboxMethods[m].Tag)
                {
                    case InterpolationMethod.Pessimistic:
                        pen = _penPessimistic;
                        break;
                    case InterpolationMethod.Shortest:
                        pen = _penShortest;
                        break;
                    case InterpolationMethod.Nearest:
                        pen = _penNearest;
                        break;
                    case InterpolationMethod.Optimistic:
                        pen = _penOptimistic;
                        break;
                    case InterpolationMethod.Random:
                        pen = _penRandom;
                        break;
                }
                g.DrawLines(pen, apntLines);
            }

            // Draws the average balance
            var apntBalance = new PointF[bars];
            for (int bar = 0; bar < bars; bar++)
            {
                apntBalance[bar].X = border + space + bar*fScaleX;
                apntBalance[bar].Y = yBottom - (_afBalance[bar] - min)*scaleY;
            }
            g.DrawLines(_penBalance, apntBalance);

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), border + space - 1, yTop - space, border + space - 1,
                       yBottom);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), border + space, yBottom, xRight, yBottom);

            // Balance label
            float fBalanceY = yBottom - (_afBalance[bars - 1] - min)*scaleY;
            g.DrawLine(new Pen(LayoutColors.ColorChartCross), border + space, fBalanceY, xRight - space, fBalanceY);

            var szBalance = new Size(labelWidth + space, Font.Height + 2);
            var point = new Point(xRight - space + 2, (int) (fBalanceY - Font.Height/2.0 - 1));
            var rec = new Rectangle(point, szBalance);
            string sBalance = ((int) _afBalance[bars - 1]).ToString(CultureInfo.InvariantCulture);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorLabelBack), rec);
            g.DrawRectangle(new Pen(LayoutColors.ColorChartCross), rec);
            g.DrawString(sBalance, Font, new SolidBrush(LayoutColors.ColorLabelText), rec, stringFormatCaption);

            // Scanning note
            var fontNote = new Font(Font.FontFamily, Font.Size - 1);
            if (Configs.Autoscan && !Data.DataSet.IsIntrabarData)
                g.DrawString(Language.T("Load intrabar data"), fontNote, Brushes.Red, border + space, fCaptionHeight - 2);
            else if (Backtester.IsScanPerformed)
                g.DrawString(Language.T("Scanned") + " MQ " + ModelingQuality.ToString("N2") + "%", fontNote,
                             Brushes.LimeGreen, border + space, fCaptionHeight - 2);

            // Scanned bars
            if (Data.DataSet.IntraBars != null && Data.DataSet.IsIntrabarData && Backtester.IsScanPerformed)
            {
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), border + space - 1, yBottom, border + space - 1,
                           yBottom + 8);
                DataPeriods dataPeriod = Data.DataSet.Period;
                Color color = Data.PeriodColor[Data.DataSet.Period];
                int iFromBar = Backtester.Strategy.FirstBar;
                for (int bar = Backtester.Strategy.FirstBar; bar < Data.DataSet.Bars; bar++)
                {
                    if (Data.DataSet.IntraBarsPeriods[bar] != dataPeriod || bar == Data.DataSet.Bars - 1)
                    {
                        int xStart = (int) ((iFromBar - Backtester.Strategy.FirstBar)*fScaleX) + border + space;
                        int xEnd = (int) ((bar - Backtester.Strategy.FirstBar)*fScaleX) + border + space;
                        iFromBar = bar;
                        dataPeriod = Data.DataSet.IntraBarsPeriods[bar];
                        ColorMagic.GradientPaint(g, new RectangleF(xStart, yBottom + 3, xEnd - xStart + 2, 5), color, 60);
                        color = Data.PeriodColor[Data.DataSet.IntraBarsPeriods[bar]];
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the Modeling Quality
        /// </summary>
        private double ModelingQuality
        {
            get
            {
                if (!Backtester.IsScanPerformed)
                    return 0;

                int startGen = 0;

                for (int i = 0; i < Data.DataSet.Bars; i++)
                    if (Data.DataSet.IntraBarsPeriods[i] < Data.DataSet.Period)
                    {
                        startGen = i;
                        break;
                    }

                int startGenM1 = Data.DataSet.Bars - 1;

                for (int i = 0; i < Data.DataSet.Bars; i++)
                    if (Data.DataSet.IntraBarsPeriods[i] == DataPeriods.min1)
                    {
                        startGenM1 = i;
                        break;
                    }

                double modellingQuality = (0.25 * (startGen - Backtester.Strategy.FirstBar) + 0.5 * (startGenM1 - startGen) +
                                           0.9 * (Data.DataSet.Bars - startGenM1)) / (Data.DataSet.Bars - Backtester.Strategy.FirstBar) * 100;

                return modellingQuality;
            }
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}