// Bar Explorer
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    internal sealed class BarExplorer : Form
    {
        private const int Border = 2;
        private readonly Backtester _backtester;
        private readonly Brush _brushCaptionText;
        private readonly Brush _brushEvenRow;
        private readonly Brush _brushGridText;
        private readonly Brush _brushRed;

        private readonly Color _colorBarBlack1;
        private readonly Color _colorBarBlack2;
        private readonly Color _colorBarWight1;
        private readonly Color _colorBarWight2;
        private readonly Color _colorClosedTrade1;
        private readonly Color _colorClosedTrade2;

        private readonly Color _colorLongTrade1;
        private readonly Color _colorLongTrade2;
        private readonly Color _colorShortTrade1;
        private readonly Color _colorShortTrade2;
        private readonly Font _fontInfo;
        private readonly int _infoRowHeight;
        private readonly Pen _penAxes;
        private readonly Pen _penBarBorder;
        private readonly Pen _penCross;
        private readonly Pen _penGrid;
        private readonly Panel _pnlChart;
        private readonly Panel _pnlInfo;
        private readonly ToolTip _toolTip;
        private int[] _aiColumnX; // The horizontal position of the column
        private int[] _aiX; // The scaled horizontal position of the column
        private string[] _asContent; // The text journal data
        private string[] _asTitles; // Journal title second row
        private int _barCurrent;
        private string _barInfo;
        private int _columns; // The number of the columns
        private int _maxWayPoints = 7;
        private Size _szPrice;

        /// <summary>
        /// Initialize the form and controls
        /// </summary>
        public BarExplorer(Backtester backtester, int barNumber)
        {
            _backtester = backtester;
            _pnlChart = new Panel();
            _pnlInfo = new Panel();
            _toolTip = new ToolTip();

            _barCurrent = barNumber < _backtester.Strategy.FirstBar ? _backtester.Strategy.FirstBar : barNumber;

            Text = Language.T("Bar Explorer");
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = Data.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            _fontInfo = new Font(Font.FontFamily, 9);
            _infoRowHeight = Math.Max(_fontInfo.Height, 18);

            _barInfo = Language.T("Bar") + ": " + (_barCurrent + 1) + " " +
                       Data.DataSet.Time[_barCurrent].ToString(Data.DF) + " " +
                       Data.DataSet.Time[_barCurrent].ToString("HH:mm") + "; " +
                       Language.T("Interpolation method") + ": " +
                       _backtester.InterpolationMethodToString();

            _pnlChart.Parent = this;
            _pnlChart.Paint += PnlChartPaint;

            _pnlInfo.Parent = this;
            _pnlInfo.Paint += PnlInfoPaint;

            BtnsNavigate = new Button[6];
            var btnNavigateText = new[] {"< !", "<<", "<", ">", ">>", "! >"};
            var btnNavigateTips = new[]
                                      {
                                          Language.T("Previous ambiguous bar."),
                                          Language.T("Previous deal."),
                                          Language.T("Previous bar."),
                                          Language.T("Next bar."),
                                          Language.T("Next deal."),
                                          Language.T("Next ambiguous bar.")
                                      };

            for (int i = 0; i < 6; i++)
            {
                BtnsNavigate[i] = new Button {Parent = this, Text = btnNavigateText[i], Name = btnNavigateText[i]};
                BtnsNavigate[i].Click += BtnNavigateClick;
                BtnsNavigate[i].MouseWheel += BarExplorerMouseWheel;
                BtnsNavigate[i].KeyUp += BtnNavigateKeyUp;
                BtnsNavigate[i].UseVisualStyleBackColor = true;
                _toolTip.SetToolTip(BtnsNavigate[i], btnNavigateTips[i]);
            }

            BtnsNavigate[0].Enabled = _backtester.AmbiguousBars > 0;
            BtnsNavigate[1].Enabled = _backtester.PositionsTotal > 0;
            BtnsNavigate[4].Enabled = _backtester.PositionsTotal > 0;
            BtnsNavigate[5].Enabled = _backtester.AmbiguousBars > 0;

            NUDGo = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            NUDGo.BeginInit();
            NUDGo.Minimum = _backtester.Strategy.FirstBar + 1;
            NUDGo.Maximum = Data.DataSet.Bars;
            NUDGo.Increment = 1;
            NUDGo.Value = _barCurrent + 1;
            NUDGo.KeyUp += BtnNavigateKeyUp;
            NUDGo.EndInit();

            BtnGo = new Button {Parent = this, Name = "Go", Text = Language.T("Go"), UseVisualStyleBackColor = true};
            BtnGo.Click += BtnNavigateClick;
            BtnGo.MouseWheel += BarExplorerMouseWheel;
            BtnGo.KeyUp += BtnNavigateKeyUp;
            _toolTip.SetToolTip(BtnGo, Language.T("Go to the chosen bar."));

            //Button Close
            BtnClose = new Button
                           {
                               Parent = this,
                               Text = Language.T("Close"),
                               DialogResult = DialogResult.Cancel,
                               UseVisualStyleBackColor = true
                           };

            // Colors
            _brushRed = new SolidBrush(LayoutColors.ColorSignalRed);

            _brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            _brushEvenRow = new SolidBrush(LayoutColors.ColorEvenRowBack);
            _brushGridText = new SolidBrush(LayoutColors.ColorChartFore);

            _penGrid = new Pen(LayoutColors.ColorChartGrid)
                           {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            _penAxes = new Pen(LayoutColors.ColorChartFore);
            _penCross = new Pen(LayoutColors.ColorChartCross);
            _penBarBorder = new Pen(LayoutColors.ColorBarBorder);

            _colorBarWight1 = ColorMagic.GetGradientColor(LayoutColors.ColorBarWhite, 30);
            _colorBarWight2 = ColorMagic.GetGradientColor(LayoutColors.ColorBarWhite, -30);
            _colorBarBlack1 = ColorMagic.GetGradientColor(LayoutColors.ColorBarBlack, 30);
            _colorBarBlack2 = ColorMagic.GetGradientColor(LayoutColors.ColorBarBlack, -30);

            _colorLongTrade1 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeLong, 30);
            _colorLongTrade2 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeLong, -30);
            _colorShortTrade1 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeShort, 30);
            _colorShortTrade2 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeShort, -30);
            _colorClosedTrade1 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeClose, 30);
            _colorClosedTrade2 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeClose, -30);

            SetJournalPoints();
        }

        private Button BtnClose { get; set; }
        private Button BtnGo { get; set; }
        private Button[] BtnsNavigate { get; set; }
        private NumericUpDown NUDGo { get; set; }

        /// <summary>
        /// Sets the journal columns with.
        /// </summary>
        private void SetJournalPoints()
        {
            _columns = 7;
            _aiColumnX = new int[8];
            _aiX = new int[8];

            Graphics g = CreateGraphics();

            _asTitles = new[]
                            {
                                Language.T("Number"),
                                Language.T("Description"),
                                Language.T("Price"),
                                Language.T("Direction"),
                                Language.T("Lots"),
                                Language.T("Position"),
                                Language.T("Order")
                            };

            string longestDescription = "";
            foreach (WayPointType wpType in Enum.GetValues(typeof (WayPointType)))
                if (g.MeasureString(Language.T(WayPoint.WPTypeToString(wpType)), _fontInfo).Width >
                    g.MeasureString(longestDescription, _fontInfo).Width)
                    longestDescription = Language.T(WayPoint.WPTypeToString(wpType));

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof (PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), _fontInfo).Width >
                    g.MeasureString(longestDirection, _fontInfo).Width)
                    longestDirection = Language.T(posDir.ToString());

            _asContent = new[]
                             {
                                 "99",
                                 longestDescription,
                                 "99.99999",
                                 longestDirection,
                                 "99999",
                                 "99999",
                                 "99999"
                             };

            _aiColumnX[0] = Border;
            for (int i = 0; i < _columns; i++)
                _aiColumnX[i + 1] = _aiColumnX[i] +
                                    (int)
                                    Math.Max(g.MeasureString(_asContent[i], _fontInfo).Width,
                                             g.MeasureString(_asTitles[i], _fontInfo).Width) + 4;

            _szPrice = g.MeasureString("9.9999", _fontInfo).ToSize();

            g.Dispose();
        }

        /// <summary>
        /// Resizes the form.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetBtnNavigate();
            for (int bar = _backtester.Strategy.FirstBar; bar < Data.DataSet.Bars; bar++)
                if (_backtester.WayPoints(bar) > _maxWayPoints)
                    _maxWayPoints = _backtester.WayPoints(bar);

            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int clientSizeWidth = (Math.Max(_aiColumnX[_columns] + 2*btnHrzSpace, 550));
            ClientSize = new Size(clientSizeWidth, 310 + _infoRowHeight*(_maxWayPoints + 2));
        }

        /// <summary>
        /// Arrange the controls.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;

            int width = ClientSize.Width - 2*space;

            NUDGo.Size = new Size(65, buttonHeight);
            NUDGo.Location = new Point(space, ClientSize.Height - buttonHeight - btnVertSpace + 3);

            BtnGo.Size = new Size(65, buttonHeight);
            BtnGo.Location = new Point(NUDGo.Right + btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Close
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            int btnNavigateWidth = buttonWidth*2/5;
            int btnNavigateSapce = btnHrzSpace*2/3;
            int position = BtnGo.Right + ((BtnClose.Left - BtnGo.Right) - (6*btnNavigateWidth + 5*btnNavigateSapce))/2;
            for (int btn = 0; btn < BtnsNavigate.Length; btn++)
            {
                BtnsNavigate[btn].Size = new Size(btnNavigateWidth, buttonHeight);
                BtnsNavigate[btn].Location = new Point(position + (btnNavigateWidth + btnNavigateSapce)*btn,
                                                       ClientSize.Height - buttonHeight - btnVertSpace);
            }

            _pnlInfo.Size = new Size(width, _infoRowHeight*(_maxWayPoints + 2));
            _pnlInfo.Location = new Point(space, BtnClose.Top - btnVertSpace - _pnlInfo.Height);
            _pnlChart.Location = new Point(space, space);
            _pnlChart.Size = new Size(width, _pnlInfo.Top - 2*space);

            // Scales the columns position
            double scale = (double) _pnlInfo.Width/_aiColumnX[_columns];
            for (int i = 0; i <= _columns; i++)
                _aiX[i] = (int) (_aiColumnX[i]*scale);
        }

        /// <summary>
        /// Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Navigate to a bar on mouse wheel.
        /// </summary>
        private void BarExplorerMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && BtnsNavigate[3].Enabled)
                Navigate(">");
            else if (e.Delta < 0 && BtnsNavigate[2].Enabled)
                Navigate("<");
        }

        /// <summary>
        /// Navigate to a bar on button click.
        /// </summary>
        private void BtnNavigateClick(object sender, EventArgs e)
        {
            Navigate(((Button) sender).Name);
        }

        /// <summary>
        /// Navigate to a bar on button KeyUp.
        /// </summary>
        private void BtnNavigateKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.PageUp)
                Navigate(">max");
            else if (e.KeyCode == Keys.PageUp)
                Navigate(">>");
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.PageDown)
                Navigate("<max");
            else if (e.KeyCode == Keys.PageDown)
                Navigate("<<");
        }

        /// <summary>
        /// Navigates to a bar.
        /// </summary>
        private void Navigate(string sDir)
        {
            switch (sDir)
            {
                case "< !":
                    for (int i = _barCurrent - 1; i >= _backtester.Strategy.FirstBar; i--)
                        if (_backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                        {
                            _barCurrent = i;
                            break;
                        }
                    break;
                case "! >":
                    for (int i = _barCurrent + 1; i < Data.DataSet.Bars; i++)
                        if (_backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                        {
                            _barCurrent = i;
                            break;
                        }
                    break;
                case "<<":
                    for (int i = _barCurrent - 1; i >= _backtester.Strategy.FirstBar; i--)
                        if (_backtester.SummaryTrans(i) != Transaction.Transfer &&
                            _backtester.SummaryTrans(i) != Transaction.None)
                        {
                            _barCurrent = i;
                            break;
                        }
                    break;
                case ">>":
                    for (int i = _barCurrent + 1; i < Data.DataSet.Bars; i++)
                        if (_backtester.SummaryTrans(i) != Transaction.Transfer &&
                            _backtester.SummaryTrans(i) != Transaction.None)
                        {
                            _barCurrent = i;
                            break;
                        }
                    break;
                case "<max":
                    int maxWP = 0;
                    int maxBar = _barCurrent;
                    for (int i = _barCurrent - 1; i >= _backtester.Strategy.FirstBar; i--)
                        if (_backtester.WayPoints(i) > maxWP)
                        {
                            maxWP = _backtester.WayPoints(i);
                            maxBar = i;
                        }
                    _barCurrent = maxBar;
                    break;
                case ">max":
                    maxWP = 0;
                    maxBar = _barCurrent;
                    for (int i = _barCurrent + 1; i < Data.DataSet.Bars; i++)
                        if (_backtester.WayPoints(i) > maxWP)
                        {
                            maxWP = _backtester.WayPoints(i);
                            maxBar = i;
                        }
                    _barCurrent = maxBar;
                    break;
                case "<":
                    if (_barCurrent > _backtester.Strategy.FirstBar)
                        _barCurrent--;
                    break;
                case ">":
                    if (_barCurrent < Data.DataSet.Bars - 1)
                        _barCurrent++;
                    break;
                case "Go":
                    _barCurrent = (int) NUDGo.Value - 1;
                    break;
            }

            SetBtnNavigate();

            _barInfo = Language.T("Bar") + ": " + (_barCurrent + 1) +
                       " " + Data.DataSet.Time[_barCurrent].ToString(Data.DF) +
                       " " + Data.DataSet.Time[_barCurrent].ToString("HH:mm") + "; " +
                       Language.T("Interpolation method") + ": " +
                       _backtester.InterpolationMethodToString();

            var rectPnlChart = new Rectangle(Border, _infoRowHeight, _pnlChart.ClientSize.Width - 2*Border,
                                             _pnlChart.ClientSize.Height - _infoRowHeight - Border);
            _pnlChart.Invalidate(rectPnlChart);

            var rectPnlInfo = new Rectangle(Border, 2*_infoRowHeight, _pnlInfo.ClientSize.Width - 2*Border,
                                            _pnlInfo.ClientSize.Height - 2*_infoRowHeight - Border);
            _pnlInfo.Invalidate(rectPnlInfo);

            NUDGo.Value = _barCurrent + 1;
        }

        /// <summary>
        /// Sets the navigation buttons
        /// </summary>
        private void SetBtnNavigate()
        {
            // Buttons "Ambiguous"
            if (_backtester.AmbiguousBars > 0)
            {
                bool isButtonAmbiguous = false;
                for (int i = _backtester.Strategy.FirstBar; i < _barCurrent; i++)
                    if (_backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                    {
                        isButtonAmbiguous = true;
                        break;
                    }
                BtnsNavigate[0].Enabled = isButtonAmbiguous;

                isButtonAmbiguous = false;
                for (int i = _barCurrent + 1; i < Data.DataSet.Bars; i++)
                    if (_backtester.BackTestEval(i) == BacktestEval.Ambiguous)
                    {
                        isButtonAmbiguous = true;
                        break;
                    }
                BtnsNavigate[5].Enabled = isButtonAmbiguous;
            }

            // Buttons "Deals"
            if (_backtester.PositionsTotal > 0)
            {
                bool isButtonDeal = false;
                for (int i = _backtester.Strategy.FirstBar; i < _barCurrent; i++)
                    if (_backtester.Positions(i) > 0)
                    {
                        isButtonDeal = true;
                        break;
                    }
                BtnsNavigate[1].Enabled = isButtonDeal;

                isButtonDeal = false;
                for (int i = _barCurrent + 1; i < Data.DataSet.Bars; i++)
                    if (_backtester.Positions(i) > 0)
                    {
                        isButtonDeal = true;
                        break;
                    }
                BtnsNavigate[4].Enabled = isButtonDeal;
            }

            BtnsNavigate[2].Enabled = _barCurrent > _backtester.Strategy.FirstBar;
            BtnsNavigate[3].Enabled = _barCurrent < Data.DataSet.Bars - 1;

            BtnsNavigate[0].ForeColor = BtnsNavigate[0].Enabled ? Color.Red : BtnsNavigate[2].ForeColor;
            BtnsNavigate[5].ForeColor = BtnsNavigate[5].Enabled ? Color.Red : BtnsNavigate[2].ForeColor;
        }

        /// <summary>
        /// Paints panel pnlChart
        /// </summary>
        private void PnlChartPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorChartBack);

            if (!Data.IsData || !Data.IsResult) return;

            var pnl = (Panel) sender;
            int width = pnl.ClientSize.Width;

            // Caption background
            var pntStart = new PointF(0, 0);
            SizeF szfCaption = new Size(width, _infoRowHeight);
            var rectfCaption = new RectangleF(pntStart, szfCaption);
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            var stringFormat = new StringFormat
                                   {
                                       LineAlignment = StringAlignment.Center,
                                       Trimming = StringTrimming.EllipsisCharacter,
                                       FormatFlags = StringFormatFlags.NoWrap,
                                       Alignment = StringAlignment.Center
                                   };
            string stringCaptionText = Language.T("Price Route Inside the Bar");
            rectfCaption = new RectangleF(Border, 0, pnl.ClientSize.Width - 2*Border, _infoRowHeight);
            g.DrawString(stringCaptionText, _fontInfo, _brushCaptionText, rectfCaption, stringFormat);

            // Paint the panel background
            var rectClient = new RectangleF(0, _infoRowHeight, pnl.ClientSize.Width, pnl.Height - _infoRowHeight);
            ColorMagic.GradientPaint(g, rectClient, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Paint bar info
            var rectBarInfo = new RectangleF(Border, _infoRowHeight + 1, pnl.ClientSize.Width - 2*Border, _infoRowHeight);
            g.DrawString(_barInfo, _fontInfo, _brushGridText, rectBarInfo, stringFormat);

            // Searching the min and the max price and volume
            width = pnl.ClientSize.Width - 2*Border;
            double maxPrice = Data.DataSet.High[_barCurrent];
            double minPrice = Data.DataSet.Low[_barCurrent];
            const int space = 8;
            int spcRight = _szPrice.Width + 4;
            const int xLeft = Border + space;
            int xRight = width - spcRight;
            int yTop = 2*_infoRowHeight + 6;
            int yBottom = pnl.ClientSize.Height - 22;
            int barPixels = _maxWayPoints < 10 ? 28 : _maxWayPoints < 15 ? 24 : 20;
            const int spcLeft = 3;
            int x = barPixels + spcLeft;

            int pointLeft = x + barPixels + 30;
            int pointRight = xRight - 20;
            int points = _backtester.WayPoints(_barCurrent);
            const int pointRadius = 3;

            // Grid
            var iCntLabels = (int) Math.Max((yBottom - yTop)/30d, 1);
            double deltaPoint = (Data.DataSet.InstrProperties.Digits == 5 || Data.DataSet.InstrProperties.Digits == 3)
                                    ? Data.DataSet.InstrProperties.Point*100
                                    : Data.DataSet.InstrProperties.Point*10;
            double delta =
                Math.Max(
                    Math.Round((maxPrice - minPrice)/iCntLabels, Data.DataSet.InstrProperties.Point < 0.001 ? 3 : 1),
                    deltaPoint);

            minPrice = Math.Round(minPrice, Data.DataSet.InstrProperties.Point < 0.001f ? 3 : 1) -
                       Data.DataSet.InstrProperties.Point*10;
            minPrice -= delta;
            maxPrice += delta;
            iCntLabels = (int) Math.Ceiling((maxPrice - minPrice)/delta);
            maxPrice = minPrice + iCntLabels*delta;

            double scaleY = (yBottom - yTop)/(iCntLabels*delta);
            var yOpen = (int) (yBottom - (Data.DataSet.Open[_barCurrent] - minPrice)*scaleY);
            var yHigh = (int) (yBottom - (Data.DataSet.High[_barCurrent] - minPrice)*scaleY);
            var yLow = (int) (yBottom - (Data.DataSet.Low[_barCurrent] - minPrice)*scaleY);
            var yClose = (int) (yBottom - (Data.DataSet.Close[_barCurrent] - minPrice)*scaleY);

            // Find the price distance
            double priceDistance = 0;
            for (int point = 1; point < points; point++)
            {
                priceDistance +=
                    Math.Abs(_backtester.WayPoint(_barCurrent, point).Price -
                             _backtester.WayPoint(_barCurrent, point - 1).Price);
            }
            double dPriceForAPixel = (pointRight - pointLeft)/priceDistance;

            // Points X
            var aiPointX = new int[points];
            aiPointX[0] = pointLeft;
            for (int point = 1; point < points - 1; point++)
            {
                var iDistance =
                    (int)
                    (Math.Abs(_backtester.WayPoint(_barCurrent, point).Price -
                              _backtester.WayPoint(_barCurrent, point - 1).Price)*dPriceForAPixel);
                aiPointX[point] = aiPointX[point - 1] + iDistance;
            }
            aiPointX[points - 1] = pointRight;
            for (int point = 1; point < points - 1; point++)
            {
                if (aiPointX[point] - aiPointX[point - 1] < barPixels + 1)
                    aiPointX[point] = aiPointX[point - 1] + barPixels + 1;
            }
            for (int point = points - 2; point > 0; point--)
            {
                if (aiPointX[point + 1] - aiPointX[point] < barPixels + 1)
                    aiPointX[point] = aiPointX[point + 1] - barPixels - 1;
            }

            // Find coordinates of the Way Points
            var pntWay = new Point[points];
            for (int point = 0; point < points; point++)
            {
                var pointY = (int) (yBottom - (_backtester.WayPoint(_barCurrent, point).Price - minPrice)*scaleY);
                pntWay[point] = new Point(aiPointX[point], pointY);
            }

            // Horizontal grid and Price labels
            for (double label = minPrice; label <= maxPrice + Data.DataSet.InstrProperties.Point; label += delta)
            {
                var labelY = (int) (yBottom - (label - minPrice)*scaleY);
                g.DrawString(label.ToString(Data.FF), Font, _brushGridText, xRight, labelY - Font.Height/2 - 1);
                g.DrawLine(_penGrid, Border + space, labelY, xRight, labelY);
            }

            // Vertical Grid
            g.DrawLine(_penGrid, x + barPixels/2 - 1, yTop, x + barPixels/2 - 1, yBottom + 2);
            for (int point = 0; point < points; point++)
            {
                var pt1 = new Point(pntWay[point].X, yTop);
                var pt2 = new Point(pntWay[point].X, yBottom + 2);
                var pt3 = new Point(pntWay[point].X - 5, yBottom + 4);
                g.DrawLine(_penGrid, pt1, pt2);
                g.DrawString((point + 1).ToString(CultureInfo.InvariantCulture), Font, _brushGridText, pt3);
            }

            // Bar Number
            string barNumber = (_barCurrent + 1).ToString(CultureInfo.InvariantCulture);
            int stringX = x + barPixels/2 - 1 - g.MeasureString(barNumber, Font).ToSize().Width/2;
            Brush barBrush = _backtester.BackTestEval(_barCurrent) == BacktestEval.Ambiguous
                                 ? _brushRed
                                 : _brushGridText;
            g.DrawString(barNumber, Font, barBrush, stringX, yBottom + 4);

            // Draw the bar
            g.DrawLine(_penBarBorder, x + barPixels/2 - 1, yLow, x + barPixels/2 - 1, yHigh);
            if (yClose < yOpen) // White bar
            {
                var rect = new Rectangle(x, yClose, barPixels - 2, yOpen - yClose);
                var lgBrush = new LinearGradientBrush(rect, _colorBarWight1, _colorBarWight2, 5f);
                g.FillRectangle(lgBrush, rect);
                g.DrawRectangle(_penBarBorder, x, yClose, barPixels - 2, yOpen - yClose);
            }
            else if (yClose > yOpen) // Black bar
            {
                var rect = new Rectangle(x, yOpen, barPixels - 2, yClose - yOpen);
                var lgBrush = new LinearGradientBrush(rect, _colorBarBlack1, _colorBarBlack2, 5f);
                g.FillRectangle(lgBrush, rect);
                g.DrawRectangle(_penBarBorder, x, yOpen, barPixels - 2, yClose - yOpen);
            }
            else // Cross
            {
                g.DrawLine(_penBarBorder, x, yClose, x + barPixels - 2, yClose);
            }

            // Draw cancelled orders
            for (int orderIndex = 0; orderIndex < _backtester.Orders(_barCurrent); orderIndex++)
            {
                int ordNumber = _backtester.OrdNumb(_barCurrent, orderIndex);
                Order order = _backtester.OrdFromNumb(ordNumber);
                if (order.OrdStatus != OrderStatus.Cancelled)
                    continue;

                if (order.OrdPrice > Data.DataSet.High[_barCurrent] || order.OrdPrice < Data.DataSet.Low[_barCurrent])
                    continue;

                int d = barPixels/2 - 1;
                int x1 = x + d;
                int x2 = x + barPixels - 2;
                var yDeal = (int) (yBottom - (order.OrdPrice - minPrice)*scaleY);
                var pen = new Pen(LayoutColors.ColorChartGrid, 2);

                if (order.OrdDir == OrderDirection.Buy)
                {
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                    g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                    g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                }
                else if (order.OrdDir == OrderDirection.Sell)
                {
                    g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                    g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                    g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                    g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                }
            }

            // Draw the deals on the bar
            for (int pos = 0; pos < _backtester.Positions(_barCurrent); pos++)
            {
                if (_backtester.PosTransaction(_barCurrent, pos) == Transaction.Transfer)
                    continue;

                var yDeal = (int) (yBottom - (_backtester.PosOrdPrice(_barCurrent, pos) - minPrice)*scaleY);

                if (_backtester.PosDir(_barCurrent, pos) == PosDirection.Long ||
                    _backtester.PosDir(_barCurrent, pos) == PosDirection.Short)
                {
                    int d = barPixels/2 - 1;
                    int x1 = x + d;
                    int x2 = x + barPixels - 2;
                    if (_backtester.OrdFromNumb(_backtester.PosOrdNumb(_barCurrent, pos)).OrdDir == OrderDirection.Buy)
                    {
                        // Buy
                        var pen = new Pen(LayoutColors.ColorTradeLong, 2);
                        g.DrawLine(pen, x, yDeal, x1, yDeal);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                        g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                        g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                    }
                    else
                    {
                        // Sell
                        var pen = new Pen(LayoutColors.ColorTradeShort, 2);
                        g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                        g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                        g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                    }
                }
                else if (_backtester.PosDir(_barCurrent, pos) == PosDirection.Closed)
                {
                    // Close position
                    int d = barPixels/2 - 1;
                    int x1 = x + d;
                    int x2 = x + barPixels - 3;
                    var pen = new Pen(LayoutColors.ColorTradeClose, 2);
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal + d/2, x2, yDeal - d/2);
                    g.DrawLine(pen, x1, yDeal - d/2, x2, yDeal + d/2);
                }
            }

            // Draw position lots
            for (int point = 0; point < points; point++)
            {
                int posNumber = _backtester.WayPoint(_barCurrent, point).PosNumb;
                if (posNumber == -1) continue;

                Position position = _backtester.PosFromNumb(posNumber);
                double posLots = position.PosLots;
                PosDirection posDirection = position.PosDir;
                WayPointType wpType = _backtester.WayPoint(_barCurrent, point).WPType;

                var hight = (int) (Math.Max(posLots*2, 2));
                int lenght = barPixels;
                int posX = pntWay[point].X - (barPixels - 1)/2;
                int posY = yBottom - hight;

                if (point < points - 1)
                    lenght = pntWay[point + 1].X - pntWay[point].X + 1;

                if (posDirection == PosDirection.Long)
                {
                    // Long
                    var rect = new Rectangle(posX - 1, posY - 1, lenght, hight + 2);
                    var lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2,
                                                          LinearGradientMode.Vertical);
                    rect = new Rectangle(posX - 1, posY, lenght, hight);
                    g.FillRectangle(lgBrush, rect);
                }
                else if (posDirection == PosDirection.Short)
                {
                    // Short
                    var rect = new Rectangle(posX - 1, posY - 1, lenght, hight + 2);
                    var lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2,
                                                          LinearGradientMode.Vertical);
                    rect = new Rectangle(posX - 1, posY, lenght, hight);
                    g.FillRectangle(lgBrush, rect);
                }
                else if (posDirection == PosDirection.Closed && wpType == WayPointType.Exit)
                {
                    // Closed
                    var rect = new Rectangle(posX - 1, yBottom - 2, barPixels + 1, 2);
                    var lgBrush = new LinearGradientBrush(rect, _colorClosedTrade1, _colorClosedTrade2,
                                                          LinearGradientMode.Vertical);
                    rect = new Rectangle(posX, yBottom - 2, barPixels - 1, 2);
                    g.FillRectangle(lgBrush, rect);
                }
            }

            // Draw the Beziers
            for (int point = 1; point < points; point++)
            {
                Point ptKnot1 = pntWay[point - 1];
                Point ptKnot2 = pntWay[point];

                int ctrlX1 = (ptKnot1.X + ptKnot2.X)/2;
                int ctrlX2 = (ptKnot1.X + ptKnot2.X)/2;

                int ctrlY1 = ptKnot1.Y;
                int ctrlY2 = ptKnot2.Y;

                if (point > 1)
                {
                    if (pntWay[point - 2].Y > pntWay[point - 1].Y && pntWay[point - 1].Y > pntWay[point].Y ||
                        pntWay[point - 2].Y < pntWay[point - 1].Y && pntWay[point - 1].Y < pntWay[point].Y)
                    {
                        ctrlY1 = (pntWay[point - 1].Y + pntWay[point].Y)/2;
                    }
                }
                if (point < points - 1)
                {
                    if (pntWay[point - 1].Y > pntWay[point].Y && pntWay[point].Y > pntWay[point + 1].Y ||
                        pntWay[point - 1].Y < pntWay[point].Y && pntWay[point].Y < pntWay[point + 1].Y)
                    {
                        ctrlY2 = (pntWay[point - 1].Y + pntWay[point].Y)/2;
                    }
                }

                if (point == 1)
                {
                    ctrlX1 = ptKnot1.X;
                    ctrlY1 = ptKnot1.Y;
                }
                if (point == points - 1)
                {
                    ctrlX2 = ptKnot2.X;
                    ctrlY2 = ptKnot2.Y;
                }

                var ptControl1 = new Point(ctrlX1, ctrlY1);
                var ptControl2 = new Point(ctrlX2, ctrlY2);

                g.DrawBezier(_penCross, ptKnot1, ptControl1, ptControl2, ptKnot2);
            }

            // Draw the WayPoints
            Brush brushWeyPnt = new SolidBrush(LayoutColors.ColorChartBack);
            for (int point = 0; point < points; point++)
            {
                g.FillEllipse(brushWeyPnt, pntWay[point].X - pointRadius, pntWay[point].Y - pointRadius, 2*pointRadius,
                              2*pointRadius);
                g.DrawEllipse(_penCross, pntWay[point].X - pointRadius, pntWay[point].Y - pointRadius, 2*pointRadius,
                              2*pointRadius);
            }

            // Draw O, H, L, C labels
            for (int point = 0; point < points; point++)
            {
                WayPointType wpType = _backtester.WayPoint(_barCurrent, point).WPType;
                if (wpType != WayPointType.Open && wpType != WayPointType.High &&
                    wpType != WayPointType.Low && wpType != WayPointType.Close)
                    continue;

                string label = "?";
                switch (wpType)
                {
                    case WayPointType.Open:
                        label = "O";
                        break;
                    case WayPointType.High:
                        label = "H";
                        break;
                    case WayPointType.Low:
                        label = "L";
                        break;
                    case WayPointType.Close:
                        label = "C";
                        break;
                }

                int xPoint = pntWay[point].X;
                int yPoint = pntWay[point].Y - Font.Height - 3;

                var stringFormatLabel = new StringFormat {Alignment = StringAlignment.Center};
                g.DrawString(label, Font, _brushGridText, xPoint, yPoint, stringFormatLabel);
            }

            // Draw the deals on the route
            for (int point = 0; point < points; point++)
            {
                int posNumber = _backtester.WayPoint(_barCurrent, point).PosNumb;
                int ordNumber = _backtester.WayPoint(_barCurrent, point).OrdNumb;

                if (posNumber < 0 || ordNumber < 0)
                    continue;

                PosDirection posDirection = _backtester.PosFromNumb(posNumber).PosDir;
                OrderDirection ordDirection = _backtester.OrdFromNumb(ordNumber).OrdDir;
                WayPointType wpType = _backtester.WayPoint(_barCurrent, point).WPType;

                if (wpType == WayPointType.None || wpType == WayPointType.Open || wpType == WayPointType.High ||
                    wpType == WayPointType.Low || wpType == WayPointType.Close)
                    continue;

                int yDeal = pntWay[point].Y;

                if (posDirection == PosDirection.Long || posDirection == PosDirection.Short ||
                    wpType == WayPointType.Cancel)
                {
                    int d = barPixels/2 - 1;
                    x = pntWay[point].X - d;
                    int x1 = pntWay[point].X;
                    int x2 = x + barPixels - 2;
                    if (ordDirection == OrderDirection.Buy)
                    {
                        // Buy
                        var pen = new Pen(LayoutColors.ColorTradeLong, 2);
                        if (wpType == WayPointType.Cancel)
                            pen = new Pen(LayoutColors.ColorChartGrid, 2);
                        g.DrawLine(pen, x, yDeal, x1, yDeal);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                        g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                        g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                    }
                    else
                    {
                        // Sell
                        var pen = new Pen(LayoutColors.ColorTradeShort, 2);
                        if (wpType == WayPointType.Cancel)
                            pen = new Pen(LayoutColors.ColorChartGrid, 2);
                        g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                        g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                        g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                    }
                }

                if (posDirection == PosDirection.Closed)
                {
                    // Close position
                    int d = barPixels/2 - 1;
                    x = pntWay[point].X - d;
                    int x1 = pntWay[point].X;
                    int x2 = x + barPixels - 3;
                    var pen = new Pen(LayoutColors.ColorTradeClose, 2);
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal + d/2, x2, yDeal - d/2);
                    g.DrawLine(pen, x1, yDeal - d/2, x2, yDeal + d/2);
                }
            }

            // Coordinate axes
            g.DrawLine(_penAxes, xLeft, yTop - 4, xLeft, yBottom); // Vertical left line
            g.DrawLine(_penAxes, xLeft, yBottom, xRight, yBottom);

            // Border
            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);
            g.DrawLine(penBorder, 1, _infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - Border + 1, _infoRowHeight, pnl.ClientSize.Width - Border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - Border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - Border + 1);
        }

        /// <summary>
        /// Paints panel pnlInfo
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            // ---------------------------------------------------------------------+
            // |                          Way points description                    |
            // |--------------------------------------------------------------------+
            // | Number | Description | Price | Direction | Lots | Position | Order |
            // |--------------------------------------------------------------------+
            //xp0      xp1           xp2     xp3         xp4    xp5        xp6     xp7

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (!Data.IsData || !Data.IsResult) return;

            var pnl = (Panel) sender;
            string ff = Data.FF; // Format modifier to print the floats
            var size = new Size(_aiX[_columns] - _aiX[0], _infoRowHeight);
            var sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near};

            // Caption background
            var pntStart = new PointF(0, 0);
            SizeF szfCaption = new Size(pnl.ClientSize.Width, 2*_infoRowHeight);
            var rectfCaption = new RectangleF(pntStart, szfCaption);
            ColorMagic.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            var stringFormat = new StringFormat
                                   {
                                       LineAlignment = StringAlignment.Center,
                                       Trimming = StringTrimming.EllipsisCharacter,
                                       FormatFlags = StringFormatFlags.NoWrap,
                                       Alignment = StringAlignment.Near
                                   };
            string stringCaptionText = Language.T("Way Points Description");
            float captionWidth = Math.Min(_pnlInfo.ClientSize.Width, _aiX[_columns] - _aiX[0]);
            float fCaptionTextWidth = g.MeasureString(stringCaptionText, _fontInfo).Width;
            float fCaptionTextX = Math.Max((captionWidth - fCaptionTextWidth)/2f, 0);
            var pfCaptionText = new PointF(fCaptionTextX, 0);
            var sfCaptionText = new SizeF(captionWidth - fCaptionTextX, _infoRowHeight);
            rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);

            // First caption raw
            g.DrawString(stringCaptionText, _fontInfo, _brushCaptionText, rectfCaption, stringFormat);

            // Second caption raw
            for (int i = 0; i < _columns; i++)
                g.DrawString(_asTitles[i], _fontInfo, _brushCaptionText, (_aiX[i] + _aiX[i + 1])/2f, _infoRowHeight, sf);

            Brush brush = new SolidBrush(LayoutColors.ColorControlText);

            for (int pnt = 0; pnt < _backtester.WayPoints(_barCurrent); pnt++)
            {
                int y = (pnt + 2)*_infoRowHeight;
                var point = new Point(_aiX[0], y);

                // Even row
                if (Math.Abs(pnt%2f - 0) > 0.00001)
                    g.FillRectangle(_brushEvenRow, new Rectangle(point, size));

                int positionNumber = _backtester.WayPoint(_barCurrent, pnt).PosNumb;
                WayPointType wpType = _backtester.WayPoint(_barCurrent, pnt).WPType;
                PosDirection posDirection = _backtester.PosFromNumb(positionNumber).PosDir;
                double posLots = _backtester.PosFromNumb(positionNumber).PosLots;
                int ordNumber = _backtester.WayPoint(_barCurrent, pnt).OrdNumb;

                g.DrawString((pnt + 1).ToString(CultureInfo.InvariantCulture), _fontInfo, brush, (_aiX[0] + _aiX[1])/2f,
                             y, sf);
                g.DrawString(Language.T(WayPoint.WPTypeToString(wpType)), _fontInfo, brush, _aiX[1] + 2, y);
                g.DrawString(_backtester.WayPoint(_barCurrent, pnt).Price.ToString(ff), _fontInfo, brush,
                             (_aiX[3] + _aiX[2])/2f, y, sf);

                if (positionNumber > -1)
                {
                    g.DrawString(Language.T(posDirection.ToString()), _fontInfo, brush, (_aiX[4] + _aiX[3])/2f, y, sf);
                    g.DrawString(posLots.ToString(CultureInfo.InvariantCulture), _fontInfo, brush,
                                 (_aiX[5] + _aiX[4])/2f, y, sf);
                    g.DrawString((positionNumber + 1).ToString(CultureInfo.InvariantCulture), _fontInfo, brush,
                                 (_aiX[6] + _aiX[5])/2f, y, sf);
                }

                if (ordNumber > -1)
                {
                    g.DrawString((ordNumber + 1).ToString(CultureInfo.InvariantCulture), _fontInfo, brush,
                                 (_aiX[7] + _aiX[6])/2f, y, sf);
                }
            }

            // Vertical lines
            var penLine = new Pen(LayoutColors.ColorJournalLines);
            for (int i = 1; i < _columns; i++)
                g.DrawLine(penLine, _aiX[i], 2*_infoRowHeight, _aiX[i], ClientSize.Height - Border);

            // Border
            var penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);
            g.DrawLine(penBorder, 1, 2*_infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - Border + 1, 2*_infoRowHeight, pnl.ClientSize.Width - Border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - Border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - Border + 1);
        }
    }
}