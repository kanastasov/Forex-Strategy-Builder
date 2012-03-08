// Fancy Panel class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public class FancyPanel : Panel
    {
        private readonly string _caption;
        private readonly bool _showCaption = true;
        private const int Border = 2;
        private Brush _brushCaption;
        private Color _colorCaptionBack;
        private Font _fontCaption;
        private Pen _penBorder;
        private RectangleF _rectfCaption;
        private StringFormat _stringFormatCaption;
        private float _height;
        private float _width;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FancyPanel()
        {
            _caption = "";
            _showCaption = false;
            InitializeParameters();
            SetColors();
            Padding = new Padding(Border, 2*Border, Border, Border);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FancyPanel(string captionText)
        {
            _caption = captionText;
            InitializeParameters();
            SetColors();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FancyPanel(string captionText, Color borderColor)
        {
            _caption = captionText;
            _colorCaptionBack = borderColor;
            _brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            _penBorder = new Pen(ColorMagic.GetGradientColor(borderColor, -LayoutColors.DepthCaption), Border);

            InitializeParameters();
        }

        /// <summary>
        /// Gets the caption height.
        /// </summary>
        public float CaptionHeight { get; private set; }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        private void SetColors()
        {
            _colorCaptionBack = LayoutColors.ColorCaptionBack;
            _brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            _penBorder = new Pen(ColorMagic.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        private void InitializeParameters()
        {
            _fontCaption = new Font(Font.FontFamily, 9);
            _stringFormatCaption = new StringFormat
                                      {
                                          Alignment = StringAlignment.Center,
                                          LineAlignment = StringAlignment.Center,
                                          Trimming = StringTrimming.EllipsisCharacter,
                                          FormatFlags = StringFormatFlags.NoWrap
                                      };

            CaptionHeight = _showCaption ? Math.Max(_fontCaption.Height, 18) : 2*Border;
        }

        /// <summary>
        /// On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption
            ColorMagic.GradientPaint(g, _rectfCaption, _colorCaptionBack, LayoutColors.DepthCaption);
            if (_showCaption)
                g.DrawString(_caption, _fontCaption, _brushCaption, _rectfCaption, _stringFormatCaption);

            g.DrawLine(_penBorder, 1, CaptionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, CaptionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paint the panel background
            var rectClient = new RectangleF(Border, CaptionHeight, _width, _height);
            ColorMagic.GradientPaint(g, rectClient, LayoutColors.ColorControlBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (_showCaption)
            {
                _width = ClientSize.Width - 2*Border;
                _height = ClientSize.Height - CaptionHeight - Border;
                _rectfCaption = new RectangleF(0, 0, ClientSize.Width, CaptionHeight);
            }
            else
            {
                _width = ClientSize.Width - 2*Border;
                _height = ClientSize.Height - CaptionHeight - Border;
                _rectfCaption = new RectangleF(0, 0, ClientSize.Width, CaptionHeight);
            }

            Invalidate();
        }
    }
}