// InfoPanel Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.CustomControls;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public class InfoPanel : ContextPanel
    {
        private const int Border = 2;
        private const float PaddingParamData = 4;
        private Brush _brushCaption;
        private Brush _brushData;
        private Brush _brushParams;
        private string _caption;
        private float _captionHeight;
        private Color _colorBackroundEvenRows;
        private Color _colorBackroundOddRows;
        private Color _colorBackroundWarningRow;
        private Color _colorCaptionBack;
        private Color _colorTextWarningRow;
        private Font _fontCaption;
        private Font _fontData;
        private HScrollBar _hScrollBar;
        private float _height;
        private float _maxParamWidth;
        private float _maxValueWidth;
        private float _paramTab;
        private Pen _penBorder;
        private RectangleF _rectfCaption;
        private float _requiredHeight;
        private float _rowHeight;
        private int _rows;
        private StringFormat _stringFormatCaption;
        private StringFormat _stringFormatData;
        private VScrollBar _vScrollBar;
        private float _valueTab;
        private int _visibleRows;
        private float _width;
        private bool[] _flagsList;
        private string[] _paramsList;
        private string[] _valuesList;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public InfoPanel()
        {
            InitializeParameters();
            SetColors();
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            _colorCaptionBack = LayoutColors.ColorCaptionBack;
            _colorBackroundEvenRows = LayoutColors.ColorEvenRowBack;
            _colorBackroundWarningRow = LayoutColors.ColorWarningRowBack;
            _colorTextWarningRow = LayoutColors.ColorWarningRowText;
            _colorBackroundOddRows = LayoutColors.ColorOddRowBack;

            _brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            _brushParams = new SolidBrush(LayoutColors.ColorControlText);
            _brushData = new SolidBrush(LayoutColors.ColorControlText);

            _penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        private void InitializeParameters()
        {
            // Caption
            _stringFormatCaption = new StringFormat
                                       {
                                           Alignment = StringAlignment.Center,
                                           LineAlignment = StringAlignment.Center,
                                           Trimming = StringTrimming.EllipsisCharacter,
                                           FormatFlags = StringFormatFlags.NoWrap
                                       };

            _fontCaption = new Font(Font.FontFamily, 9);
            _captionHeight = Math.Max(_fontCaption.Height, 18);
            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);

            // Data row
            _stringFormatData = new StringFormat {Trimming = StringTrimming.EllipsisCharacter};
            _fontData = new Font(Font.FontFamily, 9);
            _rowHeight = _fontData.Height + 4;

            Padding = new Padding(Border, (int) _captionHeight, Border, Border);

            _hScrollBar = new HScrollBar
                              {
                                  Parent = this,
                                  Dock = DockStyle.Bottom,
                                  Enabled = false,
                                  Visible = false,
                                  SmallChange = 10,
                                  LargeChange = 30
                              };
            _hScrollBar.Scroll += ScrollBarScroll;

            _vScrollBar = new VScrollBar
                              {
                                  Parent = this,
                                  Dock = DockStyle.Right,
                                  TabStop = true,
                                  Enabled = false,
                                  Visible = false,
                                  SmallChange = 1,
                                  LargeChange = 3,
                                  Maximum = 20
                              };
            _vScrollBar.Scroll += ScrollBarScroll;

            MouseUp += InfoPanelMouseUp;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update(string[] asParams, string[] asValues, bool[] abFlags, string caption)
        {
            _paramsList = asParams;
            _valuesList = asValues;
            _flagsList = abFlags;
            _caption = caption;

            _rows = _paramsList.Length;
            _requiredHeight = _captionHeight + _rows*_rowHeight + Border;

            // Maximum width
            _maxParamWidth = 0;
            _maxValueWidth = 0;
            Graphics g = CreateGraphics();
            for (int i = 0; i < _rows; i++)
            {
                float fWidthParam = g.MeasureString(_paramsList[i], _fontData).Width;
                if (fWidthParam > _maxParamWidth)
                    _maxParamWidth = fWidthParam;

                float fValueWidth = g.MeasureString(_valuesList[i], _fontData).Width;
                if (fValueWidth > _maxValueWidth)
                    _maxValueWidth = fValueWidth;
            }
            g.Dispose();

            CalculateScrollBarStatus();
            CalculateTabs();
            Invalidate();
        }

        /// <summary>
        /// On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption
            ColorMagic.GradientPaint(g, _rectfCaption, _colorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_caption, _fontCaption, _brushCaption, _rectfCaption, _stringFormatCaption);

            for (int i = 0; i*_rowHeight + _captionHeight < _height; i++)
            {
                float fVerticalPosition = i*_rowHeight + _captionHeight;
                var pointFParam = new PointF(_paramTab + 2, fVerticalPosition);
                var pointFValue = new PointF(_valueTab + 2, fVerticalPosition);
                var rectRow = new RectangleF(Border, fVerticalPosition, _width, _rowHeight);

                // Row background
                if (i + _vScrollBar.Value < _rows && _flagsList[i + _vScrollBar.Value])
                    g.FillRectangle(new SolidBrush(_colorBackroundWarningRow), rectRow);
                else if (Math.Abs(i%2f - 0) > 0.001)
                    g.FillRectangle(new SolidBrush(_colorBackroundEvenRows), rectRow);
                else
                    g.FillRectangle(new SolidBrush(_colorBackroundOddRows), rectRow);

                if (i + _vScrollBar.Value >= _rows)
                    continue;

                if (i + _vScrollBar.Value < _rows && _flagsList[i + _vScrollBar.Value])
                {
                    Brush brush = new SolidBrush(_colorTextWarningRow);
                    g.DrawString(_paramsList[i + _vScrollBar.Value], _fontData, brush, pointFParam, _stringFormatData);
                    g.DrawString(_valuesList[i + _vScrollBar.Value], _fontData, brush, pointFValue, _stringFormatData);
                }
                else
                {
                    g.DrawString(_paramsList[i + _vScrollBar.Value], _fontData, _brushParams, pointFParam,
                                 _stringFormatData);
                    g.DrawString(_valuesList[i + _vScrollBar.Value], _fontData, _brushData, pointFValue,
                                 _stringFormatData);
                }
            }

            // Border
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            CalculateScrollBarStatus();
            CalculateTabs();
            base.OnResize(eventargs);
            Invalidate();
        }

        /// <summary>
        /// Scroll Bars status
        /// </summary>
        private void CalculateScrollBarStatus()
        {
            _width = ClientSize.Width - 2*Border;
            _height = ClientSize.Height - Border;

            bool needHorisontal = _width < _maxParamWidth + PaddingParamData + _maxValueWidth - 2;
            bool needVertical = _height < _requiredHeight;
            bool isHorisontal = needHorisontal;
            bool isVertical = needVertical;

            if (needHorisontal && !needVertical)
            {
                _height = ClientSize.Height - _hScrollBar.Height - Border;
                isVertical = _height < _requiredHeight;
            }
            else if (needVertical && !needHorisontal)
            {
                _width = ClientSize.Width - _vScrollBar.Width - 2*Border;
                isHorisontal = _width < _maxParamWidth + PaddingParamData + _maxValueWidth - 2;
            }

            if (isHorisontal)
                _height = ClientSize.Height - _hScrollBar.Height - Border;

            if (isVertical)
                _width = ClientSize.Width - _vScrollBar.Width - 2*Border;

            _vScrollBar.Enabled = isVertical;
            _vScrollBar.Visible = isVertical;
            _hScrollBar.Enabled = isHorisontal;
            _hScrollBar.Visible = isHorisontal;

            _hScrollBar.Value = 0;
            if (isHorisontal)
            {
                var iPoinShort = (int) (_maxParamWidth + PaddingParamData + _maxValueWidth - _width);
                _hScrollBar.Maximum = iPoinShort + _hScrollBar.LargeChange - 2;
            }

            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            _visibleRows = (int) Math.Min(((_height - _captionHeight)/_rowHeight), _rows);

            _vScrollBar.Value = 0;
            _vScrollBar.Maximum = _rows - _visibleRows + _vScrollBar.LargeChange - 1;
        }

        /// <summary>
        /// Tabs
        /// </summary>
        private void CalculateTabs()
        {
            if (_width < _maxParamWidth + PaddingParamData + _maxValueWidth)
            {
                _paramTab = -_hScrollBar.Value + Border;
                _valueTab = _paramTab + _maxParamWidth;
            }
            else
            {
                float fSpace = (_width - (_maxParamWidth + _maxValueWidth))/5;
                _paramTab = 2*fSpace;
                _valueTab = _paramTab + _maxParamWidth + fSpace;
            }
        }

        /// <summary>
        /// ScrollBar_Scroll
        /// </summary>
        private void ScrollBarScroll(object sender, ScrollEventArgs e)
        {
            CalculateTabs();
            int horizontal = _hScrollBar.Visible ? _hScrollBar.Height : 0;
            int vertical = _vScrollBar.Visible ? _vScrollBar.Width : 0;
            var rect = new Rectangle(Border, (int) _captionHeight, ClientSize.Width - vertical - 2*Border,
                                     ClientSize.Height - (int) _captionHeight - horizontal - Border);
            Invalidate(rect);
        }

        /// <summary>
        /// Selects the vertical scrollbar
        /// </summary>
        private void InfoPanelMouseUp(object sender, MouseEventArgs e)
        {
            _vScrollBar.Select();
        }
    }
}