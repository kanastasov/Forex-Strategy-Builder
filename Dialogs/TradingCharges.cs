// Forex Strategy Builder - Trading Charges
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
    /// <summary>
    /// Market Settings
    /// </summary>
    public sealed class TradingCharges : Form
    {
        private readonly Color _colorText;
        private bool _editInstrument;

        /// <summary>
        /// Constructor
        /// </summary>
        public TradingCharges()
        {
            PnlBase = new FancyPanel();

            LblSpread = new Label();
            LblSwapLong = new Label();
            LblSwapShort = new Label();
            LblCommission = new Label();
            LblSlippage = new Label();

            NUDSpread = new NumericUpDown();
            NUDSwapLong = new NumericUpDown();
            NUDSwapShort = new NumericUpDown();
            NUDCommission = new NumericUpDown();
            NUDSlippage = new NumericUpDown();

            BtnEditInstrument = new Button();
            BtnAccept = new Button();
            BtnCancel = new Button();

            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Trading Charges") + " - " + Data.Symbol;

            var toolTip = new ToolTip();


            // pnlBase
            PnlBase.Parent = this;

            // Label Spread
            LblSpread.Parent = PnlBase;
            LblSpread.ForeColor = _colorText;
            LblSpread.BackColor = Color.Transparent;
            LblSpread.AutoSize = true;
            LblSpread.Text = Language.T("Spread") + " [" + Language.T("pips") + "]";

            // Label Swap Long
            LblSwapLong.Parent = PnlBase;
            LblSwapLong.ForeColor = _colorText;
            LblSwapLong.BackColor = Color.Transparent;
            LblSwapLong.AutoSize = true;
            LblSwapLong.Text = Language.T("Swap number for a long position rollover") + " [" +
                               (Data.DataSet.InstrProperties.SwapType == CommissionType.money
                                    ? Data.DataSet.InstrProperties.PriceIn
                                    : Language.T(Data.DataSet.InstrProperties.SwapType.ToString())) + "]" +
                               Environment.NewLine +
                               "(" + Language.T("A positive value decreases your profit.") + ")";

            // Label Swap Short
            LblSwapShort.Parent = PnlBase;
            LblSwapShort.ForeColor = _colorText;
            LblSwapShort.BackColor = Color.Transparent;
            LblSwapShort.AutoSize = true;
            LblSwapShort.Text = Language.T("Swap number for a short position rollover") + " [" +
                                (Data.DataSet.InstrProperties.SwapType == CommissionType.money
                                     ? Data.DataSet.InstrProperties.PriceIn
                                     : Language.T(Data.DataSet.InstrProperties.SwapType.ToString())) + "]" +
                                Environment.NewLine +
                                "(" + Language.T("A negative value decreases your profit.") + ")";

            // Label Commission
            LblCommission.Parent = PnlBase;
            LblCommission.ForeColor = _colorText;
            LblCommission.BackColor = Color.Transparent;
            LblCommission.AutoSize = true;
            LblCommission.Text = Language.T("Commission in") + " " +
                                 Data.DataSet.InstrProperties.CommissionTypeToString + " " +
                                 Data.DataSet.InstrProperties.CommissionScopeToString + " " +
                                 Data.DataSet.InstrProperties.CommissionTimeToString +
                                 (Data.DataSet.InstrProperties.CommissionType == CommissionType.money
                                      ? " [" + Data.DataSet.InstrProperties.PriceIn + "]"
                                      : "");

            // Label Slippage
            LblSlippage.Parent = PnlBase;
            LblSlippage.ForeColor = _colorText;
            LblSlippage.BackColor = Color.Transparent;
            LblSlippage.AutoSize = true;
            LblSlippage.Text = Language.T("Slippage") + " [" + Language.T("pips") + "]";

            // NumericUpDown Spread
            NUDSpread.BeginInit();
            NUDSpread.Parent = PnlBase;
            NUDSpread.Name = Language.T("Spread");
            NUDSpread.TextAlign = HorizontalAlignment.Center;
            NUDSpread.Minimum = 0;
            NUDSpread.Maximum = 500;
            NUDSpread.Increment = 0.01M;
            NUDSpread.DecimalPlaces = 2;
            NUDSpread.Value = 4;
            NUDSpread.EndInit();
            toolTip.SetToolTip(NUDSpread, Language.T("Difference between Bid and Ask prices."));

            // NumericUpDown Swap Long
            NUDSwapLong.BeginInit();
            NUDSwapLong.Parent = PnlBase;
            NUDSwapLong.Name = "SwapLong";
            NUDSwapLong.TextAlign = HorizontalAlignment.Center;
            NUDSwapLong.Minimum = -500;
            NUDSwapLong.Maximum = 500;
            NUDSwapLong.Increment = 0.01M;
            NUDSwapLong.DecimalPlaces = 2;
            NUDSwapLong.Value = 1;
            NUDSwapLong.EndInit();
            toolTip.SetToolTip(NUDSwapLong, Language.T("A position changes its average price with the selected number during a rollover."));

            // NumericUpDown Swap Short
            NUDSwapShort.BeginInit();
            NUDSwapShort.Parent = PnlBase;
            NUDSwapShort.Name = "SwapShort";
            NUDSwapShort.TextAlign = HorizontalAlignment.Center;
            NUDSwapShort.Minimum = -500;
            NUDSwapShort.Maximum = 500;
            NUDSwapShort.Increment = 0.01M;
            NUDSwapShort.DecimalPlaces = 2;
            NUDSwapShort.Value = -1;
            NUDSwapShort.EndInit();
            toolTip.SetToolTip(NUDSwapShort, Language.T("A position changes its average price with the selected number during a rollover."));

            // NumericUpDown Commission
            NUDCommission.BeginInit();
            NUDCommission.Parent = PnlBase;
            NUDCommission.Name = Language.T("Commission");
            NUDCommission.TextAlign = HorizontalAlignment.Center;
            NUDCommission.Minimum = -500;
            NUDCommission.Maximum = 500;
            NUDCommission.Increment = 0.01M;
            NUDCommission.DecimalPlaces = 2;
            NUDCommission.Value = 0;
            NUDCommission.EndInit();

            // NumericUpDown Slippage
            NUDSlippage.BeginInit();
            NUDSlippage.Parent = PnlBase;
            NUDSlippage.Name = "Slippage";
            NUDSlippage.TextAlign = HorizontalAlignment.Center;
            NUDSlippage.Minimum = 0;
            NUDSlippage.Maximum = 200;
            NUDSlippage.Increment = 1;
            NUDSlippage.Value = 0;
            NUDSlippage.EndInit();
            toolTip.SetToolTip(NUDSlippage, Language.T("Number of pips you lose due to an inaccurate order execution."));

            //Button btnEditInstrument
            BtnEditInstrument.Parent = this;
            BtnEditInstrument.Name = "EditInstrument";
            BtnEditInstrument.Text = Language.T("More");
            BtnEditInstrument.Click += BtnEditInstrumentClick;
            BtnEditInstrument.UseVisualStyleBackColor = true;

            //Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Accept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlBase { get; set; }

        private Label LblSpread { get; set; }
        private Label LblSwapLong { get; set; }
        private Label LblSwapShort { get; set; }
        private Label LblCommission { get; set; }
        private Label LblSlippage { get; set; }

        private NumericUpDown NUDSpread { get; set; }
        private NumericUpDown NUDSwapLong { get; set; }
        private NumericUpDown NUDSwapShort { get; set; }
        private NumericUpDown NUDCommission { get; set; }
        private NumericUpDown NUDSlippage { get; set; }

        private Button BtnEditInstrument { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }


        /// <summary>
        /// Spread
        /// </summary>
        public double Spread
        {
            get { return (double) NUDSpread.Value; }
            set { NUDSpread.Value = (decimal) value; }
        }

        /// <summary>
        /// Swap Long
        /// </summary>
        public double SwapLong
        {
            get { return (double) NUDSwapLong.Value; }
            set { NUDSwapLong.Value = (decimal) value; }
        }

        /// <summary>
        /// Swap Short
        /// </summary>
        public double SwapShort
        {
            get { return (double) NUDSwapShort.Value; }
            set { NUDSwapShort.Value = (decimal) value; }
        }

        /// <summary>
        /// Commission
        /// </summary>
        public double Commission
        {
            get { return (double) NUDCommission.Value; }
            set { NUDCommission.Value = (decimal) value; }
        }

        /// <summary>
        /// Slippage
        /// </summary>
        public int Slippage
        {
            get { return (int) NUDSlippage.Value; }
            set { NUDSlippage.Value = value; }
        }

        /// <summary>
        /// Whether to edit the instrument
        /// </summary>
        public bool EditInstrument
        {
            get { return _editInstrument; }
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(350, 208);

            BtnAccept.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;
            const int border = 2;
            const int nudWidth = 70;

            // pnlBase
            PnlBase.Size = new Size(ClientSize.Width - 2*space,
                                    ClientSize.Height - 2*btnVertSpace - buttonHeight - space);
            PnlBase.Location = new Point(space, space);

            // Labels
            LblSpread.Location = new Point(btnHrzSpace + border, 0*buttonHeight + 1*space + 8);
            LblSwapLong.Location = new Point(btnHrzSpace + border, 1*buttonHeight + 2*space + 2);
            LblSwapShort.Location = new Point(btnHrzSpace + border, 2*buttonHeight + 3*space + 2);
            LblCommission.Location = new Point(btnHrzSpace + border, 3*buttonHeight + 4*space + 8);
            LblSlippage.Location = new Point(btnHrzSpace + border, 4*buttonHeight + 5*space + 8);

            // NUD Parameters
            int nudLeft = PnlBase.ClientSize.Width - nudWidth - btnHrzSpace - border;
            NUDSpread.Size = new Size(nudWidth, buttonHeight);
            NUDSpread.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 6);
            NUDSwapLong.Size = new Size(nudWidth, buttonHeight);
            NUDSwapLong.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 6);
            NUDSwapShort.Size = new Size(nudWidth, buttonHeight);
            NUDSwapShort.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 6);
            NUDCommission.Size = new Size(nudWidth, buttonHeight);
            NUDCommission.Location = new Point(nudLeft, 3*buttonHeight + 4*space + 6);
            NUDSlippage.Size = new Size(nudWidth, buttonHeight);
            NUDSlippage.Location = new Point(nudLeft, 4*buttonHeight + 5*space + 6);

            // Button btnEditInstrument
            BtnEditInstrument.Size = new Size(buttonWidth, buttonHeight);
            BtnEditInstrument.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            //btnDefault.Size     = new Size(buttonWidth, buttonHeight);
            //btnDefault.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Resize if necessary
            int iMaxLblRight = LblSpread.Right;
            if (LblSwapLong.Right > iMaxLblRight) iMaxLblRight = LblSwapLong.Right;
            if (LblSwapShort.Right > iMaxLblRight) iMaxLblRight = LblSwapShort.Right;
            if (LblCommission.Right > iMaxLblRight) iMaxLblRight = LblCommission.Right;
            if (LblSlippage.Right > iMaxLblRight) iMaxLblRight = LblSlippage.Right;

            if (nudLeft - iMaxLblRight < btnVertSpace)
                Width += btnVertSpace - nudLeft + iMaxLblRight;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Shows Instrument Editor
        /// </summary>
        private void BtnEditInstrumentClick(object sender, EventArgs e)
        {
            _editInstrument = true;
            Close();
        }
    }
}