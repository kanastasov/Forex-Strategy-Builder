// Forex Strategy Builder - MetaTrader4Import
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.
//
// Contribution by Adam Burgess (February 2012)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public sealed class MetaTrader4Import : Form
    {
        private readonly BackgroundWorker _bgWorker;
        private readonly Color _colorText;
        private readonly List<string> _files = new List<string>();
        private bool _isImporting;
        private DateTime _endingDate;
        private DateTime _startingDate;

        /// <summary>
        /// Constructor
        /// </summary>
        public MetaTrader4Import()
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
            ProgressBarFile = new ProgressBar();
            ProgressBar = new ProgressBar();

            LblStartingDate = new Label();
            DtpStartingDate = new DateTimePicker();
            LblEndingDate = new Label();
            DtpEndingDate = new DateTimePicker();

            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnImport;
            CancelButton = BtnClose;
            Text = Language.T("MetaTrader 4 Import");

            // Label Intro
            LblIntro.Parent = PnlSettings;
            LblIntro.ForeColor = _colorText;
            LblIntro.BackColor = Color.Transparent;
            LblIntro.AutoSize = true;
            LblIntro.Text = Language.T("Directory containing MetaTrader 4 HST files:");

            // Data Directory
            TxbDataDirectory.Parent = PnlSettings;
            TxbDataDirectory.BackColor = LayoutColors.ColorControlBack;
            TxbDataDirectory.ForeColor = _colorText;
            TxbDataDirectory.Text = Configs.MetaTrader4DataPath;

            // Button Browse
            BtnBrowse.Parent = PnlSettings;
            BtnBrowse.Name = "Browse";
            BtnBrowse.Text = Language.T("Browse");
            BtnBrowse.Click += BtnBrowseClick;
            BtnBrowse.UseVisualStyleBackColor = true;

            // Label Starting Date
            LblStartingDate.Parent = PnlSettings;
            LblStartingDate.ForeColor = _colorText;
            LblStartingDate.BackColor = Color.Transparent;
            LblStartingDate.AutoSize = true;
            LblStartingDate.Text = Language.T("Starting Date:");

            // Starting Date
            DtpStartingDate.Parent = PnlSettings;
            DtpStartingDate.ForeColor = LayoutColors.ColorCaptionText;
            DtpStartingDate.ShowUpDown = true;

            // Label Ending Date
            LblEndingDate.Parent = PnlSettings;
            LblEndingDate.ForeColor = _colorText;
            LblEndingDate.BackColor = Color.Transparent;
            LblEndingDate.AutoSize = true;
            LblEndingDate.Text = Language.T("Ending Date:");

            // Ending Date
            DtpEndingDate.Parent = PnlSettings;
            DtpEndingDate.ForeColor = LayoutColors.ColorCaptionText;
            DtpEndingDate.ShowUpDown = true;

            // PnlSettings
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

            // ProgressBarFile
            ProgressBarFile.Parent = this;

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

        private Label LblIntro { get; set; }
        private TextBox TxbDataDirectory { get; set; }
        private Button BtnBrowse { get; set; }
        private Label LblStartingDate { get; set; }
        private DateTimePicker DtpStartingDate { get; set; }
        private Label LblEndingDate { get; set; }
        private DateTimePicker DtpEndingDate { get; set; }

        private FancyPanel PnlSettings { get; set; }
        private FancyPanel PnlInfoBase { get; set; }
        private ProgressBar ProgressBarFile { get; set; }
        private ProgressBar ProgressBar { get; set; }
        private TextBox TbxInfo { get; set; }
        private Button BtnHelp { get; set; }
        private Button BtnImport { get; set; }
        private Button BtnClose { get; set; }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 400);

            // Starting DateTime
            DtpStartingDate.Value = string.IsNullOrEmpty(Configs.ImportStartingDate)
                                        ? DateTime.Now.AddYears(-2)
                                        : DateTime.Parse(Configs.ImportStartingDate);

            // Ending DateTime
            DtpEndingDate.Value = string.IsNullOrEmpty(Configs.ImportEndingDate)
                                      ? DateTime.Now
                                      : DateTime.Parse(Configs.ImportEndingDate);

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
            int textHeight = Font.Height;
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

            // ProgressBarFile
            ProgressBarFile.Size = new Size(ClientSize.Width - 2*border, (int) (Data.VerticalDLU*9));
            ProgressBarFile.Location = new Point(border, ProgressBar.Top - ProgressBar.Height - btnVertSpace);

            PnlSettings.Size = new Size(ClientSize.Width - 2*btnHrzSpace, 110);
            PnlSettings.Location = new Point(btnHrzSpace, border);

            PnlInfoBase.Size = new Size(ClientSize.Width - 2*btnHrzSpace,
                                        ProgressBarFile.Top - PnlSettings.Bottom - 2*border);
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

            // Date Pickers
            const int pickerWidth = 200;
            int pickerLeft = PnlSettings.ClientSize.Width - nudWidth - btnHrzSpace - border + nudWidth - pickerWidth;
            DtpStartingDate.Size = new Size(pickerWidth, textHeight);
            DtpStartingDate.Location = new Point(pickerLeft, BtnBrowse.Bottom + border);
            DtpEndingDate.Size = new Size(pickerWidth, textHeight);
            DtpEndingDate.Location = new Point(pickerLeft, DtpStartingDate.Bottom + border);

            // Labels
            LblStartingDate.Location = new Point(btnHrzSpace + border, DtpStartingDate.Top + 2);
            LblEndingDate.Location = new Point(btnHrzSpace + border, DtpEndingDate.Top + 2);
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
                BeginInvoke(new SetInfoTextDelegate(SetInfoText), new object[] {text});
            }
            else
            {
                TbxInfo.AppendText(text);
            }
        }

        private void UpdateProgressBar(int increment)
        {
            if (ProgressBar.InvokeRequired)
            {
                BeginInvoke(new UpdateProgressBarDelegate(UpdateProgressBar), new object[] {increment});
            }
            else
            {
                ProgressBar.Value = ProgressBar.Value + increment;
            }
        }

        private void SetupProgressBarFile(long maximum)
        {
            if (ProgressBarFile.InvokeRequired)
            {
                BeginInvoke(new SetupProgressBarFileDelegate(SetupProgressBarFile), new object[] {maximum});
            }
            else
            {
                if (maximum > Int32.MaxValue)
                    ProgressBarFile.Maximum = Int32.MaxValue;
                else
                    ProgressBarFile.Maximum = (int) maximum;
            }
        }

        private void UpdateProgressBarFile(long position)
        {
            if (ProgressBarFile.InvokeRequired)
            {
                BeginInvoke(new UpdateProgressBarFileDelegate(UpdateProgressBarFile), new object[] {position});
            }
            else
            {
                if (position > Int32.MaxValue)
                    ProgressBarFile.Value = Int32.MaxValue;
                else
                    ProgressBarFile.Value = (int) position;
            }
        }

        /// <summary>
        /// Button Browse Click
        /// </summary>
        private void BtnBrowseClick(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                Configs.MetaTrader4DataPath = fd.SelectedPath;
                TxbDataDirectory.Text = fd.SelectedPath;
            }
        }

        /// <summary>
        /// Button Help Click
        /// </summary>
        private void BtnHelpClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/fsb/manual/metatrader4_data");
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

            Configs.MetaTrader4DataPath = TxbDataDirectory.Text;
            Cursor = Cursors.WaitCursor;
            _isImporting = true;
            BtnImport.Text = Language.T("Stop");
            Configs.ImportStartingDate = String.Format("{0:u}", DtpStartingDate.Value);
            Configs.ImportEndingDate = String.Format("{0:u}", DtpEndingDate.Value);

            // Load MetaTrader4 HST Files
            string[] hstFiles = Directory.GetFiles(TxbDataDirectory.Text, "*.hst", SearchOption.TopDirectoryOnly);
            var orderedFiles = new List<string>();
            foreach (string file in hstFiles)
                orderedFiles.Add(GetSortableName(file));
            orderedFiles.Sort();
            _files.Clear();
            foreach (string file in orderedFiles)
                _files.Add(Path.Combine(TxbDataDirectory.Text, GetFullName(file)));

            // Setup the Progress Bars
            ProgressBarFile.Minimum = 0;
            ProgressBarFile.Value = 0;
            ProgressBarFile.Maximum = 0;

            ProgressBar.Minimum = 0;
            ProgressBar.Value = 0;
            ProgressBar.Maximum = _files.Count;

            // Setup date Filters
            _startingDate = DtpStartingDate.Value;
            _endingDate = DtpEndingDate.Value;

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

            foreach (string file in _files)
                if (!worker.CancellationPending)
                {
                    ImportHSTFile(file);
                    UpdateProgressBar(1);
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

            ProgressBar.Value = ProgressBar.Maximum;
            ProgressBarFile.Value = ProgressBarFile.Maximum;
        }

        private void ImportHSTFile(string file)
        {
            string outpath = Path.Combine(Data.DefaultOfflineDataDir, Path.GetFileNameWithoutExtension(file) + ".csv");

            string message = " ({0} bars)";
            if (File.Exists(outpath))
                message = " ({0} bars) *";

            /* MetaTrader4 HST File Format
            * 
            * ~~ HEADER ~~
            * int      version (400)
            * string   copyright
            * int      period (TimeSpan)
            * int      digits
            * int      timesign
            * int      lastSync
            * byte[]   13 reserved bytes
            * byte[]   nn reserved bytes
            * 
            * ~~ DATA ~~
            * int      time
            * double   open
            * double   low
            * double   high
            * double   close
            * double   volume
            */

            // Update Status
            SetInfoText(Language.T("Import") + " " + Path.GetFileNameWithoutExtension(file) + " ... ");

            try
            {
                var data = new StringBuilder(); // Output Data Buffer
                int importCount = 0; // Imported Record Count
                int exportCount = 0; // Exported Record Count

                // Open the Destination File
                var sw = new StreamWriter(new FileStream(outpath, FileMode.Create));

                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    // Setup the (File) ProgressBar
                    SetupProgressBarFile(fs.Length);

                    // Open the HST File
                    var br = new BinaryReader(fs, Encoding.ASCII);

                    // Read the File Header
                    long version = br.ReadInt32(); // database version
                    string copyright = new string(br.ReadChars(64)).TrimEnd('\0'); // copyright info
                    string symbol = new string(br.ReadChars(12)).TrimEnd('\0'); // symbol name
                    TimeSpan period = TimeSpan.FromMinutes(br.ReadInt32()); // symbol timeframe
                    int digits = br.ReadInt32(); // the amount of digits after decimal point in the symbol
                    int timesign = br.ReadInt32(); // timesign of the database creation
                    int lastSync = br.ReadInt32(); // the last synchronization time
                    byte[] res1 = br.ReadBytes(13); // to be used in the future
                    var res2 = new List<byte>(); // reserved
                    int chars = 0;
                    while (br.PeekChar() == 0 && chars < 39) // seems to be some additional 'reserved' bytes here
                    {
                        chars++;
                        res2.Add(br.ReadByte());
                    }

                    // Read the File Data
                    while (fs.Position < fs.Length - 1)
                    {
                        // Update the (File) ProgressBar
                        importCount++;
                        if ((int) Math.IEEERemainder(importCount, 5000) == 0)
                            UpdateProgressBarFile(fs.Position);

                        // Output data is saved after each 50000 records are imported
                        if (exportCount > 0 && (int) Math.IEEERemainder(exportCount, 50000) == 0)
                        {
                            sw.Write(data);
                            data = new StringBuilder();
                        }

                        int seconds = br.ReadInt32(); // bar time in seconds since 1/1/1970
                        DateTime? dt = new DateTime(1970, 1, 1).AddSeconds(seconds); // bar time
                        decimal open = Math.Round((decimal) br.ReadDouble(), digits); // open
                        decimal low = Math.Round((decimal) br.ReadDouble(), digits); // low
                        decimal high = Math.Round((decimal) br.ReadDouble(), digits); // high
                        decimal close = Math.Round((decimal) br.ReadDouble(), digits); // close
                        decimal volume = Math.Round((decimal) br.ReadDouble(), digits); // vol                       
                        DateTime dateTime = dt.GetValueOrDefault();

                        if (dateTime > _startingDate && dateTime < _endingDate)
                        {
                            var output = new StringBuilder();
                            output.Append(dateTime.ToString("yyyy"));
                            output.Append("-");
                            output.Append(dateTime.ToString("MM"));
                            output.Append("-");
                            output.Append(dateTime.ToString("dd"));
                            output.Append("\t");
                            output.Append(dateTime.ToString("HH"));
                            output.Append(":");
                            output.Append(dateTime.ToString("mm"));
                            output.Append("\t");
                            output.Append(open.ToString(CultureInfo.InvariantCulture));
                            output.Append("\t");
                            output.Append(high.ToString(CultureInfo.InvariantCulture));
                            output.Append("\t");
                            output.Append(low.ToString(CultureInfo.InvariantCulture));
                            output.Append("\t");
                            output.Append(close.ToString(CultureInfo.InvariantCulture));
                            output.Append("\t");
                            output.Append(volume.ToString(CultureInfo.InvariantCulture));

                            data.AppendLine(output.ToString());

                            exportCount++;
                        }
                    }
                }
                sw.Write(data);
                sw.Flush();
                sw.Close();

                SetInfoText(string.Format(message, exportCount));
                SetInfoText(Environment.NewLine);
            }
            catch (Exception ex)
            {
                SetInfoText("Error (" + ex.Message + ")");
            }
        }

        /// <summary>
        /// Converts a file name in an ASCII sortable format
        /// </summary>
        /// <param name="file">File Name</param>
        /// <returns>Sortable Name</returns>
        private string GetSortableName(string file)
        {
            var symbol = new StringBuilder();
            var interval = new StringBuilder();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            if (fileNameWithoutExtension != null)
                foreach (char c in fileNameWithoutExtension.Trim().ToUpper())
                {
                    if (Char.IsLetter(c))
                        symbol.Append(c);
                    else if (Char.IsDigit(c))
                        interval.Append(c);
                }

            return symbol + Int32.Parse(interval.ToString()).ToString("00000");
        }

        /// <summary>
        /// Converts a Sortable Name back to the Full File Name
        /// </summary>
        /// <param name="sortableName">Sortable Name</param>
        /// <returns>File Name (without directory information)</returns>
        private string GetFullName(string sortableName)
        {
            var symbol = new StringBuilder();
            var interval = new StringBuilder();

            foreach (char c in sortableName)
            {
                if (Char.IsLetter(c))
                    symbol.Append(c);
                else if (Char.IsDigit(c))
                    interval.Append(c);
            }

            return symbol + Int32.Parse(interval.ToString()).ToString(CultureInfo.InvariantCulture) + ".hst";
        }

        #region Nested type: SetInfoTextDelegate

        private delegate void SetInfoTextDelegate(string text);

        #endregion

        #region Nested type: SetupProgressBarFileDelegate

        private delegate void SetupProgressBarFileDelegate(long maximum);

        #endregion

        #region Nested type: UpdateProgressBarDelegate

        private delegate void UpdateProgressBarDelegate(int increment);

        #endregion

        #region Nested type: UpdateProgressBarFileDelegate

        private delegate void UpdateProgressBarFileDelegate(long position);

        #endregion
    }
}