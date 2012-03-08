// Strategy Analyzer - OverOptimization Charts
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

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    internal class OverOptimizationCharts : Panel
    {
        private const int Space = 5;
        private const int Border = 2;
        private PointF[][] _apntParameters;
        private Brush _brushFore;
        private float _captionHeight;
        private string _chartTitle;
        private int _countLabels;
        private float _delta;
        private int _devSteps;
        private Font _font;
        private int _labelWidth;
        private int _maximum;
        private int _minimum;
        private int _parameters;
        private Pen _penBorder;
        private Pen _penGrid;
        private int _percentDev;
        private RectangleF _rectfCaption;
        private int _step;
        private StringFormat _stringFormatCaption;
        private int _xLeft;
        private int _xMiddle;
        private int _xRight;
        private float _xScale;
        private int _yBottom;
        private float _yScale;
        private int _yTop;
        private OverOptimizationDataTable Table { get; set; }

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        public void InitChart(OverOptimizationDataTable table)
        {
            Table = table;

            // Chart Title
            _chartTitle = table.Name;
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
            _penGrid = new Pen(LayoutColors.ColorChartGrid)
                           {DashStyle = DashStyle.Dash, DashPattern = new float[] {4, 2}};
            _penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                 Border);

            _devSteps = table.CountDeviationSteps;
            _percentDev = (_devSteps - 1)/2;
            _parameters = table.CountStrategyParams;

            if (_parameters == 0)
                return;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            for (int param = 0; param < _parameters; param++)
            {
                double min = double.MaxValue;
                double max = double.MinValue;
                for (int dev = 0; dev < _devSteps; dev++)
                {
                    int index = _percentDev - dev;
                    double value = table.GetData(index, param);
                    if (min > value) min = value;
                    if (max < value) max = value;
                }
                if (minValue > min) minValue = min;
                if (maxValue < max) maxValue = max;
            }

            _maximum = (int) Math.Round(maxValue + 0.1*Math.Abs(maxValue));
            _minimum = (int) Math.Round(minValue - 0.1*Math.Abs(minValue));
            int roundStep = Math.Abs(_minimum) > 1 ? 10 : 1;
            _minimum = (int) (Math.Floor(_minimum/(float) roundStep)*roundStep);
            if (Math.Abs(_maximum - _minimum) < 1) _maximum = _minimum + 1;

            _yTop = (int) _captionHeight + 2*Space + 1;
            _yBottom = ClientSize.Height - 2*Space - 1 - Border - Space - Font.Height;

            Graphics g = CreateGraphics();
            _labelWidth =
                (int)
                Math.Max(g.MeasureString(_minimum.ToString(CultureInfo.InvariantCulture), _font).Width,
                         g.MeasureString(_maximum.ToString(CultureInfo.InvariantCulture), _font).Width);
            g.Dispose();

            _labelWidth = Math.Max(_labelWidth, 30);
            _xLeft = Border + 3*Space;
            _xRight = ClientSize.Width - Border - 2*Space - _labelWidth;
            _xScale = (_xRight - _xLeft)/(float) (_devSteps - 1);
            _xMiddle = (int) (_xLeft + _percentDev*_xScale);

            _countLabels = Math.Max((_yBottom - _yTop)/20, 2);
            _delta = (float) Math.Max(Math.Round((_maximum - _minimum)/(float) _countLabels), roundStep);
            _step = (int) Math.Ceiling(_delta/roundStep)*roundStep;
            _countLabels = (int) Math.Ceiling((_maximum - _minimum)/(float) _step);
            _maximum = _minimum + _countLabels*_step;
            _yScale = (_yBottom - _yTop)/(_countLabels*(float) _step);

            _apntParameters = new PointF[_parameters][];

            for (int param = 0; param < _parameters; param++)
            {
                _apntParameters[param] = new PointF[_devSteps];
                for (int dev = 0; dev < _devSteps; dev++)
                {
                    int index = _percentDev - dev;
                    _apntParameters[param][dev].X = _xLeft + dev*_xScale;
                    _apntParameters[param][dev].Y =
                        (float) (_yBottom - (table.GetData(index, param) - _minimum)*_yScale);
                }
            }
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption bar.
            ColorMagic.GradientPaint(g, _rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), _rectfCaption,
                         _stringFormatCaption);

            // Border.
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paints the background by gradient.
            var rectField = new RectangleF(Border, _captionHeight, ClientSize.Width - 2*Border,
                                           ClientSize.Height - _captionHeight - Border);
            ColorMagic.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (Table == null || _parameters == 0)
                return;

            // Grid and value labels.
            for (int label = _minimum; label <= _maximum; label += _step)
            {
                var iLabelY = (int) (_yBottom - (label - _minimum)*_yScale);
                g.DrawString(label.ToString(CultureInfo.InvariantCulture), Font, _brushFore, _xRight + Space,
                             iLabelY - Font.Height/2 - 1);
                g.DrawLine(_penGrid, _xLeft, iLabelY, _xRight, iLabelY);
            }

            // Deviation labels.
            var strFormatDevLabel = new StringFormat {Alignment = StringAlignment.Center};
            int xLabelRight = 0;
            for (int dev = 0; dev < _devSteps; dev++)
            {
                int index = _percentDev - dev;
                var xVertLine = (int) (_xLeft + dev*_xScale);
                g.DrawLine(_penGrid, xVertLine, _yTop, xVertLine, _yBottom + 2);
                string devLabel = index + (index != 0 ? "%" : "");
                var devLabelWidth = (int) g.MeasureString(devLabel, _font).Width;
                if (xVertLine - devLabelWidth/2 > xLabelRight)
                {
                    g.DrawString(devLabel, _font, _brushFore, xVertLine, _yBottom + Space, strFormatDevLabel);
                    xLabelRight = xVertLine + devLabelWidth/2;
                }
            }

            // Vertical coordinate axes.
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xMiddle - 1, _yTop - Space, _xMiddle - 1, _yBottom + 1);

            // Chart lines
            for (int param = 0; param < _parameters; param++)
                g.DrawLines(new Pen(GetNextColor(param)), _apntParameters[param]);

            // Horizontal coordinate axes.
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xLeft - 1, _yBottom + 1, _xRight, _yBottom + 1);
        }

        /// <summary>
        /// Gets color for the chart lines.
        /// </summary>
        private Color GetNextColor(int param)
        {
            var colors = new[]
                             {
                                 Color.DarkViolet,
                                 Color.Red,
                                 Color.Peru,
                                 Color.DarkSalmon,
                                 Color.Orange,
                                 Color.Green,
                                 Color.Lime,
                                 Color.Gold
                             };

            if (param >= colors.Length)
                param = param%colors.Length;

            return colors[param];
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            if (Table == null)
                return;

            InitChart(Table);
            Invalidate();
        }
    }
}