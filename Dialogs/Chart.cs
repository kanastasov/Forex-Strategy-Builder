// Chart class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Indicator Chart : Form
    /// </summary>
    public sealed class Chart : Form
    {
        private readonly Brush _brushBack;
        private readonly Brush _brushDiIndicator;
        private readonly Brush _brushDynamicInfo;
        private readonly Brush _brushEvenRows;
        private readonly Brush _brushFore;
        private readonly Brush _brushLabelBkgrd;
        private readonly Brush _brushLabelFore;
        private readonly Brush _brushSignalRed;
        private readonly Brush _brushTradeClose;
        private readonly Brush _brushTradeLong;
        private readonly Brush _brushTradeShort;
        private readonly Color _colorBarBlack1;
        private readonly Color _colorBarBlack2;
        private readonly Color _colorBarWhite1;
        private readonly Color _colorBarWhite2;
        private readonly Color _colorClosedTrade1;
        private readonly Color _colorClosedTrade2;
        private readonly Color _colorLongTrade1;
        private readonly Color _colorLongTrade2;
        private readonly Color _colorShortTrade1;
        private readonly Color _colorShortTrade2;
        private readonly Font _font;
        private readonly Font _fontDI; // Font for Dynamic info
        private readonly Font _fontDIInd; // Font for Dynamic info Indicators
        private readonly Pen _penBarBorder;
        private readonly Pen _penBarThick;
        private readonly Pen _penCross;
        private readonly Pen _penGrid;
        private readonly Pen _penGridSolid;
        private readonly Pen _penTradeClose;
        private readonly Pen _penTradeLong;
        private readonly Pen _penTradeShort;
        private readonly Pen _penVolume;
        private readonly int _spcBottom; // pnlPrice bottom margin
        private readonly int _spcLeft; // pnlPrice left margin
        private readonly int _spcRight; // pnlPrice right margin
        private readonly int _spcTop; // pnlPrice top margin
        private int[] _aiInfoType; // 0 - text; 1 - Indicator;
        private string[] _asInfoTitle;
        private string[] _asInfoValue;
        private int _barOld;

        private int _chartBars;
        private string _chartTitle;
        private int _chartWidth;
        private int _dynInfoScrollValue; // Dynamic info vertical scrolling position
        private int _dynInfoWidth; // Dynamic info width
        private int _firstBar;
        private int _indPanels;
        private int _infoRows; // Dynamic info rows
        private bool _isCandleChart = true;
        private bool _isDebug;
        private bool _isDrawDinInfo; // Draw or not
        private bool _isKeyCtrlPressed;
        private bool _isMouseInIndicatorChart;
        private bool _isMouseInPriceChart;
        private int _lastBar;
        private double _maxPrice;
        private int _maxVolume; // Max Volume in the chart
        private double _minPrice;
        private int _mouseX;
        private int _mouseXOld;
        private int _mouseY;
        private int _mouseYOld;
        private int _posCount;
        private bool[] _repeatedIndicators;
        private HScrollBar _scroll;
        private Size _szDate;
        private Size _szDateL;
        private Size _szPrice;

        private int _verticalScale = 1;
        private int _xDynInfoCol2; // Dynamic info second column X
        private int _xLeft; // pnlPrice left coordinate
        private int _xRight; // pnlPrice right coordinate
        private int _yBottom; // pnlPrice bottom coordinate
        private int _yBottomText; // pnlPrice bottom coordinate for date text
        private double _yScale;
        private int _yTop; // pnlPrice top coordinate
        private double _yVolScale; // The scale for drawing the Volume

        /// <summary>
        /// The default constructor.
        /// </summary>
        public Chart(Backtester backtester)
        {
            _backtester = backtester;

            BarPixels = 8;
            Text = Language.T("Chart") + " " + _backtester.DataSet.Symbol + " " + _backtester.DataSet.PeriodString + " - " + Data.ProgramName;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;

            PnlCharts = new Panel {Parent = this, Dock = DockStyle.Fill};

            PnlInfo = new Panel {Parent = this, BackColor = LayoutColors.ColorControlBack, Dock = DockStyle.Right};
            PnlInfo.Resize += PnlInfoResize;
            PnlInfo.Paint += PnlInfoPaint;

            ShowInfoPanel = true;
            _dynInfoScrollValue = 0;

            _font = new Font(Font.FontFamily, Font.Size);

            // Dynamic info fonts
            _fontDI = _font;
            _fontDIInd = new Font(Font.FontFamily, 10);

            Graphics g = CreateGraphics();

            _szDate = g.MeasureString("99/99 99:99", _font).ToSize();
            _szDateL = g.MeasureString("99/99/99 99:99", _font).ToSize();
            _szPrice = g.MeasureString("9.99999", _font).ToSize();

            g.Dispose();

            _spcTop = _font.Height;
            _spcBottom = _font.Height*8/5;
            _spcLeft = 2;
            _spcRight = _szPrice.Width + 4;

            _brushBack = new SolidBrush(LayoutColors.ColorChartBack);
            _brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            _brushLabelBkgrd = new SolidBrush(LayoutColors.ColorLabelBack);
            _brushLabelFore = new SolidBrush(LayoutColors.ColorLabelText);
            _brushDynamicInfo = new SolidBrush(LayoutColors.ColorControlText);
            _brushDiIndicator = new SolidBrush(LayoutColors.ColorSlotIndicatorText);
            _brushEvenRows = new SolidBrush(LayoutColors.ColorEvenRowBack);
            _brushTradeLong = new SolidBrush(LayoutColors.ColorTradeLong);
            _brushTradeShort = new SolidBrush(LayoutColors.ColorTradeShort);
            _brushTradeClose = new SolidBrush(LayoutColors.ColorTradeClose);
            _brushSignalRed = new SolidBrush(LayoutColors.ColorSignalRed);

            _penGrid = new Pen(LayoutColors.ColorChartGrid);
            _penGridSolid = new Pen(LayoutColors.ColorChartGrid);
            _penCross = new Pen(LayoutColors.ColorChartCross);
            _penVolume = new Pen(LayoutColors.ColorVolume);
            _penBarBorder = new Pen(LayoutColors.ColorBarBorder);
            _penBarThick = new Pen(LayoutColors.ColorBarBorder, 3);
            _penTradeLong = new Pen(LayoutColors.ColorTradeLong);
            _penTradeShort = new Pen(LayoutColors.ColorTradeShort);
            _penTradeClose = new Pen(LayoutColors.ColorTradeClose);

            _penGrid.DashStyle = DashStyle.Dash;
            _penGrid.DashPattern = new float[] {4, 2};

            _colorBarWhite1 = ColorMagic.GetGradientColor(LayoutColors.ColorBarWhite, 30);
            _colorBarWhite2 = ColorMagic.GetGradientColor(LayoutColors.ColorBarWhite, -30);
            _colorBarBlack1 = ColorMagic.GetGradientColor(LayoutColors.ColorBarBlack, 30);
            _colorBarBlack2 = ColorMagic.GetGradientColor(LayoutColors.ColorBarBlack, -30);

            _colorLongTrade1 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeLong, 30);
            _colorLongTrade2 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeLong, -30);
            _colorShortTrade1 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeShort, 30);
            _colorShortTrade2 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeShort, -30);
            _colorClosedTrade1 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeClose, 30);
            _colorClosedTrade2 = ColorMagic.GetGradientColor(LayoutColors.ColorTradeClose, -30);
        }

        private readonly Backtester _backtester;

        private Panel PnlCharts { get; set; }
        private Panel PnlInfo { get; set; }
        private ToolStripButton[] AChartButtons { get; set; }
        private ToolStrip TsChartButtons { get; set; }
        private Panel PnlBalanceChart { get; set; }
        private Panel PnlFloatingPLChart { get; set; }
        private Panel[] PnlInd { get; set; }
        private Panel PnlPrice { get; set; }
        private Splitter SpliterBalance { get; set; }
        private Splitter SpliterFloatingPL { get; set; }
        private Splitter[] SplitterInd { get; set; }

        public int BarPixels { get; set; }
        public bool ShowInfoPanel { get; set; }
        public bool ShowGrid { get; set; }
        public bool ShowCross { get; set; }
        public bool ShowVolume { get; set; }
        public bool ShowPositionLots { get; set; }
        public bool ShowOrders { get; set; }
        public bool ShowDynInfo { get; set; }
        public bool ShowPositionPrice { get; set; }
        public bool ShowBalanceEquity { get; set; }
        public bool ShowFloatingPL { get; set; }
        public bool ShowIndicators { get; set; }
        public bool ShowProtections { get; set; }
        public bool ShowAmbiguousBars { get; set; }
        public bool TrueCharts { get; set; }

        /// <summary>
        /// After loading select the Scrollbar
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PnlInfo.Visible = ShowInfoPanel;
            PnlCharts.Padding = ShowInfoPanel ? new Padding(0, 0, 2, 0) : new Padding(0);

            if (ShowInfoPanel)
                Text = Language.T("Chart") + " " + _backtester.DataSet.Symbol + " " + _backtester.DataSet.PeriodString + " - " + Data.ProgramName;
            else
                Text = Data.ProgramName + @"   http://forexsb.com";

            SetupDynInfoWidth();
            SetupIndicatorPanels();
            SetupButtons();
            SetupDynamicInfo();
            SetupChartTitle();

            PnlCharts.Resize += PnlBaseResize;
            PnlPrice.Resize += PnlPriceResize;

            Size = new Size(720, 500);
            MinimumSize = new Size(500, 500);

            if (ShowInfoPanel)
            {
                GenerateDynamicInfo(_lastBar);
                SetupDynamicInfo();
                _isDrawDinInfo = true;
                PnlInfo.Invalidate();
            }

            _scroll.Select();
        }

        /// <summary>
        /// Sets ups the chart's buttons.
        /// </summary>
        private void SetupButtons()
        {
            TsChartButtons = new ToolStrip {Parent = PnlCharts};

            AChartButtons = new ToolStripButton[17];
            for (int i = 0; i < 17; i++)
            {
                AChartButtons[i] = new ToolStripButton
                                       {Tag = (ChartButtons) i, DisplayStyle = ToolStripItemDisplayStyle.Image};
                AChartButtons[i].Click += ButtonChartClick;
                TsChartButtons.Items.Add(AChartButtons[i]);
                if (i > 13)
                    AChartButtons[i].Alignment = ToolStripItemAlignment.Right;
                if (i == 1 || i == 6 || i == 10 || i == 12)
                    TsChartButtons.Items.Add(new ToolStripSeparator());
            }

            // Grid
            AChartButtons[(int) ChartButtons.Grid].Image = Resources.chart_grid;
            AChartButtons[(int) ChartButtons.Grid].ToolTipText = Language.T("Grid") + "   G";
            AChartButtons[(int) ChartButtons.Grid].Checked = ShowGrid;

            // Cross
            AChartButtons[(int) ChartButtons.Cross].Image = Resources.chart_cross;
            AChartButtons[(int) ChartButtons.Cross].ToolTipText = Language.T("Cross") + "   C";
            AChartButtons[(int) ChartButtons.Cross].Checked = ShowCross;

            // Volume
            AChartButtons[(int) ChartButtons.Volume].Image = Resources.chart_volume;
            AChartButtons[(int) ChartButtons.Volume].ToolTipText = Language.T("Volume") + "   V";
            AChartButtons[(int) ChartButtons.Volume].Checked = ShowVolume;

            // Orders
            AChartButtons[(int) ChartButtons.Orders].Image = Resources.chart_entry_points;
            AChartButtons[(int) ChartButtons.Orders].ToolTipText = Language.T("Entry / Exit points") + "   O";
            AChartButtons[(int) ChartButtons.Orders].Checked = ShowOrders;

            // Position lots
            AChartButtons[(int) ChartButtons.PositionLots].Image = Resources.chart_lots;
            AChartButtons[(int) ChartButtons.PositionLots].ToolTipText = Language.T("Position lots") + "   L";
            AChartButtons[(int) ChartButtons.PositionLots].Checked = ShowPositionLots;

            // Position price
            AChartButtons[(int) ChartButtons.PositionPrice].Image = Resources.chart_pos_price;
            AChartButtons[(int) ChartButtons.PositionPrice].ToolTipText = Language.T("Corrected position price") +
                                                                          "   P";
            AChartButtons[(int) ChartButtons.PositionPrice].Checked = ShowPositionPrice;

            // Position price
            AChartButtons[(int) ChartButtons.Protections].Image = Resources.chart_protection;
            AChartButtons[(int) ChartButtons.Protections].ToolTipText =
                Language.T("Break Even, Permanent SL, Permanent TP") + "   E";
            AChartButtons[(int) ChartButtons.Protections].Checked = ShowProtections;

            // Ambiguous Bars
            AChartButtons[(int) ChartButtons.AmbiguousBars].Image = Resources.chart_ambiguous_bars;
            AChartButtons[(int) ChartButtons.AmbiguousBars].ToolTipText = Language.T("Ambiguous bars mark") + "   M";
            AChartButtons[(int) ChartButtons.AmbiguousBars].Checked = ShowAmbiguousBars;

            // Indicators
            AChartButtons[(int) ChartButtons.Indicators].Image = Resources.chart_indicators;
            AChartButtons[(int) ChartButtons.Indicators].ToolTipText = Language.T("Indicators charts") + "   D";
            AChartButtons[(int) ChartButtons.Indicators].Checked = ShowIndicators;

            // Balance Equity
            AChartButtons[(int) ChartButtons.BalanceEquity].Image = Resources.chart_balance_equity;
            AChartButtons[(int) ChartButtons.BalanceEquity].ToolTipText = Language.T("Balance / Equity chart") + "   B";
            AChartButtons[(int) ChartButtons.BalanceEquity].Checked = ShowBalanceEquity;

            // Floating P/L
            AChartButtons[(int) ChartButtons.FloatingPL].Image = Resources.chart_floating_pl;
            AChartButtons[(int) ChartButtons.FloatingPL].ToolTipText = Language.T("Floating P/L chart") + "   F";
            AChartButtons[(int) ChartButtons.FloatingPL].Checked = ShowFloatingPL;

            // Zoom in
            AChartButtons[(int) ChartButtons.ZoomIn].Image = Resources.chart_zoom_in;
            AChartButtons[(int) ChartButtons.ZoomIn].ToolTipText = Language.T("Zoom in") + "   +";

            // Zoom out
            AChartButtons[(int) ChartButtons.ZoomOut].Image = Resources.chart_zoom_out;
            AChartButtons[(int) ChartButtons.ZoomOut].ToolTipText = Language.T("Zoom out") + "   -";

            // True Charts
            AChartButtons[(int) ChartButtons.TrueCharts].Image = Resources.chart_true_charts;
            AChartButtons[(int) ChartButtons.TrueCharts].Checked = TrueCharts;
            AChartButtons[(int) ChartButtons.TrueCharts].ToolTipText = Language.T("True indicator charts") + "   T";

            // Show dynamic info
            AChartButtons[(int) ChartButtons.DynamicInfo].Image = Resources.chart_dyninfo;
            AChartButtons[(int) ChartButtons.DynamicInfo].Checked = ShowInfoPanel;
            AChartButtons[(int) ChartButtons.DynamicInfo].ToolTipText = Language.T("Show / hide the info panel") +
                                                                        "   I";

            // Move Dynamic Info Up
            AChartButtons[(int) ChartButtons.DInfoUp].Image = Resources.chart_dinfo_up;
            AChartButtons[(int) ChartButtons.DInfoUp].ToolTipText = Language.T("Scroll info upwards") + "   A, S";
            AChartButtons[(int) ChartButtons.DInfoUp].Visible = ShowInfoPanel;

            // Move Dynamic Info Down
            AChartButtons[(int) ChartButtons.DInfoDwn].Image = Resources.chart_dinfo_down;
            AChartButtons[(int) ChartButtons.DInfoDwn].ToolTipText = Language.T("Scroll info downwards") + "   Z, X";
            AChartButtons[(int) ChartButtons.DInfoDwn].Visible = ShowInfoPanel;
        }

        /// <summary>
        /// Create and sets the indicator panels
        /// </summary>
        private void SetupIndicatorPanels()
        {
            PnlPrice = new Panel {Parent = PnlCharts, Dock = DockStyle.Fill, BackColor = LayoutColors.ColorChartBack};
            PnlPrice.MouseLeave += PnlPriceMouseLeave;
            PnlPrice.MouseDoubleClick += PnlPriceMouseDoubleClick;
            PnlPrice.MouseMove += PnlPriceMouseMove;
            PnlPrice.MouseDown += PanelMouseDown;
            PnlPrice.MouseUp += PanelMouseUp;
            PnlPrice.Paint += PnlPricePaint;

            // Indicator panels
            _indPanels = 0;
            var asIndicatorTexts = new string[_backtester.Strategy.Slots];
            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                Indicator indicator = IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                                        _backtester.DataSet,
                                                                        _backtester.Strategy.Slot[slot].SlotType);
                indicator.IndParam = _backtester.Strategy.Slot[slot].IndParam;
                asIndicatorTexts[slot] = indicator.ToString();
                _indPanels += _backtester.Strategy.Slot[slot].SeparatedChart ? 1 : 0;
            }

            // Repeated indicators
            _repeatedIndicators = new bool[_backtester.Strategy.Slots];
            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                _repeatedIndicators[slot] = false;
                for (int i = 0; i < slot; i++)
                    _repeatedIndicators[slot] = asIndicatorTexts[slot] == asIndicatorTexts[i];
            }

            PnlInd = new Panel[_indPanels];
            SplitterInd = new Splitter[_indPanels];
            for (int i = 0; i < _indPanels; i++)
            {
                SplitterInd[i] = new Splitter {BorderStyle = BorderStyle.None, Dock = DockStyle.Bottom, Height = 2};

                PnlInd[i] = new Panel {Dock = DockStyle.Bottom, BackColor = LayoutColors.ColorControlBack};
                PnlInd[i].Paint += PnlIndPaint;
                PnlInd[i].Resize += PnlIndResize;
                PnlInd[i].MouseMove += SepChartMouseMove;
                PnlInd[i].MouseLeave += SepChartMouseLeave;
                PnlInd[i].MouseDown += PanelMouseDown;
                PnlInd[i].MouseUp += PanelMouseUp;
            }

            int iIndex = 0;
            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                if (!_backtester.Strategy.Slot[slot].SeparatedChart) continue;
                PnlInd[iIndex].Tag = slot;
                iIndex++;
            }

            for (int i = 0; i < _indPanels && ShowIndicators; i++)
            {
                SplitterInd[i].Parent = PnlCharts;
                PnlInd[i].Parent = PnlCharts;
            }

            // Balance chart
            SpliterBalance = new Splitter {BorderStyle = BorderStyle.None, Dock = DockStyle.Bottom, Height = 2};

            PnlBalanceChart = new Panel {Dock = DockStyle.Bottom, BackColor = LayoutColors.ColorChartBack};
            PnlBalanceChart.Paint += PnlBalancePaint;
            PnlBalanceChart.Resize += PnlBalanceResize;
            PnlBalanceChart.MouseMove += SepChartMouseMove;
            PnlBalanceChart.MouseLeave += SepChartMouseLeave;
            PnlBalanceChart.MouseDown += PanelMouseDown;
            PnlBalanceChart.MouseUp += PanelMouseUp;

            if (ShowBalanceEquity)
            {
                SpliterBalance.Parent = PnlCharts;
                PnlBalanceChart.Parent = PnlCharts;
            }

            // Floating Profit Loss chart
            SpliterFloatingPL = new Splitter {BorderStyle = BorderStyle.None, Dock = DockStyle.Bottom, Height = 2};

            PnlFloatingPLChart = new Panel {Dock = DockStyle.Bottom, BackColor = LayoutColors.ColorChartBack};
            PnlFloatingPLChart.Paint += PnlFloatingPLPaint;
            PnlFloatingPLChart.Resize += PnlFloatingPLResize;
            PnlFloatingPLChart.MouseMove += SepChartMouseMove;
            PnlFloatingPLChart.MouseLeave += SepChartMouseLeave;
            PnlFloatingPLChart.MouseDown += PanelMouseDown;
            PnlFloatingPLChart.MouseUp += PanelMouseUp;

            if (ShowFloatingPL)
            {
                SpliterFloatingPL.Parent = PnlCharts;
                PnlFloatingPLChart.Parent = PnlCharts;
            }

            _scroll = new HScrollBar
                          {
                              Parent = PnlCharts,
                              Dock = DockStyle.Bottom,
                              TabStop = true,
                              Minimum = _backtester.Strategy.FirstBar,
                              Maximum = _backtester.DataSet.Bars - 1,
                              SmallChange = 1
                          };
            _scroll.ValueChanged += ScrollValueChanged;
            _scroll.MouseWheel += ScrollMouseWheel;
            _scroll.KeyUp += ScrollKeyUp;
            _scroll.KeyDown += ScrollKeyDown;
        }

        /// <summary>
        /// Sets the chart's parameters.
        /// </summary>
        private void SetPriceChartParam()
        {
            _chartBars = _chartWidth/BarPixels;
            _chartBars = Math.Min(_chartBars, _backtester.DataSet.Bars - _backtester.Strategy.FirstBar);
            _firstBar = Math.Max(_backtester.Strategy.FirstBar, _backtester.DataSet.Bars - _chartBars);
            _firstBar = Math.Min(_firstBar, _backtester.DataSet.Bars - 1);
            _lastBar = Math.Max(_firstBar + _chartBars - 1, _firstBar);

            _scroll.Value = _firstBar;
            _scroll.LargeChange = Math.Max(_chartBars, 1);
        }

        /// <summary>
        /// Sets the indicator chart title
        /// </summary>
        private void SetupChartTitle()
        {
            // Chart title
            _chartTitle = _backtester.DataSet.Symbol + "  " + _backtester.DataSet.PeriodString + " " + _backtester.Strategy.StrategyName;

            if (!ShowIndicators) return;

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                if (_backtester.Strategy.Slot[slot].SeparatedChart) continue;

                bool isChart = false;
                for (int iComp = 0; iComp < _backtester.Strategy.Slot[slot].Component.Length; iComp++)
                {
                    if (_backtester.Strategy.Slot[slot].Component[iComp].ChartType != IndChartType.NoChart)
                    {
                        isChart = true;
                        break;
                    }
                }
                if (isChart)
                {
                    Indicator indicator = IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                                            _backtester.DataSet,
                                                                            _backtester.Strategy.Slot[slot].SlotType);
                    indicator.IndParam = _backtester.Strategy.Slot[slot].IndParam;
                    if (!_chartTitle.Contains(indicator.ToString()))
                        _chartTitle += Environment.NewLine + indicator;
                }
            }
        }

        /// <summary>
        /// Sets the sizes of the panels after resizing.
        /// </summary>
        private void PnlBaseResize(object sender, EventArgs e)
        {
            SetAllPanelsHeight();
            SetPriceChartParam();
            _dynInfoScrollValue = 0;
        }

        /// <summary>
        /// Calculates the panels' height
        /// </summary>
        private void SetAllPanelsHeight()
        {
            int panelNumber = ShowIndicators ? _indPanels : 0;
            panelNumber += ShowFloatingPL ? 1 : 0;
            panelNumber += ShowBalanceEquity ? 1 : 0;

            int iAvailableHeight = PnlCharts.ClientSize.Height - TsChartButtons.Height - _scroll.Height - panelNumber*4;

            int iPnlIndHeight = iAvailableHeight/(2 + panelNumber);

            for (int i = 0; i < _indPanels && ShowIndicators; i++)
                PnlInd[i].Height = iPnlIndHeight;

            if (ShowFloatingPL)
                PnlFloatingPLChart.Height = iPnlIndHeight;

            if (ShowBalanceEquity)
                PnlBalanceChart.Height = iPnlIndHeight;
        }

        /// <summary>
        /// Sets the parameters after resizing of the PnlPrice.
        /// </summary>
        private void PnlPriceResize(object sender, EventArgs e)
        {
            _xLeft = _spcLeft;
            _xRight = PnlPrice.ClientSize.Width - _spcRight;
            _chartWidth = Math.Max(_xRight - _xLeft, 0);
            _yTop = _spcTop;
            _yBottom = PnlPrice.ClientSize.Height - _spcBottom;
            _yBottomText = PnlPrice.ClientSize.Height - _spcBottom*5/8 - 4;
            PnlPrice.Invalidate();
        }

        /// <summary>
        /// Invalidates the panels
        /// </summary>
        private void PnlIndResize(object sender, EventArgs e)
        {
            if (!ShowIndicators) return;
            ((Panel) sender).Invalidate();
        }

        /// <summary>
        /// Invalidates the panel
        /// </summary>
        private void PnlBalanceResize(object sender, EventArgs e)
        {
            if (!ShowBalanceEquity) return;

            ((Panel) sender).Invalidate();
        }

        /// <summary>
        /// Invalidates the panel
        /// </summary>
        private void PnlFloatingPLResize(object sender, EventArgs e)
        {
            if (!ShowFloatingPL) return;

            ((Panel) sender).Invalidate();
        }

        /// <summary>
        /// Paints the panel PnlPrice
        /// </summary>
        private void PnlPricePaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(LayoutColors.ColorChartBack);

            if (_chartBars == 0) return;

            // Searching the min and the max price and volume
            _maxPrice = double.MinValue;
            _minPrice = double.MaxValue;
            _maxVolume = int.MinValue;
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                if (_backtester.DataSet.High[bar] > _maxPrice) _maxPrice = _backtester.DataSet.High[bar];
                if (_backtester.DataSet.Low[bar] < _minPrice) _minPrice = _backtester.DataSet.Low[bar];
                if (_backtester.DataSet.Volume[bar] > _maxVolume) _maxVolume = _backtester.DataSet.Volume[bar];
            }

            double pricePixel = (_maxPrice - _minPrice)/(_yBottom - _yTop);
            if (ShowVolume)
                _minPrice -= pricePixel*30;
            else if (ShowPositionLots)
                _minPrice -= pricePixel*10;

            _maxPrice += pricePixel*_verticalScale;
            _minPrice -= pricePixel*_verticalScale;

            // Grid
            int countLabels = Math.Max((_yBottom - _yTop)/30, 1);
            double deltaPoint = (_backtester.DataSet.InstrProperties.Digits == 5 || _backtester.DataSet.InstrProperties.Digits == 3)
                                    ? _backtester.DataSet.InstrProperties.Point*100
                                    : _backtester.DataSet.InstrProperties.Point*10;
            double deltaLabel =
                Math.Max(Math.Round((_maxPrice - _minPrice)/countLabels, _backtester.DataSet.InstrProperties.Point < 0.001 ? 3 : 1),
                         deltaPoint);
            _minPrice = Math.Round(_minPrice, _backtester.DataSet.InstrProperties.Point < 0.001 ? 3 : 1) - deltaPoint;
            countLabels = (int) Math.Ceiling((_maxPrice - _minPrice)/deltaLabel);
            _maxPrice = _minPrice + countLabels*deltaLabel;
            _yScale = (_yBottom - _yTop)/(countLabels*deltaLabel);
            _yVolScale = _maxVolume > 0 ? 40.0f/_maxVolume : 0f; // 40 - the highest volume line

            // Price labels
            for (double label = _minPrice; label <= _maxPrice + _backtester.DataSet.InstrProperties.Point; label += deltaLabel)
            {
                var iLabelY = (int) Math.Round(_yBottom - (label - _minPrice)*_yScale);
                g.DrawString(label.ToString(_backtester.DataSet.FF), Font, _brushFore, _xRight, iLabelY - Font.Height / 2 - 1);
                if (ShowGrid || Math.Abs(label - _minPrice) < 0.000001)
                    g.DrawLine(_penGrid, _spcLeft, iLabelY, _xRight, iLabelY);
                else
                    g.DrawLine(_penGrid, _xRight - 5, iLabelY, _xRight, iLabelY);
            }
            // Date labels
            for (int iVertLineBar = _lastBar;
                 iVertLineBar > _firstBar;
                 iVertLineBar -= (int) Math.Round((_szDate.Width + 10.0)/BarPixels + 1))
            {
                int iXVertLine = (iVertLineBar - _firstBar)*BarPixels + _spcLeft + BarPixels/2 - 1;
                if (ShowGrid)
                    g.DrawLine(_penGrid, iXVertLine, _yTop, iXVertLine, _yBottom + 2);
                string date = String.Format("{0} {1}", _backtester.DataSet.Time[iVertLineBar].ToString(Data.DFS),
                                            _backtester.DataSet.Time[iVertLineBar].ToString("HH:mm"));
                g.DrawString(date, _font, _brushFore, iXVertLine - _szDate.Width/2, _yBottomText);
            }

            // Cross
            if (ShowCross && _mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
            {
                int crossBar = (_mouseX - _spcLeft)/BarPixels;
                crossBar = Math.Max(0, crossBar);
                crossBar = Math.Min(_chartBars - 1, crossBar);
                crossBar += _firstBar;
                crossBar = Math.Min(_backtester.DataSet.Bars - 1, crossBar);

                // Vertical positions
                var point = new Point(_mouseX - _szDateL.Width/2, _yBottomText);
                var rec = new Rectangle(point, _szDateL);

                // Vertical line
                if (_isMouseInPriceChart && _mouseY > _yTop - 1 && _mouseY < _yBottom + 1)
                {
                    g.DrawLine(_penCross, _mouseX, _yTop, _mouseX, _mouseY - 10);
                    g.DrawLine(_penCross, _mouseX, _mouseY + 10, _mouseX, _yBottomText);
                }
                else if (_isMouseInPriceChart || _isMouseInIndicatorChart)
                {
                    g.DrawLine(_penCross, _mouseX, _yTop, _mouseX, _yBottomText);
                }

                // Date Window
                if (_isMouseInPriceChart || _isMouseInIndicatorChart)
                {
                    g.FillRectangle(_brushLabelBkgrd, rec);
                    g.DrawRectangle(_penCross, rec);
                    string sDate = _backtester.DataSet.Time[crossBar].ToString(Data.DF) + " " + _backtester.DataSet.Time[crossBar].ToString("HH:mm");
                    g.DrawString(sDate, _font, _brushLabelFore, point);
                }

                if (_isMouseInPriceChart && _mouseY > _yTop - 1 && _mouseY < _yBottom + 1)
                {
                    //Horizontal positions
                    point = new Point(_xRight, _mouseY - _szPrice.Height/2);
                    rec = new Rectangle(point, _szPrice);
                    // Horizontal line
                    g.DrawLine(_penCross, _xLeft, _mouseY, _mouseX - 10, _mouseY);
                    g.DrawLine(_penCross, _mouseX + 10, _mouseY, _xRight, _mouseY);
                    // Price Window
                    g.FillRectangle(_brushLabelBkgrd, rec);
                    g.DrawRectangle(_penCross, rec);
                    string sPrice = ((_yBottom - _mouseY) / _yScale + _minPrice).ToString(_backtester.DataSet.FF);
                    g.DrawString(sPrice, _font, _brushLabelFore, point);
                }
            }

            // Draws Volume, Lots and Bars
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                int x = (bar - _firstBar)*BarPixels + _spcLeft;
                var yOpen = (int)Math.Round(_yBottom - (_backtester.DataSet.Open[bar] - _minPrice) * _yScale);
                var yHigh = (int)Math.Round(_yBottom - (_backtester.DataSet.High[bar] - _minPrice) * _yScale);
                var yLow = (int)Math.Round(_yBottom - (_backtester.DataSet.Low[bar] - _minPrice) * _yScale);
                var yClose = (int)Math.Round(_yBottom - (_backtester.DataSet.Close[bar] - _minPrice) * _yScale);
                var yVolume = (int)Math.Round(_yBottom - _backtester.DataSet.Volume[bar] * _yVolScale);

                // Draw the volume
                if (ShowVolume && yVolume != _yBottom)
                    g.DrawLine(_penVolume, x + BarPixels/2 - 1, yVolume, x + BarPixels/2 - 1, _yBottom);

                // Draw position lots
                if (ShowPositionLots && _backtester.IsPos(bar))
                {
                    var iPosHight = (int) Math.Round(Math.Max(_backtester.SummaryLots(bar)*2, 2));
                    int iPosY = _yBottom - iPosHight + 1;

                    if (_backtester.SummaryDir(bar) == PosDirection.Long)
                    {
                        // Long
                        var rect = new Rectangle(x - 1, iPosY, BarPixels + 1, iPosHight);
                        var lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                        rect = new Rectangle(x, iPosY, BarPixels - 1, iPosHight);
                        g.FillRectangle(lgBrush, rect);
                    }
                    else if (_backtester.SummaryDir(bar) == PosDirection.Short)
                    {
                        // Short
                        var rect = new Rectangle(x - 1, iPosY, BarPixels + 1, iPosHight);
                        var lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                        rect = new Rectangle(x, iPosY, BarPixels - 1, iPosHight);
                        g.FillRectangle(lgBrush, rect);
                    }
                    else
                    {
                        // Closed
                        var rect = new Rectangle(x - 1, 2, BarPixels + 1, 2);
                        var lgBrush = new LinearGradientBrush(rect, _colorClosedTrade1, _colorClosedTrade2, 0f);
                        rect = new Rectangle(x, _yBottom - 1, BarPixels - 1, 2);
                        g.FillRectangle(lgBrush, rect);
                    }
                }

                // Draw the bar
                if (_isCandleChart)
                {
                    g.DrawLine(BarPixels < 25 ? _penBarBorder : _penBarThick, x + BarPixels/2 - 1, yLow,
                               x + BarPixels/2 - 1, yHigh);

                    if (BarPixels == 2)
                        g.DrawLine(_penBarBorder, x, yClose, x + 1, yClose);
                    else
                    {
                        if (yClose < yOpen)
                        {
                            // White bar
                            var rect = new Rectangle(x, yClose, BarPixels - 2, yOpen - yClose);
                            var lgBrush = new LinearGradientBrush(rect, _colorBarWhite1, _colorBarWhite2, 5f);
                            g.FillRectangle(lgBrush, rect);
                            g.DrawRectangle(_penBarBorder, x, yClose, BarPixels - 2, yOpen - yClose);
                        }
                        else if (yClose > yOpen)
                        {
                            // Black bar
                            var rect = new Rectangle(x, yOpen, BarPixels - 2, yClose - yOpen);
                            var lgBrush = new LinearGradientBrush(rect, _colorBarBlack1, _colorBarBlack2, 5f);
                            g.FillRectangle(lgBrush, rect);
                            g.DrawRectangle(_penBarBorder, rect);
                        }
                        else
                        {
                            // Cross
                            g.DrawLine(BarPixels < 25 ? _penBarBorder : _penBarThick, x, yClose, x + BarPixels - 2,
                                       yClose);
                        }
                    }
                }
                else
                {
                    if (BarPixels == 2)
                    {
                        g.DrawLine(_penBarBorder, x, yClose, x + 1, yClose);
                        g.DrawLine(_penBarBorder, x + BarPixels/2 - 1, yLow, x + BarPixels/2 - 1, yHigh);
                    }
                    else if (BarPixels <= 16)
                    {
                        g.DrawLine(_penBarBorder, x + BarPixels/2 - 1, yLow, x + BarPixels/2 - 1, yHigh);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(_penBarBorder, x, yOpen, x + BarPixels/2 - 1, yOpen);
                            g.DrawLine(_penBarBorder, x + BarPixels/2 - 1, yClose, x + BarPixels - 2, yClose);
                        }
                        else
                        {
                            g.DrawLine(_penBarBorder, x, yClose, x + BarPixels - 2, yClose);
                        }
                    }
                    else
                    {
                        g.DrawLine(_penBarThick, x + BarPixels/2 - 1, yLow + 2, x + BarPixels/2 - 1, yHigh - 1);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(_penBarThick, x + 1, yOpen, x + BarPixels/2 - 1, yOpen);
                            g.DrawLine(_penBarThick, x + BarPixels/2, yClose, x + BarPixels - 2, yClose);
                        }
                        else
                        {
                            g.DrawLine(_penBarThick, x, yClose, x + BarPixels - 2, yClose);
                        }
                    }
                }
            }

            // Drawing the indicators in the chart
            g.SetClip(new RectangleF(0, _yTop, _xRight, _yBottom - _yTop));
            for (int slot = 0; slot < _backtester.Strategy.Slots && ShowIndicators; slot++)
            {
                if (_backtester.Strategy.Slot[slot].SeparatedChart || _repeatedIndicators[slot]) continue;

                int cloudUp = -1; // For Ichimoku and similar
                int cloudDown = -1; // For Ichimoku and similar

                bool isIndicatorValueAtClose = true;
                int indicatorValueShift = 1;
                foreach (ListParam listParam in _backtester.Strategy.Slot[slot].IndParam.ListParam)
                    if (listParam.Caption == "Base price" && listParam.Text == "Open")
                    {
                        isIndicatorValueAtClose = false;
                        indicatorValueShift = 0;
                    }

                for (int comp = 0; comp < _backtester.Strategy.Slot[slot].Component.Length; comp++)
                {
                    var pen = new Pen(_backtester.Strategy.Slot[slot].Component[comp].ChartColor);
                    var penTc = new Pen(_backtester.Strategy.Slot[slot].Component[comp].ChartColor)
                                    {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};

                    if (_backtester.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Line)
                    {
                        // Line
                        if (TrueCharts)
                        {
                            // True Charts
                            var point = new Point[_lastBar - _firstBar + 1];
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = _backtester.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = _spcLeft + (bar - _firstBar)*BarPixels + indicatorValueShift*(BarPixels - 2);
                                var y = (int) Math.Round(_yBottom - (value - _minPrice)*_yScale);

                                if (Math.Abs(value - 0) < 0.00001)
                                    point[bar - _firstBar] = point[Math.Max(bar - _firstBar - 1, 0)];
                                else
                                    point[bar - _firstBar] = new Point(x, y);
                            }

                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                // All bars except the last one
                                int i = bar - _firstBar;

                                // The indicator value point
                                g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                                g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                                if (bar == _firstBar && isIndicatorValueAtClose)
                                {
                                    // First bar
                                    double value = _backtester.Strategy.Slot[slot].Component[comp].Value[bar - 1];
                                    int x = _spcLeft + (bar - _firstBar)*BarPixels;
                                    var y = (int) Math.Round(_yBottom - (value - _minPrice)*_yScale);

                                    int deltaY = Math.Abs(y - point[i].Y);
                                    if (BarPixels > 3)
                                    {
                                        // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, x + 1, y, x + BarPixels - 5, y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, x + 1, y, x + BarPixels - 4, y);
                                        else
                                            g.DrawLine(pen, x + 1, y, x + BarPixels - 2, y);
                                    }
                                    if (deltaY > 4)
                                    {
                                        // Vertical part
                                        if (point[i].Y > y)
                                            g.DrawLine(penTc, x + BarPixels - 2, y + 2, x + BarPixels - 2,
                                                       point[i].Y - 2);
                                        else
                                            g.DrawLine(penTc, x + BarPixels - 2, y - 2, x + BarPixels - 2,
                                                       point[i].Y + 2);
                                    }
                                }

                                if (bar < _lastBar)
                                {
                                    int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);

                                    if (BarPixels > 3)
                                    {
                                        // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                        else
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                    }
                                    if (deltaY > 4)
                                    {
                                        // Vertical part
                                        if (point[i + 1].Y > point[i].Y)
                                            g.DrawLine(penTc, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                       point[i + 1].Y - 2);
                                        else
                                            g.DrawLine(penTc, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                       point[i + 1].Y + 2);
                                    }
                                }

                                if (bar == _lastBar && !isIndicatorValueAtClose && BarPixels > 3)
                                {
                                    // Last bar
                                    g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + BarPixels - 2, point[i].Y);
                                }
                            }
                        }
                        else
                        {
                            // Regular Charts
                            var aPoint = new Point[_lastBar - _firstBar + 1];
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double dValue = _backtester.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = _spcLeft + (bar - _firstBar)*BarPixels + BarPixels/2 - 1;
                                var y = (int) Math.Round(_yBottom - (dValue - _minPrice)*_yScale);

                                if (Math.Abs(dValue - 0) < 0.00001)
                                    aPoint[bar - _firstBar] = aPoint[Math.Max(bar - _firstBar - 1, 0)];
                                else
                                    aPoint[bar - _firstBar] = new Point(x, y);
                            }
                            g.DrawLines(pen, aPoint);
                        }
                    }
                    else if (_backtester.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Dot)
                    {
                        // Dots
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double dValue = _backtester.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - _firstBar)*BarPixels + BarPixels/2 - 1 + _spcLeft;
                            var y = (int) Math.Round(_yBottom - (dValue - _minPrice)*_yScale);
                            if (BarPixels == 2)
                                g.FillRectangle(pen.Brush, x, y, 1, 1);
                            else
                            {
                                g.DrawLine(pen, x - 1, y, x + 1, y);
                                g.DrawLine(pen, x, y - 1, x, y + 1);
                            }
                        }
                    }
                    else if (_backtester.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Level)
                    {
                        // Level
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double dValue = _backtester.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - _firstBar)*BarPixels + _spcLeft;
                            var y = (int) Math.Round(_yBottom - (dValue - _minPrice)*_yScale);
                            g.DrawLine(pen, x, y, x + BarPixels - 1, y);
                        }
                    }
                    else if (_backtester.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudUp)
                    {
                        cloudUp = comp;
                    }
                    else if (_backtester.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudDown)
                    {
                        cloudDown = comp;
                    }
                }

                // Clouds
                if (cloudUp >= 0 && cloudDown >= 0)
                {
                    var apntUp = new PointF[_lastBar - _firstBar + 1];
                    var apntDown = new PointF[_lastBar - _firstBar + 1];
                    for (int bar = _firstBar; bar <= _lastBar; bar++)
                    {
                        double dValueUp = _backtester.Strategy.Slot[slot].Component[cloudUp].Value[bar];
                        double dValueDown = _backtester.Strategy.Slot[slot].Component[cloudDown].Value[bar];
                        apntUp[bar - _firstBar].X = (bar - _firstBar)*BarPixels + BarPixels/2 - 1 + _spcLeft;
                        apntUp[bar - _firstBar].Y = (int) Math.Round(_yBottom - (dValueUp - _minPrice)*_yScale);
                        apntDown[bar - _firstBar].X = (bar - _firstBar)*BarPixels + BarPixels/2 - 1 + _spcLeft;
                        apntDown[bar - _firstBar].Y = (int) Math.Round(_yBottom - (dValueDown - _minPrice)*_yScale);
                    }

                    var pathUp = new GraphicsPath();
                    pathUp.AddLine(new PointF(apntUp[0].X, 0), apntUp[0]);
                    pathUp.AddLines(apntUp);
                    pathUp.AddLine(apntUp[_lastBar - _firstBar], new PointF(apntUp[_lastBar - _firstBar].X, 0));
                    pathUp.AddLine(new PointF(apntUp[_lastBar - _firstBar].X, 0), new PointF(apntUp[0].X, 0));

                    var pathDown = new GraphicsPath();
                    pathDown.AddLine(new PointF(apntDown[0].X, 0), apntDown[0]);
                    pathDown.AddLines(apntDown);
                    pathDown.AddLine(apntDown[_lastBar - _firstBar], new PointF(apntDown[_lastBar - _firstBar].X, 0));
                    pathDown.AddLine(new PointF(apntDown[_lastBar - _firstBar].X, 0), new PointF(apntDown[0].X, 0));

                    Color colorUp = Color.FromArgb(50, _backtester.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Color colorDown = Color.FromArgb(50, _backtester.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    var penUp = new Pen(_backtester.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    var penDown = new Pen(_backtester.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    penUp.DashStyle = DashStyle.Dash;
                    penDown.DashStyle = DashStyle.Dash;

                    Brush brushUp = new SolidBrush(colorUp);
                    Brush brushDown = new SolidBrush(colorDown);

                    var regionUp = new Region(pathUp);
                    regionUp.Exclude(pathDown);
                    g.FillRegion(brushDown, regionUp);

                    var regionDown = new Region(pathDown);
                    regionDown.Exclude(pathUp);
                    g.FillRegion(brushUp, regionDown);

                    g.DrawLines(penUp, apntUp);
                    g.DrawLines(penDown, apntDown);
                }
            }
            g.ResetClip();

            // Draws position price, deals and Ambiguous note
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                int x = (bar - _firstBar)*BarPixels + _spcLeft;
                var yHigh = (int)Math.Round(_yBottom - (_backtester.DataSet.High[bar] - _minPrice) * _yScale);

                // Draw the corrected position price
                for (int iPos = 0; iPos < _backtester.Positions(bar) && ShowPositionPrice; iPos++)
                {
                    var yPrice = (int) Math.Round(_yBottom - (_backtester.SummaryPrice(bar) - _minPrice)*_yScale);

                    if (yPrice >= _yBottom || yPrice <= _yTop) continue;

                    if (_backtester.SummaryDir(bar) == PosDirection.Long)
                    {
                        // Long
                        g.DrawLine(_penTradeLong, x, yPrice, x + BarPixels - 2, yPrice);
                    }
                    else if (_backtester.SummaryDir(bar) == PosDirection.Short)
                    {
                        // Short
                        g.DrawLine(_penTradeShort, x, yPrice, x + BarPixels - 2, yPrice);
                    }
                    else if (_backtester.SummaryDir(bar) == PosDirection.Closed)
                    {
                        // Closed
                        g.DrawLine(_penTradeClose, x, yPrice, x + BarPixels - 2, yPrice);
                    }
                }

                // Draw Break Even

                for (int ord = 0; ord < _backtester.Orders(bar) && ShowProtections; ord++)
                {
                    Order order = _backtester.OrdFromNumb(_backtester.OrdNumb(bar, ord));
                    if (order.OrdOrigin == OrderOrigin.BreakEven)
                    {
                        var yOrder = (int) Math.Round(_yBottom - (order.OrdPrice - _minPrice)*_yScale);
                        if (yOrder < _yBottom && yOrder > _yTop)
                        {
                            var penBreakEven = new Pen(LayoutColors.ColorTradeClose) {DashStyle = DashStyle.Dash};
                            g.DrawLine(penBreakEven, x, yOrder, x + BarPixels - 2, yOrder);
                        }
                    }
                    else if (order.OrdOrigin == OrderOrigin.PermanentStopLoss)
                    {
                        var yOrder = (int) Math.Round(_yBottom - (order.OrdPrice - _minPrice)*_yScale);
                        if (yOrder < _yBottom && yOrder > _yTop)
                        {
                            var penPermSL = new Pen(LayoutColors.ColorTradeShort) {DashStyle = DashStyle.Dash};
                            g.DrawLine(penPermSL, x, yOrder, x + BarPixels - 2, yOrder);
                        }
                    }
                    else if (order.OrdOrigin == OrderOrigin.PermanentTakeProfit)
                    {
                        var yOrder = (int) Math.Round(_yBottom - (order.OrdPrice - _minPrice)*_yScale);
                        if (yOrder < _yBottom && yOrder > _yTop)
                        {
                            var penPermTP = new Pen(LayoutColors.ColorTradeLong) {DashStyle = DashStyle.Dash};
                            g.DrawLine(penPermTP, x, yOrder, x + BarPixels - 2, yOrder);
                        }
                    }
                }

                // Draw the deals
                for (int iPos = 0; iPos < _backtester.Positions(bar) && ShowOrders; iPos++)
                {
                    if (_backtester.PosTransaction(bar, iPos) == Transaction.Transfer) continue;

                    var yDeal = (int) Math.Round(_yBottom - (_backtester.PosOrdPrice(bar, iPos) - _minPrice)*_yScale);

                    if (_backtester.PosDir(bar, iPos) == PosDirection.Long ||
                        _backtester.PosDir(bar, iPos) == PosDirection.Short)
                    {
                        if (_backtester.OrdFromNumb(_backtester.PosOrdNumb(bar, iPos)).OrdDir == OrderDirection.Buy)
                        {
                            // Buy
                            var pen = new Pen(_brushTradeLong, 2);
                            if (BarPixels < 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + BarPixels - 1, yDeal);
                            }
                            else if (BarPixels == 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + 4, yDeal);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yDeal, x + 5, yDeal - 3);
                            }
                            else if (BarPixels > 8)
                            {
                                int d = BarPixels/2 - 1;
                                int x1 = x + d;
                                int x2 = x + BarPixels - 2;
                                g.DrawLine(pen, x, yDeal, x1, yDeal);
                                g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                                g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d/2 + 1, yDeal - d + 1);
                                g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d/2);
                            }
                        }
                        else
                        {
                            // Sell
                            var pen = new Pen(_brushTradeShort, 2);
                            if (BarPixels < 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + BarPixels - 1, yDeal);
                            }
                            else if (BarPixels == 8)
                            {
                                g.DrawLine(pen, x, yDeal + 1, x + 4, yDeal + 1);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yDeal, x + 5, yDeal + 3);
                            }
                            else if (BarPixels > 8)
                            {
                                int d = BarPixels/2 - 1;
                                int x1 = x + d;
                                int x2 = x + BarPixels - 2;
                                g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                                g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                                g.DrawLine(pen, x1 + d/2 + 1, yDeal + d, x2, yDeal + d);
                                g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d/2 + 1);
                            }
                        }
                    }
                    else if (_backtester.PosDir(bar, iPos) == PosDirection.Closed)
                    {
                        // Close
                        var pen = new Pen(_brushTradeClose, 2);

                        if (BarPixels < 8)
                        {
                            g.DrawLine(pen, x, yDeal, x + BarPixels - 1, yDeal);
                        }
                        else if (BarPixels == 8)
                        {
                            g.DrawLine(pen, x, yDeal, x + 7, yDeal);
                            g.DrawLine(pen, x + 5, yDeal - 2, x + 5, yDeal + 2);
                        }
                        else if (BarPixels > 8)
                        {
                            int d = BarPixels/2 - 1;
                            int x1 = x + d;
                            int x2 = x + BarPixels - 3;
                            g.DrawLine(pen, x, yDeal, x1, yDeal);
                            g.DrawLine(pen, x1, yDeal + d/2, x2, yDeal - d/2);
                            g.DrawLine(pen, x1, yDeal - d/2, x2, yDeal + d/2);
                        }
                    }
                }

                // Ambiguous note
                if (ShowAmbiguousBars && _backtester.BackTestEval(bar) == BacktestEval.Ambiguous)
                    g.DrawString("!", Font, _brushSignalRed, x + BarPixels/2 - 4, yHigh - 20);
            }

            // Chart title
            g.DrawString(_chartTitle, _font, _brushFore, _spcLeft, 0);
        }

        /// <summary>
        /// Paints the panel PnlInd
        /// </summary>
        private void PnlIndPaint(object sender, PaintEventArgs e)
        {
            if (!ShowIndicators) return;

            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            var slot = (int) pnl.Tag;

            int topSpace = _font.Height/2 + 2;
            int bottomSpace = _font.Height/2;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            g.Clear(LayoutColors.ColorChartBack);

            if (_chartBars == 0) return;

            foreach (IndicatorComp component in _backtester.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(_firstBar - 1, component.FirstBar); bar <= _lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, _backtester.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, _backtester.Strategy.Slot[slot].MaxValue);

            foreach (double value in _backtester.Strategy.Slot[slot].SpecValue)
                if (Math.Abs(value - 0) < 0.00001)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            double scale = (pnl.ClientSize.Height - topSpace - bottomSpace)/(Math.Max(maxValue - minValue, 0.0001));

            // Grid
            String strFormat;
            double labelAbs;
            int xGridRight = pnl.ClientSize.Width - _spcRight + 2;

            double label = 0;
            var labelYZero = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (label >= minValue && label <= maxValue)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10
                                ? "F4"
                                : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), _font, _brushFore, _xRight, labelYZero - _font.Height/2 - 1);
                g.DrawLine(_penGridSolid, _spcLeft, labelYZero, xGridRight, labelYZero);
            }

            label = minValue; // Bottom line
            var labelYMin = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMin) >= _font.Height)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10
                                ? "F4"
                                : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), _font, _brushFore, _xRight, labelYMin - _font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(_penGrid, _spcLeft, labelYMin, xGridRight, labelYMin);
                else
                    g.DrawLine(_penGrid, xGridRight - 5, labelYMin, xGridRight, labelYMin);
            }

            label = maxValue; // Top line
            var labelYMax = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMax) >= _font.Height)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10
                                ? "F4"
                                : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), _font, _brushFore, _xRight, labelYMax - _font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(_penGrid, _spcLeft, labelYMax, xGridRight, labelYMax);
                else
                    g.DrawLine(_penGrid, xGridRight - 5, labelYMax, xGridRight, labelYMax);
            }

            if (_backtester.Strategy.Slot[slot].SpecValue != null)
                for (int i = 0; i < _backtester.Strategy.Slot[slot].SpecValue.Length; i++)
                {
                    label = _backtester.Strategy.Slot[slot].SpecValue[i];
                    if (label <= maxValue && label >= minValue)
                    {
                        var labelY = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
                        if (Math.Abs(labelY - labelYZero) < _font.Height) continue;
                        if (Math.Abs(labelY - labelYMin) < _font.Height) continue;
                        if (Math.Abs(labelY - labelYMax) < _font.Height) continue;
                        labelAbs = Math.Abs(label);
                        strFormat = labelAbs < 10
                                        ? "F4"
                                        : labelAbs < 100
                                              ? "F3"
                                              : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                        g.DrawString(label.ToString(strFormat), _font, _brushFore, _xRight, labelY - _font.Height/2 - 1);
                        if (ShowGrid)
                            g.DrawLine(_penGrid, _spcLeft, labelY, xGridRight, labelY);
                        else
                            g.DrawLine(_penGrid, xGridRight - 5, labelY, xGridRight, labelY);
                    }
                }

            // Vertical line
            if (ShowGrid)
            {
                string date = _backtester.DataSet.Time[_firstBar].ToString("dd.MM") + " " + _backtester.DataSet.Time[_firstBar].ToString("HH:mm");
                var dateWidth = (int) g.MeasureString(date, _font).Width;
                for (int vertLineBar = _lastBar;
                     vertLineBar > _firstBar;
                     vertLineBar -= (int) Math.Round((dateWidth + 10.0)/BarPixels + 1))
                {
                    int xVertLine = (vertLineBar - _firstBar)*BarPixels + BarPixels/2 - 1 + _spcLeft;
                    g.DrawLine(_penGrid, xVertLine, topSpace, xVertLine, pnl.ClientSize.Height - bottomSpace);
                }
            }

            bool isIndicatorValueAtClose = true;
            int indicatorValueShift = 1;
            foreach (ListParam listParam in _backtester.Strategy.Slot[slot].IndParam.ListParam)
                if (listParam.Caption == "Base price" && listParam.Text == "Open")
                {
                    isIndicatorValueAtClose = false;
                    indicatorValueShift = 0;
                }

            // Indicator chart
            foreach (IndicatorComp component in _backtester.Strategy.Slot[slot].Component)
            {
                if (component.ChartType == IndChartType.Histogram)
                {
                    // Histogram
                    double zero = 0;
                    if (zero < minValue) zero = minValue;
                    if (zero > maxValue) zero = maxValue;
                    var y0 = (int) Math.Round(pnl.ClientSize.Height - 5 - (zero - minValue)*scale);

                    var penGreen = new Pen(LayoutColors.ColorTradeLong);
                    var penRed = new Pen(LayoutColors.ColorTradeShort);

                    bool isPrevBarGreen = false;

                    if (TrueCharts)
                    {
                        if (isIndicatorValueAtClose)
                        {
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = component.Value[bar - 1];
                                double prevValue = component.Value[bar - 2];
                                int x = _spcLeft + (bar - _firstBar)*BarPixels + BarPixels/2 - 1;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penGreen, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penRed, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penRed, x, y0 - 2, x, y);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = _spcLeft + (bar - _firstBar)*BarPixels + BarPixels - 2;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    isPrevBarGreen = true;
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    isPrevBarGreen = false;
                                }
                            }
                        }
                        else
                        {
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = _spcLeft + (bar - _firstBar)*BarPixels + BarPixels/2 - 1;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penGreen, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penRed, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penRed, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            double prevValue = component.Value[bar - 1];
                            int x = (bar - _firstBar)*BarPixels + _spcLeft;
                            var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                            LinearGradientBrush lgBrush;
                            Rectangle rect;
                            if (value > prevValue || Math.Abs(value - prevValue) < 0.000001 && isPrevBarGreen)
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = true;
                            }
                            else
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = false;
                            }
                        }
                    }
                }

                if (component.ChartType == IndChartType.Line)
                {
                    // Line
                    var pen = new Pen(component.ChartColor);
                    var penTc = new Pen(component.ChartColor)
                                    {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};
                    int yIndChart = pnl.ClientSize.Height - 7;

                    if (TrueCharts)
                    {
                        // True Charts
                        var point = new Point[_lastBar - _firstBar + 1];
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = _spcLeft + (bar - _firstBar + indicatorValueShift)*BarPixels - 2*indicatorValueShift;
                            var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                            point[bar - _firstBar] = new Point(x, y);
                        }

                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            // All bars except the last one
                            int i = bar - _firstBar;

                            // The indicator value point
                            g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                            g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                            if (bar == _firstBar && isIndicatorValueAtClose)
                            {
                                // First bar
                                double value = component.Value[bar - 1];
                                int x = _spcLeft + (bar - _firstBar)*BarPixels;
                                var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                                int deltaY = Math.Abs(y - point[i].Y);
                                if (BarPixels > 3)
                                {
                                    // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, x + 1, y, x + BarPixels - 5, y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, x + 1, y, x + BarPixels - 4, y);
                                    else
                                        g.DrawLine(pen, x + 1, y, x + BarPixels - 2, y);
                                }
                                if (deltaY > 4)
                                {
                                    // Vertical part
                                    if (point[i].Y > y)
                                        g.DrawLine(penTc, x + BarPixels - 2, y + 2, x + BarPixels - 2, point[i].Y - 2);
                                    else
                                        g.DrawLine(penTc, x + BarPixels - 2, y - 2, x + BarPixels - 2, point[i].Y + 2);
                                }
                            }

                            if (bar < _lastBar)
                            {
                                int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);
                                if (BarPixels > 3)
                                {
                                    // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                    else
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                }
                                if (deltaY > 4)
                                {
                                    // Vertical part
                                    if (point[i + 1].Y > point[i].Y)
                                        g.DrawLine(penTc, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                   point[i + 1].Y - 2);
                                    else
                                        g.DrawLine(penTc, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                   point[i + 1].Y + 2);
                                }
                            }

                            if (bar == _lastBar && !isIndicatorValueAtClose && BarPixels > 3)
                            {
                                // Last bar
                                g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + BarPixels - 2, point[i].Y);
                            }
                        }
                    }
                    else
                    {
                        // Regular Charts
                        var aPoint = new Point[_lastBar - _firstBar + 1];
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = (bar - _firstBar)*BarPixels + BarPixels/2 - 1 + _spcLeft;
                            var y = (int) Math.Round(yIndChart - (value - minValue)*scale);
                            aPoint[bar - _firstBar] = new Point(x, y);
                        }
                        g.DrawLines(pen, aPoint);
                    }
                }
            }

            // Vertical cross line
            if (ShowCross && _mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                g.DrawLine(_penCross, _mouseX, 0, _mouseX, pnl.ClientSize.Height);


            // Chart title
            Indicator indicator = IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                                    _backtester.DataSet,
                                                                    _backtester.Strategy.Slot[slot].SlotType);
            indicator.IndParam = _backtester.Strategy.Slot[slot].IndParam;
            string indicatorText = indicator.ToString();
            Size sizeTitle = g.MeasureString(indicatorText, Font).ToSize();
            g.FillRectangle(_brushBack, new Rectangle(_spcLeft, 0, sizeTitle.Width, sizeTitle.Height));
            g.DrawString(indicatorText, Font, _brushFore, _spcLeft, 0);
        }

        /// <summary>
        /// Paints the panel FloatingPL
        /// </summary>
        private void PnlFloatingPLPaint(object sender, PaintEventArgs e)
        {
            if (!ShowFloatingPL) return;

            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            int topSpace = _font.Height/2 + 2;
            int bottmSpace = _font.Height/2;
            int maxValue = 10;
            int minValue = -10;
            int value;

            g.Clear(LayoutColors.ColorChartBack);

            if (_chartBars == 0) return;

            for (int bar = Math.Max(_firstBar, _backtester.Strategy.FirstBar); bar <= _lastBar; bar++)
            {
                if (!_backtester.IsPos(bar)) continue;
                value = Configs.AccountInMoney
                            ? (int) Math.Round(_backtester.MoneyProfitLoss(bar) + _backtester.MoneyFloatingPL(bar))
                            : _backtester.ProfitLoss(bar) + _backtester.FloatingPL(bar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }

            minValue = 10*(int) Math.Floor(minValue/10.0);
            maxValue = 10*(int) Math.Ceiling(maxValue/10.0);

            double scale = (pnl.ClientSize.Height - topSpace - bottmSpace)/((double) Math.Max(maxValue - minValue, 1));

            // Grid
            int xGridRight = pnl.ClientSize.Width - _spcRight + 2;
            int label = 0;
            var labelYZero = (int) Math.Round(pnl.ClientSize.Height - bottmSpace - (label - minValue)*scale);
            if (label >= minValue && label <= maxValue)
            {
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, _brushFore, _xRight,
                             labelYZero - Font.Height/2 - 1);
                g.DrawLine(_penGridSolid, _spcLeft, labelYZero, xGridRight, labelYZero);
            }

            label = minValue;
            var labelYMin = (int) Math.Round(pnl.ClientSize.Height - bottmSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMin) >= Font.Height)
            {
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, _brushFore, _xRight,
                             labelYMin - Font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(_penGrid, _spcLeft, labelYMin, xGridRight, labelYMin);
                else
                    g.DrawLine(_penGrid, xGridRight - 5, labelYMin, xGridRight, labelYMin);
            }
            label = maxValue;
            var labelYMax = (int) Math.Round(pnl.ClientSize.Height - bottmSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMax) >= Font.Height)
            {
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, _brushFore, _xRight,
                             labelYMax - Font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(_penGrid, _spcLeft, labelYMax, xGridRight, labelYMax);
                else
                    g.DrawLine(_penGrid, xGridRight - 5, labelYMax, xGridRight, labelYMax);
            }

            // Vertical line
            if (ShowGrid)
            {
                string date = _backtester.DataSet.Time[_firstBar].ToString("dd.MM") + " " + _backtester.DataSet.Time[_firstBar].ToString("HH:mm");
                var isDataWidth = (int) g.MeasureString(date, Font).Width;
                for (int vertLineBar = _lastBar;
                     vertLineBar > _firstBar;
                     vertLineBar -= (int) Math.Round((isDataWidth + 10.0)/BarPixels + 1))
                {
                    int xVertLine = (vertLineBar - _firstBar)*BarPixels + BarPixels/2 - 1 + _spcLeft;
                    g.DrawLine(_penGrid, xVertLine, topSpace, xVertLine, pnl.ClientSize.Height - bottmSpace);
                }
            }

            // Chart
            var y0 = (int) Math.Round(pnl.ClientSize.Height - 5 + minValue*scale);
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                if (!_backtester.IsPos(bar)) continue;
                value = Configs.AccountInMoney
                            ? (int) Math.Round(_backtester.MoneyProfitLoss(bar) + _backtester.MoneyFloatingPL(bar))
                            : _backtester.ProfitLoss(bar) + _backtester.FloatingPL(bar);
                int x = (bar - _firstBar)*BarPixels + _spcLeft;
                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                if (y == y0) continue;
                Rectangle rect;
                LinearGradientBrush lgBrush;
                if (_backtester.SummaryDir(bar) == PosDirection.Long)
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                        rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                        rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
                else if (_backtester.SummaryDir(bar) == PosDirection.Short)
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                        rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                        rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
                else
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, BarPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, _colorClosedTrade1, _colorClosedTrade2, 0f);
                        rect = new Rectangle(x, y0, BarPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, BarPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, _colorClosedTrade1, _colorClosedTrade2, 0f);
                        rect = new Rectangle(x, y, BarPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
            }

            // Vertical cross line
            if (ShowCross && _mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                g.DrawLine(_penCross, _mouseX, 0, _mouseX, pnl.ClientSize.Height);


            // Chart title
            string sTitle = Language.T("Floating P/L") + " [" +
                            (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            Size szTitle = g.MeasureString(sTitle, Font).ToSize();
            g.FillRectangle(_brushBack, new Rectangle(_spcLeft, 0, szTitle.Width, szTitle.Height));
            g.DrawString(sTitle, Font, _brushFore, _spcLeft, 0);
        }

        /// <summary>
        /// Paints the panel PnlBalance
        /// </summary>
        private void PnlBalancePaint(object sender, PaintEventArgs e)
        {
            if (!ShowBalanceEquity) return;

            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            g.Clear(LayoutColors.ColorChartBack);

            if (_chartBars == 0) return;

            int topSpace = Font.Height/2 + 2;
            int bottmSpace = Font.Height/2;
            int yTop = topSpace;
            int yBottom = pnl.ClientSize.Height - bottmSpace;
            int xLeft = _xLeft;
            int xRight = _xRight;

            // Min and Max values
            int maxValue = int.MinValue;
            int minValue = int.MaxValue;
            int value;
            for (int iBar = Math.Max(_firstBar, _backtester.Strategy.FirstBar); iBar <= _lastBar; iBar++)
            {
                value = Configs.AccountInMoney ? (int) _backtester.MoneyBalance(iBar) : _backtester.Balance(iBar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
                value = Configs.AccountInMoney ? (int) _backtester.MoneyEquity(iBar) : _backtester.Equity(iBar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }

            if (maxValue == 0 && minValue == 0)
            {
                maxValue = 10;
                minValue = -10;
            }

            if (maxValue == minValue)
            {
                maxValue += 10;
                minValue -= 10;
            }

            int countLabels = Math.Max((yBottom - yTop)/30, 1);
            int deltaLabels = 10*((Math.Max((maxValue - minValue)/countLabels, 10))/10);

            minValue = 10*(int) Math.Floor(minValue/10.0);
            countLabels = (int) Math.Ceiling((maxValue - minValue)/(double) deltaLabels);
            maxValue = minValue + countLabels*deltaLabels;

            double scale = (yBottom - yTop)/((double) countLabels*deltaLabels);

            // Grid
            for (int label = minValue; label <= maxValue; label += deltaLabels)
            {
                var yLabel = (int) Math.Round(yBottom - (label - minValue)*scale);
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, _brushFore, _xRight,
                             yLabel - Font.Height/2 - 1);
                if (ShowGrid)
                    g.DrawLine(_penGrid, xLeft, yLabel, xRight, yLabel);
                else
                    g.DrawLine(_penGrid, xRight - 5, yLabel, xRight, yLabel);
            }

            // Vertical grid lines
            if (ShowGrid)
            {
                for (int vertLineBar = _lastBar;
                     vertLineBar > _firstBar;
                     vertLineBar -= (int) Math.Round((_szDate.Width + 10.0)/BarPixels + 1))
                {
                    int xVertLine = (vertLineBar - _firstBar)*BarPixels + xLeft + BarPixels/2 - 1;
                    g.DrawLine(_penGrid, xVertLine, yTop, xVertLine, yBottom);
                }
            }

            // Chart
            var apntBalance = new Point[_lastBar - _firstBar + 1];
            var apntEquity = new Point[_lastBar - _firstBar + 1];
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                value = Configs.AccountInMoney ? (int) _backtester.MoneyBalance(bar) : _backtester.Balance(bar);
                int x = (bar - _firstBar)*BarPixels + BarPixels/2 - 1 + xLeft;
                var y = (int) Math.Round(yBottom - (value - minValue)*scale);
                apntBalance[bar - _firstBar] = new Point(x, y);
                value = Configs.AccountInMoney ? (int) _backtester.MoneyEquity(bar) : _backtester.Equity(bar);
                y = (int) Math.Round(yBottom - (value - minValue)*scale);
                apntEquity[bar - _firstBar] = new Point(x, y);
            }
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), apntEquity);
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);

            // Vertical cross line
            if (ShowCross && _mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                g.DrawLine(_penCross, _mouseX, 0, _mouseX, pnl.ClientSize.Height);

            // Chart title
            string sTitle = Language.T("Balance") + " / " + Language.T("Equity") +
                            " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            Size szTitle = g.MeasureString(sTitle, Font).ToSize();
            g.FillRectangle(_brushBack, new Rectangle(_spcLeft, 0, szTitle.Width, szTitle.Height));
            g.DrawString(sTitle, Font, _brushFore, _spcLeft, 0);
        }

        /// <summary>
        ///  Invalidates the panels
        /// </summary>
        private void InvalidateAllPanels()
        {
            PnlPrice.Invalidate();

            if (ShowIndicators)
                foreach (Panel pnlind in PnlInd)
                    pnlind.Invalidate();

            if (ShowFloatingPL)
                PnlFloatingPLChart.Invalidate();

            if (ShowBalanceEquity)
                PnlBalanceChart.Invalidate();
        }

        /// <summary>
        /// Sets the width of the info panel
        /// </summary>
        private void SetupDynInfoWidth()
        {
            _asInfoTitle = new string[200];
            _aiInfoType = new int[200];
            _infoRows = 0;

            string sUnit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";

            // Dynamic info titles
            _asInfoTitle[_infoRows++] = Language.T("Bar number");
            _asInfoTitle[_infoRows++] = Language.T("Date");
            _asInfoTitle[_infoRows++] = Language.T("Opening time");
            _asInfoTitle[_infoRows++] = Language.T("Opening price");
            _asInfoTitle[_infoRows++] = Language.T("Highest price");
            _asInfoTitle[_infoRows++] = Language.T("Lowest price");
            _asInfoTitle[_infoRows++] = Language.T("Closing price");
            _asInfoTitle[_infoRows++] = Language.T("Volume");
            _asInfoTitle[_infoRows++] = Language.T("Balance") + sUnit;
            _asInfoTitle[_infoRows++] = Language.T("Equity") + sUnit;
            _asInfoTitle[_infoRows++] = Language.T("Profit Loss") + sUnit;
            _asInfoTitle[_infoRows++] = Language.T("Floating P/L") + sUnit;

            for (int iSlot = 0; iSlot < _backtester.Strategy.Slots; iSlot++)
            {
                int iCompToShow = 0;
                foreach (IndicatorComp indComp in _backtester.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) iCompToShow++;
                if (iCompToShow == 0) continue;

                _aiInfoType[_infoRows] = 1;
                _asInfoTitle[_infoRows++] = _backtester.Strategy.Slot[iSlot].IndicatorName +
                                            (_backtester.Strategy.Slot[iSlot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in _backtester.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) _asInfoTitle[_infoRows++] = indComp.CompName;
            }

            _asInfoTitle[_infoRows++] = "";
            _asInfoTitle[_infoRows++] = Language.T("Position direction");
            _asInfoTitle[_infoRows++] = Language.T("Number of open lots");
            _asInfoTitle[_infoRows++] = Language.T("Type of the transaction");
            _asInfoTitle[_infoRows++] = Language.T("Forming order number");
            _asInfoTitle[_infoRows++] = Language.T("Forming order price");
            _asInfoTitle[_infoRows++] = Language.T("Corrected position price");
            _asInfoTitle[_infoRows++] = Language.T("Profit Loss") + sUnit;
            _asInfoTitle[_infoRows++] = Language.T("Floating P/L") + sUnit;

            Graphics g = CreateGraphics();

            int iMaxLenght = 0;
            foreach (string str in _asInfoTitle)
            {
                var iLenght = (int) g.MeasureString(str, _fontDI).Width;
                if (iMaxLenght < iLenght) iMaxLenght = iLenght;
            }

            _xDynInfoCol2 = iMaxLenght + 10;
            int iMaxInfoWidth = Configs.AccountInMoney
                                    ? (int)
                                      Math.Max(
                                          g.MeasureString(_backtester.MinMoneyEquity.ToString("F2"), _fontDI).Width,
                                          g.MeasureString(_backtester.MaxMoneyEquity.ToString("F2"), _fontDI).Width)
                                    : (int)
                                      Math.Max(
                                          g.MeasureString(_backtester.MinEquity.ToString(CultureInfo.InvariantCulture),
                                                          _fontDI).Width,
                                          g.MeasureString(_backtester.MaxEquity.ToString(CultureInfo.InvariantCulture),
                                                          _fontDI).Width);
            iMaxInfoWidth = (int) Math.Max(g.MeasureString("99/99/99", _fontDI).Width, iMaxInfoWidth);

            foreach (PosDirection posDir in Enum.GetValues(typeof (PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), _fontDI).Width > iMaxInfoWidth)
                    iMaxInfoWidth = (int) g.MeasureString(Language.T(posDir.ToString()), _fontDI).Width;

            foreach (Transaction transaction in Enum.GetValues(typeof (Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), _fontDI).Width > iMaxInfoWidth)
                    iMaxInfoWidth = (int) g.MeasureString(Language.T(transaction.ToString()), _fontDI).Width;

            g.Dispose();

            _dynInfoWidth = _xDynInfoCol2 + iMaxInfoWidth + (_isDebug ? 40 : 5);

            PnlInfo.ClientSize = new Size(_dynInfoWidth, PnlInfo.ClientSize.Height);
            _isDrawDinInfo = false;
        }

        /// <summary>
        /// Sets the dynamic info panel
        /// </summary>
        private void SetupDynamicInfo()
        {
            _asInfoTitle = new string[200];
            _aiInfoType = new int[200];
            _infoRows = 0;

            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";

            // Dynamic info titles
            _asInfoTitle[_infoRows++] = Language.T("Bar number");
            _asInfoTitle[_infoRows++] = Language.T("Date");
            _asInfoTitle[_infoRows++] = Language.T("Opening time");
            _asInfoTitle[_infoRows++] = Language.T("Opening price");
            _asInfoTitle[_infoRows++] = Language.T("Highest price");
            _asInfoTitle[_infoRows++] = Language.T("Lowest price");
            _asInfoTitle[_infoRows++] = Language.T("Closing price");
            _asInfoTitle[_infoRows++] = Language.T("Volume");
            _asInfoTitle[_infoRows++] = "";
            _asInfoTitle[_infoRows++] = Language.T("Balance") + unit;
            _asInfoTitle[_infoRows++] = Language.T("Equity") + unit;
            _asInfoTitle[_infoRows++] = Language.T("Profit Loss") + unit;
            _asInfoTitle[_infoRows++] = Language.T("Floating P/L") + unit;

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                int compToShow = 0;
                foreach (IndicatorComp indComp in _backtester.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) compToShow++;
                if (compToShow == 0) continue;

                _asInfoTitle[_infoRows++] = "";
                _aiInfoType[_infoRows] = 1;
                _asInfoTitle[_infoRows++] = _backtester.Strategy.Slot[slot].IndicatorName +
                                            (_backtester.Strategy.Slot[slot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in _backtester.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) _asInfoTitle[_infoRows++] = indComp.CompName;
            }

            for (int pos = 0; pos < _posCount; pos++)
            {
                _asInfoTitle[_infoRows++] = "";
                _asInfoTitle[_infoRows++] = Language.T("Position direction");
                _asInfoTitle[_infoRows++] = Language.T("Number of open lots");
                _asInfoTitle[_infoRows++] = Language.T("Type of the transaction");
                _asInfoTitle[_infoRows++] = Language.T("Forming order number");
                _asInfoTitle[_infoRows++] = Language.T("Forming order price");
                _asInfoTitle[_infoRows++] = Language.T("Corrected position price");
                _asInfoTitle[_infoRows++] = Language.T("Profit Loss") + unit;
                _asInfoTitle[_infoRows++] = Language.T("Floating P/L") + unit;
            }

            _isDrawDinInfo = false;
        }

        /// <summary>
        /// Generates the DynamicInfo.
        /// </summary>
        private void GenerateDynamicInfo(int barNumb)
        {
            if (!ShowDynInfo || !ShowInfoPanel) return;

            barNumb = Math.Max(0, barNumb);
            barNumb = Math.Min(_chartBars - 1, barNumb);

            int bar = _firstBar + barNumb;
            bar = Math.Min(_backtester.DataSet.Bars - 1, bar);

            if (_barOld == bar) return;
            _barOld = bar;

            int row = 0;
            _asInfoValue = new String[200];
            _asInfoValue[row++] = (bar + 1).ToString(CultureInfo.InvariantCulture);
            _asInfoValue[row++] = _backtester.DataSet.Time[bar].ToString(Data.DF);
            _asInfoValue[row++] = _backtester.DataSet.Time[bar].ToString("HH:mm");
            if (_isDebug)
            {
                _asInfoValue[row++] = _backtester.DataSet.Open[bar].ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _backtester.DataSet.High[bar].ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _backtester.DataSet.Low[bar].ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _backtester.DataSet.Close[bar].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                string ff = _backtester.DataSet.FF;
                _asInfoValue[row++] = _backtester.DataSet.Open[bar].ToString(ff);
                _asInfoValue[row++] = _backtester.DataSet.High[bar].ToString(ff);
                _asInfoValue[row++] = _backtester.DataSet.Low[bar].ToString(ff);
                _asInfoValue[row++] = _backtester.DataSet.Close[bar].ToString(ff);
            }
            _asInfoValue[row++] = _backtester.DataSet.Volume[bar].ToString(CultureInfo.InvariantCulture);

            _asInfoValue[row++] = "";
            if (Configs.AccountInMoney)
            {
                // Balance
                _asInfoValue[row++] = _backtester.MoneyBalance(bar).ToString("F2");

                // Equity
                _asInfoValue[row++] = _backtester.MoneyEquity(bar).ToString("F2");

                // Profit Loss
                if (_backtester.SummaryTrans(bar) == Transaction.Close ||
                    _backtester.SummaryTrans(bar) == Transaction.Reduce ||
                    _backtester.SummaryTrans(bar) == Transaction.Reverse)
                    _asInfoValue[row++] = _backtester.MoneyProfitLoss(bar).ToString("F2");
                else
                    _asInfoValue[row++] = "   -";

                // Floating P/L
                if (_backtester.Positions(bar) > 0 && _backtester.SummaryTrans(bar) != Transaction.Close)
                    _asInfoValue[row++] = _backtester.MoneyFloatingPL(bar).ToString("F2");
                else
                    _asInfoValue[row++] = "   -";
            }
            else
            {
                // Balance
                _asInfoValue[row++] = _backtester.Balance(bar).ToString(CultureInfo.InvariantCulture);

                // Equity
                _asInfoValue[row++] = _backtester.Equity(bar).ToString(CultureInfo.InvariantCulture);

                // Profit Loss
                if (_backtester.SummaryTrans(bar) == Transaction.Close ||
                    _backtester.SummaryTrans(bar) == Transaction.Reduce ||
                    _backtester.SummaryTrans(bar) == Transaction.Reverse)
                    _asInfoValue[row++] = _backtester.ProfitLoss(bar).ToString(CultureInfo.InvariantCulture);
                else
                    _asInfoValue[row++] = "   -";

                // Profit Loss
                if (_backtester.Positions(bar) > 0 && _backtester.SummaryTrans(bar) != Transaction.Close)
                    _asInfoValue[row++] = _backtester.FloatingPL(bar).ToString(CultureInfo.InvariantCulture);
                else
                    _asInfoValue[row++] = "   -";
            }

            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                if (_backtester.Strategy.Slot[slot] != null)
                {
                    int compToShow = 0;
                    foreach (IndicatorComp indComp in _backtester.Strategy.Slot[slot].Component)
                        if (indComp.ShowInDynInfo) compToShow++;
                    if (compToShow == 0) continue;

                    _asInfoValue[row++] = "";
                    _asInfoValue[row++] = "";
                    foreach (IndicatorComp indComp in _backtester.Strategy.Slot[slot].Component)
                    {
                        if (indComp.ShowInDynInfo)
                        {
                            IndComponentType indDataTipe = indComp.DataType;
                            if (indDataTipe == IndComponentType.AllowOpenLong ||
                                indDataTipe == IndComponentType.AllowOpenShort ||
                                indDataTipe == IndComponentType.ForceClose ||
                                indDataTipe == IndComponentType.ForceCloseLong ||
                                indDataTipe == IndComponentType.ForceCloseShort)
                                _asInfoValue[row++] = (indComp.Value[bar] < 1 ? Language.T("No") : Language.T("Yes"));
                            else
                            {
                                if (_isDebug)
                                {
                                    _asInfoValue[row++] = indComp.Value[bar].ToString(CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    double dl = Math.Abs(indComp.Value[bar]);
                                    string sFR = dl < 10
                                                     ? "F5"
                                                     : dl < 100
                                                           ? "F4"
                                                           : dl < 1000
                                                                 ? "F3"
                                                                 : dl < 10000 ? "F2" : dl < 100000 ? "F1" : "F0";
                                    if (Math.Abs(indComp.Value[bar] - 0) > 0.000001)
                                        _asInfoValue[row++] = indComp.Value[bar].ToString(sFR);
                                    else
                                        _asInfoValue[row++] = "   -";
                                }
                            }
                        }
                    }
                }
            }

            // Positions
            int pos;
            for (pos = 0; pos < _backtester.Positions(bar); pos++)
            {
                _asInfoValue[row++] = "";
                _asInfoValue[row++] = Language.T(_backtester.PosDir(bar, pos).ToString());
                _asInfoValue[row++] = _backtester.PosLots(bar, pos).ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = Language.T(_backtester.PosTransaction(bar, pos).ToString());
                _asInfoValue[row++] = _backtester.PosOrdNumb(bar, pos).ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _backtester.PosOrdPrice(bar, pos).ToString(_backtester.DataSet.FF);
                _asInfoValue[row++] = _backtester.PosPrice(bar, pos).ToString(_backtester.DataSet.FF);

                // Profit Loss
                if (_backtester.PosTransaction(bar, pos) == Transaction.Close ||
                    _backtester.PosTransaction(bar, pos) == Transaction.Reduce ||
                    _backtester.PosTransaction(bar, pos) == Transaction.Reverse)
                    _asInfoValue[row++] = Configs.AccountInMoney
                                              ? _backtester.PosMoneyProfitLoss(bar, pos).ToString("F2")
                                              : Math.Round(_backtester.PosProfitLoss(bar, pos)).ToString(
                                                  CultureInfo.InvariantCulture);
                else
                    _asInfoValue[row++] = "   -";

                // Floating P/L
                if (pos == _backtester.Positions(bar) - 1 && _backtester.PosTransaction(bar, pos) != Transaction.Close)
                    _asInfoValue[row++] = Configs.AccountInMoney
                                              ? _backtester.PosMoneyFloatingPL(bar, pos).ToString("F2")
                                              : Math.Round(_backtester.PosFloatingPL(bar, pos)).ToString(
                                                  CultureInfo.InvariantCulture);
                else
                    _asInfoValue[row++] = "   -";
            }

            if (_posCount != pos)
            {
                _posCount = pos;
                SetupDynamicInfo();
                _isDrawDinInfo = true;
                PnlInfo.Invalidate();
            }
            else
            {
                PnlInfo.Invalidate(new Rectangle(_xDynInfoCol2, 0, _dynInfoWidth - _xDynInfoCol2,
                                                 PnlInfo.ClientSize.Height));
            }
        }

        /// <summary>
        /// PnlInfo Resize
        /// </summary>
        private void PnlInfoResize(object sender, EventArgs e)
        {
            PnlInfo.Invalidate();
        }

        /// <summary>
        /// Paints the panel PnlInfo.
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            if (!ShowInfoPanel) return;

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (_isDrawDinInfo && ShowDynInfo)
            {
                int rowHeight = _fontDI.Height + 1;
                var size = new Size(_dynInfoWidth, rowHeight);

                for (int i = 0; i < _infoRows; i++)
                {
                    if (Math.Abs(i%2f - 0) > 0.00001)
                        g.FillRectangle(_brushEvenRows, new Rectangle(new Point(0, i*rowHeight + 1), size));

                    if (_aiInfoType[i + _dynInfoScrollValue] == 1)
                        g.DrawString(_asInfoTitle[i + _dynInfoScrollValue], _fontDIInd, _brushDiIndicator, 5,
                                     i*rowHeight - 1);
                    else
                        g.DrawString(_asInfoTitle[i + _dynInfoScrollValue], _fontDI, _brushDynamicInfo, 5,
                                     i*rowHeight + 1);

                    g.DrawString(_asInfoValue[i + _dynInfoScrollValue], _fontDI, _brushDynamicInfo, _xDynInfoCol2,
                                 i*rowHeight + 1);
                }
            }
        }

        /// <summary>
        /// Invalidate Cross Old/New position and Dynamic Info
        /// </summary>
        private void PnlPriceMouseMove(object sender, MouseEventArgs e)
        {
            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = e.X;
            _mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (_mouseX > _xRight)
                {
                    if (_mouseY > _mouseYOld)
                        VerticalScaleDecrease();
                    else
                        VerticalScaleIncrease();
                    return;
                }
                int newScrollValue = _scroll.Value;

                if (_mouseX > _mouseXOld)
                    newScrollValue -= (int) Math.Round(_scroll.SmallChange*0.1*(100 - BarPixels));
                else if (_mouseX < _mouseXOld)
                    newScrollValue += (int) Math.Round(_scroll.SmallChange*0.1*(100 - BarPixels));

                if (newScrollValue < _scroll.Minimum)
                    newScrollValue = _scroll.Minimum;
                else if (newScrollValue > _scroll.Maximum + 1 - _scroll.LargeChange)
                    newScrollValue = _scroll.Maximum + 1 - _scroll.LargeChange;

                _scroll.Value = newScrollValue;
            }

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Adding the old positions
                if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
                {
                    if (_mouseYOld >= _yTop && _mouseYOld <= _yBottom)
                    {
                        // Horizontal Line
                        path.AddRectangle(new Rectangle(0, _mouseYOld, PnlPrice.ClientSize.Width, 1));
                        // PriceBox
                        path.AddRectangle(new Rectangle(_xRight - 1, _mouseYOld - _font.Height/2 - 1, _szPrice.Width + 2,
                                                        _font.Height + 2));
                    }
                    // Vertical Line
                    path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1,
                                                    _szDateL.Width + 2,
                                                    _font.Height + 3));
                }

                // Adding the new positions
                if (_mouseX >= _xLeft && _mouseX <= _xRight)
                {
                    if (_mouseYOld >= _yTop && _mouseYOld <= _yBottom)
                    {
                        // Horizontal Line
                        path.AddRectangle(new Rectangle(0, _mouseY, PnlPrice.ClientSize.Width, 1));
                        // PriceBox
                        path.AddRectangle(new Rectangle(_xRight - 1, _mouseY - _font.Height/2 - 1, _szPrice.Width + 2,
                                                        _font.Height + 2));
                    }
                    // Vertical Line
                    path.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(_mouseX - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                    _font.Height + 3));
                }
                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < _indPanels && ShowIndicators; i++)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                    if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                    PnlInd[i].Invalidate(new Region(path1));
                }

                if (ShowBalanceEquity)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));
                    if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlBalanceChart.ClientSize.Height));
                    PnlBalanceChart.Invalidate(new Region(path1));
                }

                if (ShowFloatingPL)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    PnlFloatingPLChart.Invalidate(new Region(path1));
                }
            }

            // Determines the shown bar.
            if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
            {
                if (_mouseX >= _xLeft && _mouseX <= _xRight)
                {
                    // Moving inside the chart
                    _isMouseInPriceChart = true;
                    _isDrawDinInfo = true;
                    GenerateDynamicInfo((e.X - _xLeft)/BarPixels);
                }
                else
                {
                    // Escaping from the bar area of chart
                    _isMouseInPriceChart = false;
                    PnlPrice.Cursor = Cursors.Default;
                }
            }
            else if (_mouseX >= _xLeft && _mouseX <= _xRight)
            {
                // Entering into the chart
                if (ShowCross)
                    PnlPrice.Cursor = Cursors.Cross;
                _isMouseInPriceChart = true;
                _isDrawDinInfo = true;
                PnlInfo.Invalidate();
                GenerateDynamicInfo((e.X - _xLeft)/BarPixels);
            }
        }

        /// <summary>
        /// Deletes the cross and Dynamic Info
        /// </summary>
        private void PnlPriceMouseLeave(object sender, EventArgs e)
        {
            PnlPrice.Cursor = Cursors.Default;
            _isMouseInPriceChart = false;

            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = -1;
            _mouseY = -1;
            _barOld = -1;

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Horizontal Line
                path.AddRectangle(new Rectangle(0, _mouseYOld, PnlPrice.ClientSize.Width, 1));
                // PriceBox
                path.AddRectangle(new Rectangle(_xRight - 1, _mouseYOld - _font.Height/2 - 1, _szPrice.Width + 2,
                                                _font.Height + 2));
                // Vertical Line
                path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                _font.Height + 3));

                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < _indPanels && ShowIndicators; i++)
                    PnlInd[i].Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));

                if (ShowBalanceEquity)
                    PnlBalanceChart.Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));

                if (ShowFloatingPL)
                    PnlFloatingPLChart.Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
            }
        }

        /// <summary>
        /// Mouse moves inside a chart
        /// </summary>
        private void SepChartMouseMove(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;

            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = e.X;
            _mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                int newScrollValue = _scroll.Value;

                if (_mouseX > _mouseXOld)
                    newScrollValue -= (int) Math.Round(_scroll.SmallChange*0.1*(100 - BarPixels));
                else if (_mouseX < _mouseXOld)
                    newScrollValue += (int) Math.Round(_scroll.SmallChange*0.1*(100 - BarPixels));

                if (newScrollValue < _scroll.Minimum)
                    newScrollValue = _scroll.Minimum;
                else if (newScrollValue > _scroll.Maximum + 1 - _scroll.LargeChange)
                    newScrollValue = _scroll.Maximum + 1 - _scroll.LargeChange;

                _scroll.Value = newScrollValue;
            }

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Adding the old positions
                if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
                {
                    // Vertical Line
                    path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1,
                                                    _szDateL.Width + 2,
                                                    _font.Height + 3));
                }

                // Adding the new positions
                if (_mouseX >= _xLeft && _mouseX <= _xRight)
                {
                    // Vertical Line
                    path.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(_mouseX - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                    _font.Height + 3));
                }
                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < _indPanels && ShowIndicators; i++)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                    if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                    PnlInd[i].Invalidate(new Region(path1));
                }

                if (ShowBalanceEquity)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));
                    if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlBalanceChart.ClientSize.Height));
                    PnlBalanceChart.Invalidate(new Region(path1));
                }

                if (ShowFloatingPL)
                {
                    var path1 = new GraphicsPath(FillMode.Winding);
                    if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                        path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlFloatingPLChart.ClientSize.Height));
                    PnlFloatingPLChart.Invalidate(new Region(path1));
                }
            }

            // Determines the shown bar.
            if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
            {
                if (_mouseX >= _xLeft && _mouseX <= _xRight)
                {
                    // Moving inside the chart
                    _isMouseInIndicatorChart = true;
                    _isDrawDinInfo = true;
                    GenerateDynamicInfo((e.X - _xLeft)/BarPixels);
                }
                else
                {
                    // Escaping from the bar area of chart
                    _isMouseInIndicatorChart = false;
                    panel.Cursor = Cursors.Default;
                }
            }
            else if (_mouseX >= _xLeft && _mouseX <= _xRight)
            {
                // Entering into the chart
                if (ShowCross)
                    panel.Cursor = Cursors.Cross;
                _isMouseInIndicatorChart = true;
                _isDrawDinInfo = true;
                PnlInfo.Invalidate();
                GenerateDynamicInfo((e.X - _xLeft)/BarPixels);
            }
        }

        /// <summary>
        /// Mouse leaves a chart.
        /// </summary>
        private void SepChartMouseLeave(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = Cursors.Default;

            _isMouseInIndicatorChart = false;

            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = -1;
            _mouseY = -1;
            _barOld = -1;

            if (ShowCross)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Vertical Line
                path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                _font.Height + 3));

                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < _indPanels && ShowIndicators; i++)
                    PnlInd[i].Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));

                if (ShowBalanceEquity)
                    PnlBalanceChart.Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlBalanceChart.ClientSize.Height));

                if (ShowFloatingPL)
                    PnlFloatingPLChart.Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlFloatingPLChart.ClientSize.Height));
            }
        }

        /// <summary>
        /// Sets the parameters when scrolling.
        /// </summary>
        private void ScrollValueChanged(object sender, EventArgs e)
        {
            _firstBar = _scroll.Value;
            _lastBar = Math.Min(_backtester.DataSet.Bars - 1, _firstBar + _chartBars - 1);
            _lastBar = Math.Max(_lastBar, _firstBar);

            InvalidateAllPanels();

            // Dynamic Info
            _barOld = 0;
            // Sends the shown bar from the chart beginning
            if (_isDrawDinInfo && ShowDynInfo)
            {
                int selectedBar = (_mouseX - _spcLeft)/BarPixels;
                GenerateDynamicInfo(selectedBar);
            }
        }

        /// <summary>
        /// Scrolls the scrollbar when turning the mouse wheel.
        /// </summary>
        private void ScrollMouseWheel(object sender, MouseEventArgs e)
        {
            if (_isKeyCtrlPressed)
            {
                if (e.Delta > 0)
                    ZoomIn();
                if (e.Delta < 0)
                    ZoomOut();
            }
            else
            {
                int newScrollValue = _scroll.Value +
                                     _scroll.LargeChange*e.Delta/SystemInformation.MouseWheelScrollLines/120;

                if (newScrollValue < _scroll.Minimum)
                    newScrollValue = _scroll.Minimum;
                else if (newScrollValue > _scroll.Maximum + 1 - _scroll.LargeChange)
                    newScrollValue = _scroll.Maximum + 1 - _scroll.LargeChange;

                _scroll.Value = newScrollValue;
            }
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        private void ScrollKeyUp(object sender, KeyEventArgs e)
        {
            _isKeyCtrlPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        private void ScrollKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
                _isKeyCtrlPressed = true;
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            _isKeyCtrlPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        /// Mouse button down on a panel.
        /// </summary>
        private void PanelMouseDown(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            if (panel == PnlPrice && _mouseX > _xRight)
                panel.Cursor = Cursors.SizeNS;
            else
                panel.Cursor = Cursors.SizeWE;
        }

        /// <summary>
        /// Mouse button up on a panel.
        /// </summary>
        private void PanelMouseUp(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = ShowCross ? Cursors.Cross : Cursors.Default;
        }

        /// <summary>
        /// Shows the Bar Explorer
        /// </summary>
        private void PnlPriceMouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Determines the shown bar.
            if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight && _mouseYOld >= _yTop && _mouseYOld <= _yBottom)
            {
                // Moving inside the chart
                if (_mouseX >= _xLeft && _mouseX <= _xRight && _mouseY >= _yTop && _mouseY <= _yBottom)
                {
                    int selectedBar = (e.X - _xLeft)/BarPixels + _firstBar;
                    var be = new BarExplorer(_backtester, selectedBar);
                    be.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Changes chart's settings after a button click.
        /// </summary>
        private void ButtonChartClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;
            var button = (ChartButtons) btn.Tag;

            switch (button)
            {
                case ChartButtons.Grid:
                    ShortcutKeyUp(new KeyEventArgs(Keys.G));
                    break;
                case ChartButtons.Cross:
                    ShortcutKeyUp(new KeyEventArgs(Keys.C));
                    break;
                case ChartButtons.Volume:
                    ShortcutKeyUp(new KeyEventArgs(Keys.V));
                    break;
                case ChartButtons.Orders:
                    ShortcutKeyUp(new KeyEventArgs(Keys.O));
                    break;
                case ChartButtons.PositionLots:
                    ShortcutKeyUp(new KeyEventArgs(Keys.L));
                    break;
                case ChartButtons.PositionPrice:
                    ShortcutKeyUp(new KeyEventArgs(Keys.P));
                    break;
                case ChartButtons.Protections:
                    ShortcutKeyUp(new KeyEventArgs(Keys.E));
                    break;
                case ChartButtons.AmbiguousBars:
                    ShortcutKeyUp(new KeyEventArgs(Keys.M));
                    break;
                case ChartButtons.Indicators:
                    ShortcutKeyUp(new KeyEventArgs(Keys.D));
                    break;
                case ChartButtons.BalanceEquity:
                    ShortcutKeyUp(new KeyEventArgs(Keys.B));
                    break;
                case ChartButtons.FloatingPL:
                    ShortcutKeyUp(new KeyEventArgs(Keys.F));
                    break;
                case ChartButtons.ZoomIn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Add));
                    break;
                case ChartButtons.ZoomOut:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Subtract));
                    break;
                case ChartButtons.TrueCharts:
                    ShortcutKeyUp(new KeyEventArgs(Keys.T));
                    break;
                case ChartButtons.DynamicInfo:
                    ShortcutKeyUp(new KeyEventArgs(Keys.I));
                    break;
                case ChartButtons.DInfoDwn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Z));
                    break;
                case ChartButtons.DInfoUp:
                    ShortcutKeyUp(new KeyEventArgs(Keys.A));
                    break;
            }
        }

        /// <summary>
        /// Shortcut keys
        /// </summary>
        private void ShortcutKeyUp(KeyEventArgs e)
        {
            // Zoom in
            if (!e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                ZoomIn();
            }
                // Zoom out
            else if (!e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                ZoomOut();
            }
                // Vertical scale increase
            else if (e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                VerticalScaleIncrease();
            }
                // Vertical scale decrease
            else if (e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                VerticalScaleDecrease();
            }
            else if (e.KeyCode == Keys.Space)
            {
                _isCandleChart = !_isCandleChart;
                PnlPrice.Invalidate();
            }
                // Dynamic info scroll up
            else if (e.KeyCode == Keys.A)
            {
                if (!ShowInfoPanel)
                    return;
                _dynInfoScrollValue -= 5;
                _dynInfoScrollValue = _dynInfoScrollValue < 0 ? 0 : _dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Dynamic info scroll up fast
            else if (e.KeyCode == Keys.S)
            {
                if (!ShowInfoPanel)
                    return;
                _dynInfoScrollValue -= 10;
                _dynInfoScrollValue = _dynInfoScrollValue < 0 ? 0 : _dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Dynamic info scroll down
            else if (e.KeyCode == Keys.Z)
            {
                if (!ShowInfoPanel)
                    return;
                _dynInfoScrollValue += 5;
                _dynInfoScrollValue = _dynInfoScrollValue > _infoRows - 5 ? _infoRows - 5 : _dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Dynamic info scroll down fast
            else if (e.KeyCode == Keys.X)
            {
                if (!ShowInfoPanel)
                    return;
                _dynInfoScrollValue += 10;
                _dynInfoScrollValue = _dynInfoScrollValue > _infoRows - 5 ? _infoRows - 5 : _dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Grid
            else if (e.KeyCode == Keys.G)
            {
                ShowGrid = !ShowGrid;
                AChartButtons[(int) ChartButtons.Grid].Checked = ShowGrid;
                InvalidateAllPanels();
            }
                // Cross
            else if (e.KeyCode == Keys.C)
            {
                ShowCross = !ShowCross;
                AChartButtons[(int) ChartButtons.Cross].Checked = ShowCross;
                PnlPrice.Cursor = ShowCross ? Cursors.Cross : Cursors.Default;
                InvalidateAllPanels();
            }
                // Volume
            else if (e.KeyCode == Keys.V)
            {
                ShowVolume = !ShowVolume;
                AChartButtons[(int) ChartButtons.Volume].Checked = ShowVolume;
                PnlPrice.Invalidate();
            }
                // Lots
            else if (e.KeyCode == Keys.L)
            {
                ShowPositionLots = !ShowPositionLots;
                AChartButtons[(int) ChartButtons.PositionLots].Checked = ShowPositionLots;
                PnlPrice.Invalidate();
            }
                // Orders
            else if (e.KeyCode == Keys.O)
            {
                ShowOrders = !ShowOrders;
                AChartButtons[(int) ChartButtons.Orders].Checked = ShowOrders;
                PnlPrice.Invalidate();
            }
                // Position price
            else if (e.KeyCode == Keys.P)
            {
                ShowPositionPrice = !ShowPositionPrice;
                AChartButtons[(int) ChartButtons.PositionPrice].Checked = ShowPositionPrice;
                PnlPrice.Invalidate();
            }
                // Protections
            else if (e.KeyCode == Keys.E)
            {
                ShowProtections = !ShowProtections;
                AChartButtons[(int) ChartButtons.Protections].Checked = ShowProtections;
                PnlPrice.Invalidate();
            }
                // Ambiguous bars mark
            else if (e.KeyCode == Keys.M)
            {
                ShowAmbiguousBars = !ShowAmbiguousBars;
                AChartButtons[(int) ChartButtons.AmbiguousBars].Checked = ShowAmbiguousBars;
                PnlPrice.Invalidate();
            }
                // True Charts
            else if (e.KeyCode == Keys.T)
            {
                TrueCharts = !TrueCharts;
                AChartButtons[(int) ChartButtons.TrueCharts].Checked = TrueCharts;
                InvalidateAllPanels();
            }
                // Indicator Charts
            else if (e.KeyCode == Keys.D)
            {
                ShowIndicators = !ShowIndicators;
                AChartButtons[(int) ChartButtons.Indicators].Checked = ShowIndicators;
                if (ShowIndicators)
                {
                    _scroll.Parent = null;
                    for (int i = 0; i < _indPanels; i++)
                    {
                        SplitterInd[i].Parent = PnlCharts;
                        PnlInd[i].Parent = PnlCharts;
                    }
                    _scroll.Parent = PnlCharts;
                }
                else
                {
                    for (int i = 0; i < _indPanels; i++)
                    {
                        PnlInd[i].Parent = null;
                        SplitterInd[i].Parent = null;
                    }
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                _scroll.Focus();
            }
                // FloatingPL Charts
            else if (e.KeyCode == Keys.F)
            {
                ShowFloatingPL = !ShowFloatingPL;
                AChartButtons[(int) ChartButtons.FloatingPL].Checked = ShowFloatingPL;
                if (ShowFloatingPL)
                {
                    _scroll.Parent = null;
                    SpliterFloatingPL.Parent = PnlCharts;
                    PnlFloatingPLChart.Parent = PnlCharts;
                    _scroll.Parent = PnlCharts;
                }
                else
                {
                    SpliterFloatingPL.Parent = null;
                    PnlFloatingPLChart.Parent = null;
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                _scroll.Focus();
            }
                // Balance/Equity Charts
            else if (e.KeyCode == Keys.B)
            {
                ShowBalanceEquity = !ShowBalanceEquity;
                AChartButtons[(int) ChartButtons.BalanceEquity].Checked = ShowBalanceEquity;
                if (ShowBalanceEquity)
                {
                    _scroll.Parent = null;
                    SpliterBalance.Parent = PnlCharts;
                    PnlBalanceChart.Parent = PnlCharts;
                    _scroll.Parent = PnlCharts;
                }
                else
                {
                    SpliterBalance.Parent = null;
                    PnlBalanceChart.Parent = null;
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                _scroll.Focus();
            }
                // Show info panel
            else if (e.KeyCode == Keys.I)
            {
                ShowInfoPanel = !ShowInfoPanel;
                PnlInfo.Visible = ShowInfoPanel;
                PnlCharts.Padding = ShowInfoPanel ? new Padding(0, 0, 2, 0) : new Padding(0);
                Text = ShowInfoPanel
                           ? Language.T("Chart") + " " + _backtester.DataSet.Symbol + " " + _backtester.DataSet.PeriodString
                           : Data.ProgramName + @"   http://forexsb.com";
                AChartButtons[(int) ChartButtons.DynamicInfo].Checked = ShowInfoPanel;
                AChartButtons[(int) ChartButtons.DInfoUp].Visible = ShowInfoPanel;
                AChartButtons[(int) ChartButtons.DInfoDwn].Visible = ShowInfoPanel;
                if (ShowInfoPanel)
                {
                    GenerateDynamicInfo(_lastBar);
                    SetupDynamicInfo();
                    _isDrawDinInfo = true;
                    PnlInfo.Invalidate();
                }
            }
                // Debug
            else if (e.KeyCode == Keys.F12)
            {
                _isDebug = !_isDebug;
                SetupDynInfoWidth();
                SetupDynamicInfo();
                PnlInfo.Invalidate();
            }
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleDecrease()
        {
            if (_verticalScale <= 10) return;
            _verticalScale -= 10;
            PnlPrice.Invalidate();
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleIncrease()
        {
            if (_verticalScale >= 300) return;
            _verticalScale += 10;
            PnlPrice.Invalidate();
        }

        /// <summary>
        /// Zooms the chart in.
        /// </summary>
        private void ZoomIn()
        {
            BarPixels += 4;
            if (BarPixels == 6)
                BarPixels = 4;
            if (BarPixels > 40)
                BarPixels = 40;

            int oldChartBars = _chartBars;

            _chartBars = _chartWidth/BarPixels;
            if (_chartBars > _backtester.DataSet.Bars - _backtester.Strategy.FirstBar)
                _chartBars = _backtester.DataSet.Bars - _backtester.Strategy.FirstBar;

            if (_lastBar < _backtester.DataSet.Bars - 1)
            {
                _firstBar += (oldChartBars - _chartBars)/2;
                if (_firstBar > _backtester.DataSet.Bars - _chartBars)
                    _firstBar = _backtester.DataSet.Bars - _chartBars;
            }
            else
            {
                _firstBar = Math.Max(_backtester.Strategy.FirstBar, _backtester.DataSet.Bars - _chartBars);
            }

            _lastBar = _firstBar + _chartBars - 1;

            _scroll.Value = _firstBar;
            _scroll.LargeChange = _chartBars;

            InvalidateAllPanels();
        }

        /// <summary>
        /// Zooms the chart out.
        /// </summary>
        private void ZoomOut()
        {
            BarPixels -= 4;
            if (BarPixels < 4)
                BarPixels = 2;

            int oldChartBars = _chartBars;

            _chartBars = _chartWidth/BarPixels;
            if (_chartBars > _backtester.DataSet.Bars - _backtester.Strategy.FirstBar)
                _chartBars = _backtester.DataSet.Bars - _backtester.Strategy.FirstBar;

            if (_lastBar < _backtester.DataSet.Bars - 1)
            {
                _firstBar -= (_chartBars - oldChartBars)/2;
                if (_firstBar < _backtester.Strategy.FirstBar)
                    _firstBar = _backtester.Strategy.FirstBar;

                if (_firstBar > _backtester.DataSet.Bars - _chartBars)
                    _firstBar = _backtester.DataSet.Bars - _chartBars;
            }
            else
            {
                _firstBar = Math.Max(_backtester.Strategy.FirstBar, _backtester.DataSet.Bars - _chartBars);
            }

            _lastBar = _firstBar + _chartBars - 1;

            _scroll.Value = _firstBar;
            _scroll.LargeChange = _chartBars;

            InvalidateAllPanels();
        }
    }
}