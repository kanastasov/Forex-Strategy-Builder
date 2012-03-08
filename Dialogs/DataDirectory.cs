// Data Directory class
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
    /// DataDirectory
    /// </summary>
    public sealed class DataDirectory : Form
    {
        private readonly Color _colorText;
        private readonly Font _font;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataDirectory()
        {
            LblIntro = new Label();
            TxbDataDirectory = new TextBox();
            BtnBrowse = new Button();
            BtnDefault = new Button();
            BtnCancel = new Button();
            BtnAccept = new Button();

            _font = Font;
            _colorText = LayoutColors.ColorControlText;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Data Directory");

            // Label Intro
            LblIntro.Parent = this;
            LblIntro.ForeColor = _colorText;
            LblIntro.BackColor = Color.Transparent;
            LblIntro.Text = Language.T("Offline data directory:");

            // Data Directory
            TxbDataDirectory.Parent = this;
            TxbDataDirectory.BackColor = LayoutColors.ColorControlBack;
            TxbDataDirectory.ForeColor = _colorText;
            TxbDataDirectory.Text = Data.OfflineDataDir;

            //Button Browse
            BtnBrowse.Parent = this;
            BtnBrowse.Name = "Browse";
            BtnBrowse.Text = Language.T("Browse");
            BtnBrowse.Click += BtnBrowseClick;
            BtnBrowse.UseVisualStyleBackColor = true;

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

        private Label LblIntro { get; set; }
        private TextBox TxbDataDirectory { get; set; }
        private Button BtnBrowse { get; set; }
        private Button BtnDefault { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }

        /// <summary>
        /// Gets the selected Data Directory
        /// </summary>
        public string DataFolder
        {
            get { return TxbDataDirectory.Text; }
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width = 450;
            Height = 130;

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
            int border = btnHrzSpace;

            // Label Intro
            LblIntro.Size = new Size(ClientSize.Width - 2*btnVertSpace, _font.Height);
            LblIntro.Location = new Point(btnHrzSpace, btnVertSpace);

            //Button Browse
            BtnBrowse.Size = new Size(buttonWidth, buttonHeight);
            BtnBrowse.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, LblIntro.Bottom + border);

            //TextBox txbDataDirectory
            TxbDataDirectory.Width = BtnBrowse.Left - 2*btnHrzSpace;
            TxbDataDirectory.Location = new Point(btnHrzSpace,
                                                  BtnBrowse.Top + (buttonHeight - TxbDataDirectory.Height)/2);

            //Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Default
            BtnDefault.Size = new Size(buttonWidth, buttonHeight);
            BtnDefault.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                            ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Accept
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
        /// Button Browse Click
        /// </summary>
        private void BtnBrowseClick(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() != DialogResult.OK) return;
            TxbDataDirectory.Text = fd.SelectedPath;
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        private void BtnDefaultClick(object sender, EventArgs e)
        {
            TxbDataDirectory.Text = "";
        }
    }
}