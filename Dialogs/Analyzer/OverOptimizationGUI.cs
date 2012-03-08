// Strategy Analyzer - OverOptimization GUI
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    public sealed partial class OverOptimization : FancyPanel
    {
        private readonly BackgroundWorker _bgWorker;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public OverOptimization(string caption) : base(caption)
        {
            LblIntro = new Label();
            LblDeviation = new Label();
            LblParams = new Label();
            LblNoParams = new Label();
            NUDDeviation = new NumericUpDown();
            NUDParams = new NumericUpDown();
            BtnStart = new Button();
            BtnViewCharts = new Button();
            BtnOpenFolder = new Button();
            BtnOpenReport = new Button();
            ProgressBar = new ProgressBar();

            Color colorText = LayoutColors.ColorControlText;

            CountStrategyParams();
            bool isParams = _countStratParams > 0;

            // Label Intro
            LblIntro.Parent = this;
            LblIntro.ForeColor = colorText;
            LblIntro.BackColor = Color.Transparent;
            LblIntro.AutoSize = false;
            LblIntro.Text =
                Language.T(
                    "The Over-optimization Report shows how the results of the backtest vary as the numerical parameters of the strategy change by a given percent.");

            // Label Deviation
            LblDeviation.Parent = this;
            LblDeviation.ForeColor = colorText;
            LblDeviation.BackColor = Color.Transparent;
            LblDeviation.AutoSize = true;
            LblDeviation.Text = Language.T("Parameters deviation % [recommended 20]");

            // Label Parameters
            LblParams.Parent = this;
            LblParams.ForeColor = colorText;
            LblParams.BackColor = Color.Transparent;
            LblParams.AutoSize = true;
            LblParams.Text = Language.T("Parameters number [recommended 20]");

            // lblNoParams
            LblNoParams.Parent = this;
            LblNoParams.Text = Language.T("There are no parameters suitable for analysis.");
            LblNoParams.ForeColor = LayoutColors.ColorSignalRed;
            LblNoParams.BackColor = Color.Transparent;
            LblNoParams.AutoSize = true;
            LblNoParams.Visible = !isParams;

            // NumericUpDown Deviation
            NUDDeviation.BeginInit();
            NUDDeviation.Parent = this;
            NUDDeviation.Name = "Deviation";
            NUDDeviation.TextAlign = HorizontalAlignment.Center;
            NUDDeviation.Minimum = 1;
            NUDDeviation.Maximum = 100;
            NUDDeviation.Value = 20;
            NUDDeviation.EndInit();

            // NumericUpDown Swap Long
            NUDParams.BeginInit();
            NUDParams.Parent = this;
            NUDParams.Name = "Parameters";
            NUDParams.TextAlign = HorizontalAlignment.Center;
            NUDParams.Minimum = 1;
            NUDParams.Maximum = 100;
            NUDParams.Value = 20;
            NUDParams.EndInit();

            // Button View Charts
            BtnViewCharts.Parent = this;
            BtnViewCharts.Name = "btnViewCharts";
            BtnViewCharts.Text = Language.T("View Charts");
            BtnViewCharts.ImageAlign = ContentAlignment.MiddleLeft;
            BtnViewCharts.Image = Resources.overoptimization_chart;
            BtnViewCharts.Enabled = false;
            BtnViewCharts.Click += ViewChartsClick;
            BtnViewCharts.UseVisualStyleBackColor = true;

            // Button Open report folder
            BtnOpenFolder.Parent = this;
            BtnOpenFolder.Name = "btnOpenFolder";
            BtnOpenFolder.Text = Language.T("Open Folder");
            BtnOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            BtnOpenFolder.Image = Resources.folder_open;
            BtnOpenFolder.Enabled = false;
            BtnOpenFolder.Click += OpenFolderClick;
            BtnOpenFolder.UseVisualStyleBackColor = true;

            // Button Open Report
            BtnOpenReport.Parent = this;
            BtnOpenReport.Name = "btnOpenReport";
            BtnOpenReport.Text = Language.T("Open Report");
            BtnOpenReport.ImageAlign = ContentAlignment.MiddleLeft;
            BtnOpenReport.Image = Resources.export;
            BtnOpenReport.Enabled = false;
            BtnOpenReport.Click += OpenReportClick;
            BtnOpenReport.UseVisualStyleBackColor = true;

            // Button Start
            BtnStart.Parent = this;
            BtnStart.Text = Language.T("Start");
            BtnStart.Enabled = isParams;
            BtnStart.Click += BtnStartClick;
            BtnStart.UseVisualStyleBackColor = true;

            // ProgressBar
            ProgressBar.Parent = this;
            ProgressBar.Minimum = 1;
            ProgressBar.Maximum = 100;
            ProgressBar.Step = 1;

            // BackgroundWorker
            _bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _bgWorker.DoWork += BgWorkerDoWork;
            _bgWorker.ProgressChanged += BgWorkerProgressChanged;
            _bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            Resize += PnlOverOptimizationResize;
        }

        private Label LblIntro { get; set; }
        private Label LblDeviation { get; set; }
        private Label LblParams { get; set; }
        private Label LblNoParams { get; set; }
        private NumericUpDown NUDDeviation { get; set; }
        private NumericUpDown NUDParams { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private Button BtnStart { get; set; }
        private Button BtnViewCharts { get; set; }
        private Button BtnOpenFolder { get; set; }
        private Button BtnOpenReport { get; set; }

        public bool IsRunning { get; private set; }

        /// <summary>
        /// Calculates controls positions on resizing.
        /// </summary>
        private void PnlOverOptimizationResize(object sender, EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            int space = btnHrzSpace;
            const int border = 2;

            // Labels
            LblIntro.Width = Width - 2*(btnHrzSpace + border);
            LblIntro.Height = 2*buttonHeight;
            LblIntro.Location = new Point(btnHrzSpace + border, 2*space + 18);

            LblDeviation.Location = new Point(btnHrzSpace + border, 3*buttonHeight + 2*space + 18);
            LblParams.Location = new Point(btnHrzSpace + border, 4*buttonHeight + 3*space + 18);
            LblNoParams.Location = new Point(btnHrzSpace + border, 5*buttonHeight + 4*space + 18 + 20);

            // NUD Parameters
            int maxLabelRight = LblDeviation.Right;
            if (LblParams.Right > maxLabelRight) maxLabelRight = LblParams.Right;
            int nudLeft = maxLabelRight + btnHrzSpace;
            const int nudWidth = 70;
            NUDDeviation.Size = new Size(nudWidth, buttonHeight);
            NUDDeviation.Location = new Point(nudLeft, 3*buttonHeight + 2*space + 16);
            NUDParams.Size = new Size(nudWidth, buttonHeight);
            NUDParams.Location = new Point(nudLeft, 4*buttonHeight + 3*space + 16);

            int btnWideWidth = (ClientSize.Width - 2*border - 4*btnHrzSpace)/3;
            BtnViewCharts.Size = new Size(btnWideWidth, buttonHeight);
            BtnOpenFolder.Size = BtnViewCharts.Size;
            BtnOpenReport.Size = BtnViewCharts.Size;

            BtnViewCharts.Location = new Point(border + btnHrzSpace, Height - buttonHeight - btnVertSpace);
            BtnOpenFolder.Location = new Point(BtnViewCharts.Right + btnHrzSpace, BtnViewCharts.Top);
            BtnOpenReport.Location = new Point(BtnOpenFolder.Right + btnHrzSpace, BtnViewCharts.Top);

            // Progress Bar
            ProgressBar.Size = new Size(ClientSize.Width - 2*border - 2*btnHrzSpace, (int) (Data.VerticalDLU*9));
            ProgressBar.Location = new Point(border + btnHrzSpace, BtnOpenFolder.Top - ProgressBar.Height - btnVertSpace);

            // Button Run
            BtnStart.Size = BtnViewCharts.Size;
            BtnStart.Location = new Point(border + btnHrzSpace, ProgressBar.Top - buttonHeight - btnVertSpace);
        }

        /// <summary>
        /// Button Run clicked.
        /// </summary>
        private void BtnStartClick(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                // Cancel the asynchronous operation
                _bgWorker.CancelAsync();
            }
            else
            {
                // Start the bgWorker
                Cursor = Cursors.WaitCursor;
                BtnStart.Text = Language.T("Stop");
                IsRunning = true;

                NUDDeviation.Enabled = false;
                NUDParams.Enabled = false;

                BtnViewCharts.Enabled = false;
                BtnOpenFolder.Enabled = false;
                BtnOpenReport.Enabled = false;

                ProgressBar.Value = 1;
                _progressPercent = 0;
                _computedCycles = 0;

                _bgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Does the job.
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            SetParametersValues((int) NUDDeviation.Value, (int) NUDParams.Value);
            CalculateStatsTables((int) NUDDeviation.Value, (int) NUDParams.Value);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            //Backtester.Calculate();
            //Backtester.CalculateAccountStats();

            string report = GenerateReport();
            SaveReport(report);

            foreach (Control control in Controls)
                control.Enabled = true;

            BtnStart.Enabled = true;

            BtnStart.Text = Language.T("Start");
            Cursor = Cursors.Default;
            IsRunning = false;
        }

        /// <summary>
        /// Opens a chart screen.
        /// </summary>
        private void ViewChartsClick(object sender, EventArgs e)
        {
            var chartForm = new OverOptimizationChartsForm(_tableReport);
            chartForm.ShowDialog();
        }

        /// <summary>
        /// Opens the report folder.
        /// </summary>
        private void OpenFolderClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Data.StrategyDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Opens the report.
        /// </summary>
        private void OpenReportClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(_pathReportFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}