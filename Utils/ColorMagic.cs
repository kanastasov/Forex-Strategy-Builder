// ColorMagic class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Forex_Strategy_Builder.Utils
{
    public static class ColorMagic
    {
        /// <summary>
        /// Paints a rectangle with gradient
        /// </summary>
        public static void GradientPaint(Graphics g, RectangleF rect, Color color, int depth)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (depth > 0 && Configs.GradientView)
            {
                Color color1 = GetGradientColor(color, +depth);
                Color color2 = GetGradientColor(color, -depth);
                var rect1 = new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height + 2);
                var linearGradientBrush = new LinearGradientBrush(rect1, color1, color2, 90);
                g.FillRectangle(linearGradientBrush, rect);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), rect);
            }
        }

        /// <summary>
        /// Color change
        /// </summary>
        public static Color GetGradientColor(Color baseColor, int depth)
        {
            if (!Configs.GradientView)
                return baseColor;

            int r = Math.Max(Math.Min(baseColor.R + depth, 255), 0);
            int g = Math.Max(Math.Min(baseColor.G + depth, 255), 0);
            int b = Math.Max(Math.Min(baseColor.B + depth, 255), 0);

            return Color.FromArgb(r, g, b);
        }

        public static Color GetIntermediateColor(Color start, Color end, float factor)
        {
            var r = (int)Math.Round((end.R - start.R) * factor + start.R);
            var g = (int)Math.Round((end.G - start.G) * factor + start.G);
            var b = (int)Math.Round((end.B - start.B) * factor + start.B);
            return Color.FromArgb(r, g, b);
        }
    }
}
