// Forex Strategy Builder - JForexImport
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder.Dialogs.JForex
{
    public sealed class JForexImport : Form
    {
        private readonly BackgroundWorker _bgWorker;
        private readonly Color _colorText;
        private readonly List<JForexDataFiles> _files = new List<JForexDataFiles>();
        private bool _isImporting;

        /// <summary>
        /// Constructor
        /// </summary>
        public JForexImport()
        {
            LblIntro = new Label();
            TxbDataDirectory = new TextBox();
            BtnBrowse = new Button();
            PnlSettings = new FancyPanel();
            PnlInfoBase = new FancyPanel(Language.T("Imported Files"));
            TbxInfo = new TextBox();
            BtnHelp = new Button();
            BtnClose = new Button();
            BtnImport = new Button();
            ProgressBar = new ProgressBar();

            LblMarketClose = new Label();
            LblMarketOpen = new Label();
            NUDMarketClose = new NumericUpDown();
            NUDMarketOpen = new NumericUpDown();

            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnImport;
            CancelButton = BtnClose;
            Text = Language.T("JForex Import");

            // Label Intro
            LblIntro.Parent = PnlSettings;
            LblIntro.ForeColor = _colorText;
            LblIntro.BackColor = Color.Transparent;
            LblIntro.AutoSize = true;
            LblIntro.Text = Language.T("Directory containing JForex data files:");

            // Data Directory
            TxbDataDirectory.Parent = PnlSettings;
            TxbDataDirectory.BackColor = LayoutColors.ColorControlBack;
            TxbDataDirectory.ForeColor = _colorText;
            TxbDataDirectory.Text = Configs.JForexDataPath;

            // Button Browse
            BtnBrowse.Parent = PnlSettings;
            BtnBrowse.Name = "Browse";
            BtnBrowse.Text = Language.T("Browse");
            BtnBrowse.Click += BtnBrowseClick;
            BtnBrowse.UseVisualStyleBackColor = true;

            // Label Market Close
            LblMarketClose.Parent = PnlSettings;
            LblMarketClose.ForeColor = _colorText;
            LblMarketClose.BackColor = Color.Transparent;
            LblMarketClose.AutoSize = true;
            LblMarketClose.Text = Language.T("Market closing hour on Friday:");

            // Label Market Open
            LblMarketOpen.Parent = PnlSettings;
            LblMarketOpen.ForeColor = _colorText;
            LblMarketOpen.BackColor = Color.Transparent;
            LblMarketOpen.AutoSize = true;
            LblMarketOpen.Text = Language.T("Market opening hour on Sunday:");

            // NUDMarketClose
            NUDMarketClose.BeginInit();
            NUDMarketClose.Parent = PnlSettings;
            NUDMarketClose.TextAlign = HorizontalAlignment.Center;
            NUDMarketClose.Minimum = 0;
            NUDMarketClose.Maximum = 24;
            NUDMarketClose.Increment = 1;
            NUDMarketClose.Value = Configs.MarketClosingHour;
            NUDMarketClose.EndInit();

            // NUDMarketOpen
            NUDMarketOpen.BeginInit();
            NUDMarketOpen.Parent = PnlSettings;
            NUDMarketOpen.TextAlign = HorizontalAlignment.Center;
            NUDMarketOpen.Minimum = 0;
            NUDMarketOpen.Maximum = 24;
            NUDMarketOpen.Increment = 1;
            NUDMarketOpen.Value = Configs.MarketOpeningHour;
            NUDMarketOpen.EndInit();

            // pnlSettings
            PnlSettings.Parent = this;

            // PnlInfoBase
            PnlInfoBase.Parent = this;
            PnlInfoBase.Padding = new Padding(4, (int) PnlInfoBase.CaptionHeight, 2, 2);

            // TbxInfo
            TbxInfo.Parent = PnlInfoBase;
            TbxInfo.BorderStyle = BorderStyle.None;
            TbxInfo.Dock = DockStyle.Fill;
            TbxInfo.BackColor = LayoutColors.ColorControlBack;
            TbxInfo.ForeColor = LayoutColors.ColorControlText;
            TbxInfo.Multiline = true;
            TbxInfo.AcceptsReturn = true;
            TbxInfo.AcceptsTab = true;
            TbxInfo.ScrollBars = ScrollBars.Vertical;

            // ProgressBar
            ProgressBar.Parent = this;

            // Button Help
            BtnHelp.Parent = this;
            BtnHelp.Name = "Help";
            BtnHelp.Text = Language.T("Help");
            BtnHelp.Click += BtnHelpClick;
            BtnHelp.UseVisualStyleBackColor = true;

            // Button Close
            BtnClose.Parent = this;
            BtnClose.Text = Language.T("Close");
            BtnClose.DialogResult = DialogResult.Cancel;
            BtnClose.UseVisualStyleBackColor = true;

            // Button Import
            BtnImport.Parent = this;
            BtnImport.Name = "Import";
            BtnImport.Text = Language.T("Import");
            BtnImport.Click += BtnImportClick;
            BtnImport.UseVisualStyleBackColor = true;

            // BackGroundWorker
            _bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _bgWorker.DoWork += BgWorkerDoWork;
            _bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
        }

        private Button BtnBrowse { get; set; }
        private Button BtnClose { get; set; }
        private Button BtnHelp { get; set; }
        private Button BtnImport { get; set; }
        private Label LblIntro { get; set; }
        private Label LblMarketClose { get; set; }
        private Label LblMarketOpen { get; set; }
        private NumericUpDown NUDMarketClose { get; set; }
        private NumericUpDown NUDMarketOpen { get; set; }

        private FancyPanel PnlInfoBase { get; set; }
        private FancyPanel PnlSettings { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private TextBox TbxInfo { get; set; }
        private TextBox TxbDataDirectory { get; set; }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 400);

            BtnImport.Focus();
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
            int border = btnHrzSpace;
            const int nudWidth = 70;

            // Button Cancel
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            BtnHelp.Size = new Size(buttonWidth, buttonHeight);
            BtnHelp.Location = new Point(BtnClose.Left - buttonWidth - btnHrzSpace,
                                         ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Import
            BtnImport.Size = new Size(buttonWidth, buttonHeight);
            BtnImport.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            ProgressBar.Size = new Size(ClientSize.Width - 2*border, (int) (Data.VerticalDLU*9));
            ProgressBar.Location = new Point(border, BtnClose.Top - ProgressBar.Height - btnVertSpace);

            PnlSettings.Size = new Size(ClientSize.Width - 2*btnHrzSpace, 110);
            PnlSettings.Location = new Point(btnHrzSpace, border);

            PnlInfoBase.Size = new Size(ClientSize.Width - 2*btnHrzSpace,
                                        ProgressBar.Top - PnlSettings.Bottom - 2*border);
            PnlInfoBase.Location = new Point(btnHrzSpace, PnlSettings.Bottom + border);

            // Label Intro
            LblIntro.Location = new Point(btnHrzSpace + border, btnVertSpace);

            // Button Browse
            BtnBrowse.Size = new Size(buttonWidth, buttonHeight);
            BtnBrowse.Location = new Point(PnlSettings.Width - buttonWidth - btnHrzSpace, LblIntro.Bottom + border);

            // TextBox txbDataDirectory
            TxbDataDirectory.Width = BtnBrowse.Left - 2*btnHrzSpace - border;
            TxbDataDirectory.Location = new Point(btnHrzSpace + border,
                                                  BtnBrowse.Top + (buttonHeight - TxbDataDirectory.Height)/2);

            int nudLeft = PnlSettings.ClientSize.Width - nudWidth - btnHrzSpace - border;
            NUDMarketClose.Size = new Size(nudWidth, buttonHeight);
            NUDMarketClose.Location = new Point(nudLeft, BtnBrowse.Bottom + border);
            NUDMarketOpen.Size = new Size(nudWidth, buttonHeight);
            NUDMarketOpen.Location = new Point(nudLeft, NUDMarketClose.Bottom + border);

            // Labels
            LblMarketClose.Location = new Point(btnHrzSpace + border, NUDMarketClose.Top + 2);
            LblMarketOpen.Location = new Point(btnHrzSpace + border, NUDMarketOpen.Top + 2);
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        private void SetInfoText(string text)
        {
            if (TbxInfo.InvokeRequired)
            {
                BeginInvoke(new SetInfoTextCallback(SetInfoText), new object[] {text});
            }
            else
            {
                TbxInfo.AppendText(text);
            }
        }

        /// <summary>
        /// Button Browse Click
        /// </summary>
        private void BtnBrowseClick(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() != DialogResult.OK) return;
            Configs.JForexDataPath = fd.SelectedPath;
            TxbDataDirectory.Text = fd.SelectedPath;
        }

        /// <summary>
        /// Button Help Click
        /// </summary>
        private void BtnHelpClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/fsb/manual/jforex_data");
            }
            catch (Exception exc)
            {
                Console.WriteLine("BtnHelpClick: " + exc.Message);
            }
        }

        /// <summary>
        /// Button Import Click
        /// </summary>
        private void BtnImportClick(object sender, EventArgs e)
        {
            if (_isImporting)
            {
                // Cancel the asynchronous operation.
                _bgWorker.CancelAsync();
                return;
            }

            Configs.JForexDataPath = TxbDataDirectory.Text;
            Cursor = Cursors.WaitCursor;
            ProgressBar.Style = ProgressBarStyle.Marquee;
            _isImporting = true;
            BtnImport.Text = Language.T("Stop");
            Configs.MarketClosingHour = (int) NUDMarketClose.Value;
            Configs.MarketOpeningHour = (int) NUDMarketOpen.Value;

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
            if (worker == null) return;
            _files.Clear();
            ReadJForexFiles();
            foreach (JForexDataFiles file in _files)
            {
                if (worker.CancellationPending) break;
                if (file.Period > 0) ImportBarFile(file);
                if (worker.CancellationPending) break;
                if (file.Period == 0) ImportTicks(file);
            }
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            ProgressBar.Style = ProgressBarStyle.Blocks;
            _isImporting = false;
            BtnImport.Text = Language.T("Import");
            Cursor = Cursors.Default;
        }

        private void ReadJForexFiles()
        {
            if (!Directory.Exists(TxbDataDirectory.Text))
                return;

            string[] dataFiles = Directory.GetFiles(TxbDataDirectory.Text);
            foreach (string filePath in dataFiles)
            {
                if (Path.GetExtension(filePath) != ".csv") continue;
                var file = new JForexDataFiles(filePath);
                if (file.IsCorrect)
                    _files.Add(file);
            }
        }

        private void ImportBarFile(JForexDataFiles file)
        {
            var streamReader = new StreamReader(file.FilePath);
            var streamWriter = new StreamWriter(file.FileTargetPath);
            const string dateFormat = "yyyy.MM.dd HH:mm:ss";

            const char fieldSplitter = ',';
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            int bars = 0;

            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line == null) continue;
                    if (line.StartsWith("Time"))
                        continue; // Skips annotation line.
                    string[] data = line.Split(new[] {fieldSplitter});

                    DateTime time = DateTime.ParseExact(data[0], dateFormat, formatProvider);
                    double open = StringToDouble(data[1]);
                    double high = StringToDouble(data[2]);
                    double low = StringToDouble(data[3]);
                    double close = StringToDouble(data[4]);
                    var volume = (int) StringToDouble(data[5]);

                    if (volume > 0 &&
                        !(Math.Abs(open - high) < 0.000001 && Math.Abs(open - low) < 0.000001 &&
                          Math.Abs(open - close) < 0.000001))
                    {
                        streamWriter.WriteLine(
                            time.ToString("yyyy-MM-dd\tHH:mm") + "\t" +
                            open + "\t" +
                            high + "\t" +
                            low + "\t" +
                            close + "\t" +
                            volume
                            );
                        bars++;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            streamWriter.Close();
            streamReader.Close();
            SetInfoText(file.Symbol + " " + Data.DataPeriodToString((DataPeriods) file.Period) + " - " +
                        (Language.T("Bars")).ToLower() + ": " + bars + Environment.NewLine);
        }

        private void ImportTicks(JForexDataFiles file)
        {
            var streamReader = new StreamReader(file.FilePath);
            var outStream = new FileStream(file.FileTargetPath, FileMode.Create);
            var binaryWriter = new BinaryWriter(outStream);

            DateTime time = DateTime.MinValue;
            int volume = 0;
            var reccord = new List<double>();
            int count1MinBars = 1;
            int totalVolume = 0;

            const string dateFormat = "yyyy.MM.dd HH:mm:ss";
            const char fieldSplitter = ',';

            IFormatProvider formatProvider = CultureInfo.InvariantCulture;

            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line != null && line.StartsWith("Time"))
                        continue; // Skips annotation line.
                    if (line != null)
                    {
                        string[] data = line.Split(new[] {fieldSplitter});
                        DateTime t = DateTime.ParseExact(data[0], dateFormat, formatProvider);
                        var tickTime = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);
                        double bid = StringToDouble(data[2]);

                        if (tickTime.Minute != time.Minute || volume == 0)
                        {
                            if (volume > 0 && !IsWeekend(time))
                            {
                                FilterReccord(reccord);
                                SaveReccord(binaryWriter, time, volume, reccord);
                                count1MinBars++;
                            }

                            time = tickTime;
                            volume = 0;
                            reccord.Clear();
                            reccord.Add(bid);
                        }
                        else if (reccord.Count > 0 && Math.Abs(bid - reccord[reccord.Count - 1]) > 0.000001)
                        {
                            reccord.Add(bid);
                        }
                    }

                    volume++;
                    totalVolume++;
                }
                if (volume > 0 && !IsWeekend(time))
                {
                    FilterReccord(reccord);
                    SaveReccord(binaryWriter, time, volume, reccord);
                    count1MinBars++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            streamReader.Close();
            binaryWriter.Close();
            outStream.Close();

            totalVolume--;
            count1MinBars--;

            SetInfoText(file.Symbol + " " + Language.T("Ticks") + " - " + (Language.T("Ticks")).ToLower() + ": " +
                        totalVolume + " - 1M " + (Language.T("Bars")).ToLower() + ": " + count1MinBars +
                        Environment.NewLine);
        }

        private bool IsWeekend(DateTime time)
        {
            bool isWeekend = time.DayOfWeek == DayOfWeek.Friday && time.Hour >= Configs.MarketClosingHour;

            if (time.DayOfWeek == DayOfWeek.Saturday)
                isWeekend = true;
            if (time.DayOfWeek == DayOfWeek.Sunday && time.Hour < Configs.MarketOpeningHour)
                isWeekend = true;

            return isWeekend;
        }

        private static void FilterReccord(List<double> reccord)
        {
            int count = reccord.Count;
            if (count < 3)
                return;

            var bids = new List<double>(count - 1) {reccord[0], reccord[1]};

            int b = 1;
            bool isChanged = false;

            for (int i = 2; i < count; i++)
            {
                double bid = reccord[i];

                if (bids[b - 1] < bids[b] && bids[b] < bid || bids[b - 1] > bids[b] && bids[b] > bid)
                {
                    bids[b] = bid;
                    isChanged = true;
                }
                else
                {
                    bids.Add(bid);
                    b++;
                }
            }

            if (isChanged)
            {
                reccord.Clear();
                foreach (double bid in bids)
                    reccord.Add(bid);
            }
        }

        private static void SaveReccord(BinaryWriter binaryWriter, DateTime time, int volume, List<double> reccord)
        {
            int count = reccord.Count;
            if (count < 1)
                return;

            binaryWriter.Write(time.ToBinary());
            binaryWriter.Write(volume);
            binaryWriter.Write(count);
            foreach (double bid in reccord)
                binaryWriter.Write(bid);
        }

        /// <summary>
        /// Converts a string to a double number.
        /// </summary>
        private static double StringToDouble(string input)
        {
            string sDecimalPoint = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(sDecimalPoint))
            {
                input = input.Replace(".", sDecimalPoint);
                input = input.Replace(",", sDecimalPoint);
            }

            double number = double.Parse(input);

            return number;
        }

        #region Nested type: SetInfoTextCallback

        private delegate void SetInfoTextCallback(string text);

        #endregion
    }
}