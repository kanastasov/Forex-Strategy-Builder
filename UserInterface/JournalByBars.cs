// Journal Bars
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Common;
using Forex_Strategy_Builder.CustomControls;
using Forex_Strategy_Builder.Interfaces;
using Forex_Strategy_Builder.Properties;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public sealed class JournalByBars : ContextPanel
    {
        private const int Border = 2; // The width of outside border of the panel
        private int _bars; // The total number of bars
        private int _columns; // The number of the columns
        private int _firstBar; // The number of the first shown bar
        private string[,] _journalData; // The text journal data
        private int _lastBar; // The number of the last shown bar
        private Image[] _positionIcons; // Shows the position's type and transaction
        private int _rowHeight; // The journal row height
        private int _rows; // The number of bars can be shown (without the caption bar)
        private int _selectedBarOld; // The old selected bar
        private int _selectedRow; // The number of selected row
        private int _shownBars; // How many bars are shown
        private string[] _titlesInMoney; // Journal title second row
        private string[] _titlesInPips; // Journal title second row
        private int[] _xPositions; // The horizontal position of the column
        private int[] _xScaled; // The scaled horizontal position of the column
        private readonly IDataSet _dataSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public JournalByBars(IDataSet dataSet)
        {
            _dataSet = dataSet;

            InitializeJournal();
            SetUpJournal();
            UpdateJournalData();
        }

        private VScrollBar VScrollBar { get; set; }
        private HScrollBar HScrollBar { get; set; }


        /// <summary>
        /// Gets the selected bar.
        /// </summary>
        public int SelectedBar
        {
            get { return _firstBar + _selectedRow; }
        }

        public event EventHandler SelectedBarChange;

        /// <summary>
        /// Initializes the Journal
        /// </summary>
        private void InitializeJournal()
        {
            // Horizontal ScrollBar
            HScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 100, LargeChange = 300};
            HScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            VScrollBar = new VScrollBar
                             {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 4};
            VScrollBar.ValueChanged += VScrollBarValueChanged;

            Graphics g = CreateGraphics();
            var font = new Font(Font.FontFamily, 9);

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof (PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), font).Width >
                    g.MeasureString(longestDirection, font).Width)
                    longestDirection = Language.T(posDir.ToString());

            string longestTransaction = "";
            foreach (Transaction transaction in Enum.GetValues(typeof (Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), font).Width >
                    g.MeasureString(longestTransaction, font).Width)
                    longestTransaction = Language.T(transaction.ToString());

            string longestBacktestEval = "";
            foreach (BacktestEval eval in Enum.GetValues(typeof (BacktestEval)))
                if (g.MeasureString(Language.T(eval.ToString()), font).Width >
                    g.MeasureString(longestBacktestEval, font).Width)
                    longestBacktestEval = Language.T(eval.ToString());

            _titlesInPips = new[]
                                {
                                    Language.T("Bar"),
                                    Language.T("Date"),
                                    Language.T("Hour"),
                                    Language.T("Open"),
                                    Language.T("High"),
                                    Language.T("Low"),
                                    Language.T("Close"),
                                    Language.T("Volume"),
                                    Language.T("Transaction"),
                                    Language.T("Direction"),
                                    Language.T("Lots"),
                                    Language.T("Price"),
                                    Language.T("Profit Loss"),
                                    Language.T("Floating P/L"),
                                    Language.T("Balance"),
                                    Language.T("Equity"),
                                    Language.T("Required"),
                                    Language.T("Free"),
                                    Language.T("Interpolation")
                                };

            _titlesInMoney = new[]
                                 {
                                     Language.T("Bar"),
                                     Language.T("Date"),
                                     Language.T("Hour"),
                                     Language.T("Open"),
                                     Language.T("High"),
                                     Language.T("Low"),
                                     Language.T("Close"),
                                     Language.T("Volume"),
                                     Language.T("Transaction"),
                                     Language.T("Direction"),
                                     Language.T("Amount"),
                                     Language.T("Price"),
                                     Language.T("Profit Loss"),
                                     Language.T("Floating P/L"),
                                     Language.T("Balance"),
                                     Language.T("Equity"),
                                     Language.T("Required"),
                                     Language.T("Free"),
                                     Language.T("Interpolation")
                                 };

            var asColumContent = new[]
                                     {
                                         "99999",
                                         "99/99/99",
                                         "99:99",
                                         "99.99999",
                                         "99.99999",
                                         "99.99999",
                                         "99.99999",
                                         "999999",
                                         longestTransaction,
                                         longestDirection,
                                         "-9999999",
                                         "99.99999",
                                         "-9999999.99",
                                         "-9999999.99",
                                         "-9999999.99",
                                         "-9999999.99",
                                         "-9999999.99",
                                         "-9999999.99",
                                         longestBacktestEval
                                     };

            _rowHeight = Math.Max(font.Height, 18);
            Padding = new Padding(Border, 2*_rowHeight, Border, Border);

            _columns = 19;
            _xPositions = new int[20];
            _xScaled = new int[20];

            _xPositions[0] = Border;
            _xPositions[1] = _xPositions[0] +
                             (int)
                             Math.Max(g.MeasureString(asColumContent[0], font).Width + 16,
                                      g.MeasureString(_titlesInMoney[0], font).Width) + 4;
            for (int i = 1; i < _columns; i++)
                _xPositions[i + 1] = _xPositions[i] +
                                     (int)
                                     Math.Max(g.MeasureString(asColumContent[i], font).Width,
                                              g.MeasureString(_titlesInMoney[i], font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        /// Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            _bars = _dataSet.Bars;

            if (_bars == 0)
            {
                _firstBar = 0;
                _lastBar = 0;
                _shownBars = 0;

                VScrollBar.Enabled = false;
            }
            else if (_bars < _rows)
            {
                _firstBar = 0;
                _lastBar = _rows;
                _shownBars = _bars;

                VScrollBar.Enabled = false;
            }
            else
            {
                VScrollBar.Enabled = true;
                VScrollBar.Maximum = _bars - 1;

                _firstBar = VScrollBar.Value;
                if (_firstBar + _rows > _bars)
                {
                    _lastBar = _bars - 1;
                    _shownBars = _lastBar - _firstBar + 1;
                }
                else
                {
                    _shownBars = _rows;
                    _lastBar = _firstBar + _shownBars - 1;
                }
            }

            _selectedRow = Math.Min(_selectedRow, _shownBars - 1);
            _selectedRow = Math.Max(_selectedRow, 0);

            SetButtonsColor();
        }

        /// <summary>
        /// Updates the journal data from StatsBuffer
        /// </summary>
        public void UpdateJournalData()
        {
            _journalData = new string[_shownBars,_columns];
            _positionIcons = new Image[_shownBars];

            for (int bar = _firstBar; bar < _firstBar + _shownBars; bar++)
            {
                int row = bar - _firstBar;
                int col = 0;
                bool isPos = StatsBuffer.IsPos(bar);
                bool inMoney = Configs.AccountInMoney;

                _journalData[row, col++] = (bar + 1).ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = _dataSet.Time[bar].ToString(Data.DF);
                _journalData[row, col++] = _dataSet.Time[bar].ToString("HH:mm");
                _journalData[row, col++] = _dataSet.Open[bar].ToString(_dataSet.FF);
                _journalData[row, col++] = _dataSet.High[bar].ToString(_dataSet.FF);
                _journalData[row, col++] = _dataSet.Low[bar].ToString(_dataSet.FF);
                _journalData[row, col++] = _dataSet.Close[bar].ToString(_dataSet.FF);
                _journalData[row, col++] = _dataSet.Volume[bar].ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = isPos ? Language.T(StatsBuffer.SummaryTrans(bar).ToString()) : "";
                _journalData[row, col++] = isPos ? Language.T(StatsBuffer.SummaryDir(bar).ToString()) : "";
                _journalData[row, col++] = isPos ? GetPositionAmmountString(bar) : "";
                _journalData[row, col++] = isPos ? StatsBuffer.SummaryPrice(bar).ToString(_dataSet.FF) : "";
                _journalData[row, col++] = isPos ? GetPositionProfitString(bar) : "";
                _journalData[row, col++] = isPos ? GetPositionFloatingPLString(bar) : "";
                _journalData[row, col++] = inMoney
                                               ? StatsBuffer.MoneyBalance(bar).ToString("F2")
                                               : StatsBuffer.Balance(bar).ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = inMoney
                                               ? StatsBuffer.MoneyEquity(bar).ToString("F2")
                                               : StatsBuffer.Equity(bar).ToString(CultureInfo.InvariantCulture);
                _journalData[row, col++] = StatsBuffer.SummaryRequiredMargin(bar).ToString("F2");
                _journalData[row, col++] = StatsBuffer.SummaryFreeMargin(bar).ToString("F2");
                _journalData[row, col] = Language.T(StatsBuffer.BackTestEvalToString(bar));

                _positionIcons[row] = isPos
                                          ? Position.PositionIconImage(StatsBuffer.SummaryPositionIcon(bar))
                                          : Resources.pos_square;
            }
        }

        private string GetPositionAmmountString(int bar)
        {
            string sign = StatsBuffer.SummaryDir(bar) == PosDirection.Short ? "-" : "";
            if (Configs.AccountInMoney)
                return sign + StatsBuffer.SummaryAmount(bar).ToString(CultureInfo.InvariantCulture);
            return StatsBuffer.SummaryLots(bar).ToString(CultureInfo.InvariantCulture);
        }

        private string GetPositionProfitString(int bar)
        {
            Transaction trans = StatsBuffer.SummaryTrans(bar);
            if (trans == Transaction.Close || trans == Transaction.Reduce || trans == Transaction.Reverse)
            {
                return Configs.AccountInMoney
                           ? StatsBuffer.MoneyProfitLoss(bar).ToString("F2")
                           : StatsBuffer.ProfitLoss(bar).ToString(CultureInfo.InvariantCulture);
            }
            return "-";
        }

        private string GetPositionFloatingPLString(int bar)
        {
            if (StatsBuffer.SummaryTrans(bar) == Transaction.Close)
                return "-";
            return Configs.AccountInMoney
                       ? StatsBuffer.MoneyFloatingPL(bar).ToString("F2")
                       : StatsBuffer.FloatingPL(bar).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Set parameters on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            SetCountOfRows();
            SetColumnsWidth();
            SetHScrollBar();
            SetUpJournal();
            UpdateJournalData();
            base.OnResize(eventargs);
            Invalidate();
        }

        private void SetButtonsColor()
        {
            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }

        private void SetCountOfRows()
        {
            _rows = ClientSize.Height > 2*_rowHeight ? (ClientSize.Height - 2*_rowHeight)/_rowHeight : 0;
        }

        private void SetHScrollBar()
        {
            if (ClientSize.Width - VScrollBar.Width - 2*Border < _xPositions[_columns])
            {
                HScrollBar.Visible = true;
                int pointShort = _xPositions[_columns] - ClientSize.Width + VScrollBar.Width + 2*Border;
                if (HScrollBar.Value > pointShort)
                    HScrollBar.Value = pointShort;
                HScrollBar.Maximum = pointShort + HScrollBar.LargeChange - 2;
            }
            else
            {
                HScrollBar.Value = 0;
                HScrollBar.Visible = false;
            }
        }

        private void SetColumnsWidth()
        {
            if (ClientSize.Width - VScrollBar.Width - 2*Border <= _xPositions[_columns])
                _xPositions.CopyTo(_xScaled, 0);
            else
            {
                // Scales the columns position
                float scale = (float) (ClientSize.Width - VScrollBar.Width - 2*Border)/_xPositions[_columns];
                for (int i = 0; i <= _columns; i++)
                    _xScaled[i] = (int) (_xPositions[i]*scale);
            }
        }

        /// <summary>
        /// Paints the journal
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int iHScrll = -HScrollBar.Value;
            var sf = new StringFormat {Alignment = StringAlignment.Center};

            // Caption background
            var rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2*_rowHeight);
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            var font = new Font(Font.FontFamily, 9);
            Color colorBack = LayoutColors.ColorControlBack;
            var brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            var brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            var brushSelectedRowBack = new SolidBrush(LayoutColors.ColorSelectedRowBack);
            var brushSelectedRowText = new SolidBrush(LayoutColors.ColorSelectedRowText);
            var brushRowText = new SolidBrush(LayoutColors.ColorControlText);
            var brushWarningBack = new SolidBrush(LayoutColors.ColorWarningRowBack);
            var brushWarningText = new SolidBrush(LayoutColors.ColorWarningRowText);
            var penLines = new Pen(LayoutColors.ColorJournalLines);
            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);

            // Print the journal caption
            string unit = Configs.AccountInMoney
                              ? " [" + Configs.AccountCurrency + "]"
                              : " [" + Language.T("pips") + "]";
            string accUnit = " [" + Configs.AccountCurrency + "]";
            g.SetClip(new RectangleF(Border, 0, ClientSize.Width - 2*Border, 2*_rowHeight));
            g.DrawString(Language.T("Market Data"), font, brushCaptionText, iHScrll + (_xScaled[8] + _xScaled[0])/2, 0,
                         sf);
            g.DrawString(Language.T("Summary") + unit, font, brushCaptionText, iHScrll + (_xScaled[14] + _xScaled[8])/2,
                         0, sf);
            g.DrawString(Language.T("Account") + unit, font, brushCaptionText, iHScrll + (_xScaled[16] + _xScaled[14])/2,
                         0, sf);
            g.DrawString(Language.T("Margin") + accUnit, font, brushCaptionText,
                         iHScrll + (_xScaled[18] + _xScaled[16])/2, 0, sf);
            g.DrawString(Language.T("Backtest"), font, brushCaptionText, iHScrll + (_xScaled[19] + _xScaled[18])/2, 0,
                         sf);
            if (Configs.AccountInMoney)
            {
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesInMoney[i], font, brushCaptionText, iHScrll + (_xScaled[i] + _xScaled[i + 1])/2,
                                 _rowHeight, sf);
            }
            else
            {
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesInPips[i], font, brushCaptionText, iHScrll + (_xScaled[i] + _xScaled[i + 1])/2,
                                 _rowHeight, sf);
            }
            g.ResetClip();

            var rectField = new RectangleF(Border, 2*_rowHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - 2*_rowHeight - Border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            var size = new Size(ClientSize.Width - VScrollBar.Width - 2*Border, _rowHeight);

            // Prints the journal data
            for (int bar = _firstBar; bar < _firstBar + _shownBars; bar++)
            {
                int y = (bar - _firstBar + 2)*_rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs((bar - _firstBar)%2f - 0) > 0.0001)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Warning row
                bool isWarningRow = false;
                if (_journalData[bar - _firstBar, _columns - 1] == Language.T("Ambiguous"))
                {
                    g.FillRectangle(brushWarningBack, new Rectangle(point, size));
                    isWarningRow = true;
                }

                // Selected row
                Brush brush;
                if (bar - _firstBar == _selectedRow)
                {
                    g.FillRectangle(brushSelectedRowBack, new Rectangle(point, size));
                    brush = brushSelectedRowText;
                }
                else
                {
                    brush = isWarningRow ? brushWarningText : brushRowText;
                }

                int index = bar - _firstBar;

                // Draw the position icon
                int imgY = y + (int) Math.Floor((_rowHeight - 16)/2.0);
                g.DrawImage(_positionIcons[index], iHScrll + 2, imgY, 16, 16);

                // Prints the data
                g.DrawString(_journalData[index, 0], font, brush, iHScrll + (16 + _xScaled[1])/2, (index + 2)*_rowHeight,
                             sf);
                for (int i = 1; i < _columns; i++)
                    g.DrawString(_journalData[index, i], font, brush, iHScrll + (_xScaled[i] + _xScaled[i + 1])/2,
                                 (index + 2)*_rowHeight, sf);
            }

            // Vertical grid lines
            for (int i = 1; i < _columns; i++)
            {
                if (i == 8 || i == 14 || i == 16 || i == 18)
                {
                    var rectfSeparator = new RectangleF(_xScaled[i] + iHScrll, (float) (_rowHeight/2.0), 1,
                                                        (float) (3*_rowHeight/2.0));
                    ColorMagic.GradientPaint(g, rectfSeparator, LayoutColors.ColorCaptionBack, -2*LayoutColors.DepthCaption);
                }
                g.DrawLine(penLines, _xScaled[i] + iHScrll, 2*_rowHeight, _xScaled[i] + iHScrll, ClientSize.Height);
            }

            // Borders
            g.DrawLine(penBorder, 1, 2*_rowHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, 2*_rowHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            OnSelectedBarChange(new EventArgs());
        }

        /// <summary>
        /// Selects a row on Mouse Up
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _selectedRow = e.Y/_rowHeight - 2;

            if (_selectedRow < 0)
                _selectedRow = 0;
            else if (_selectedRow > _shownBars - 1)
                _selectedRow = _shownBars - 1;

            var rect = new Rectangle(0, 2*_rowHeight, ClientSize.Width, ClientSize.Height - 2*_rowHeight);
            Invalidate(rect);
            VScrollBar.Select();
        }

        /// <summary>
        /// Raises the event by invoking the delegates
        /// </summary>
        private void OnSelectedBarChange(EventArgs e)
        {
            if (SelectedBarChange == null || _selectedBarOld == SelectedBar) return;
            SelectedBarChange(this, e);
            _selectedBarOld = SelectedBar;
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int scrollBarHeight = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, 1, ClientSize.Width - 2*Border,
                                     ClientSize.Height - scrollBarHeight - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            UpdateJournalData();
            int scrallBarSize = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*_rowHeight, ClientSize.Width - 2*Border,
                                     ClientSize.Height - 2*_rowHeight - scrallBarSize - Border);
            Invalidate(rect);
        }
    }
}