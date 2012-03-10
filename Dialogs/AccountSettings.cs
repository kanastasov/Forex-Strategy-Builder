// Account Settings class
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
    /// Account Settings
    /// </summary>
    public sealed class AccountSettings : Form
    {
        private readonly Color _colorText;
        private string _accountCurrency;
        private int _initialAccount;
        private int _leverage;
        private double _rateToEUR;
        private double _rateToUSD;
        private readonly Backtester _backtester;

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountSettings(Backtester backtester)
        {
            _backtester = backtester;

            PnlBase = new FancyPanel();

            LblAccountCurrency = new Label();
            LblInitialAccount = new Label();
            LblLeverage = new Label();
            LblExchangeRate = new Label();
            LblExchangeRateInfo = new Label();

            CbxAccountCurrency = new ComboBox();
            NUDInitialAccount = new NumericUpDown();
            CbxLeverage = new ComboBox();
            NUDExchangeRate = new NumericUpDown();
            TbxExchangeRate = new TextBox();

            BtnDefault = new Button();
            BtnCancel = new Button();
            BtnAccept = new Button();

            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Account Settings");

            // pnlBase
            PnlBase.Parent = this;

            // Label Account Currency
            LblAccountCurrency.Parent = PnlBase;
            LblAccountCurrency.ForeColor = _colorText;
            LblAccountCurrency.BackColor = Color.Transparent;
            LblAccountCurrency.Text = Language.T("Account currency");
            LblAccountCurrency.AutoSize = true;

            // Label Initial Account
            LblInitialAccount.Parent = PnlBase;
            LblInitialAccount.ForeColor = _colorText;
            LblInitialAccount.BackColor = Color.Transparent;
            LblInitialAccount.Text = Language.T("Initial account");
            LblInitialAccount.AutoSize = true;

            // Label Leverage
            LblLeverage.Parent = PnlBase;
            LblLeverage.ForeColor = _colorText;
            LblLeverage.BackColor = Color.Transparent;
            LblLeverage.Text = Language.T("Leverage");
            LblLeverage.AutoSize = true;

            // Label Exchange Rate
            LblExchangeRate.Parent = PnlBase;
            LblExchangeRate.ForeColor = _colorText;
            LblExchangeRate.BackColor = Color.Transparent;
            LblExchangeRate.Text = Language.T("Account exchange rate");
            LblExchangeRate.AutoSize = true;

            // Label Exchange Rate Info
            LblExchangeRateInfo.Parent = PnlBase;
            LblExchangeRateInfo.ForeColor = _colorText;
            LblExchangeRateInfo.BackColor = Color.Transparent;
            LblExchangeRateInfo.Text =
                Language.T("Forex Strategy Builder uses the account exchange rate to calculate the trading statistics in your account currency.") +
                " " +
                Language.T("When your account currency does not take part in the trading couple the account exchange rate is a fixed figure.");

            // ComboBox Account Currency
            CbxAccountCurrency.Parent = PnlBase;
            CbxAccountCurrency.Name = "cbxAccountCurrency";
            CbxAccountCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxAccountCurrency.Items.AddRange(new object[] {"USD", "EUR"});
            CbxAccountCurrency.SelectedIndex = 0;

            // NumericUpDown Initial Account
            NUDInitialAccount.Parent = PnlBase;
            NUDInitialAccount.Name = "nudInitialAccount";
            NUDInitialAccount.BeginInit();
            NUDInitialAccount.Minimum = 100;
            NUDInitialAccount.Maximum = 100000;
            NUDInitialAccount.Increment = 1000;
            NUDInitialAccount.Value = _initialAccount;
            NUDInitialAccount.EndInit();

            // ComboBox Leverage
            CbxLeverage.Parent = PnlBase;
            CbxLeverage.Name = "cbxLeverage";
            CbxLeverage.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxLeverage.Items.AddRange(new object[] { "1/1", "1/10", "1/20", "1/30", "1/50", "1/100", "1/200", "1/300", "1/400", "1/500" });
            CbxLeverage.SelectedIndex = 5;

            // tbxExchangeRate
            TbxExchangeRate.Parent = PnlBase;
            TbxExchangeRate.BackColor = LayoutColors.ColorControlBack;
            TbxExchangeRate.ForeColor = _colorText;
            TbxExchangeRate.ReadOnly = true;
            TbxExchangeRate.Visible = false;
            TbxExchangeRate.Text = Language.T("Deal price");

            // NumericUpDown Exchange Rate
            NUDExchangeRate.BeginInit();
            NUDExchangeRate.Parent = PnlBase;
            NUDExchangeRate.Name = "nudExchangeRate";
            NUDExchangeRate.Minimum = 0;
            NUDExchangeRate.Maximum = 100000;
            NUDExchangeRate.Increment = 0.0001M;
            NUDExchangeRate.DecimalPlaces = 4;
            NUDExchangeRate.Value = 1;
            NUDExchangeRate.EndInit();

            //Button Default
            BtnDefault.Parent = this;
            BtnDefault.Name = "Default";
            BtnDefault.Text = Language.T("Default");
            BtnDefault.Click += BtnDefaultClick;
            BtnDefault.UseVisualStyleBackColor = true;

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

        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnDefault { get; set; }
        private ComboBox CbxAccountCurrency { get; set; }
        private ComboBox CbxLeverage { get; set; }
        private Label LblAccountCurrency { get; set; }
        private Label LblExchangeRate { get; set; }
        private Label LblExchangeRateInfo { get; set; }
        private Label LblInitialAccount { get; set; }
        private Label LblLeverage { get; set; }

        private NumericUpDown NUDExchangeRate { get; set; }
        private NumericUpDown NUDInitialAccount { get; set; }
        private FancyPanel PnlBase { get; set; }
        private TextBox TbxExchangeRate { get; set; }

        /// <summary>
        /// Account Currency
        /// </summary>
        public string AccountCurrency
        {
            get { return _accountCurrency; }
            set { _accountCurrency = value; }
        }

        /// <summary>
        /// Initial Account
        /// </summary>
        public int InitialAccount
        {
            get { return _initialAccount; }
            set { _initialAccount = value; }
        }

        /// <summary>
        /// Leverage
        /// </summary>
        public int Leverage
        {
            get { return _leverage; }
            set { _leverage = value; }
        }

        /// <summary>
        /// Exchange Rate to USD
        /// </summary>
        public double RateToUSD
        {
            get { return _rateToUSD; }
            set { _rateToUSD = value; }
        }

        /// <summary>
        /// Exchange Rate to EUR
        /// </summary>
        public double RateToEUR
        {
            get { return _rateToEUR; }
            set { _rateToEUR = value; }
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);

            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 257);

            CbxAccountCurrency.SelectedIndexChanged += ParamChanged;
            NUDInitialAccount.ValueChanged += ParamChanged;
            CbxLeverage.SelectedIndexChanged += ParamChanged;
            NUDExchangeRate.ValueChanged += ParamChanged;

            BtnAccept.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;
            int nudWidth = buttonWidth - space - 1;
            const int border = 2;

            // pnlBase
            PnlBase.Size = new Size(ClientSize.Width - 2*space, ClientSize.Height - 2*btnVertSpace - buttonHeight - space);
            PnlBase.Location = new Point(space, space);

            // Labels
            LblAccountCurrency.Location = new Point(border + btnHrzSpace, 0*buttonHeight + 1*space + 8);
            LblInitialAccount.Location = new Point(border + btnHrzSpace, 1*buttonHeight + 2*space + 6);
            LblLeverage.Location = new Point(border + btnHrzSpace, 2*buttonHeight + 3*space + 8);
            LblExchangeRate.Location = new Point(border + btnHrzSpace, 3*buttonHeight + 4*space + 7);
            LblExchangeRateInfo.Location = new Point(border + btnHrzSpace, 4*buttonHeight + 5*space + 8);
            LblExchangeRateInfo.Size = new Size(PnlBase.ClientSize.Width - 2*space - 2*border,
                                                PnlBase.ClientSize.Height - border - space - LblExchangeRateInfo.Top);

            // Params
            int nudLeft = PnlBase.ClientSize.Width - nudWidth - btnHrzSpace - border;
            CbxAccountCurrency.Size = new Size(nudWidth, buttonHeight);
            CbxAccountCurrency.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 4);
            NUDInitialAccount.Size = new Size(nudWidth, buttonHeight);
            NUDInitialAccount.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 4);
            CbxLeverage.Size = new Size(nudWidth, buttonHeight);
            CbxLeverage.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 4);
            NUDExchangeRate.Size = new Size(nudWidth, buttonHeight);
            NUDExchangeRate.Location = new Point(nudLeft, 3*buttonHeight + 4*space + 4);
            TbxExchangeRate.Size = new Size(nudWidth, buttonHeight);
            TbxExchangeRate.Location = new Point(nudLeft, 3*buttonHeight + 4*space + 4);

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            BtnDefault.Size = new Size(buttonWidth, buttonHeight);
            BtnDefault.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                            ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnDefault.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Sets the controls' text
        /// </summary>
        public void SetParams()
        {
            // ComboBox Account Currency
            CbxAccountCurrency.SelectedItem = _accountCurrency;

            // NumericUpDown Initial Account
            NUDInitialAccount.Value = _initialAccount;

            // ComboBox Leverage
            CbxLeverage.SelectedItem = "1/" + _leverage;

            SetAcountExchangeRate();
        }

        /// <summary>
        /// Calculates the account exchange rate.
        /// </summary>
        private void SetAcountExchangeRate()
        {
            LblExchangeRate.Text = Language.T("Account exchange rate");

            if (_backtester.DataSet.InstrProperties.PriceIn == CbxAccountCurrency.Text)
            {
                TbxExchangeRate.Text = Language.T("Not used");
                TbxExchangeRate.Visible = true;
                NUDExchangeRate.Value = 1;
                NUDExchangeRate.Visible = false;
                if (CbxAccountCurrency.Text == "USD")
                    _rateToUSD = 1;
                else if (CbxAccountCurrency.Text == "EUR")
                    _rateToEUR = 1;
            }
            else if (_backtester.DataSet.InstrProperties.InstrType == InstrumetType.Forex &&
                     _backtester.DataSet.InstrProperties.Symbol.StartsWith(CbxAccountCurrency.Text))
            {
                TbxExchangeRate.Text = Language.T("Deal price");
                TbxExchangeRate.Visible = true;
                NUDExchangeRate.Value = 0;
                NUDExchangeRate.Visible = false;
                if (CbxAccountCurrency.Text == "USD")
                    _rateToUSD = 0;
                else if (CbxAccountCurrency.Text == "EUR")
                    _rateToEUR = 0;
            }
            else
            {
                LblExchangeRate.Text += " " + CbxAccountCurrency.Text + _backtester.DataSet.InstrProperties.PriceIn;
                TbxExchangeRate.Visible = false;
                if (CbxAccountCurrency.Text == "USD")
                    NUDExchangeRate.Value = (decimal) _rateToUSD;
                else if (CbxAccountCurrency.Text == "EUR")
                    NUDExchangeRate.Value = (decimal) _rateToEUR;
                NUDExchangeRate.Visible = true;
            }
        }

        /// <summary>
        /// Sets the params values
        /// </summary>
        private void ParamChanged(object sender, EventArgs e)
        {
            string name = ((Control) sender).Name;

            // ComboBox Account Currency
            if (name == "cbxAccountCurrency")
            {
                _accountCurrency = CbxAccountCurrency.Text;
                SetAcountExchangeRate();
            }

            // NumericUpDown Initial Account
            if (name == "nudInitialAccount")
            {
                _initialAccount = (int) NUDInitialAccount.Value;
            }

            // ComboBox Leverage
            if (name == "cbxLeverage")
            {
                _leverage = int.Parse(CbxLeverage.Text.Substring(2));
            }

            // NumericUpDown Exchange Rate
            if (name == "nudExchangeRate")
            {
                if (CbxAccountCurrency.Text == "USD")
                    _rateToUSD = (double) NUDExchangeRate.Value;
                else if (CbxAccountCurrency.Text == "EUR")
                    _rateToEUR = (double) NUDExchangeRate.Value;
            }
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        private void BtnDefaultClick(object sender, EventArgs e)
        {
            _accountCurrency = "USD";
            _initialAccount = 10000;
            _leverage = 100;

            SetParams();
        }
    }
}