// Small Indicator Chart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Forex_Strategy_Builder.Common;
using Forex_Strategy_Builder.CustomControls;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Draws a small indicator chart
    /// </summary>
    public class SmallIndicatorChart : ContextPanel
    {
        private const int Space = 5;
        private const int Border = 2;
        private readonly Pen _penDarkGray = new Pen(Color.DarkGray);
        private readonly Pen _penGreen = new Pen(Color.Green);
        private readonly Pen _penRed = new Pen(Color.Red);
        private readonly HScrollBar _scrollBar;
        private Brush[] _brushPosition;
        private Brush _captionBrush;

        private Font _captionFont;
        private float _captionHeight;
        private RectangleF _captionRectangle;
        private StringFormat _captionStringFormat;
        private string _captionText;
        private float _captionWidth;

        private int _chartBarWidth;
        private int _chartBars;
        private Brush[][] _chartBrush;
        private Rectangle[][][] _chartDot;
        private int _chartFirstBar;
        private int _chartLastBar;
        private Rectangle[][][] _chartLevel;
        private Point[][][] _chartLine;
        private Pen[][][] _chartPen;
        private IndChartType[][] _chartType;
        private double[][][] _chartValue;
        private int _chartWidth;
        private int[] _componentLenght;
        private int[] _indicatorSlots;
        private bool[] _isSeparatedChart;
        private bool _isShowDynamicInfo;
        private bool _isValueChangedAktive;

        private double _maxPrice;
        private double[] _maxValues;
        private int _maxVolume;
        private double _minPrice;
        private double[] _minValues;
        private Pen _penBorder;
        private Pen _penFore;
        private Pen _penVolume;
        private Rectangle[] _rectPosition;
        private double[] _scales;
        private double _scaleY;
        private double _scaleYVol;
        private int _separateIndicatorsChartHeight;
        private int _separateIndicatorsCount;

        private int[] _x;
        private int _xLeft;
        private int _xRight;
        private int _yBottom;
        private int[] _yClose;
        private int[] _yHigh;
        private int[] _yIndBottom;
        private int[] _yIndTop;
        private int[] _yLow;
        private int[] _yOpen;
        private int _yPriceBottom;
        private int _yTop;
        private int[] _yVolume;

        /// <summary>
        /// Public constructor
        /// </summary>
        public SmallIndicatorChart()
        {
            Padding = new Padding(Border, 0, Border, Border);

            // Horizontal scroll bar
            _scrollBar = new HScrollBar
                             {
                                 Parent = this,
                                 Dock = DockStyle.Bottom,
                                 SmallChange = 1,
                                 LargeChange = 50,
                                 Minimum = 0,
                                 Maximum = 1000,
                                 Visible = true
                             };
            _scrollBar.ValueChanged += ScrollBarValueChanged;
        }

        /// <summary>
        /// Gets or sets whether to show dynamic info or not
        /// </summary>
        public bool ShowDynamicInfo
        {
            set { _isShowDynamicInfo = value; }
        }

        /// <summary>
        /// Returns dynamic info
        /// </summary>
        public string CurrentBarInfo { get; private set; }


        /// <summary>
        /// Sets the parameters of the Indicators Chart
        /// </summary>
        public void InitChart()
        {
            if (!Data.IsData || !Data.IsResult) return;

            _chartBarWidth = 2;
            _xLeft = Space;
            _xRight = ClientSize.Width - Space;
            _chartWidth = _xRight - _xLeft;

            _chartBars = _chartWidth/_chartBarWidth;
            _chartBars = Math.Min(_chartBars, Data.DataSet.Bars - StatsBuffer.FirstBar);

            _isValueChangedAktive = false;
            _scrollBar.Minimum = Math.Max(StatsBuffer.FirstBar, 0);
            _scrollBar.Maximum = Math.Max(Data.DataSet.Bars - 1, 1);
            _scrollBar.LargeChange = Math.Max(_chartBars, 1);

            _chartFirstBar = Math.Max(StatsBuffer.FirstBar, Data.DataSet.Bars - _chartBars);
            _chartFirstBar = Math.Min(_chartFirstBar, Data.DataSet.Bars - 1);
            _chartFirstBar = Math.Max(_chartFirstBar, 1);
            _chartLastBar = Math.Max(_chartFirstBar + _chartBars - 1, _chartFirstBar);

            _scrollBar.Value = _chartFirstBar;
            _isValueChangedAktive = true;

            SetUpPaintData();

            // Context button colors.
            ButtonsColorBack = LayoutColors.ColorCaptionBack;
            ButtonColorFore = LayoutColors.ColorCaptionText;
            ContextMenuColorBack = LayoutColors.ColorControlBack;
            ContextMenuColorFore = LayoutColors.ColorControlText;
        }


        /// <summary>
        /// Prepare the parameters
        /// </summary>
        private void SetUpPaintData()
        {
            // Panel caption
            _captionText = Language.T("Indicator Chart");
            _captionFont = new Font(Font.FontFamily, 9);
            _captionHeight = Math.Max(_captionFont.Height, 18);
            _captionWidth = ClientSize.Width;
            _captionBrush = new SolidBrush(LayoutColors.ColorCaptionText);
            _captionRectangle = new RectangleF(0, 0, _captionWidth, _captionHeight);
            _captionStringFormat = new StringFormat
                                       {
                                           Alignment = StringAlignment.Center,
                                           LineAlignment = StringAlignment.Center,
                                           Trimming = StringTrimming.EllipsisCharacter,
                                           FormatFlags = StringFormatFlags.NoWrap
                                       };

            if (!Data.IsData || !Data.IsResult || Data.DataSet.Bars <= StatsBuffer.FirstBar) return;

            _xLeft = Space;
            _xRight = ClientSize.Width - Space;
            _yTop = (int) _captionHeight + Space;
            _yBottom = ClientSize.Height - _scrollBar.Height - Space;
            _yPriceBottom = _yBottom;
            _separateIndicatorsCount = 0;
            _separateIndicatorsChartHeight = 0;
            _indicatorSlots = new int[Configs.MaxEntryFilters + Configs.MaxExitFilters + 2];

            _penFore = new Pen(LayoutColors.ColorChartFore);
            _penVolume = new Pen(LayoutColors.ColorVolume);
            _penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                 Border);

            for (int slot = StatsBuffer.Strategy.Slots - 1; slot >= 0; slot--)
                if (StatsBuffer.Strategy.Slot[slot].SeparatedChart)
                    _indicatorSlots[_separateIndicatorsCount++] = slot;

            if (_separateIndicatorsCount > 0)
            {
                _separateIndicatorsChartHeight = (_yBottom - _yTop)/(2 + _separateIndicatorsCount);
                _yPriceBottom = _yBottom - _separateIndicatorsCount*_separateIndicatorsChartHeight;
            }

            _maxPrice = double.MinValue;
            _minPrice = double.MaxValue;
            _maxVolume = int.MinValue;

            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
            {
                if (Data.DataSet.High[bar] > _maxPrice) _maxPrice = Data.DataSet.High[bar];
                if (Data.DataSet.Low[bar] < _minPrice) _minPrice = Data.DataSet.Low[bar];
                if (Data.DataSet.Volume[bar] > _maxVolume) _maxVolume = Data.DataSet.Volume[bar];
            }
            _minPrice = Math.Round(_minPrice, Data.DataSet.InstrProperties.Point < 0.001 ? 3 : 1) -
                        Data.DataSet.InstrProperties.Point*10;
            _maxPrice = Math.Round(_maxPrice, Data.DataSet.InstrProperties.Point < 0.001 ? 3 : 1) +
                        Data.DataSet.InstrProperties.Point*10;
            _scaleY = (_yPriceBottom - _yTop)/(_maxPrice - _minPrice);
            _scaleYVol = _maxVolume > 0 ? ((_yPriceBottom - _yTop)/8d)/_maxVolume : 0d;

            // Volume, Lots and Price
            _x = new int[_chartBars];
            _yOpen = new int[_chartBars];
            _yHigh = new int[_chartBars];
            _yLow = new int[_chartBars];
            _yClose = new int[_chartBars];
            _yVolume = new int[_chartBars];
            _rectPosition = new Rectangle[_chartBars];
            _brushPosition = new Brush[_chartBars];

            int index = 0;
            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
            {
                _x[index] = (bar - _chartFirstBar)*_chartBarWidth + _xLeft;
                _yOpen[index] = (int)(_yPriceBottom - (Data.DataSet.Open[bar] - _minPrice) * _scaleY);
                _yHigh[index] = (int)(_yPriceBottom - (Data.DataSet.High[bar] - _minPrice) * _scaleY);
                _yLow[index] = (int)(_yPriceBottom - (Data.DataSet.Low[bar] - _minPrice) * _scaleY);
                _yClose[index] = (int)(_yPriceBottom - (Data.DataSet.Close[bar] - _minPrice) * _scaleY);
                _yVolume[index] = (int)(_yPriceBottom - Data.DataSet.Volume[bar] * _scaleYVol);

                // Draw position lots
                if (StatsBuffer.IsPos(bar))
                {
                    var posHight = (int) (Math.Max(StatsBuffer.SummaryLots(bar)*2, 2));
                    int yPos = _yPriceBottom - posHight;

                    switch (StatsBuffer.SummaryDir(bar))
                    {
                        case PosDirection.Long:
                            _rectPosition[index] = new Rectangle(_x[index], yPos, 1, posHight);
                            _brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeLong);
                            break;
                        case PosDirection.Short:
                            _rectPosition[index] = new Rectangle(_x[index], yPos, 1, posHight);
                            _brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeShort);
                            break;
                        case PosDirection.Closed:
                            _rectPosition[index] = new Rectangle(_x[index], yPos - 2, 1, 2);
                            _brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeClose);
                            break;
                    }
                }
                else
                {
                    // There is no position
                    _rectPosition[index] = Rectangle.Empty;
                    _brushPosition[index] = new SolidBrush(LayoutColors.ColorChartBack);
                }
                index++;
            }

            // Indicators in the chart
            int slots = StatsBuffer.Strategy.Slots;
            _isSeparatedChart = new bool[slots];
            _componentLenght = new int[slots];
            _chartType = new IndChartType[slots][];
            _chartLine = new Point[slots][][];
            _chartDot = new Rectangle[slots][][];
            _chartLevel = new Rectangle[slots][][];
            _chartValue = new double[slots][][];
            _chartPen = new Pen[slots][][];
            _chartBrush = new Brush[slots][];

            for (int slot = 0; slot < slots; slot++)
            {
                _isSeparatedChart[slot] = StatsBuffer.Strategy.Slot[slot].SeparatedChart;
                int count = StatsBuffer.Strategy.Slot[slot].Component.Length;
                _componentLenght[slot] = count;
                _chartType[slot] = new IndChartType[count];
                _chartLine[slot] = new Point[count][];
                _chartDot[slot] = new Rectangle[count][];
                _chartLevel[slot] = new Rectangle[count][];
                _chartValue[slot] = new double[count][];
                _chartPen[slot] = new Pen[count][];
                _chartBrush[slot] = new Brush[count];
            }

            for (int slot = 0; slot < slots; slot++)
            {
                if (_isSeparatedChart[slot]) continue;

                for (int comp = 0; comp < _componentLenght[slot]; comp++)
                {
                    _chartType[slot][comp] = StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType;
                    switch (StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType)
                    {
                        case IndChartType.Line:
                        case IndChartType.CloudUp:
                        case IndChartType.CloudDown:
                            _chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            _chartLine[slot][comp] = new Point[_chartLastBar - _chartFirstBar + 1];
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - _chartFirstBar)*_chartBarWidth + _xLeft;
                                var y = (int) (_yPriceBottom - (value - _minPrice)*_scaleY);

                                if (Math.Abs(value - 0) < 0.0001)
                                    _chartLine[slot][comp][bar - _chartFirstBar] =
                                        _chartLine[slot][comp][Math.Max(bar - _chartFirstBar - 1, 0)];
                                else
                                    _chartLine[slot][comp][bar - _chartFirstBar] = new Point(x, y);
                            }
                            break;
                        case IndChartType.Dot:
                            _chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            _chartDot[slot][comp] = new Rectangle[_chartLastBar - _chartFirstBar + 1];
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - _chartFirstBar)*_chartBarWidth + _xLeft;
                                var y = (int) (_yPriceBottom - (value - _minPrice)*_scaleY);
                                _chartDot[slot][comp][bar - _chartFirstBar] = new Rectangle(x, y, 1, 1);
                            }
                            break;
                        case IndChartType.Level:
                            _chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            _chartLevel[slot][comp] = new Rectangle[_chartLastBar - _chartFirstBar + 1];
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - _chartFirstBar)*_chartBarWidth + _xLeft;
                                var y = (int) (_yPriceBottom - (value - _minPrice)*_scaleY);
                                _chartLevel[slot][comp][bar - _chartFirstBar] = new Rectangle(x, y, _chartBarWidth, 1);
                            }
                            break;
                    }
                }
            }

            // Separate indicators
            _yIndTop = new int[_separateIndicatorsCount];
            _yIndBottom = new int[_separateIndicatorsCount];
            _maxValues = new double[_separateIndicatorsCount];
            _minValues = new double[_separateIndicatorsCount];
            _scales = new double[_separateIndicatorsCount];

            for (int ind = 0; ind < _separateIndicatorsCount; ind++)
            {
                _yIndTop[ind] = _yBottom - (ind + 1)*_separateIndicatorsChartHeight + 1;
                _yIndBottom[ind] = _yBottom - ind*_separateIndicatorsChartHeight - 1;
                _maxValues[ind] = double.MinValue;
                _minValues[ind] = double.MaxValue;
                int slot = _indicatorSlots[ind];

                for (int comp = 0; comp < _componentLenght[slot]; comp++)
                    if (StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType != IndChartType.NoChart)
                        for (
                            int bar = Math.Max(_chartFirstBar, StatsBuffer.Strategy.Slot[slot].Component[comp].FirstBar);
                            bar <= _chartLastBar;
                            bar++)
                        {
                            double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                            if (value > _maxValues[ind]) _maxValues[ind] = value;
                            if (value < _minValues[ind]) _minValues[ind] = value;
                        }

                _maxValues[ind] = Math.Max(_maxValues[ind], StatsBuffer.Strategy.Slot[slot].MaxValue);
                _minValues[ind] = Math.Min(_minValues[ind], StatsBuffer.Strategy.Slot[slot].MinValue);

                foreach (double specialValue in StatsBuffer.Strategy.Slot[slot].SpecValue)
                    if (Math.Abs(specialValue - 0) < 0.0001)
                    {
                        _maxValues[ind] = Math.Max(_maxValues[ind], 0);
                        _minValues[ind] = Math.Min(_minValues[ind], 0);
                    }

                _scales[ind] = (_yIndBottom[ind] - _yIndTop[ind] - 2)/ (Math.Max(_maxValues[ind] - _minValues[ind], 0.0001f));

                // Indicator chart
                for (int comp = 0; comp < StatsBuffer.Strategy.Slot[slot].Component.Length; comp++)
                {
                    _chartType[slot][comp] = StatsBuffer.Strategy.Slot[slot].Component[comp].ChartType;
                    switch (_chartType[slot][comp])
                    {
                        case IndChartType.Line:
                            _chartBrush[slot][comp] =
                                new SolidBrush(StatsBuffer.Strategy.Slot[slot].Component[comp].ChartColor);
                            _chartLine[slot][comp] = new Point[_chartLastBar - _chartFirstBar + 1];
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - _chartFirstBar)*_chartBarWidth + _xLeft;
                                var y = (int) (_yIndBottom[ind] - 1 - (value - _minValues[ind])*_scales[ind]);
                                _chartLine[slot][comp][bar - _chartFirstBar] = new Point(x, y);
                            }
                            break;
                        case IndChartType.Histogram:
                            _chartValue[slot][comp] = new double[_chartLastBar - _chartFirstBar + 1];
                            _chartPen[slot][comp] = new Pen[_chartLastBar - _chartFirstBar + 1];
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                            {
                                double value = StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar];
                                _chartValue[slot][comp][bar - _chartFirstBar] = value;
                                if (value > StatsBuffer.Strategy.Slot[slot].Component[comp].Value[bar - 1])
                                    _chartPen[slot][comp][bar - _chartFirstBar] = _penGreen;
                                else
                                    _chartPen[slot][comp][bar - _chartFirstBar] = _penRed;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            var chartArea = new Rectangle(Border, (int) _captionHeight, ClientSize.Width - 2*Border, ClientSize.Height - (int) _captionHeight - Border);
            ColorMagic.GradientPaint(g, chartArea, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Panel caption
            ColorMagic.GradientPaint(g, _captionRectangle, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_captionText, _captionFont, _captionBrush, _captionRectangle, _captionStringFormat);

            // Border
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1, ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            if (!Data.IsData || !Data.IsResult || Data.DataSet.Bars <= StatsBuffer.FirstBar) return;

            // Limits the drawing into the chart area only
            g.SetClip(new Rectangle(_xLeft, _yTop, _xRight - _xLeft, _yPriceBottom - _yTop));

            // Draws Volume, Lots and Price
            int index = 0;
            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
            {
                // Draw the volume
                if (_yVolume[index] != _yPriceBottom)
                    g.DrawLine(_penVolume, _x[index], _yVolume[index], _x[index], _yPriceBottom - 1);

                // Draw position lots
                if (_rectPosition[index] != Rectangle.Empty)
                    g.FillRectangle(_brushPosition[index], _rectPosition[index]);

                // Draw the bar
                var penBar = new Pen(LayoutColors.ColorBarBorder);
                g.DrawLine(penBar, _x[index], _yLow[index], _x[index], _yHigh[index]);
                g.DrawLine(penBar, _x[index], _yClose[index], _x[index] + 1, _yClose[index]);
                index++;
            }

            // Drawing the indicators in the chart
            int slots = StatsBuffer.Strategy.Slots;
            for (int slot = 0; slot < slots; slot++)
            {
                if (_isSeparatedChart[slot]) continue;
                for (int comp = 0; comp < _componentLenght[slot]; comp++)
                {
                    switch (_chartType[slot][comp])
                    {
                        case IndChartType.Line:
                            g.DrawLines(new Pen(_chartBrush[slot][comp]), _chartLine[slot][comp]);
                            break;
                        case IndChartType.Dot:
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                                g.FillRectangle(_chartBrush[slot][comp], _chartDot[slot][comp][bar - _chartFirstBar]);
                            break;
                        case IndChartType.Level:
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                                g.FillRectangle(_chartBrush[slot][comp], _chartLevel[slot][comp][bar - _chartFirstBar]);
                            break;
                        case IndChartType.CloudUp:
                            g.DrawLines(new Pen(_chartBrush[slot][comp]) { DashStyle = DashStyle.Dash }, _chartLine[slot][comp]);
                            break;
                        case IndChartType.CloudDown:
                            g.DrawLines(new Pen(_chartBrush[slot][comp]) { DashStyle = DashStyle.Dash }, _chartLine[slot][comp]);
                            break;
                    }
                }
            }
            g.ResetClip();

            // Separate indicators
            for (int ind = 0; ind < _separateIndicatorsCount; ind++)
            {
                int slot = _indicatorSlots[ind];

                for (int comp = 0; comp < _componentLenght[slot]; comp++)
                {
                    switch (_chartType[slot][comp])
                    {
                        case IndChartType.Line:
                            g.DrawLines(new Pen(_chartBrush[slot][comp]), _chartLine[slot][comp]);
                            break;
                        case IndChartType.Histogram:
                            double zero = 0;
                            if (zero < _minValues[ind]) zero = _minValues[ind];
                            if (zero > _maxValues[ind]) zero = _maxValues[ind];
                            var y0 = (int) (_yIndBottom[ind] - (zero - _minValues[ind])*_scales[ind]);
                            g.DrawLine(_penDarkGray, _xLeft, y0, _xRight, y0);
                            for (int bar = _chartFirstBar; bar <= _chartLastBar; bar++)
                            {
                                double val = _chartValue[slot][comp][bar - _chartFirstBar];
                                int x = (bar - _chartFirstBar)*_chartBarWidth + _xLeft;
                                var y = (int) (_yIndBottom[ind] - (val - _minValues[ind])*_scales[ind]);
                                g.DrawLine(_chartPen[slot][comp][bar - _chartFirstBar], x, y0, x, y);
                            }
                            break;
                    }
                }
            }

            // Lines
            for (int ind = 0; ind < _separateIndicatorsCount; ind++)
            {
                int y = _yBottom - (ind + 1)*_separateIndicatorsChartHeight;
                g.DrawLine(_penFore, _xLeft, y, _xRight, y);
            }
            g.DrawLine(_penFore, _xLeft, _yBottom, _xRight, _yBottom);
            g.DrawLine(_penFore, _xLeft, _yTop, _xLeft, _yBottom);
        }

        /// <summary>
        /// Generates dynamic info on the status bar
        /// when we are Moving the mouse over the Indicator Chart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_isShowDynamicInfo || !Data.IsData || !Data.IsResult || Data.DataSet.Bars < StatsBuffer.FirstBar) return;

            int currentBar = (e.X - Space)/_chartBarWidth;
            currentBar = Math.Max(0, currentBar);
            currentBar = Math.Min(_chartBars - 1, currentBar);

            int bar = Math.Min(Data.DataSet.Bars - 1, _chartFirstBar + currentBar);

            CurrentBarInfo = string.Format("{0} {1} O:{2} H:{3} L:{4} C:{5} V:{6}",
                                           Data.DataSet.Time[bar].ToString(Data.DF),
                                           Data.DataSet.Time[bar].ToString("HH:mm"),
                                           Data.DataSet.Open[bar].ToString(Data.FF),
                                           Data.DataSet.High[bar].ToString(Data.FF),
                                           Data.DataSet.Low[bar].ToString(Data.FF),
                                           Data.DataSet.Close[bar].ToString(Data.FF),
                                           Data.DataSet.Volume[bar]);
        }

        /// <summary>
        /// Sets the parameters after the horizontal scrollbar position has been changed.
        /// </summary>
        private void ScrollBarValueChanged(object sender, EventArgs e)
        {
            if (!_isValueChangedAktive) return;

            _chartFirstBar = _scrollBar.Value;
            _chartLastBar = Math.Max(_chartFirstBar + _chartBars - 1, _chartFirstBar);

            SetUpPaintData();
            var chartArea = new Rectangle(_xLeft + 1, _yTop, _xRight - _xLeft, _yBottom - _yTop);
            Invalidate(chartArea);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            InitChart();
            base.OnResize(eventargs);
            Invalidate();
        }
    }
}
