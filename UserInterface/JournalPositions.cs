// JournalPositions Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Common;
using Forex_Strategy_Builder.Interfaces;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public class JournalPositions : Panel
    {
        private int[] _xColumns; // The horizontal position of the column
        private int[] _xScaled; // The scaled horizontal position of the column
        private Image[] _positionIcons; // Shows the position's type and transaction
        private string[,] _journalData; // The text journal data
        private string[] _titlesMoney; // Journal title
        private string[] _titlesPips; // Journal title

        private const int Border = 2; // The width of outside border of the panel

        private int _columns; // The number of the columns
        private int _firstPos; // The number of the first shown positions
        private int _lastPos; // The number of the last shown positions
        private int _positions; // The total number of the positions during the bar
        private int _rowHeight; // The journal row height
        private int _rows; // The number of rows can be shown (without the caption bar)
        private int _selectedBarOld; // The old selected bar
        private int _shownPos; // How many positions are shown
        private int _visibalWidth; // The visible part width of the panel
        private HScrollBar HScrollBar { get; set; }
        private VScrollBar VScrollBar { get; set; }
        private readonly IDataSet _dataSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public JournalPositions(IDataSet dataSet)
        {
            _dataSet = dataSet;

            InitializeJournal();
            SetUpJournal();
        }

        /// <summary>
        /// Sets the selected bar
        /// </summary>
        public int SelectedBar { private get; set; }

        /// <summary>
        /// Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            SetSizes();
            UpdateJournalData();
        }

        /// <summary>
        /// Initializes the Journal.
        /// </summary>
        private void InitializeJournal()
        {
            var font = new Font(Font.FontFamily, 9);
            _rowHeight = Math.Max(font.Height, 18);
            _columns = 9;
            _xColumns = new int[10];
            _xScaled = new int[10];
            Padding = new Padding(Border, 2*_rowHeight, Border, Border);

            // Horizontal ScrollBar
            HScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 50, LargeChange = 200};
            HScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            VScrollBar = new VScrollBar {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 2};
            VScrollBar.ValueChanged += VScrollBarValueChanged;

            Graphics g = CreateGraphics();

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

            _titlesPips = new[]
            {
                Language.T("Position"),
                Language.T("Transaction"),
                Language.T("Direction"),
                Language.T("Lots"),
                Language.T("Order"),
                Language.T("Ord Price"),
                Language.T("Price"),
                Language.T("Profit Loss"),
                Language.T("Floating P/L")
            };

            _titlesMoney = new[]
            {
                Language.T("Position"),
                Language.T("Transaction"),
                Language.T("Direction"),
                Language.T("Amount"),
                Language.T("Order"),
                Language.T("Ord Price"),
                Language.T("Price"),
                Language.T("Profit Loss"),
                Language.T("Floating P/L")
            };

            var asColumContent = new[]
            {
                "99999",
                longestTransaction,
                longestDirection,
                "-9999999",
                "99999",
                "99.99999",
                "99.99999",
                "-99999.99",
                "-99999.99"
            };

            _xColumns[0] = Border;
            _xColumns[1] = _xColumns[0] + (int) Math.Max(g.MeasureString(asColumContent[0], font).Width + 16, g.MeasureString(_titlesMoney[0], font).Width) + 4;
            for (int i = 1; i < _columns; i++)
                _xColumns[i + 1] = _xColumns[i] + (int) Math.Max(g.MeasureString(asColumContent[i], font).Width, g.MeasureString(_titlesMoney[i], font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        /// Updates the journal data from the backtester
        /// </summary>
        private void UpdateJournalData()
        {
            _journalData = new string[_positions,_columns];
            _positionIcons = new Image[_shownPos];

            for (int pos = _firstPos; pos < _firstPos + _shownPos; pos++)
            {
                int row = pos - _firstPos;

                _journalData[row, 0] = (StatsBuffer.PosNumb(SelectedBar, pos) + 1).ToString(CultureInfo.InvariantCulture);
                _journalData[row, 1] = Language.T(StatsBuffer.PosTransaction(SelectedBar, pos).ToString());
                _journalData[row, 2] = Language.T(StatsBuffer.PosDir(SelectedBar, pos).ToString());
                _journalData[row, 3] = Configs.AccountInMoney
                                 ? (StatsBuffer.PosDir(SelectedBar, pos) == PosDirection.Short ? "-" : "") +
                                    (StatsBuffer.PosLots(SelectedBar, pos)*_dataSet.InstrProperties.LotSize)
                                 : StatsBuffer.PosLots(SelectedBar, pos).ToString(CultureInfo.InvariantCulture);
                _journalData[row, 4] = (StatsBuffer.PosOrdNumb(SelectedBar, pos) + 1).ToString(CultureInfo.InvariantCulture);
                _journalData[row, 5] = StatsBuffer.PosOrdPrice(SelectedBar, pos).ToString(_dataSet.FF);
                _journalData[row, 6] = StatsBuffer.PosPrice(SelectedBar, pos).ToString(_dataSet.FF);

                // Profit Loss
                if (StatsBuffer.PosTransaction(SelectedBar, pos) == Transaction.Close ||
                    StatsBuffer.PosTransaction(SelectedBar, pos) == Transaction.Reduce ||
                    StatsBuffer.PosTransaction(SelectedBar, pos) == Transaction.Reverse)
                {
                    _journalData[row, 7] = Configs.AccountInMoney
                        ? StatsBuffer.PosMoneyProfitLoss(SelectedBar, pos).ToString("F2")
                        : Math.Round(StatsBuffer.PosProfitLoss(SelectedBar, pos)).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _journalData[row, 7] = "-";
                }

                // Floating Profit Loss
                if (pos == _positions - 1 && StatsBuffer.PosTransaction(SelectedBar, pos) != Transaction.Close)
                {
                    // Last bar position only
                    _journalData[row, 8] = Configs.AccountInMoney
                        ? StatsBuffer.PosMoneyFloatingPL(SelectedBar, pos).ToString("F2")
                        : Math.Round(StatsBuffer.PosFloatingPL(SelectedBar, pos)).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _journalData[row, 8] = "-";
                }

                // Icons
                _positionIcons[row] = Position.PositionIconImage(StatsBuffer.PosIcon(SelectedBar, pos));
            }
        }

        /// <summary>
        /// Sets the size and position of the controls
        /// </summary>
        private void SetSizes()
        {
            if (!StatsBuffer.IsBufferReady) return;

            _positions = StatsBuffer.Positions(SelectedBar);
            _rows = ClientSize.Height > 2 * _rowHeight + Border ? (ClientSize.Height - 2 * _rowHeight - Border) / _rowHeight : 0;

            if (_positions == 0)
            {
                _firstPos = 0;
                _lastPos = 0;
                _shownPos = 0;

                VScrollBar.Visible = false;
                _visibalWidth = ClientSize.Width;
            }
            else if (_positions < _rows)
            {
                _firstPos = 0;
                _lastPos = _rows;
                _shownPos = _positions;

                VScrollBar.Visible = false;
                _visibalWidth = ClientSize.Width;
            }
            else
            {
                VScrollBar.Visible = true;
                if (SelectedBar != _selectedBarOld)
                    VScrollBar.Value = 0;
                VScrollBar.Maximum = _positions - 1;
                _visibalWidth = VScrollBar.Left;

                _firstPos = VScrollBar.Value;
                if (_firstPos + _rows > _positions)
                {
                    _lastPos = _positions - 1;
                    _shownPos = _lastPos - _firstPos + 1;
                }
                else
                {
                    _shownPos = _rows;
                    _lastPos = _firstPos + _shownPos - 1;
                }
            }

            if (_visibalWidth <= _xColumns[_columns])
                _xColumns.CopyTo(_xScaled, 0);
            else
            {
                // Scales the columns position
                float fScale = (float) _visibalWidth/_xColumns[_columns];
                for (int i = 0; i <= _columns; i++)
                    _xScaled[i] = (int) (_xColumns[i]*fScale);
            }

            if (_visibalWidth < _xColumns[_columns])
            {
                HScrollBar.Visible = true;
                int iPoinShort = _xColumns[_columns] - _visibalWidth;
                if (HScrollBar.Value > iPoinShort)
                    HScrollBar.Value = iPoinShort;
                HScrollBar.Maximum = iPoinShort + HScrollBar.LargeChange - 2;
            }
            else
            {
                HScrollBar.Value = 0;
                HScrollBar.Visible = false;
            }

            _selectedBarOld = SelectedBar;
        }

        /// <summary>
        /// Set parameters on resize.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            SetUpJournal();
            base.OnResize(eventargs);
            Invalidate();
        }

        /// <summary>
        /// Paints the journal
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int hScrll = -HScrollBar.Value;
            var size = new Size(_visibalWidth, _rowHeight);
            var sf = new StringFormat {Alignment = StringAlignment.Center};

            // Caption background
            var rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2*_rowHeight);
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            var colorBack = LayoutColors.ColorControlBack;
            var brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            var brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            var brushRowText = new SolidBrush(LayoutColors.ColorControlText);
            var penLines = new Pen(LayoutColors.ColorJournalLines);
            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
            var font = new Font(Font.FontFamily, 9);

            // Print the journal caption
            string caption = Language.T("Positions During the Bar") + (Configs.AccountInMoney ? " [" + Configs.AccountCurrency + "]" : " [" + Language.T("pips") + "]");
            g.DrawString(caption, font, brushCaptionText, new RectangleF(Point.Empty, size), sf);
            g.SetClip(new RectangleF(Border, _rowHeight, ClientSize.Width - 2*Border, _rowHeight));
            if (Configs.AccountInMoney)
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesMoney[i], font, brushCaptionText, hScrll + (_xScaled[i] + _xScaled[i + 1])/2, _rowHeight, sf);
            else
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesPips[i], font, brushCaptionText, hScrll + (_xScaled[i] + _xScaled[i + 1])/2, _rowHeight, sf);
            g.ResetClip();

            // Paints the journal's data field
            var rectField = new RectangleF(Border, 2*_rowHeight, ClientSize.Width - 2*Border, ClientSize.Height - 2*_rowHeight - Border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            size = new Size(ClientSize.Width - VScrollBar.Width - 2*Border, _rowHeight);

            // Prints the journal data
            for (int pos = _firstPos; pos < _firstPos + _shownPos; pos++)
            {
                int row = pos - _firstPos;
                int y = (row + 2)*_rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs(row%2f - 0) > 0.0001)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Draw the position icon
                int iImgY = y + (int) Math.Floor((_rowHeight - 16)/2.0);
                g.DrawImage(_positionIcons[pos - _firstPos], hScrll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(_journalData[row, 0], font, brushRowText, hScrll + (16 + _xScaled[1])/2, (row + 2)*_rowHeight, sf);
                for (int i = 1; i < _columns; i++)
                    g.DrawString(_journalData[row, i], font, brushRowText, hScrll + (_xScaled[i] + _xScaled[i + 1])/2, (row + 2)*_rowHeight, sf);
            }

            for (int i = 1; i < _columns; i++)
                g.DrawLine(penLines, _xScaled[i] + hScrll, 2*_rowHeight, _xScaled[i] + hScrll, ClientSize.Height);

            // Border
            g.DrawLine(penBorder, 1, 2*_rowHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, 2*_rowHeight, ClientSize.Width - Border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int height = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, _rowHeight + 1, ClientSize.Width - 2*Border, ClientSize.Height - _rowHeight - height - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int height = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*_rowHeight, ClientSize.Width - 2*Border, ClientSize.Height - 2*_rowHeight - height - Border);
            Invalidate(rect);
        }
    }
}