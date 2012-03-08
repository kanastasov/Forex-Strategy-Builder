// StrategyProperties Form
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
    public sealed class StrategyProperties : Form
    {
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnDefault { get; set; }
        private ComboBox CbxOppDirAction { get; set; }
        private ComboBox CbxPermaSLType { get; set; }
        private ComboBox CbxPermaTPType { get; set; }
        private ComboBox CbxSameDirAction { get; set; }
        private CheckBox CbxUseMartingale { get; set; }
        private CheckBox ChbBreakEven { get; set; }
        private CheckBox ChbPermaSL { get; set; }
        private CheckBox ChbPermaTP { get; set; }
        private Color ColorText { get; set; }
        private Label LblAddingLots { get; set; }
        private Label LblEntryLots { get; set; }
        private Label LblMaxOpenLots { get; set; }
        private Label LblOppDirAction { get; set; }
        private Label LblPercent1 { get; set; }
        private Label LblPercent2 { get; set; }
        private Label LblPercent3 { get; set; }
        private Label LblReducingLots { get; set; }
        private Label LblSameDirAction { get; set; }
        private NUD NUDAddingLots { get; set; }
        private NUD NUDBreakEven { get; set; }
        private NUD NUDEntryLots { get; set; }
        private NUD NUDMartingaleMultiplier { get; set; }
        private NUD NUDMaxOpenLots { get; set; }
        private NUD NUDPermaSL { get; set; }
        private NUD NUDPermaTP { get; set; }
        private NUD NUDReducingLots { get; set; }
        private FancyPanel PnlAmounts { get; set; }
        private FancyPanel PnlAveraging { get; set; }
        private FancyPanel PnlProtection { get; set; }
        private SmallBalanceChart BalanceChart { get; set; }
        private RadioButton RbConstantUnits { get; set; }
        private RadioButton RbVariableUnits { get; set; }


        private int _leftPanelsWidth;
        private int _rightPanelsWidth;

        private readonly Backtester _backtester;

        public StrategyProperties(Backtester backtester)
        {
            _backtester = backtester;
            PnlAveraging = new FancyPanel(Language.T("Handling of Additional Entry Signals"), LayoutColors.ColorSlotCaptionBackAveraging);
            PnlAmounts = new FancyPanel(Language.T("Trading Size"), LayoutColors.ColorSlotCaptionBackAveraging);
            PnlProtection = new FancyPanel(Language.T("Permanent Protection"), LayoutColors.ColorSlotCaptionBackAveraging);
            BalanceChart = new SmallBalanceChart();

            LblPercent1 = new Label();
            LblPercent2 = new Label();
            LblPercent3 = new Label();

            LblSameDirAction = new Label();
            LblOppDirAction = new Label();

            CbxSameDirAction = new ComboBox();
            CbxOppDirAction = new ComboBox();
            NUDMaxOpenLots = new NUD();
            RbConstantUnits = new RadioButton();
            RbVariableUnits = new RadioButton();
            NUDEntryLots = new NUD();
            NUDAddingLots = new NUD();
            NUDReducingLots = new NUD();
            LblMaxOpenLots = new Label();
            LblEntryLots = new Label();
            LblAddingLots = new Label();
            LblReducingLots = new Label();
            CbxUseMartingale = new CheckBox();
            NUDMartingaleMultiplier = new NUD();

            ChbPermaSL = new CheckBox();
            CbxPermaSLType = new ComboBox();
            NUDPermaSL = new NUD();
            ChbPermaTP = new CheckBox();
            CbxPermaTPType = new ComboBox();
            NUDPermaTP = new NUD();
            ChbBreakEven = new CheckBox();
            NUDBreakEven = new NUD();

            BtnAccept = new Button();
            BtnDefault = new Button();
            BtnCancel = new Button();

            ColorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Strategy Properties");

            PnlAveraging.Parent = this;
            PnlAmounts.Parent = this;
            PnlProtection.Parent = this;
            BalanceChart.Parent = this;
            BalanceChart.SetChartData(_backtester);
        
            var toolTip = new ToolTip();

            // Label Same dir action
            LblSameDirAction.Parent = PnlAveraging;
            LblSameDirAction.ForeColor = ColorText;
            LblSameDirAction.BackColor = Color.Transparent;
            LblSameDirAction.AutoSize = true;
            LblSameDirAction.Text = Language.T("Next same direction signal behavior");

            // Label Opposite dir action
            LblOppDirAction.Parent = PnlAveraging;
            LblOppDirAction.ForeColor = ColorText;
            LblOppDirAction.BackColor = Color.Transparent;
            LblOppDirAction.AutoSize = true;
            LblOppDirAction.Text = Language.T("Next opposite direction signal behavior");

            // ComboBox SameDirAction
            CbxSameDirAction.Parent = PnlAveraging;
            CbxSameDirAction.Name = "cbxSameDirAction";
            CbxSameDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            var sameItems = new[] {Language.T("Nothing"), Language.T("Winner"), Language.T("Add")};
            foreach (var item in sameItems)
                CbxSameDirAction.Items.Add(item);
            CbxSameDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(CbxSameDirAction,
                               Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                               Language.T("Winner - adds to a winning position.") + Environment.NewLine +
                               Language.T("Add - adds to all positions."));

            // ComboBox OppDirAction
            CbxOppDirAction.Parent = PnlAveraging;
            CbxOppDirAction.Name = "cbxOppDirAction";
            CbxOppDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            var oppItems = new[] { Language.T("Nothing"), Language.T("Reduce"), Language.T("Close"), Language.T("Reverse") };
            foreach (var item in oppItems)
                CbxOppDirAction.Items.Add(item);
            CbxOppDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(CbxOppDirAction,
                               Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                               Language.T("Reduce - reduces or closes a position.") + Environment.NewLine +
                               Language.T("Close - closes the position.") + Environment.NewLine +
                               Language.T("Reverse - reverses the position."));

            // Label MaxOpen Lots
            LblMaxOpenLots.Parent = PnlAmounts;
            LblMaxOpenLots.ForeColor = ColorText;
            LblMaxOpenLots.BackColor = Color.Transparent;
            LblMaxOpenLots.AutoSize = true;
            LblMaxOpenLots.Text = Language.T("Maximum number of open lots");

            // NumericUpDown MaxOpen Lots
            NUDMaxOpenLots.Parent = PnlAmounts;
            NUDMaxOpenLots.Name = "nudMaxOpenLots";
            NUDMaxOpenLots.BeginInit();
            NUDMaxOpenLots.Minimum = 0.01M;
            NUDMaxOpenLots.Maximum = 100;
            NUDMaxOpenLots.Increment = 0.01M;
            NUDMaxOpenLots.Value = 20;
            NUDMaxOpenLots.DecimalPlaces = 2;
            NUDMaxOpenLots.TextAlign = HorizontalAlignment.Center;
            NUDMaxOpenLots.EndInit();

            // Radio Button Constant Units
            RbConstantUnits.Parent = PnlAmounts;
            RbConstantUnits.ForeColor = ColorText;
            RbConstantUnits.BackColor = Color.Transparent;
            RbConstantUnits.Checked = true;
            RbConstantUnits.AutoSize = true;
            RbConstantUnits.Name = "rbConstantUnits";
            RbConstantUnits.Text = Language.T("Trade a constant number of lots");

            // Radio Button Variable Units
            RbVariableUnits.Parent = PnlAmounts;
            RbVariableUnits.ForeColor = ColorText;
            RbVariableUnits.BackColor = Color.Transparent;
            RbVariableUnits.Checked = false;
            RbVariableUnits.AutoSize = false;
            RbVariableUnits.Name = "rbVariableUnits";
            RbVariableUnits.Text = Language.T("Trade percent of your account. The percentage values show the part of the account equity used to cover the required margin.");

            // Label Entry Lots
            LblEntryLots.Parent = PnlAmounts;
            LblEntryLots.ForeColor = ColorText;
            LblEntryLots.BackColor = Color.Transparent;
            LblEntryLots.AutoSize = true;
            LblEntryLots.Text = Language.T("Number of entry lots for a new position");

            // NumericUpDown Entry Lots
            NUDEntryLots.Parent = PnlAmounts;
            NUDEntryLots.Name = "nudEntryLots";
            NUDEntryLots.BeginInit();
            NUDEntryLots.Minimum = 0.01M;
            NUDEntryLots.Maximum = 100;
            NUDEntryLots.Increment = 0.01M;
            NUDEntryLots.Value = 1;
            NUDEntryLots.DecimalPlaces = 2;
            NUDEntryLots.TextAlign = HorizontalAlignment.Center;
            NUDEntryLots.EndInit();

            // Label Entry Lots %
            LblPercent1.Parent = PnlAmounts;
            LblPercent1.ForeColor = ColorText;
            LblPercent1.BackColor = Color.Transparent;

            // Label Adding Lots
            LblAddingLots.Parent = PnlAmounts;
            LblAddingLots.ForeColor = ColorText;
            LblAddingLots.BackColor = Color.Transparent;
            LblAddingLots.AutoSize = true;
            LblAddingLots.Text = Language.T("In case of addition - number of lots to add");

            // NumericUpDown Adding Lots
            NUDAddingLots.Parent = PnlAmounts;
            NUDAddingLots.Name = "nudAddingLots";
            NUDAddingLots.BeginInit();
            NUDAddingLots.Minimum = 0.01M;
            NUDAddingLots.Maximum = 100;
            NUDAddingLots.Increment = 0.01M;
            NUDAddingLots.Value = 1;
            NUDAddingLots.DecimalPlaces = 2;
            NUDAddingLots.TextAlign = HorizontalAlignment.Center;
            NUDAddingLots.EndInit();

            // Label Adding Lots %
            LblPercent2.Parent = PnlAmounts;
            LblPercent2.ForeColor = ColorText;
            LblPercent2.BackColor = Color.Transparent;

            // Label Reducing Lots
            LblReducingLots.Parent = PnlAmounts;
            LblReducingLots.ForeColor = ColorText;
            LblReducingLots.BackColor = Color.Transparent;
            LblReducingLots.AutoSize = true;
            LblReducingLots.Text = Language.T("In case of reduction - number of lots to close");

            // NumericUpDown Reducing Lots
            NUDReducingLots.Parent = PnlAmounts;
            NUDReducingLots.Name = "nudReducingLots";
            NUDReducingLots.BeginInit();
            NUDReducingLots.Minimum = 0.01M;
            NUDReducingLots.Maximum = 100;
            NUDReducingLots.Increment = 0.01m;
            NUDReducingLots.DecimalPlaces = 2;
            NUDReducingLots.Value = 1;
            NUDReducingLots.TextAlign = HorizontalAlignment.Center;
            NUDReducingLots.EndInit();

            // CheckBox Use Martingale
            CbxUseMartingale.Name = "cbxUseMartingale";
            CbxUseMartingale.Parent = PnlAmounts;
            CbxUseMartingale.ForeColor = ColorText;
            CbxUseMartingale.BackColor = Color.Transparent;
            CbxUseMartingale.AutoCheck = true;
            CbxUseMartingale.AutoSize = true;
            CbxUseMartingale.Checked = false;
            CbxUseMartingale.Text = Language.T("Martingale money management multiplier");

            // NumericUpDown Martingale Multiplier
            NUDMartingaleMultiplier.Parent = PnlAmounts;
            NUDMartingaleMultiplier.Name = "nudMartingaleMultiplier";
            NUDMartingaleMultiplier.BeginInit();
            NUDMartingaleMultiplier.Minimum = 0.01M;
            NUDMartingaleMultiplier.Maximum = 10;
            NUDMartingaleMultiplier.Increment = 0.01m;
            NUDMartingaleMultiplier.DecimalPlaces = 2;
            NUDMartingaleMultiplier.Value = 2;
            NUDMartingaleMultiplier.TextAlign = HorizontalAlignment.Center;
            NUDMartingaleMultiplier.EndInit();


            // Label Reducing Lots %
            LblPercent3.Parent = PnlAmounts;
            LblPercent3.ForeColor = ColorText;
            LblPercent3.BackColor = Color.Transparent;

            // CheckBox Permanent Stop Loss
            ChbPermaSL.Name = "chbPermaSL";
            ChbPermaSL.Parent = PnlProtection;
            ChbPermaSL.ForeColor = ColorText;
            ChbPermaSL.BackColor = Color.Transparent;
            ChbPermaSL.AutoCheck = true;
            ChbPermaSL.AutoSize = true;
            ChbPermaSL.Checked = false;
            ChbPermaSL.Text = Language.T("Permanent Stop Loss");

            // ComboBox cbxPermaSLType
            CbxPermaSLType.Parent = PnlProtection;
            CbxPermaSLType.Name = "cbxPermaSLType";
            CbxPermaSLType.Visible = false;
            CbxPermaSLType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPermaSLType.Items.Add(Language.T("Relative"));
            CbxPermaSLType.Items.Add(Language.T("Absolute"));
            CbxPermaSLType.SelectedIndex = 0;

            // NumericUpDown Permanent S/L
            NUDPermaSL.Name = "nudPermaSL";
            NUDPermaSL.Parent = PnlProtection;
            NUDPermaSL.BeginInit();
            NUDPermaSL.Minimum = 5;
            NUDPermaSL.Maximum = 5000;
            NUDPermaSL.Increment = 1;
            NUDPermaSL.Value = Data.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            NUDPermaSL.TextAlign = HorizontalAlignment.Center;
            NUDPermaSL.EndInit();

            // CheckBox Permanent Take Profit
            ChbPermaTP.Name = "chbPermaTP";
            ChbPermaTP.Parent = PnlProtection;
            ChbPermaTP.ForeColor = ColorText;
            ChbPermaTP.BackColor = Color.Transparent;
            ChbPermaTP.AutoCheck = true;
            ChbPermaTP.AutoSize = true;
            ChbPermaTP.Checked = false;
            ChbPermaTP.Text = Language.T("Permanent Take Profit");

            // ComboBox cbxPermaTPType
            CbxPermaTPType.Parent = PnlProtection;
            CbxPermaTPType.Name = "cbxPermaTPType";
            CbxPermaTPType.Visible = false;
            CbxPermaTPType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPermaTPType.Items.Add(Language.T("Relative"));
            CbxPermaTPType.Items.Add(Language.T("Absolute"));
            CbxPermaTPType.SelectedIndex = 0;

            // NumericUpDown Permanent Take Profit
            NUDPermaTP.Parent = PnlProtection;
            NUDPermaTP.Name = "nudPermaTP";
            NUDPermaTP.BeginInit();
            NUDPermaTP.Minimum = 5;
            NUDPermaTP.Maximum = 5000;
            NUDPermaTP.Increment = 1;
            NUDPermaTP.Value = Data.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            NUDPermaTP.TextAlign = HorizontalAlignment.Center;
            NUDPermaTP.EndInit();

            // CheckBox Break Even
            ChbBreakEven.Name = "chbBreakEven";
            ChbBreakEven.Parent = PnlProtection;
            ChbBreakEven.ForeColor = ColorText;
            ChbBreakEven.BackColor = Color.Transparent;
            ChbBreakEven.AutoCheck = true;
            ChbBreakEven.AutoSize = true;
            ChbBreakEven.Checked = false;
            ChbBreakEven.Text = Language.T("Break Even");

            // NumericUpDown Break Even
            NUDBreakEven.Parent = PnlProtection;
            NUDBreakEven.Name = "nudBreakEven";
            NUDBreakEven.BeginInit();
            NUDBreakEven.Minimum = 5;
            NUDBreakEven.Maximum = 5000;
            NUDBreakEven.Increment = 1;
            NUDBreakEven.Value = Data.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            NUDBreakEven.TextAlign = HorizontalAlignment.Center;
            NUDBreakEven.EndInit();

            //Button Default
            BtnDefault.Parent = this;
            BtnDefault.Name = "btnDefault";
            BtnDefault.Text = Language.T("Default");
            BtnDefault.Click += BtnDefaultClick;
            BtnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Name = "btnCancel";
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "btnAccept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetFormSize();
            SetParams();
            BtnAccept.Focus();
        }

        private void SetFormSize()
        {
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;
            const int leftComboBxWith = 80;
            const int rightComboBxWith = 95;
            const int nudWidth = 60;
            const int lblPercentWidth = 15;
            const int border = 2;

            _leftPanelsWidth = 3*buttonWidth + 2*btnHrzSpace;
            _rightPanelsWidth = 3*buttonWidth + 2*btnHrzSpace;

            if (_leftPanelsWidth < space + LblSameDirAction.Width + space + leftComboBxWith + space)
                _leftPanelsWidth = space + LblSameDirAction.Width + space + leftComboBxWith + space;

            if (_leftPanelsWidth < space + LblOppDirAction.Width + space + leftComboBxWith + space)
                _leftPanelsWidth = space + LblOppDirAction.Width + space + leftComboBxWith + space;

            if (_leftPanelsWidth < space + LblMaxOpenLots.Width + space + nudWidth + space)
                _leftPanelsWidth = space + LblMaxOpenLots.Width + space + nudWidth + space;

            RbVariableUnits.Width = _leftPanelsWidth - 2*space;
            Graphics g = CreateGraphics();
            while (g.MeasureString(RbVariableUnits.Text, RbVariableUnits.Font, RbVariableUnits.Width - 10).Height >
                   3*RbVariableUnits.Font.Height)
                RbVariableUnits.Width++;
            g.Dispose();
            if (_leftPanelsWidth < space + RbVariableUnits.Width + space)
                _leftPanelsWidth = space + RbVariableUnits.Width + space;

            if (_leftPanelsWidth < space + LblEntryLots.Width + space + lblPercentWidth + nudWidth + space)
                _leftPanelsWidth = space + LblEntryLots.Width + space + lblPercentWidth + nudWidth + space;

            if (_leftPanelsWidth < space + LblAddingLots.Width + space + lblPercentWidth + nudWidth + space)
                _leftPanelsWidth = space + LblAddingLots.Width + space + lblPercentWidth + nudWidth + space;

            if (_leftPanelsWidth < space + LblReducingLots.Width + space + lblPercentWidth + nudWidth + space)
                _leftPanelsWidth = space + LblReducingLots.Width + space + lblPercentWidth + nudWidth + space;

            int maxRightCheckBoxWidth = Math.Max(ChbPermaSL.Width, ChbPermaTP.Width);
            int requiredRightPanelWidth = border + space + maxRightCheckBoxWidth + space + rightComboBxWith + space +
                                          nudWidth + space + border;
            if (_rightPanelsWidth < requiredRightPanelWidth)
                _rightPanelsWidth = requiredRightPanelWidth;

            ClientSize = new Size(space + _leftPanelsWidth + space + _rightPanelsWidth + space, 390);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int buttonWidth = ((_rightPanelsWidth - 2*btnHrzSpace)/3);
            int space = btnHrzSpace;
            const int border = 2;
            const int leftComboBxWith = 80;
            const int rightComboBxWith = 95;
            const int nudWidth = 60;
            const int lblPercentWidth = 15;

            // pnlAveraging
            PnlAveraging.Size = new Size(_leftPanelsWidth, 84);
            PnlAveraging.Location = new Point(space, space);

            // pnlAmounts
            PnlAmounts.Size = new Size(_leftPanelsWidth, 252);
            PnlAmounts.Location = new Point(space, PnlAveraging.Bottom + space);

            // pnlProtection
            PnlProtection.Size = new Size(_rightPanelsWidth, 110);
            PnlProtection.Location = new Point(PnlAveraging.Right + space, PnlAveraging.Top);

            // Averaging
            int comboBxLeft = PnlAveraging.ClientSize.Width - leftComboBxWith - space - border;

            CbxSameDirAction.Width = leftComboBxWith;
            LblSameDirAction.Location = new Point(space, space + 25);
            CbxSameDirAction.Location = new Point(comboBxLeft, space + 21);

            CbxOppDirAction.Width = leftComboBxWith;
            LblOppDirAction.Location = new Point(space, buttonHeight + 2*space + 23);
            CbxOppDirAction.Location = new Point(comboBxLeft, buttonHeight + 2*space + 19);

            // Amounts
            int nudLeft = _leftPanelsWidth - nudWidth - space - border;

            LblMaxOpenLots.Location = new Point(space, 0*buttonHeight + space + 25);
            NUDMaxOpenLots.Size = new Size(nudWidth, buttonHeight);
            NUDMaxOpenLots.Location = new Point(nudLeft, 0*buttonHeight + space + 22);

            RbConstantUnits.Location = new Point(space + 3, 55);
            RbVariableUnits.Location = new Point(space + 3, 79);
            RbVariableUnits.Size = new Size(_leftPanelsWidth - 2*space, 2*buttonHeight);

            LblEntryLots.Location = new Point(btnHrzSpace, 139);
            NUDEntryLots.Size = new Size(nudWidth, buttonHeight);
            NUDEntryLots.Location = new Point(nudLeft, 137);
            LblPercent1.Width = lblPercentWidth;
            LblPercent1.Location = new Point(NUDEntryLots.Left - lblPercentWidth, LblEntryLots.Top);

            LblAddingLots.Location = new Point(btnHrzSpace, 167);
            NUDAddingLots.Size = new Size(nudWidth, buttonHeight);
            NUDAddingLots.Location = new Point(nudLeft, 165);
            LblPercent2.Width = lblPercentWidth;
            LblPercent2.Location = new Point(NUDAddingLots.Left - lblPercentWidth, LblAddingLots.Top);

            LblReducingLots.Location = new Point(btnHrzSpace, 195);
            NUDReducingLots.Size = new Size(nudWidth, buttonHeight);
            NUDReducingLots.Location = new Point(nudLeft, 193);
            LblPercent3.Width = lblPercentWidth;
            LblPercent3.Location = new Point(NUDReducingLots.Left - lblPercentWidth, LblReducingLots.Top);

            CbxUseMartingale.Location = new Point(btnHrzSpace + 2, 223);
            NUDMartingaleMultiplier.Size = new Size(nudWidth, buttonHeight);
            NUDMartingaleMultiplier.Location = new Point(nudLeft, 221);

            nudLeft = _rightPanelsWidth - nudWidth - btnHrzSpace - border;
            comboBxLeft = nudLeft - space - rightComboBxWith;

            // Permanent Stop Loss
            ChbPermaSL.Location = new Point(border + space, 0*buttonHeight + 1*space + 24);
            CbxPermaSLType.Width = rightComboBxWith;
            CbxPermaSLType.Location = new Point(comboBxLeft, 0*buttonHeight + 1*space + 21);
            NUDPermaSL.Size = new Size(nudWidth, buttonHeight);
            NUDPermaSL.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 22);

            // Permanent Take Profit
            ChbPermaTP.Location = new Point(border + space, 1*buttonHeight + 2*space + 22);
            NUDPermaTP.Size = new Size(nudWidth, buttonHeight);
            CbxPermaTPType.Width = rightComboBxWith;
            CbxPermaTPType.Location = new Point(comboBxLeft, 1*buttonHeight + 2*space + 19);
            NUDPermaTP.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 20);

            // Break Even
            ChbBreakEven.Location = new Point(border + space, 2*buttonHeight + 3*space + 20);
            NUDBreakEven.Size = new Size(nudWidth, buttonHeight);
            NUDBreakEven.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 18);

            BalanceChart.Size = new Size(_rightPanelsWidth, PnlAmounts.Bottom - PnlProtection.Bottom - space);
            BalanceChart.Location = new Point(PnlAveraging.Right + space, PnlProtection.Bottom + space);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnDefault.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            int buttonTop = ClientSize.Height - buttonHeight - btnVertSpace;

            BtnAccept.Location = new Point(PnlProtection.Left, buttonTop);
            BtnDefault.Location = new Point((PnlProtection.Left + PnlProtection.Right - buttonWidth)/2, buttonTop);
            BtnCancel.Location = new Point(PnlProtection.Right - buttonWidth, buttonTop);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        private void BtnDefaultClick(object sender, EventArgs e)
        {
            _backtester.Strategy.SameSignalAction = SameDirSignalAction.Nothing;

            _backtester.Strategy.OppSignalAction = _backtester.Strategy.Slot[_backtester.Strategy.CloseSlot].IndicatorName == "Close and Reverse" 
                ? OppositeDirSignalAction.Reverse 
                : OppositeDirSignalAction.Nothing;

            _backtester.Strategy.UsePermanentSL = false;
            _backtester.Strategy.PermanentSL = Data.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            _backtester.Strategy.PermanentSLType = PermanentProtectionType.Relative;
            _backtester.Strategy.UsePermanentTP = false;
            _backtester.Strategy.PermanentTP = Data.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            _backtester.Strategy.PermanentTPType = PermanentProtectionType.Relative;
            _backtester.Strategy.UseBreakEven = false;
            _backtester.Strategy.BreakEven = Data.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            _backtester.Strategy.UseAccountPercentEntry = false;
            _backtester.Strategy.MaxOpenLots = 20;
            _backtester.Strategy.EntryLots = 1;
            _backtester.Strategy.AddingLots = 1;
            _backtester.Strategy.ReducingLots = 1;
            _backtester.Strategy.UseMartingale = false;
            _backtester.Strategy.MartingaleMultiplier = 2;

            SetParams();
            CalculateStrategy();
            UpdateChart();
        }

        private void ParamChanged(object sender, EventArgs e)
        {
            NUDPermaSL.Enabled = ChbPermaSL.Checked;
            NUDPermaTP.Enabled = ChbPermaTP.Checked;
            NUDBreakEven.Enabled = ChbBreakEven.Checked;
            CbxPermaSLType.Enabled = ChbPermaSL.Checked;
            CbxPermaTPType.Enabled = ChbPermaTP.Checked;
            NUDMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;

            if (!RbVariableUnits.Checked)
                NUDEntryLots.Value = Math.Min(NUDEntryLots.Value, NUDMaxOpenLots.Value);

            _backtester.Strategy.SameSignalAction = (SameDirSignalAction) CbxSameDirAction.SelectedIndex;
            _backtester.Strategy.OppSignalAction = (OppositeDirSignalAction) CbxOppDirAction.SelectedIndex;
            _backtester.Strategy.UseAccountPercentEntry = RbVariableUnits.Checked;
            _backtester.Strategy.MaxOpenLots = (double) NUDMaxOpenLots.Value;
            _backtester.Strategy.EntryLots = (double) NUDEntryLots.Value;
            _backtester.Strategy.AddingLots = (double) NUDAddingLots.Value;
            _backtester.Strategy.ReducingLots = (double) NUDReducingLots.Value;
            _backtester.Strategy.UsePermanentSL = ChbPermaSL.Checked;
            _backtester.Strategy.UsePermanentTP = ChbPermaTP.Checked;
            _backtester.Strategy.UseBreakEven = ChbBreakEven.Checked;
            _backtester.Strategy.PermanentSLType = (PermanentProtectionType) CbxPermaSLType.SelectedIndex;
            _backtester.Strategy.PermanentTPType = (PermanentProtectionType) CbxPermaTPType.SelectedIndex;
            _backtester.Strategy.PermanentSL = (int) NUDPermaSL.Value;
            _backtester.Strategy.PermanentTP = (int) NUDPermaTP.Value;
            _backtester.Strategy.BreakEven = (int) NUDBreakEven.Value;
            _backtester.Strategy.UseMartingale = CbxUseMartingale.Checked;
            _backtester.Strategy.MartingaleMultiplier = (double) NUDMartingaleMultiplier.Value;

            SetLabelPercent();
            CalculateStrategy();
            UpdateChart();
        }

        private void SetParams()
        {
            RemoveParamEventHandlers();

            CbxSameDirAction.SelectedIndex = (int) _backtester.Strategy.SameSignalAction;
            CbxOppDirAction.SelectedIndex = (int) _backtester.Strategy.OppSignalAction;
            CbxOppDirAction.Enabled = _backtester.Strategy.Slot[_backtester.Strategy.CloseSlot].IndicatorName != "Close and Reverse";

            RbConstantUnits.Checked = !_backtester.Strategy.UseAccountPercentEntry;
            RbVariableUnits.Checked = _backtester.Strategy.UseAccountPercentEntry;

            NUDMaxOpenLots.Value = (decimal) _backtester.Strategy.MaxOpenLots;

            if (!RbVariableUnits.Checked)
                NUDEntryLots.Value = (decimal) Math.Min(_backtester.Strategy.EntryLots, _backtester.Strategy.MaxOpenLots);
            else
                NUDEntryLots.Value = (decimal) _backtester.Strategy.EntryLots;

            NUDAddingLots.Value = (decimal) _backtester.Strategy.AddingLots;
            NUDReducingLots.Value = (decimal) _backtester.Strategy.ReducingLots;

            CbxUseMartingale.Checked = _backtester.Strategy.UseMartingale;
            NUDMartingaleMultiplier.Value = (decimal) _backtester.Strategy.MartingaleMultiplier;
            NUDMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;

            ChbPermaSL.Checked = _backtester.Strategy.UsePermanentSL;
            NUDPermaSL.Value = _backtester.Strategy.PermanentSL;
            NUDPermaSL.Enabled = _backtester.Strategy.UsePermanentSL;
            CbxPermaSLType.Enabled = _backtester.Strategy.UsePermanentSL;
            CbxPermaSLType.SelectedIndex = (int) _backtester.Strategy.PermanentSLType;

            ChbPermaTP.Checked = _backtester.Strategy.UsePermanentTP;
            NUDPermaTP.Value = _backtester.Strategy.PermanentTP;
            NUDPermaTP.Enabled = _backtester.Strategy.UsePermanentTP;
            CbxPermaTPType.Enabled = _backtester.Strategy.UsePermanentTP;
            CbxPermaTPType.SelectedIndex = (int) _backtester.Strategy.PermanentTPType;

            ChbBreakEven.Checked = _backtester.Strategy.UseBreakEven;
            NUDBreakEven.Value = _backtester.Strategy.BreakEven;
            NUDBreakEven.Enabled = _backtester.Strategy.UseBreakEven;

            SetParamEventHandlers();
            SetLabelPercent();
        }

        private void SetParamEventHandlers()
        {
            CbxSameDirAction.SelectedIndexChanged += ParamChanged;
            CbxOppDirAction.SelectedIndexChanged += ParamChanged;
            RbConstantUnits.CheckedChanged += ParamChanged;
            RbVariableUnits.CheckedChanged += ParamChanged;
            NUDMaxOpenLots.ValueChanged += ParamChanged;
            NUDEntryLots.ValueChanged += ParamChanged;
            NUDAddingLots.ValueChanged += ParamChanged;
            NUDReducingLots.ValueChanged += ParamChanged;
            ChbPermaSL.CheckedChanged += ParamChanged;
            CbxPermaSLType.SelectedIndexChanged += ParamChanged;
            NUDPermaSL.ValueChanged += ParamChanged;
            ChbPermaTP.CheckedChanged += ParamChanged;
            CbxPermaTPType.SelectedIndexChanged += ParamChanged;
            NUDPermaTP.ValueChanged += ParamChanged;
            NUDBreakEven.ValueChanged += ParamChanged;
            ChbBreakEven.CheckedChanged += ParamChanged;
            CbxUseMartingale.CheckedChanged += ParamChanged;
            NUDMartingaleMultiplier.ValueChanged += ParamChanged;
        }

        private void RemoveParamEventHandlers()
        {
            CbxSameDirAction.SelectedIndexChanged -= ParamChanged;
            CbxOppDirAction.SelectedIndexChanged -= ParamChanged;
            RbConstantUnits.CheckedChanged -= ParamChanged;
            RbVariableUnits.CheckedChanged -= ParamChanged;
            NUDMaxOpenLots.ValueChanged -= ParamChanged;
            NUDEntryLots.ValueChanged -= ParamChanged;
            NUDAddingLots.ValueChanged -= ParamChanged;
            NUDReducingLots.ValueChanged -= ParamChanged;
            ChbPermaSL.CheckedChanged -= ParamChanged;
            CbxPermaSLType.SelectedIndexChanged -= ParamChanged;
            NUDPermaSL.ValueChanged -= ParamChanged;
            ChbPermaTP.CheckedChanged -= ParamChanged;
            CbxPermaTPType.SelectedIndexChanged -= ParamChanged;
            NUDPermaTP.ValueChanged -= ParamChanged;
            NUDBreakEven.ValueChanged -= ParamChanged;
            ChbBreakEven.CheckedChanged -= ParamChanged;
            CbxUseMartingale.CheckedChanged -= ParamChanged;
            NUDMartingaleMultiplier.ValueChanged -= ParamChanged;
        }

        private void SetLabelPercent()
        {
            string text = _backtester.Strategy.UseAccountPercentEntry ? "%" : "";
            LblPercent1.Text = text;
            LblPercent2.Text = text;
            LblPercent3.Text = text;
        }

        private void CalculateStrategy()
        {
            _backtester.Calculate(_backtester.Strategy, Data.DataSet);
            _backtester.CalculateAccountStats();
        }

        private void UpdateChart()
        {
            BalanceChart.SetChartData(_backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
        }
    }
}