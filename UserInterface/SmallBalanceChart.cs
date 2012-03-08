// Small Balance Chart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.CustomControls;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Draws a small balance chart
    /// </summary>
    public class SmallBalanceChart : ContextPanel
    {
        private const int Space = 5;
        private const int Border = 2;

        private int _xLeft;
        private float _xMarginCallBar;
        private float _xOOSBar;
        private int _xRight;
        private float _xScale;
        private float _yBalance;
        private int _yBottom;
        private float _yPriceScale;
        private float _yScale;
        private int _yTop;

        private PointF[] _balancePoints;
        private PointF[] _closePricePoints;
        private PointF[] _equityPoints;
        private PointF[] _longBalancePoints;
        private PointF[] _shortBalancePoints;

        private int _labelStep;
        private float _captionHeight;
        private int _chartBars;
        private int _countLabels;
        private float _delta;
        private int _labelWidth;

        private readonly SmallBalanceChartData _data;

        private Font _font;
        private Brush _brushFore;
        private Pen _penBorder;
        private Pen _penGrid;
        private RectangleF _rectfCaption;
        private bool _showPriceLine;
        private bool _isHideScanningLine;
        private bool _isNotPaint;
        private bool _isScanPerformed;
        private string _chartTitle;
        private StringFormat _stringFormatCaption;

        /// <summary>
        /// Whether to show dynamic info
        /// </summary>
        public bool ShowDynamicInfo { private get; set; }

        /// <summary>
        /// Returns dynamic info
        /// </summary>
        public string CurrentBarInfo { get; private set; }

        /// <summary>
        /// Set the OOS Bar
        /// </summary>
        public int OOSBar { private get; set; }

        /// <summary>
        /// Set the OOS
        /// </summary>
        public bool IsOOS { private get; set; }

        public SmallBalanceChart()
        {
            OOSBar = Data.DataSet.Bars - 1;
            _data = new SmallBalanceChartData();
        }

        /// <summary>
        /// Sets chart's back testing data.
        /// </summary>
        public void SetChartData(Backtester backtester)
        {
            _isNotPaint = !Data.IsData || !Data.IsResult || Data.DataSet.Bars <= backtester.Strategy.FirstBar;

            if (_isNotPaint) return;

            _showPriceLine = Configs.ShowPriceChartOnAccountChart && backtester.ExecutedOrders > 0;
            _isScanPerformed = backtester.IsScanPerformed;

            _data.FirstBar = backtester.Strategy.FirstBar;
            _data.Bars = Data.DataSet.Bars;
            _chartBars = Data.DataSet.Bars - backtester.Strategy.FirstBar;

            int maxBalance = Configs.AccountInMoney ? (int) backtester.MaxMoneyBalance : backtester.MaxBalance;
            int minBalance = Configs.AccountInMoney ? (int) backtester.MinMoneyBalance : backtester.MinBalance;
            int maxEquity = Configs.AccountInMoney ? (int) backtester.MaxMoneyEquity : backtester.MaxEquity;
            int minEquity = Configs.AccountInMoney ? (int) backtester.MinMoneyEquity : backtester.MinEquity;

            if (Configs.AdditionalStatistics)
            {
                int maxLongBalance = Configs.AccountInMoney ? (int) backtester.MaxLongMoneyBalance : backtester.MaxLongBalance;
                int minLongBalance = Configs.AccountInMoney ? (int) backtester.MinLongMoneyBalance : backtester.MinLongBalance;
                int maxShortBalance = Configs.AccountInMoney ? (int) backtester.MaxShortMoneyBalance : backtester.MaxShortBalance;
                int minShortBalance = Configs.AccountInMoney ? (int) backtester.MinShortMoneyBalance : backtester.MinShortBalance;
                int maxLongShortBalance = Math.Max(maxLongBalance, maxShortBalance);
                int minLongShortBalance = Math.Min(minLongBalance, minShortBalance);

                _data.Maximum = Math.Max(Math.Max(maxBalance, maxEquity), maxLongShortBalance) + 1;
                _data.Minimum = Math.Min(Math.Min(minBalance, minEquity), minLongShortBalance) - 1;
            }
            else
            {
                _data.Maximum = Math.Max(maxBalance, maxEquity) + 1;
                _data.Minimum = Math.Min(minBalance, minEquity) - 1;
            }

            _data.Minimum = (int) (Math.Floor(_data.Minimum/10f)*10);

            _data.DataMaxPrice = Data.DataStats.MaxPrice;
            _data.DataMinPrice = Data.DataStats.MinPrice;

            if (_showPriceLine)
            {
                _data.ClosePrice = new double[_data.Bars];
                Data.DataSet.Close.CopyTo(_data.ClosePrice, 0);
            }

            if (Configs.AccountInMoney)
            {
                _data.MoneyBalance = new double[_data.Bars];
                _data.MoneyEquity = new double[_data.Bars];
            }
            else
            {
                _data.Balance = new int[_data.Bars];
                _data.Equity = new int[_data.Bars];
            }

            if (Configs.AdditionalStatistics)
            {
                if (Configs.AccountInMoney)
                {
                    _data.LongMoneyBalance = new double[_data.Bars];
                    _data.ShortMoneyBalance = new double[_data.Bars];
                }
                else
                {
                    _data.LongBalance = new int[_data.Bars];
                    _data.ShortBalance = new int[_data.Bars];
                }
            }


            for (int bar = _data.FirstBar; bar < _data.Bars; bar++)
            {
                if (Configs.AccountInMoney)
                {
                    _data.MoneyBalance[bar] = backtester.MoneyBalance(bar);
                    _data.MoneyEquity[bar] = backtester.MoneyEquity(bar);
                }
                else
                {
                    _data.Balance[bar] = backtester.Balance(bar);
                    _data.Equity[bar] = backtester.Equity(bar);
                }

                if (Configs.AdditionalStatistics)
                {
                    if (Configs.AccountInMoney)
                    {
                        _data.LongMoneyBalance[bar] = backtester.LongMoneyBalance(bar);
                        _data.ShortMoneyBalance[bar] = backtester.ShortMoneyBalance(bar);
                    }
                    else
                    {
                        _data.LongBalance[bar] = backtester.LongBalance(bar);
                        _data.ShortBalance[bar] = backtester.ShortBalance(bar);
                    }
                }
            }

            _data.MarginCallBar = backtester.MarginCallBar;

            if (IsOOS && OOSBar > _data.FirstBar)
            {
                _data.NetBalance = (float) (Configs.AccountInMoney ? backtester.MoneyBalance(OOSBar) : backtester.Balance(OOSBar));
                _data.DataTimeBarOOS = Data.DataSet.Time[OOSBar];
            }
            else
                _data.NetBalance = (float)(Configs.AccountInMoney ? backtester.NetMoneyBalance : backtester.NetBalance);
        }

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        public void InitChart()
        {
            // Chart Title
            _chartTitle = Language.T("Balance / Equity Chart") + " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            _font = new Font(Font.FontFamily, 9);
            _captionHeight = Math.Max(_font.Height, 18);
            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            _stringFormatCaption = new StringFormat
                                      {
                                          Alignment = StringAlignment.Center,
                                          LineAlignment = StringAlignment.Center,
                                          Trimming = StringTrimming.EllipsisCharacter,
                                          FormatFlags = StringFormatFlags.NoWrap
                                      };
            _brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            _penGrid = new Pen(LayoutColors.ColorChartGrid) {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            _penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);

            if (_isNotPaint) return;

            _yTop = (int) _captionHeight + 2*Space + 1;
            _yBottom = ClientSize.Height - 2*Space - 1 - Border;

            Graphics g = CreateGraphics();
            float widthMinimum = g.MeasureString(_data.Minimum.ToString(CultureInfo.InvariantCulture), Font).Width;
            float widthMaximum = g.MeasureString(_data.Maximum.ToString(CultureInfo.InvariantCulture), Font).Width;
            _labelWidth = (int) Math.Max(widthMinimum, widthMaximum);
            _labelWidth = Math.Max(_labelWidth, 30);
            g.Dispose();

            _xLeft = Border + Space;
            _xRight = ClientSize.Width - Border - Space - _labelWidth;
            _xScale = (_xRight - 2*Space - Border)/(float) _chartBars;

            _countLabels = (int) Math.Max((_yBottom - _yTop)/20.0, 1);
            _delta = (float) Math.Max(Math.Round((_data.Maximum - _data.Minimum)/(float)_countLabels), 10);
            _labelStep = (int) Math.Ceiling(_delta/10)*10;
            _countLabels = (int) Math.Ceiling((_data.Maximum - _data.Minimum)/(float)_labelStep);
            _yScale = (_yBottom - _yTop)/(_countLabels*(float) _labelStep);

            _balancePoints = new PointF[_chartBars];
            _equityPoints = new PointF[_chartBars];

            if (Configs.AdditionalStatistics)
            {
                _longBalancePoints = new PointF[_chartBars];
                _shortBalancePoints = new PointF[_chartBars];
            }

            _closePricePoints = new PointF[_chartBars];

            // Close Price
            if (_showPriceLine)
                _yPriceScale = (float) ((_yBottom - _yTop)/(_data.DataMaxPrice - _data.DataMinPrice));

            for (int bar = _data.FirstBar; bar < _data.Bars; bar++)
            {
                int index = bar - _data.FirstBar;
                _balancePoints[index].X = _xLeft + index*_xScale;
                _equityPoints[index].X = _xLeft + index*_xScale;
                if (Configs.AccountInMoney)
                {
                    _balancePoints[index].Y = (float) (_yBottom - (_data.MoneyBalance[bar] - _data.Minimum)*_yScale);
                    _equityPoints[index].Y = (float) (_yBottom - (_data.MoneyEquity[bar] - _data.Minimum)*_yScale);
                }
                else
                {
                    _balancePoints[index].Y = _yBottom - (_data.Balance[bar] - _data.Minimum)*_yScale;
                    _equityPoints[index].Y = _yBottom - (_data.Equity[bar] - _data.Minimum)*_yScale;
                }

                if (Configs.AdditionalStatistics)
                {
                    _longBalancePoints[index].X = _xLeft + index*_xScale;
                    _shortBalancePoints[index].X = _xLeft + index*_xScale;
                    if (Configs.AccountInMoney)
                    {
                        _longBalancePoints[index].Y = (float) (_yBottom - (_data.LongMoneyBalance[bar] - _data.Minimum)*_yScale);
                        _shortBalancePoints[index].Y = (float) (_yBottom - (_data.ShortMoneyBalance[bar] - _data.Minimum)*_yScale);
                    }
                    else
                    {
                        _longBalancePoints[index].Y = _yBottom - (_data.LongBalance[bar] - _data.Minimum)*_yScale;
                        _shortBalancePoints[index].Y = _yBottom - (_data.ShortBalance[bar] - _data.Minimum)*_yScale;
                    }
                }

                if (_showPriceLine)
                {
                    _closePricePoints[index].X = _xLeft + index*_xScale;
                    _closePricePoints[index].Y = _yBottom - (float)(_data.ClosePrice[bar] - _data.DataMinPrice) * _yPriceScale;
                }
            }

            // Margin Call
            _xMarginCallBar = _data.MarginCallBar >= _data.FirstBar ? _xLeft + (_data.MarginCallBar - _data.FirstBar) * _xScale : 0;

            //OOS
            if (IsOOS && OOSBar > _data.FirstBar)
                _xOOSBar = _xLeft + (OOSBar - _data.FirstBar)*_xScale;
            else
                _xOOSBar = 0;

            _yBalance = _yBottom - (_data.NetBalance - _data.Minimum) * _yScale;

            _isHideScanningLine = false;
            _data.ModellingQuolity = " MQ " + ModelingQuality.ToString("N2") + "%";

            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption bar
            ColorMagic.GradientPaint(g, _rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), _rectfCaption, _stringFormatCaption);

            // Border
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1, ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient
            var rectChartField = new RectangleF(Border, _captionHeight, ClientSize.Width - 2*Border, ClientSize.Height - _captionHeight - Border);
            ColorMagic.GradientPaint(g, rectChartField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (_isNotPaint) return;

            // Grid and Price labels
            for (int labelPrice = _data.Minimum; labelPrice <= _data.Minimum + _countLabels*_labelStep; labelPrice += _labelStep)
            {
                var labelY = (int) (_yBottom - (labelPrice - _data.Minimum)*_yScale);
                g.DrawString(labelPrice.ToString(CultureInfo.InvariantCulture), Font, _brushFore, _xRight,
                             labelY - Font.Height/2 - 1);
                g.DrawLine(_penGrid, _xLeft, labelY, _xRight, labelY);
            }

            // Price close
            if (_showPriceLine)
                g.DrawLines(new Pen(LayoutColors.ColorChartGrid), _closePricePoints);

            // Equity line
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), _equityPoints);

            // Draw Long and Short balance
            if (Configs.AdditionalStatistics)
            {
                g.DrawLines(new Pen(Color.Red), _shortBalancePoints);
                g.DrawLines(new Pen(Color.Green), _longBalancePoints);
            }

            // Out of Sample
            if (IsOOS && OOSBar > 0)
            {
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xOOSBar, _yTop, _xOOSBar, _yBottom);
                Brush brushOOS = new Pen(LayoutColors.ColorChartFore).Brush;
                g.DrawString("OOS", Font, brushOOS, _xOOSBar, _yBottom - Font.Height);
                float widthOOSBarDate = g.MeasureString(_data.DataTimeBarOOS.ToShortDateString(), Font).Width;
                g.DrawString(_data.DataTimeBarOOS.ToShortDateString(), Font, brushOOS, _xOOSBar - widthOOSBarDate, _yBottom - Font.Height);
            }

            // Draw Balance Line
            if (_data.MarginCallBar > 0) // In case of Margin Call
            {
                // Draw balance line up to Margin Call
                var balancePoints = new PointF[_data.MarginCallBar - _data.FirstBar];
                for (int i = 0; i < balancePoints.Length; i++)
                    balancePoints[i] = _balancePoints[i];
                if (balancePoints.Length > 1)
                    g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), balancePoints);

                // Draw balance line after Margin Call
                var redBalancePoints = new PointF[_data.Bars - _data.MarginCallBar];
                for (int i = 0; i < redBalancePoints.Length; i++)
                    redBalancePoints[i] = _balancePoints[i + _data.MarginCallBar - _data.FirstBar];
                g.DrawLines(new Pen(LayoutColors.ColorSignalRed), redBalancePoints);

                // Margin Call line
                g.DrawLine(new Pen(LayoutColors.ColorChartCross), _xMarginCallBar, _yTop, _xMarginCallBar, _yBottom);

                // Margin Call label
                float widthMarginCallLabel = g.MeasureString(Language.T("Margin Call"), Font).Width;
                if (_xMarginCallBar < _xRight - widthMarginCallLabel)
                    g.DrawString(Language.T("Margin Call"), Font, _brushFore, _xMarginCallBar, _yTop);
                else if (_xMarginCallBar > Space + widthMarginCallLabel)
                    g.DrawString(Language.T("Margin Call"), Font, _brushFore, _xMarginCallBar - widthMarginCallLabel, _yTop);
                else
                    g.DrawString("MC", Font, _brushFore, _xMarginCallBar, _yTop);
            }
            else
            {
                // Draw the balance line
                g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), _balancePoints);
            }

            // Scanning note
            var fontNote = new Font(Font.FontFamily, Font.Size - 1);
            if (Data.DataSet.Period != DataPeriods.min1 && Configs.Autoscan && !Data.DataSet.IsIntrabarData)
                g.DrawString(Language.T("Load intrabar data"), fontNote, Brushes.Red, _xLeft, _captionHeight - 2);
            else if (Data.DataSet.Period != DataPeriods.min1 && _isScanPerformed)
                g.DrawString(Language.T("Scanned") + _data.ModellingQuolity, fontNote, Brushes.LimeGreen, _xLeft, _captionHeight - 2);

            // Scanned bars
            if (_isScanPerformed && !_isHideScanningLine &&
                (Data.DataSet.IntraBars != null && Data.DataSet.IsIntrabarData ||
                 Data.DataSet.Period == DataPeriods.min1 && Data.DataSet.IsTickData && Configs.UseTickData))
            {
                DataPeriods dataPeriod = Data.DataSet.Period;
                Color color = Data.PeriodColor[Data.DataSet.Period];
                int fromBar = _data.FirstBar;
                for (int bar = _data.FirstBar; bar < _data.Bars; bar++)
                {
                    if (Data.DataSet.IntraBarsPeriods[bar] == dataPeriod && bar != _data.Bars - 1) continue;
                    int xStart = (int) ((fromBar - _data.FirstBar)*_xScale) + _xLeft;
                    int xEnd = (int) ((bar - _data.FirstBar)*_xScale) + _xLeft;
                    fromBar = bar;
                    dataPeriod = Data.DataSet.IntraBarsPeriods[bar];
                    ColorMagic.GradientPaint(g, new RectangleF(xStart, _yBottom + 4, xEnd - xStart + 2, 5), color, 60);
                    color = Data.PeriodColor[Data.DataSet.IntraBarsPeriods[bar]];
                }

                // Tick Data
                if (Data.DataSet.IsTickData && Configs.UseTickData)
                {
                    int firstBarWithTicks = -1;
                    int lastBarWithTicks = -1;
                    for (int b = 0; b < _data.Bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.DataSet.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.DataSet.TickData[b] != null)
                            lastBarWithTicks = b;
                    }
                    int xStart = (int) ((firstBarWithTicks - _data.FirstBar)*_xScale) + _xLeft;
                    int xEnd = (int) ((lastBarWithTicks - _data.FirstBar)*_xScale) + _xLeft;
                    ColorMagic.GradientPaint(g, new RectangleF(xStart, _yBottom + 4, xEnd - xStart + 2, 5), color, 60);

                    var rectf = new RectangleF(xStart, _yBottom + 4, xEnd - xStart + 2, 5);
                    ColorMagic.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.min1], 60);
                    rectf = new RectangleF(xStart, _yBottom + 6, xEnd - xStart + 2, 1);
                    ColorMagic.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.day], 60);
                }

                // Vertical coordinate axes
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xLeft - 1, _yBottom, _xLeft - 1, _yBottom + 9);
            }

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xLeft - 1, _yTop - Space, _xLeft - 1, _yBottom + 1);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xLeft - 1, _yBottom + 1, _xRight, _yBottom + 1);

            // Balance level
            g.DrawLine(new Pen(LayoutColors.ColorChartCross), _xLeft, _yBalance, _xRight - Space + 1, _yBalance);

            // Balance label
            var labelSize = new Size(_labelWidth + Space, Font.Height + 2);
            var labelPoint = new Point(_xRight - Space + 2, (int) (_yBalance - Font.Height/2.0 - 1));
            var labelRect = new Rectangle(labelPoint, labelSize);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorLabelBack), labelRect);
            g.DrawRectangle(new Pen(LayoutColors.ColorChartCross), labelRect);
            g.DrawString((Math.Round(_data.NetBalance)).ToString(CultureInfo.InvariantCulture), Font,
                         new SolidBrush(LayoutColors.ColorLabelText), labelRect, _stringFormatCaption);
        }

        /// <summary>
        /// Generates dynamic info on the status bar
        /// when we are Moving the mouse over the SmallBalanceChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!ShowDynamicInfo || !Data.IsData || !Data.IsResult) return;

            int bar = (int) ((e.X - _xLeft)/_xScale) + _data.FirstBar;

            bar = Math.Max(_data.FirstBar, bar);
            bar = Math.Min(_data.Bars - 1, bar);

            if (Configs.AccountInMoney)
                CurrentBarInfo = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                                                 Data.DataSet.Time[bar].ToString(Data.DF),
                                                 Data.DataSet.Time[bar].ToString("HH:mm"),
                                                 Language.T("Balance"),
                                                 _data.MoneyBalance[bar].ToString("F2"),
                                                 Configs.AccountCurrency,
                                                 Language.T("Equity"),
                                                 _data.MoneyEquity[bar].ToString("F2"),
                                                 Configs.AccountCurrency);
            else
                CurrentBarInfo = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                                                 Data.DataSet.Time[bar].ToString(Data.DF),
                                                 Data.DataSet.Time[bar].ToString("HH:mm"),
                                                 Language.T("Balance"),
                                                 _data.Balance[bar],
                                                 Language.T("pips"),
                                                 Language.T("Equity"),
                                                 _data.Equity[bar],
                                                 Language.T("pips"));

            if (Configs.AdditionalStatistics)
            {
                if (Configs.AccountInMoney)
                    CurrentBarInfo += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                                                      Language.T("Long balance"),
                                                      _data.LongMoneyBalance[bar].ToString("F2"),
                                                      Configs.AccountCurrency,
                                                      Language.T("Short balance"),
                                                      _data.ShortMoneyBalance[bar].ToString("F2"),
                                                      Configs.AccountCurrency);
                else
                    CurrentBarInfo += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                                                      Language.T("Long balance"),
                                                      _data.LongBalance[bar],
                                                      Language.T("pips"),
                                                      Language.T("Short balance"),
                                                      _data.ShortBalance[bar],
                                                      Language.T("pips"));
            }
            if (Configs.ShowPriceChartOnAccountChart)
                CurrentBarInfo += String.Format(" {0}: {1}",
                                                  Language.T("Price close"),
                                                  Data.DataSet.Close[bar]);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            UpdateButtonsLocation();
            InitChart();
            Invalidate();
        }

        /// <summary>
        /// Calculates the Modeling Quality
        /// </summary>
        private double ModelingQuality
        {
            get
            {
                if (!_isScanPerformed)
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

                double modellingQuality = (0.25 * (startGen - _data.FirstBar) + 0.5 * (startGenM1 - startGen) +
                                           0.9 * (Data.DataSet.Bars - startGenM1)) / (Data.DataSet.Bars - _data.FirstBar) * 100;

                return modellingQuality;
            }
        }

    }
}