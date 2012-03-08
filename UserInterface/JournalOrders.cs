// Journal_Ord Class
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
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public class JournalOrders : ContextPanel
    {
        private const int Border = 2; // The width of outside border of the panel
        private Brush _brushCaptionText;
        private Brush _brushEvenRowBack;
        private Brush _brushRowText;
        private Color _colorBack;

        private int _columns; // The number of the columns
        private int _firstOrd; // The number of the first shown orders
        private Font _font;
        private string[,] _journalData; // The text journal data
        private int _lastOrd; // The number of the last shown orders
        private Image[] _orderIcons; // Shows the position's type and transaction
        private int _orders; // The total number of the orders during the bar
        private Pen _penBorder;
        private Pen _penLines;
        private int _rowHeight; // The journal row height
        private int _rows; // The number of rows can be shown (without the caption bar)
        private int _selectedBar; // The selected bar
        private int _selectedBarOld; // The old selected bar
        private int _shownOrd; // How many orders are shown
        private string[] _titlesMoney; // Journal title
        private string[] _titlesPips; // Journal title
        private int _visibleWidth; // The width of the panel visible part
        private int[] _xColumns; // The horizontal position of the column
        private int[] _xScaled; // The scaled horizontal position of the column

        /// <summary>
        /// Constructor
        /// </summary>
        public JournalOrders()
        {
            InitializeJournal();
            SetUpJournal();
        }

        private VScrollBar VScrollBar { get; set; }
        private HScrollBar HScrollBar { get; set; }

        /// <summary>
        /// Sets the selected bar
        /// </summary>
        public int SelectedBar
        {
            set { _selectedBar = value; }
        }

        /// <summary>
        /// Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            SetSizes();
            SetJournalColors();
            UpdateJournalData();
        }

        /// <summary>
        /// Sets the journal colors
        /// </summary>
        private void SetJournalColors()
        {
            _colorBack = LayoutColors.ColorControlBack;
            _brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            _brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            _brushRowText = new SolidBrush(LayoutColors.ColorControlText);
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
            _columns = 8;
            _xColumns = new int[9];
            _xScaled = new int[9];
            _font = new Font(Font.FontFamily, 9);
            _rowHeight = Math.Max(_font.Height, 18);
            Padding = new Padding(Border, 2*_rowHeight, Border, Border);

            // Horizontal ScrollBar
            HScrollBar = new HScrollBar {Parent = this, Dock = DockStyle.Bottom, SmallChange = 50, LargeChange = 200};
            HScrollBar.ValueChanged += HScrollBarValueChanged;

            // Vertical ScrollBar
            VScrollBar = new VScrollBar
                             {Parent = this, Dock = DockStyle.Right, TabStop = true, SmallChange = 1, LargeChange = 2};
            VScrollBar.ValueChanged += VScrollBarValueChanged;

            var asComments = new[]
                                 {
                                     "Exit Order to position",
                                     "Exit Order to order",
                                     "Take Profit to position",
                                     "Take Profit to order",
                                     "Trailing Stop to position",
                                     "Trailing Stop to order",
                                     "Permanent S/L to position",
                                     "Permanent S/L to order",
                                     "Permanent T/P to position",
                                     "Permanent T/P to order"
                                 };

            Graphics g = CreateGraphics();

            string longestDirection = "";
            foreach (OrderDirection ordDir in Enum.GetValues(typeof (OrderDirection)))
                if (g.MeasureString(Language.T(ordDir.ToString()), _font).Width >
                    g.MeasureString(longestDirection, _font).Width)
                    longestDirection = Language.T(ordDir.ToString());

            string longestType = "";
            foreach (OrderType ordType in Enum.GetValues(typeof (OrderType)))
                if (g.MeasureString(Language.T(ordType.ToString()), _font).Width >
                    g.MeasureString(longestType, _font).Width)
                    longestType = Language.T(ordType.ToString());

            string longestStatus = "";
            foreach (OrderStatus ordStatus in Enum.GetValues(typeof (OrderStatus)))
                if (g.MeasureString(Language.T(ordStatus.ToString()), _font).Width >
                    g.MeasureString(longestStatus, _font).Width)
                    longestStatus = Language.T(ordStatus.ToString());

            string longestComment = "";
            foreach (string ordComment in asComments)
                if (g.MeasureString(Language.T(ordComment) + " 99999", _font).Width >
                    g.MeasureString(longestComment, _font).Width)
                    longestComment = Language.T(ordComment) + " 99999";

            _titlesPips = new[]
                              {
                                  Language.T("Order"),
                                  Language.T("Direction"),
                                  Language.T("Type"),
                                  Language.T("Lots"),
                                  Language.T("Price 1"),
                                  Language.T("Price 2"),
                                  Language.T("Status"),
                                  Language.T("Comment")
                              };

            _titlesMoney = new[]
                               {
                                   Language.T("Order"),
                                   Language.T("Direction"),
                                   Language.T("Type"),
                                   Language.T("Amount"),
                                   Language.T("Price 1"),
                                   Language.T("Price 2"),
                                   Language.T("Status"),
                                   Language.T("Comment")
                               };

            var asColumContent = new[]
                                     {
                                         "99999",
                                         longestDirection,
                                         longestType,
                                         "-9999999",
                                         "99.99999",
                                         "99.99999",
                                         longestStatus,
                                         longestComment
                                     };

            _xColumns[0] = Border;
            _xColumns[1] = _xColumns[0] +
                           (int)
                           Math.Max(g.MeasureString(asColumContent[0], _font).Width + 16,
                                    g.MeasureString(_titlesMoney[0], _font).Width) + 4;
            for (int i = 1; i < _columns; i++)
                _xColumns[i + 1] = _xColumns[i] +
                                   (int)
                                   Math.Max(g.MeasureString(asColumContent[i], _font).Width,
                                            g.MeasureString(_titlesMoney[i], _font).Width) + 4;
            g.Dispose();
        }

        /// <summary>
        /// Updates the journal data from the StatsBuffer.
        /// </summary>
        private void UpdateJournalData()
        {
            if (!Data.IsResult)
                return;

            _journalData = new string[_orders,_columns];
            _orderIcons = new Image[_orders];
            int[] ordNumbers = ArrangeOrderNumbers();

            for (int ord = _firstOrd; ord < _firstOrd + _shownOrd; ord++)
            {
                int row = ord - _firstOrd;
                Order order = StatsBuffer.OrdFromNumb(ordNumbers[ord]);

                _journalData[row, 0] = (order.OrdNumb + 1).ToString(CultureInfo.InvariantCulture);
                _journalData[row, 1] = Language.T(order.OrdDir.ToString());
                _journalData[row, 2] = Language.T(order.OrdType.ToString());
                _journalData[row, 3] = Configs.AccountInMoney
                                           ? (order.OrdDir == OrderDirection.Sell ? "-" : "") +
                                             (order.OrdLots*Data.DataSet.InstrProperties.LotSize)
                                           : order.OrdLots.ToString(CultureInfo.InvariantCulture);
                _journalData[row, 4] = order.OrdPrice.ToString(Data.FF);
                _journalData[row, 5] = (order.OrdPrice2 > 0 ? order.OrdPrice2.ToString(Data.FF) : "-");
                _journalData[row, 6] = Language.T(order.OrdStatus.ToString());
                _journalData[row, 7] = order.OrdNote;

                // Icons
                _orderIcons[row] = Order.OrderIconImage(order.OrderIcon);
            }
        }

        private int[] ArrangeOrderNumbers()
        {
            var ordNumbers = new int[_orders];
            int index = 0;
            for (int wayPoint = 0; wayPoint < StatsBuffer.WayPoints(_selectedBar); wayPoint++)
            {
                int ordNumb = StatsBuffer.WayPoint(_selectedBar, wayPoint).OrdNumb;
                WayPointType wpType = StatsBuffer.WayPoint(_selectedBar, wayPoint).WPType;

                if (ordNumb == -1) continue; // There is no order
                if (ordNumb < StatsBuffer.OrdNumb(_selectedBar, 0)) continue; // For a transferred position

                if (wpType == WayPointType.Add || wpType == WayPointType.Cancel ||
                    wpType == WayPointType.Entry || wpType == WayPointType.Exit ||
                    wpType == WayPointType.Reduce || wpType == WayPointType.Reverse)
                {
                    ordNumbers[index] = ordNumb;
                    index++;
                }
            }

            for (int ord = 0; ord < _orders; ord++)
            {
                int ordNumb = StatsBuffer.OrdNumb(_selectedBar, ord);
                bool toIncluded = true;
                for (int i = 0; i < index; i++)
                {
                    if (ordNumb != ordNumbers[i]) continue;
                    toIncluded = false;
                    break;
                }
                if (!toIncluded) continue;
                ordNumbers[index] = ordNumb;
                index++;
            }

            return ordNumbers;
        }

        /// <summary>
        /// Sets the size and position of the controls
        /// </summary>
        private void SetSizes()
        {
            _orders = Data.IsResult ? StatsBuffer.Orders(_selectedBar) : 0;
            _rows = ClientSize.Height > 2*_rowHeight + Border
                        ? (ClientSize.Height - 2*_rowHeight - Border)/_rowHeight
                        : 0;

            if (_orders == 0)
            {
                _firstOrd = 0;
                _lastOrd = 0;
                _shownOrd = 0;

                VScrollBar.Visible = false;
                _visibleWidth = ClientSize.Width;
            }
            else if (_orders < _rows)
            {
                _firstOrd = 0;
                _lastOrd = _rows;
                _shownOrd = _orders;

                VScrollBar.Visible = false;
                _visibleWidth = ClientSize.Width;
            }
            else
            {
                VScrollBar.Visible = true;
                if (_selectedBar != _selectedBarOld)
                    VScrollBar.Value = 0;
                VScrollBar.Maximum = _orders - 1;
                _visibleWidth = VScrollBar.Left;

                _firstOrd = VScrollBar.Value;
                if (_firstOrd + _rows > _orders)
                {
                    _lastOrd = _orders - 1;
                    _shownOrd = _lastOrd - _firstOrd + 1;
                }
                else
                {
                    _shownOrd = _rows;
                    _lastOrd = _firstOrd + _shownOrd - 1;
                }
            }

            if (_visibleWidth <= _xColumns[_columns])
                _xColumns.CopyTo(_xScaled, 0);
            else
            {
                // Scales the columns position
                float scale = (float) _visibleWidth/_xColumns[_columns];
                for (int i = 0; i <= _columns; i++)
                    _xScaled[i] = (int) (_xColumns[i]*scale);
            }

            if (_visibleWidth < _xColumns[_columns])
            {
                HScrollBar.Visible = true;
                int poinShort = _xColumns[_columns] - _visibleWidth;
                if (HScrollBar.Value > poinShort)
                    HScrollBar.Value = poinShort;
                HScrollBar.Maximum = poinShort + HScrollBar.LargeChange - 2;
            }
            else
            {
                HScrollBar.Value = 0;
                HScrollBar.Visible = false;
            }

            _selectedBarOld = _selectedBar;
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

            // Caption background
            var rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2*_rowHeight);
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            int hScrll = -HScrollBar.Value;
            //var size = new Size(ContextButtonLocation.X, _rowHeight);
            var size = new Size(ClientSize.Width, _rowHeight);
            var sf = new StringFormat {Alignment = StringAlignment.Center};

            // Print the journal caption
            string stringCaptionText = Language.T("Orders During the Bar") +
                                       (Configs.AccountInMoney
                                            ? " [" + Configs.AccountCurrency + "]"
                                            : " [" + Language.T("pips") + "]");
            g.DrawString(stringCaptionText, _font, _brushCaptionText, new RectangleF(Point.Empty, size), sf);
            g.SetClip(new RectangleF(Border, _rowHeight, ClientSize.Width - 2*Border, _rowHeight));
            if (Configs.AccountInMoney)
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesMoney[i], _font, _brushCaptionText, hScrll + (_xScaled[i] + _xScaled[i + 1])/2,
                                 _rowHeight, sf);
            else
                for (int i = 0; i < _columns; i++)
                    g.DrawString(_titlesPips[i], _font, _brushCaptionText, hScrll + (_xScaled[i] + _xScaled[i + 1])/2,
                                 _rowHeight, sf);
            g.ResetClip();

            // Paints the journal's data field
            var rectField = new RectangleF(Border, 2*_rowHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - 2*_rowHeight - Border);
            g.FillRectangle(new SolidBrush(_colorBack), rectField);

            size = new Size(ClientSize.Width - VScrollBar.Width - 2*Border, _rowHeight);

            // Prints the journal data
            for (int ord = _firstOrd; ord < _firstOrd + _shownOrd; ord++)
            {
                int row = ord - _firstOrd;
                if (_journalData == null || _journalData[row, 0] == null)
                {
                    Console.WriteLine("Break");
                    break;
                }

                int y = (row + 2)*_rowHeight;
                var point = new Point(Border, y);

                // Even row
                if (Math.Abs(row%2f - 0) > 0.0001)
                    g.FillRectangle(_brushEvenRowBack, new Rectangle(point, size));

                // Draw the position icon
                int iImgY = y + (int) Math.Floor((_rowHeight - 16)/2.0);
                g.DrawImage(_orderIcons[row], hScrll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(_journalData[row, 0], _font, _brushRowText, hScrll + (16 + _xScaled[1])/2,
                             (row + 2)*_rowHeight, sf);
                for (int i = 1; i < _columns; i++)
                {
                    if (i == _columns - 1)
                        g.DrawString(_journalData[row, i], _font, _brushRowText, hScrll + _xScaled[i],
                                     (row + 2)*_rowHeight);
                    else
                        g.DrawString(_journalData[row, i], _font, _brushRowText,
                                     hScrll + (_xScaled[i] + _xScaled[i + 1])/2, (row + 2)*_rowHeight, sf);
                }
            }

            for (int i = 1; i < _columns; i++)
                g.DrawLine(_penLines, _xScaled[i] + hScrll, 2*_rowHeight, _xScaled[i] + hScrll, ClientSize.Height);

            // Border
            g.DrawLine(_penBorder, 1, 2*_rowHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, 2*_rowHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void HScrollBarValueChanged(object sender, EventArgs e)
        {
            int height = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, _rowHeight + 1, ClientSize.Width - 2*Border,
                                     ClientSize.Height - _rowHeight - height - Border - 1);
            Invalidate(rect);
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        private void VScrollBarValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int height = HScrollBar.Visible ? HScrollBar.Height : 0;
            var rect = new Rectangle(Border, 2*_rowHeight, ClientSize.Width - 2*Border,
                                     ClientSize.Height - 2*_rowHeight - height - Border);
            Invalidate(rect);
        }
    }
}