// Journal_Positions Class
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
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public sealed class JournalByPositions : ContextPanel
    {
        private const int Border = 2; // The width of outside border of the panel

        private Brush _brushCaptionText;
        private Brush _brushEvenRowBack;
        private Brush _brushRowText;
        private Brush _brushSelectedRowBack;
        private Brush _brushSelectedRowText;
        private Brush _brushWarningBack;
        private Brush _brushWarningText;
        private Color _colorBack;
        private int[] _columnX; // The horizontal position of the column
        private int _columns; // The number of the columns
        private int _firstPos; // The number of the first shown position
        private Font _font;
        private HScrollBar _hScrollBar;
        private string[,] _journalData; // The text journal data
        private int _lastPos; // The number of the last shown position
        private Pen _penBorder;
        private Pen _penLines;
        private Image[] _posIcons; // Shows the position's type and transaction
        private int[] _posNumbers; // Contains the numbers of all positions without transferred
        private int _positions; // The total number of the positions
        private int _rowHeight; // The journal row height
        private int _rows; // The number of rows can be shown (without the caption bar)
        private int[] _scaledX; // The scaled horizontal position of the column
        private int _selectedBarOld; // The number of the old selected bar
        private int _selectedRow; // The number of the selected row
        private int _shownPos; // How many positions are shown
        private string[] _titlesInMoney; // Journal title
        private string[] _titlesInPips; // Journal title
        private VScrollBar _vScrollBar;
        private readonly IDataSet _dataSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public JournalByPositions(IDataSet dataSet)
        {
            _dataSet = dataSet;

            InitializeJournal();
            SetUpJournal();
            SetJournalColors();
        }

        /// <summary>
        /// Gets the selected bar
        /// </summary>
        public int SelectedBar
        {
            get
            {
                return ShowTransfers
                           ? StatsBuffer.PosCoordinates[_firstPos + _selectedRow].Bar
                           : StatsBuffer.PosCoordinates[_posNumbers[_firstPos + _selectedRow]].Bar;
            }
        }

        /// <summary>
        /// Sets whether Journal shows transfers.
        /// </summary>
        public bool ShowTransfers { private get; set; }

        public event EventHandler SelectedBarChange;

        /// <summary>
        /// Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            if (ShowTransfers)
            {
                _positions = StatsBuffer.PositionsTotal;
            }
            else
            {
                _posNumbers = StatsBuffer.PositionsTotal > 0 ? new int[StatsBuffer.PositionsTotal] : new int[1];
                _positions = 0;
                for (int bar = 0; bar < _dataSet.Bars; bar++)
                {
                    for (int pos = 0; pos < StatsBuffer.Positions(bar); pos++)
                    {
                        Transaction transaction = StatsBuffer.PosTransaction(bar, pos);
                        if (transaction == Transaction.None || transaction == Transaction.Transfer) continue;
                        _posNumbers[_positions] = StatsBuffer.PosNumb(bar, pos);
                        _positions++;
                    }
                }
            }

            if (_positions == 0)
            {
                _firstPos = 0;
                _lastPos = 0;
                _shownPos = 0;
                _selectedRow = 0;

                _vScrollBar.Enabled = false;
            }
            else if (_positions < _rows)
            {
                _firstPos = 0;
                _lastPos = _rows;
                _shownPos = _positions;
                _selectedRow = 0;

                _vScrollBar.Enabled = false;
            }
            else
            {
                _vScrollBar.Enabled = true;
                _vScrollBar.Maximum = _positions - 1;

                _firstPos = _vScrollBar.Value;
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

            _selectedRow = Math.Min(_selectedRow, _shownPos - 1);
            _selectedRow = Math.Max(_selectedRow, 0);

            UpdateJournalData();
            SetJournalColors();
        }

        /// <summary>
        /// Sets the journal colors
        /// </summary>
        private void SetJournalColors()
        {
            _colorBack = LayoutColors.ColorControlBack;
            _brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            _brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            _brushSelectedRowBack = new SolidBrush(LayoutColors.ColorSelectedRowBack);
            _brushSelectedRowText = new SolidBrush(LayoutColors.ColorSelectedRowText);
            _brushRowText = new SolidBrush(LayoutColors.ColorControlText);
            _brushWarningBack = new SolidBrush(LayoutColors.ColorWarningRowBack);
            _brushWarningText = new SolidBrush(LayoutColors.ColorWarningRowText);
            _penLines = new Pen(LayoutColors.ColorJournalLines);
            _penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                 Border);

            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }

        /// <summary>
        /// Initializes the Journal
        /// </summary>
        private void InitializeJournal()
        {
            // Horizontal ScrollBar
            _hScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 100, LargeChange = 300};
            _hScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            _vScrollBar = new VScrollBar
                              {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 4};
            _vScrollBar.ValueChanged += VScrollBarValueChanged;

            Graphics g = CreateGraphics();
            _font = new Font(Font.FontFamily, 9);

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof (PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), _font).Width >
                    g.MeasureString(longestDirection, _font).Width)
                    longestDirection = Language.T(posDir.ToString());

            string longestTransaction = "";
            foreach (Transaction transaction in Enum.GetValues(typeof (Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), _font).Width >
                    g.MeasureString(longestTransaction, _font).Width)
                    longestTransaction = Language.T(transaction.ToString());

            string longestBacktestEval = "";
            foreach (BacktestEval eval in Enum.GetValues(typeof (BacktestEval)))
                if (g.MeasureString(Language.T(eval.ToString()), _font).Width >
                    g.MeasureString(longestBacktestEval, _font).Width)
                    longestBacktestEval = Language.T(eval.ToString());

            _titlesInPips = new[]
                                {
                                    Language.T("Position"),
                                    Language.T("Bar"),
                                    Language.T("Bar Opening Time"),
                                    Language.T("Transaction"),
                                    Language.T("Direction"),
                                    Language.T("Lots"),
                                    Language.T("Ord Price"),
                                    Language.T("Avrg Price"),
                                    Language.T("Margin"),
                                    Language.T("Spread"),
                                    Language.T("Rollover"),
                                    Language.T("Commission"),
                                    Language.T("Slippage"),
                                    Language.T("Profit Loss"),
                                    Language.T("Floating P/L"),
                                    Language.T("Balance"),
                                    Language.T("Equity"),
                                    Language.T("Backtest")
                                };

            _titlesInMoney = new[]
                                 {
                                     Language.T("Position"),
                                     Language.T("Bar"),
                                     Language.T("Bar Opening Time"),
                                     Language.T("Transaction"),
                                     Language.T("Direction"),
                                     Language.T("Amount"),
                                     Language.T("Ord Price"),
                                     Language.T("Avrg Price"),
                                     Language.T("Margin"),
                                     Language.T("Spread"),
                                     Language.T("Rollover"),
                                     Language.T("Commission"),
                                     Language.T("Slippage"),
                                     Language.T("Profit Loss"),
                                     Language.T("Floating P/L"),
                                     Language.T("Balance"),
                                     Language.T("Equity"),
                                     Language.T("Backtest")
                                 };

            var asColumContent = new[]
                                     {
                                         "99999",
                                         "99999",
                                         "12/03/08 00:00",
                                         longestTransaction,
                                         longestDirection,
                                         "-99999999",
                                         "99.99999",
                                         "99.99999",
                                         "999999.99",
                                         "-999.99",
                                         "-999.99",
                                         "-999.99",
                                         "-999.99",
                                         "-999999.99",
                                         "-999999.99",
                                         "-99999999.99",
                                         "-99999999.99",
                                         longestBacktestEval
                                     };

            _rowHeight = Math.Max(_font.Height, 18);
            Padding = new Padding(Border, 2*_rowHeight, Border, Border);

            _columns = 18;
            _columnX = new int[19];
            _scaledX = new int[19];

            _columnX[0] = Border;
            _columnX[1] = _columnX[0] +
                          (int)
                          Math.Max(g.MeasureString(asColumContent[0], _font).Width + 16,
                                   g.MeasureString(_titlesInMoney[0], _font).Width) + 4;
            for (int i = 1; i < _columns; i++)
                _columnX[i + 1] = _columnX[i] +
                                  (int)
                                  Math.Max(g.MeasureString(asColumContent[i], _font).Width,
                                           g.MeasureString(_titlesInMoney[i], _font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        /// Updates the journal data from the StatsBuffer.
        /// </summary>
        private void UpdateJournalData()
        {
            _journalData = new string[_shownPos,_columns];
            _posIcons = new Image[_shownPos];

            for (int posIndex = _firstPos; posIndex < _firstPos + _shownPos; posIndex++)
            {
                int posNumber = posIndex;

                if (!ShowTransfers)
                    posNumber = _posNumbers[posIndex];

                int row = posIndex - _firstPos;
                int bar = StatsBuffer.PosCoordinates[posNumber].Bar;
                Position position = StatsBuffer.PosFromNumb(posNumber);

                string posAmount = Configs.AccountInMoney
                                       ? (position.PosDir == PosDirection.Short ? "-" : "") +
                                         (position.PosLots*_dataSet.InstrProperties.LotSize).ToString(
                                             CultureInfo.InvariantCulture)
                                       : position.PosLots.ToString(CultureInfo.InvariantCulture);

                string profitLoss = Configs.AccountInMoney
                                        ? position.MoneyProfitLoss.ToString("F2")
                                        : position.ProfitLoss.ToString("F2");

                string floatingPL = Configs.AccountInMoney
                                        ? position.MoneyFloatingPL.ToString("F2")
                                        : position.FloatingPL.ToString("F2");

                int p = 0;
                _journalData[row, p++] = (posNumber + 1).ToString(CultureInfo.InvariantCulture);
                _journalData[row, p++] = (bar + 1).ToString(CultureInfo.InvariantCulture);
                _journalData[row, p++] = _dataSet.Time[bar].ToString(Data.DF) + _dataSet.Time[bar].ToString(" HH:mm");
                _journalData[row, p++] = Language.T(position.Transaction.ToString());
                _journalData[row, p++] = Language.T(position.PosDir.ToString());
                _journalData[row, p++] = posAmount;
                _journalData[row, p++] = position.FormOrdPrice.ToString(_dataSet.FF);
                _journalData[row, p++] = position.PosPrice.ToString(_dataSet.FF);
                _journalData[row, p++] = position.RequiredMargin.ToString("F2");

                // Charges
                if (Configs.AccountInMoney)
                {
                    // in currency
                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reverse)
                        _journalData[row, p++] = position.MoneySpread.ToString("F2");
                    else
                        _journalData[row, p++] = "-";

                    _journalData[row, p++] = position.Transaction == Transaction.Transfer
                                                 ? position.MoneyRollover.ToString("F2")
                                                 : "-";

                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Close ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reduce ||
                        position.Transaction == Transaction.Reverse)
                    {
                        _journalData[row, p++] = position.MoneyCommission.ToString("F2");
                        _journalData[row, p++] = position.MoneySlippage.ToString("F2");
                    }
                    else
                    {
                        _journalData[row, p++] = "-";
                        _journalData[row, p++] = "-";
                    }
                }
                else
                {
                    // In pips
                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reverse)
                        _journalData[row, p++] = position.Spread.ToString(CultureInfo.InvariantCulture);
                    else
                        _journalData[row, p++] = "-";

                    _journalData[row, p++] = position.Transaction == Transaction.Transfer
                                                 ? position.Rollover.ToString("F2")
                                                 : "-";

                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Close ||
                        position.Transaction == Transaction.Add ||
                        position.Transaction == Transaction.Reduce ||
                        position.Transaction == Transaction.Reverse)
                    {
                        _journalData[row, p++] = position.Commission.ToString("F2");
                        _journalData[row, p++] = position.Slippage.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        _journalData[row, p++] = "-";
                        _journalData[row, p++] = "-";
                    }
                }

                // Profit Loss
                if (position.Transaction == Transaction.Close ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                    _journalData[row, p++] = profitLoss;
                else
                    _journalData[row, p++] = "-";

                // Floating Profit Loss
                if (position.PosNumb == StatsBuffer.SummaryPosNumb(bar) &&
                    position.Transaction != Transaction.Close)
                    _journalData[row, p++] = floatingPL; //Last position of the bar only
                else
                    _journalData[row, p++] = "-";

                // Balance / Equity
                if (Configs.AccountInMoney)
                {
                    _journalData[row, p++] = position.MoneyBalance.ToString("F2");
                    _journalData[row, p++] = position.MoneyEquity.ToString("F2");
                }
                else
                {
                    _journalData[row, p++] = position.Balance.ToString("F2");
                    _journalData[row, p++] = position.Equity.ToString("F2");
                }

                _journalData[row, p] = Language.T(StatsBuffer.BackTestEvalToString(bar));

                // Icons
                _posIcons[row] = Position.PositionIconImage(position.PositionIcon);
            }
        }

        /// <summary>
        /// Set parameters on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (ClientSize.Height > 2*_rowHeight + Border)
            {
                _rows = (ClientSize.Height - 2*_rowHeight - Border)/_rowHeight;
            }
            else
            {
                _rows = 0;
            }

            if (ClientSize.Width - _vScrollBar.Width - 2*Border <= _columnX[_columns])
                _columnX.CopyTo(_scaledX, 0);
            else
            {
                // Scales the columns position
                float fScale = (float) (ClientSize.Width - _vScrollBar.Width - 2*Border)/_columnX[_columns];
                for (int i = 0; i <= _columns; i++)
                    _scaledX[i] = (int) (_columnX[i]*fScale);
            }

            if (ClientSize.Width - _vScrollBar.Width - 2*Border < _columnX[_columns])
            {
                _hScrollBar.Visible = true;
                int iPoinShort = _columnX[_columns] - ClientSize.Width + _vScrollBar.Width + 2*Border;
                if (_hScrollBar.Value > iPoinShort)
                    _hScrollBar.Value = iPoinShort;
                _hScrollBar.Maximum = iPoinShort + _hScrollBar.LargeChange - 2;
            }
            else
            {
                _hScrollBar.Value = 0;
                _hScrollBar.Visible = false;
            }

            UpdateButtonsLocation();
            SetUpJournal();
            Invalidate();
        }

        /// <summary>
        /// Paints the journal
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int scrll = -_hScrollBar.Value;
            var size = new Size(ClientSize.Width, _rowHeight);
            var stringFormat = new StringFormat {Alignment = StringAlignment.Center};

            // Caption background
            var rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2*_rowHeight);
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Print the journal caption
            string stringCaptionText = Language.T("Journal by Positions") +
                                       (ShowTransfers ? "" : " " + Language.T("without Transfers")) +
                                       (Configs.AccountInMoney
                                            ? " [" + Configs.AccountCurrency + "]"
                                            : " [" + Language.T("pips") + "]");
            g.DrawString(stringCaptionText, _font, _brushCaptionText, new RectangleF(Point.Empty, size), stringFormat);
            g.SetClip(new RectangleF(Border, _rowHeight, ClientSize.Width - 2*Border, _rowHeight));
            if (Configs.AccountInMoney)
            {
                g.DrawString(_titlesInMoney[0], _font, _brushCaptionText, scrll + (_scaledX[0] + _scaledX[1])/2,
                             _rowHeight, stringFormat);
                for (int i = 1; i < _columns; i++)
                    g.DrawString(_titlesInMoney[i], _font, _brushCaptionText, scrll + (_scaledX[i] + _scaledX[i + 1])/2,
                                 _rowHeight, stringFormat);
            }
            else
            {
                g.DrawString(_titlesInPips[0], _font, _brushCaptionText, scrll + (_scaledX[0] + _scaledX[1])/2,
                             _rowHeight, stringFormat);
                for (int i = 1; i < _columns; i++)
                    g.DrawString(_titlesInPips[i], _font, _brushCaptionText, scrll + (_scaledX[i] + _scaledX[i + 1])/2,
                                 _rowHeight, stringFormat);
            }
            g.ResetClip();

            // Paints the journal's data field
            var rectField = new RectangleF(Border, 2*_rowHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - 2*_rowHeight - Border);
            g.FillRectangle(new SolidBrush(_colorBack), rectField);

            size = new Size(ClientSize.Width - _vScrollBar.Width - 2*Border, _rowHeight);

            // Prints the journal data
            for (int pos = _firstPos; pos < _firstPos + _shownPos; pos++)
            {
                int y = (pos - _firstPos + 2)*_rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs((pos - _firstPos)%2f - 0) > 0.0001)
                    g.FillRectangle(_brushEvenRowBack, new Rectangle(point, size));

                // Warning row
                bool isWarning = false;
                if (_journalData[pos - _firstPos, _columns - 1] == Language.T("Ambiguous"))
                {
                    g.FillRectangle(_brushWarningBack, new Rectangle(point, size));
                    isWarning = true;
                }

                // Selected row
                Brush brush;
                if (pos - _firstPos == _selectedRow)
                {
                    g.FillRectangle(_brushSelectedRowBack, new Rectangle(point, size));
                    brush = _brushSelectedRowText;
                }
                else
                {
                    brush = isWarning ? _brushWarningText : _brushRowText;
                }

                // Draw the position icon
                int iImgY = y + (int) Math.Floor((_rowHeight - 16)/2.0);
                g.DrawImage(_posIcons[pos - _firstPos], scrll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(_journalData[pos - _firstPos, 0], _font, brush, scrll + (16 + _scaledX[1])/2,
                             (pos - _firstPos + 2)*_rowHeight, stringFormat);
                for (int i = 1; i < _columns; i++)
                    g.DrawString(_journalData[pos - _firstPos, i], _font, brush,
                                 scrll + (_scaledX[i] + _scaledX[i + 1])/2, (pos - _firstPos + 2)*_rowHeight,
                                 stringFormat);
            }

            //g.DrawLine(penLines, 0, iRowHeight, ClientSize.Width, iRowHeight);
            for (int i = 1; i < _columns; i++)
                g.DrawLine(_penLines, _scaledX[i] + scrll, 2*_rowHeight, _scaledX[i] + scrll, ClientSize.Height);

            // Border
            g.DrawLine(_penBorder, 1, 2*_rowHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, 2*_rowHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

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
            else if (_selectedRow > _shownPos - 1)
                _selectedRow = _shownPos - 1;

            var rect = new Rectangle(0, 2*_rowHeight, ClientSize.Width, ClientSize.Height - 2*_rowHeight);
            Invalidate(rect);
            _vScrollBar.Select();
        }

        /// <summary>
        /// Raises the event by invoking the delegates
        /// </summary>
        private void OnSelectedBarChange(EventArgs e)
        {
            // Invokes the delegate
            if (_firstPos + _selectedRow < 0)
                return;

            if (SelectedBarChange == null || _selectedBarOld == SelectedBar) return;
            SelectedBarChange(this, e);
            _selectedBarOld = SelectedBar;
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int scrallBarWidth = _hScrollBar.Visible ? _hScrollBar.Height : 0;
            var rect = new Rectangle(Border, _rowHeight + 1, ClientSize.Width - 2*Border,
                                     ClientSize.Height - _rowHeight - scrallBarWidth - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int scrallBarWidth = _hScrollBar.Visible ? _hScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*_rowHeight, ClientSize.Width - 2*Border,
                                     ClientSize.Height - 2*_rowHeight - scrallBarWidth - Border);
            Invalidate(rect);
        }
    }
}