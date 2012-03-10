// Pivot Points Calculator
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
    internal sealed class PivotPointsCalculator : Form
    {
        private readonly Color _colorText;
        private readonly Backtester _backtester;

        /// <summary>
        /// Constructor
        /// </summary>
        public PivotPointsCalculator(Backtester backtester)
        {
            _backtester = backtester;
            PnlInput = new FancyPanel(Language.T("Input Values"));
            PnlOutput = new FancyPanel(Language.T("Output Values"));

            AlblInputNames = new Label[3];
            AtbxInputValues = new TextBox[3];
            AlblOutputNames = new Label[7];
            AlblOutputValues = new Label[7];

            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = Language.T("Pivot Points");

            PnlInput.Parent = this;
            PnlOutput.Parent = this;

            // Input Names
            var inputNames = new[]
                                 {
                                     Language.T("Highest price"),
                                     Language.T("Closing price"),
                                     Language.T("Lowest price")
                                 };

            int number = 0;
            foreach (string name in inputNames)
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

            var outputNames = new[]
                                  {
                                      Language.T("Resistance") + " 3",
                                      Language.T("Resistance") + " 2",
                                      Language.T("Resistance") + " 1",
                                      Language.T("Pivot Point"),
                                      Language.T("Support") + " 1",
                                      Language.T("Support") + " 2",
                                      Language.T("Support") + " 3"
                                  };

            number = 0;
            foreach (string name in outputNames)
            {
                AlblOutputNames[number] = new Label
                                              {
                                                  Parent = PnlOutput,
                                                  ForeColor = _colorText,
                                                  BackColor = Color.Transparent,
                                                  AutoSize = true,
                                                  Text = name
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
            AlblOutputNames[3].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            AlblOutputValues[3].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
        }

        private FancyPanel PnlInput { get; set; }
        private FancyPanel PnlOutput { get; set; }

        private Label[] AlblInputNames { get; set; }
        private TextBox[] AtbxInputValues { get; set; }
        private Label[] AlblOutputNames { get; set; }
        private Label[] AlblOutputValues { get; set; }

        /// <summary>
        /// Initializes the parameters
        /// </summary>
        private void InitParams()
        {
            AtbxInputValues[0].Text = _backtester.DataSet.High[_backtester.DataSet.Bars - 1].ToString(CultureInfo.InvariantCulture);
            AtbxInputValues[1].Text = _backtester.DataSet.Close[_backtester.DataSet.Bars - 1].ToString(CultureInfo.InvariantCulture);
            AtbxInputValues[2].Text = _backtester.DataSet.Low[_backtester.DataSet.Bars - 1].ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(270, 307);

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

            // PnlInput
            PnlInput.Size = new Size(ClientSize.Width - 2*border, 112);
            PnlInput.Location = new Point(border, border);

            int left = PnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            int shift = 26;
            int vertSpace = 2;
            int numb = 0;
            foreach (Label lbl in AlblInputNames)
            {
                lbl.Location = new Point(border, numb*buttonHeight + (numb + 1)*vertSpace + shift);
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
            PnlOutput.Size = new Size(ClientSize.Width - 2*border, 180);
            PnlOutput.Location = new Point(border, PnlInput.Bottom + border);

            shift = 24;
            vertSpace = -4;
            numb = 0;
            foreach (Label lbl in AlblOutputNames)
            {
                lbl.Location = new Point(border, numb*(buttonHeight + vertSpace) + shift);
                numb++;
            }

            numb = 0;
            foreach (Label lbl in AlblOutputValues)
            {
                lbl.Location = new Point(left, numb*(buttonHeight + vertSpace) + shift);
                numb++;
            }
        }

        /// <summary>
        /// Parses a float value
        /// </summary>
        private float ParseInput(string input)
        {
            string dcmlSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            input = input.Replace(".", dcmlSeparator);
            input = input.Replace(",", dcmlSeparator);

            return float.Parse(input);
        }

        /// <summary>
        /// A parameter has been changed
        /// </summary>
        private void TbxInputTextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        /// <summary>
        /// Perform calculation
        /// </summary>
        private void Calculate()
        {
            foreach (Label lbl in AlblOutputValues)
                lbl.Text = "";

            float high;
            float close;
            float low;

            try
            {
                high = ParseInput(AtbxInputValues[0].Text);
                close = ParseInput(AtbxInputValues[1].Text);
                low = ParseInput(AtbxInputValues[2].Text);
            }
            catch
            {
                foreach (Label lbl in AlblOutputValues)
                    lbl.Text = "";

                return;
            }

            float pivot = (high + close + low)/3;
            float resistance1 = 2*pivot - low;
            float support1 = 2*pivot - high;
            float resistance2 = pivot + (resistance1 - support1);
            float support2 = pivot - (resistance1 - support1);
            float resistance3 = high + 2*(pivot - low);
            float support3 = low - 2*(high - pivot);

            AlblOutputValues[0].Text = resistance3.ToString("F4");
            AlblOutputValues[1].Text = resistance2.ToString("F4");
            AlblOutputValues[2].Text = resistance1.ToString("F4");
            AlblOutputValues[3].Text = pivot.ToString("F4");
            AlblOutputValues[4].Text = support1.ToString("F4");
            AlblOutputValues[5].Text = support2.ToString("F4");
            AlblOutputValues[6].Text = support3.ToString("F4");
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