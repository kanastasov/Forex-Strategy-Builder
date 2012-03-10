// Fibonacci Levels Calculator
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    internal sealed class FibonacciLevelsCalculator : Form
    {
        private readonly float[] _afLevels = new[]
            { 0.0f, 23.6f, 38.2f, 50.0f, 61.8f, 76.4f, 100.0f, 138.2f, 161.8f, 261.8f };

        private readonly Color _colorText;

        /// <summary>
        /// Constructor
        /// </summary>
        public FibonacciLevelsCalculator(Backtester backtester)
        {
            _backtester = backtester;

            PnlInput = new FancyPanel(Language.T("Input Values"));
            PnlOutput = new FancyPanel(Language.T("Output Values"));

            AlblInputNames = new Label[2];
            AtbxInputValues = new TextBox[2];
            AlblOutputNames = new Label[10];
            AlblOutputValues = new Label[10];

            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = Language.T("Fibonacci Levels");

            // Input
            PnlInput.Parent = this;

            // Output
            PnlOutput.Parent = this;

            // Input Names
            var asInputNames = new[]
                                   {
                                       Language.T("First price"),
                                       Language.T("Second price")
                                   };

            int number = 0;
            foreach (string name in asInputNames)
            {
                AlblInputNames[number] = new Label
                                             {
                                                 Parent = PnlInput,
                                                 ForeColor = _colorText,
                                                 BackColor = Color.Transparent,
                                                 AutoSize = true,
                                                 Text = name
                                             };

                AtbxInputValues[number] = new TextBox {Parent = PnlInput};
                AtbxInputValues[number].TextChanged += TbxInputTextChanged;
                number++;
            }

            // Output Names
            number = 0;
            foreach (float fn in _afLevels)
            {
                AlblOutputNames[number] = new Label
                                              {
                                                  Parent = PnlOutput,
                                                  ForeColor = _colorText,
                                                  BackColor = Color.Transparent,
                                                  AutoSize = true,
                                                  Text = fn.ToString("F1") + " %"
                                              };

                AlblOutputValues[number] = new Label
                                               {
                                                   Parent = PnlOutput,
                                                   ForeColor = _colorText,
                                                   BackColor = Color.Transparent,
                                                   AutoSize = true
                                               };

                number++;
            }

            Font font = Font;
            AlblOutputNames[2].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            AlblOutputNames[3].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            AlblOutputNames[4].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            AlblOutputValues[2].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            AlblOutputValues[3].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            AlblOutputValues[4].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
        }

        private readonly Backtester _backtester;
        private FancyPanel PnlInput { get; set; }
        private FancyPanel PnlOutput { get; set; }
        private Label[] AlblInputNames { get; set; }
        private TextBox[] AtbxInputValues { get; set; }
        private Label[] AlblOutputNames { get; set; }
        private Label[] AlblOutputValues { get; set; }

        /// <summary>
        /// Initial parameters
        /// </summary>
        private void InitParams()
        {
            var fibo = new Fibonacci(_backtester.DataSet, SlotTypes.Close);
            fibo.Calculate(SlotTypes.Close);
            AtbxInputValues[0].Text = fibo.Component[5].Value[_backtester.DataSet.Bars - 1].ToString(CultureInfo.InvariantCulture);
            AtbxInputValues[1].Text = fibo.Component[1].Value[_backtester.DataSet.Bars - 1].ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(270, 347);

            InitParams();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int border = btnHrzSpace;
            const int width = 100; // Right side controls

            // pnlInput
            PnlInput.Size = new Size(ClientSize.Width - 2*border, 85);
            PnlInput.Location = new Point(border, border);

            int left = PnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            int shift = 26;
            int vertSpace = 2;
            int numb = 0;
            foreach (Label label in AlblInputNames)
            {
                label.Location = new Point(border, numb*buttonHeight + (numb + 1)*vertSpace + shift);
                numb++;
            }

            shift = 24;
            vertSpace = 2;
            numb = 0;
            foreach (TextBox textBox in AtbxInputValues)
            {
                textBox.Width = width;
                textBox.Location = new Point(left, numb*buttonHeight + (numb + 1)*vertSpace + shift);
                numb++;
            }

            // pnlOutput
            PnlOutput.Size = new Size(ClientSize.Width - 2*border, 245);
            PnlOutput.Location = new Point(border, PnlInput.Bottom + border);

            shift = 24;
            vertSpace = -4;
            numb = 0;
            foreach (Label label in AlblOutputNames)
            {
                label.Location = new Point(border, numb*(buttonHeight + vertSpace) + shift);
                numb++;
            }

            numb = 0;
            foreach (Label label in AlblOutputValues)
            {
                label.Location = new Point(left, numb*(buttonHeight + vertSpace) + shift);
                numb++;
            }
        }

        /// <summary>
        /// Parses a float number
        /// </summary>
        private float ParseInput(string input)
        {
            string dcmlSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            input = input.Replace(".", dcmlSeparator);
            input = input.Replace(",", dcmlSeparator);

            return float.Parse(input);
        }

        /// <summary>
        /// Input parameter changed
        /// </summary>
        private void TbxInputTextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        /// <summary>
        /// Calculates the result
        /// </summary>
        private void Calculate()
        {
            foreach (Label label in AlblOutputValues)
                label.Text = "";

            float price1;
            float price2;

            try
            {
                price1 = ParseInput(AtbxInputValues[0].Text);
                price2 = ParseInput(AtbxInputValues[1].Text);
            }
            catch
            {
                foreach (Label label in AlblOutputValues)
                    label.Text = "";
                return;
            }

            if (price1 > price2)
            {
                for (int i = _afLevels.Length - 1; i >= 0; i--)
                    AlblOutputValues[i].Text = ((price1 - price2)*_afLevels[i]/100 + price2).ToString("F4");
            }
            else if (price1 < price2)
            {
                for (int i = 0; i < _afLevels.Length; i++)
                    AlblOutputValues[i].Text = (price2 - (price2 - price1)*_afLevels[i]/100).ToString("F4");
            }
            else
            {
                foreach (Label label in AlblOutputValues)
                    label.Text = "";
            }
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}