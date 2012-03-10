// Profit Calculator
// Part of Forex Strategy Builder & Forex Strategy Trader
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
    /// <summary>
    ///Profit Calculator
    /// </summary>
    public sealed class ProfitCalculator : Form
    {
        private readonly Color _colorText;

        private string _symbol;
        private readonly Backtester _backtester;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProfitCalculator(Backtester backtester)
        {
            _backtester = backtester;
            PnlInput = new FancyPanel(Language.T("Input Values"));
            PnlOutput = new FancyPanel(Language.T("Output Values"));

            AlblInputNames = new Label[6];
            AlblOutputNames = new Label[8];
            AlblOutputValues = new Label[8];

            LblLotSize = new Label();
            CbxDirection = new ComboBox();
            NUDLots = new NumericUpDown();
            NUDEntryPrice = new NumericUpDown();
            NUDExitPrice = new NumericUpDown();
            NUDDays = new NumericUpDown();

            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = Language.T("Profit Calculator");

            // Input
            PnlInput.Parent = this;

            // Output
            PnlOutput.Parent = this;

            // Input Names
            var asInputNames = new[]
                                   {
                                       _backtester.DataSet.InstrProperties.Symbol,
                                       Language.T("Direction"),
                                       Language.T("Number of lots"),
                                       Language.T("Entry price"),
                                       Language.T("Exit price"),
                                       Language.T("Days rollover")
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
                number++;
            }

            // Label Lot size
            LblLotSize.Parent = PnlInput;
            LblLotSize.ForeColor = _colorText;
            LblLotSize.BackColor = Color.Transparent;

            // ComboBox SameDirAction
            CbxDirection.Parent = PnlInput;
            CbxDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxDirection.Items.AddRange(new object[] {Language.T("Long"), Language.T("Short")});
            CbxDirection.SelectedIndex = 0;

            // Lots
            NUDLots.Parent = PnlInput;
            NUDLots.BeginInit();
            NUDLots.Minimum = 0.01M;
            NUDLots.Maximum = 100;
            NUDLots.Increment = 0.01M;
            NUDLots.DecimalPlaces = 2;
            NUDLots.Value = (decimal)_backtester.Strategy.EntryLots;
            NUDLots.EndInit();

            // NumericUpDown Entry Price
            NUDEntryPrice.Parent = PnlInput;

            // NumericUpDown Exit Price
            NUDExitPrice.Parent = PnlInput;

            // NumericUpDown Reducing Lots
            NUDDays.Parent = PnlInput;
            NUDDays.BeginInit();
            NUDDays.Minimum = 0;
            NUDDays.Maximum = 1000;
            NUDDays.Increment = 1;
            NUDDays.Value = 1;
            NUDDays.EndInit();

            // Output Names
            var asOutputNames = new[]
                                    {
                                        Language.T("Required margin"),
                                        Language.T("Gross profit"),
                                        Language.T("Spread"),
                                        Language.T("Entry commission"),
                                        Language.T("Exit commission"),
                                        Language.T("Rollover"),
                                        Language.T("Slippage"),
                                        Language.T("Net profit")
                                    };

            number = 0;
            foreach (string name in asOutputNames)
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

            AlblOutputNames[number - 1].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            AlblOutputValues[number - 1].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);

            Timer = new Timer {Interval = 2000};
            Timer.Tick += TimerTick;
            Timer.Start();
        }

        private FancyPanel PnlInput { get; set; }
        private FancyPanel PnlOutput { get; set; }

        private Label[] AlblInputNames { get; set; }
        private Label[] AlblOutputNames { get; set; }
        private Label[] AlblOutputValues { get; set; }

        private Label LblLotSize { get; set; }
        private ComboBox CbxDirection { get; set; }
        private NumericUpDown NUDLots { get; set; }
        private NumericUpDown NUDEntryPrice { get; set; }
        private NumericUpDown NUDExitPrice { get; set; }
        private NumericUpDown NUDDays { get; set; }

        private Timer Timer { get; set; }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CbxDirection.SelectedIndexChanged += ParamChanged;
            NUDLots.ValueChanged += ParamChanged;
            NUDEntryPrice.ValueChanged += ParamChanged;
            NUDExitPrice.ValueChanged += ParamChanged;
            NUDDays.ValueChanged += ParamChanged;

            ClientSize = new Size(270, 405);

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
            PnlInput.Size = new Size(ClientSize.Width - 2*border, 190);
            PnlInput.Location = new Point(border, border);

            int left = PnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            LblLotSize.Width = width;
            CbxDirection.Width = width;
            NUDLots.Width = width;
            NUDEntryPrice.Width = width;
            NUDExitPrice.Width = width;
            NUDDays.Width = width;

            int shift = 22;
            int vertSpace = 2;
            LblLotSize.Location = new Point(left, 0*buttonHeight + 1*vertSpace + shift - 0);
            CbxDirection.Location = new Point(left, 1*buttonHeight + 2*vertSpace + shift - 4);
            NUDLots.Location = new Point(left, 2*buttonHeight + 3*vertSpace + shift - 4);
            NUDEntryPrice.Location = new Point(left, 3*buttonHeight + 4*vertSpace + shift - 4);
            NUDExitPrice.Location = new Point(left, 4*buttonHeight + 5*vertSpace + shift - 4);
            NUDDays.Location = new Point(left, 5*buttonHeight + 6*vertSpace + shift - 4);

            int numb = 0;
            foreach (Label lbl in AlblInputNames)
            {
                lbl.Location = new Point(border, numb*buttonHeight + (numb + 1)*vertSpace + shift);
                numb++;
            }

            // pnlOutput
            PnlOutput.Size = new Size(ClientSize.Width - 2*border, 200);
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
        /// Perform periodical action.
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            if (_symbol == _backtester.DataSet.InstrProperties.Symbol) return;
            InitParams();
            InitParams();
        }

        /// <summary>
        /// Sets the initial params.
        /// </summary>
        private void InitParams()
        {
            _symbol = _backtester.DataSet.InstrProperties.Symbol;

            AlblInputNames[0].Text = _symbol;
            LblLotSize.Text = _backtester.DataSet.InstrProperties.LotSize.ToString(CultureInfo.InvariantCulture);

            // NumericUpDown Entry Price
            NUDEntryPrice.BeginInit();
            NUDEntryPrice.DecimalPlaces = _backtester.DataSet.InstrProperties.Digits;
            NUDEntryPrice.Minimum = (decimal)(_backtester.DataStats.MinPrice * 0.7);
            NUDEntryPrice.Maximum = (decimal)(_backtester.DataStats.MaxPrice * 1.3);
            NUDEntryPrice.Increment = (decimal) _backtester.DataSet.InstrProperties.Point;
            NUDEntryPrice.Value = (decimal)_backtester.DataSet.Close[_backtester.DataSet.Bars - 1];
            NUDEntryPrice.EndInit();

            // NumericUpDown Exit Price
            NUDExitPrice.BeginInit();
            NUDExitPrice.DecimalPlaces = _backtester.DataSet.InstrProperties.Digits;
            NUDExitPrice.Minimum = (decimal)(_backtester.DataStats.MinPrice * 0.7);
            NUDExitPrice.Maximum = (decimal)(_backtester.DataStats.MaxPrice * 1.3);
            NUDExitPrice.Increment = (decimal) _backtester.DataSet.InstrProperties.Point;
            NUDExitPrice.Value = (decimal)(_backtester.DataSet.Close[_backtester.DataSet.Bars - 1] + 100 * _backtester.DataSet.InstrProperties.Point);
            NUDExitPrice.EndInit();

            Calculate();
        }

        /// <summary>
        /// Sets the params values
        /// </summary>
        private void ParamChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        /// <summary>
        /// Calculates the result
        /// </summary>
        private void Calculate()
        {
            bool isLong = (CbxDirection.SelectedIndex == 0);
            PosDirection posDir = isLong ? PosDirection.Long : PosDirection.Short;
            int lotSize = _backtester.DataSet.InstrProperties.LotSize;
            var lots = (double) NUDLots.Value;
            var entryPrice = (double) NUDEntryPrice.Value;
            var exitPrice = (double) NUDExitPrice.Value;
            var daysRollover = (int) NUDDays.Value;
            double point = _backtester.DataSet.InstrProperties.Point;
            string unit = " " + Configs.AccountCurrency;
            double entryValue = lots*lotSize*entryPrice;
            double exitValue = lots*lotSize*exitPrice;

            // Required margin
            double requiredMargin = (lots*lotSize/Configs.Leverage)*
                                    (entryPrice/_backtester.AccountExchangeRate(entryPrice));
            AlblOutputValues[0].Text = requiredMargin.ToString("F2") + unit;

            // Gross Profit
            double grossProfit = (isLong ? exitValue - entryValue : entryValue - exitValue)/
                                 _backtester.AccountExchangeRate(exitPrice);
            AlblOutputValues[1].Text = grossProfit.ToString("F2") + unit;

            // Spread
            double spread = _backtester.DataSet.InstrProperties.Spread*point*lots*lotSize/_backtester.AccountExchangeRate(exitPrice);
            AlblOutputValues[2].Text = spread.ToString("F2") + unit;

            // Entry Commission
            double entryCommission = _backtester.CommissionInMoney(lots, entryPrice, false);
            AlblOutputValues[3].Text = entryCommission.ToString("F2") + unit;

            // Exit Commission
            double exitCommission = _backtester.CommissionInMoney(lots, exitPrice, true);
            AlblOutputValues[4].Text = exitCommission.ToString("F2") + unit;

            // Rollover
            double rollover = _backtester.RolloverInMoney(posDir, lots, daysRollover, exitPrice);
            AlblOutputValues[5].Text = rollover.ToString("F2") + unit;

            // Slippage
            double slippage = _backtester.DataSet.InstrProperties.Slippage*point*lots*lotSize/_backtester.AccountExchangeRate(exitPrice);
            AlblOutputValues[6].Text = slippage.ToString("F2") + unit;

            // Net Profit
            double netProfit = grossProfit - entryCommission - exitCommission - rollover - slippage;
            AlblOutputValues[7].Text = netProfit.ToString("F2") + unit;
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