// About Screen Form
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder
{
    public sealed class AboutScreen : Form
    {
        public AboutScreen()
        {
            PnlBase = new FancyPanel();
            Label1 = new Label();
            Label2 = new Label();
            Label3 = new Label();
            Label4 = new Label();
            Label5 = new Label();
            Label6 = new Label();
            PictureBox1 = new PictureBox();
            LlWebsite = new LinkLabel();
            LlForum = new LinkLabel();
            LlEmail = new LinkLabel();
            BtnOk = new Button();

            // Panel Base
            PnlBase.Parent = this;

            // pictureBox1
            PictureBox1.TabStop = false;
            PictureBox1.BackColor = Color.Transparent;
            PictureBox1.Image = Resources.Logo;

            // label1
            Label1.AutoSize = true;
            Label1.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            Label1.ForeColor = LayoutColors.ColorControlText;
            Label1.BackColor = Color.Transparent;
            Label1.Text = Data.ProgramName;

            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            // label2
            Label2.AutoSize = true;
            Label2.Font = new Font("Microsoft Sans Serif", 12F);
            Label2.ForeColor = LayoutColors.ColorControlText;
            Label2.BackColor = Color.Transparent;
            Label2.Text = Language.T("Version") + ": " + Data.ProgramVersion + stage;

            // label3
            Label3.AutoSize = true;
            Label3.Font = new Font("Microsoft Sans Serif", 10F);
            Label3.ForeColor = LayoutColors.ColorControlText;
            Label3.BackColor = Color.Transparent;
            Label3.Text = "Copyright (c) 2006 - 2011 Miroslav Popov" + Environment.NewLine + Language.T("Distributor") +
                          " - Forex Software Ltd." + Environment.NewLine + Environment.NewLine +
                          Language.T("This is a freeware program!");

            // label4
            Label4.AutoSize = true;
            Label4.ForeColor = LayoutColors.ColorControlText;
            Label4.BackColor = Color.Transparent;
            Label4.Text = Language.T("Website") + ":";

            // label5
            Label5.AutoSize = true;
            Label5.ForeColor = LayoutColors.ColorControlText;
            Label5.BackColor = Color.Transparent;
            Label5.Text = Language.T("Support forum") + ":";

            // label6
            Label6.AutoSize = true;
            Label6.ForeColor = LayoutColors.ColorControlText;
            Label6.BackColor = Color.Transparent;
            Label6.Text = Language.T("Contacts") + ":";

            // llWebsite
            LlWebsite.AutoSize = true;
            LlWebsite.TabStop = true;
            LlWebsite.BackColor = Color.Transparent;
            LlWebsite.Text = "http://forexsb.com";
            LlWebsite.Tag = "http://forexsb.com/";
            LlWebsite.LinkClicked += WebsiteLinkClicked;

            // llForum
            LlForum.AutoSize = true;
            LlForum.TabStop = true;
            LlForum.BackColor = Color.Transparent;
            LlForum.Text = "http://forexsb.com/forum";
            LlForum.Tag = "http://forexsb.com/forum/";
            LlForum.LinkClicked += WebsiteLinkClicked;

            // llEmail
            LlEmail.AutoSize = true;
            LlEmail.TabStop = true;
            LlEmail.BackColor = Color.Transparent;
            LlEmail.Text = "info@forexsb.com";
            LlEmail.Tag = "mailto:info@forexsb.com";
            LlEmail.LinkClicked += WebsiteLinkClicked;

            // Button Base
            BtnOk.Parent = this;
            BtnOk.Text = Language.T("Ok");
            BtnOk.UseVisualStyleBackColor = true;
            BtnOk.Click += BtnOkClick;

            // AboutScreen
            PnlBase.Controls.Add(Label1);
            PnlBase.Controls.Add(Label2);
            PnlBase.Controls.Add(Label3);
            PnlBase.Controls.Add(Label4);
            PnlBase.Controls.Add(Label5);
            PnlBase.Controls.Add(Label6);
            PnlBase.Controls.Add(LlEmail);
            PnlBase.Controls.Add(LlForum);
            PnlBase.Controls.Add(LlWebsite);
            PnlBase.Controls.Add(PictureBox1);

            StartPosition = FormStartPosition.CenterScreen;
            Text = Language.T("About") + " " + Data.ProgramName;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor = LayoutColors.ColorFormBack;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(360, 280);
        }

        private FancyPanel PnlBase { get; set; }
        private Label Label1 { get; set; }
        private Label Label2 { get; set; }
        private Label Label3 { get; set; }
        private Label Label4 { get; set; }
        private Label Label5 { get; set; }
        private Label Label6 { get; set; }
        private Button BtnOk { get; set; }
        private PictureBox PictureBox1 { get; set; }
        private LinkLabel LlWebsite { get; set; }
        private LinkLabel LlForum { get; set; }
        private LinkLabel LlEmail { get; set; }

        /// <summary>
        /// Form On Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int border = btnHrzSpace;

            BtnOk.Size = new Size(buttonWidth, buttonHeight);
            BtnOk.Location = new Point(ClientSize.Width - BtnOk.Width - border,
                                       ClientSize.Height - BtnOk.Height - btnVertSpace);
            PnlBase.Size = new Size(ClientSize.Width - 2*border, BtnOk.Top - border - btnVertSpace);
            PnlBase.Location = new Point(border, border);

            PictureBox1.Location = new Point(10, 3);
            PictureBox1.Size = new Size(48, 48);
            Label1.Location = new Point(63, 10);
            Label2.Location = new Point(66, 45);
            Label3.Location = new Point(66, 77);
            Label4.Location = new Point(67, 160);
            Label5.Location = new Point(67, 180);
            Label6.Location = new Point(67, 200);
            LlWebsite.Location = new Point(Label5.Right + 5, Label4.Top);
            LlForum.Location = new Point(Label5.Right + 5, Label5.Top);
            LlEmail.Location = new Point(Label5.Right + 5, Label6.Top);

            PnlBase.Invalidate();
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Connects to the web site
        /// </summary>
        private void WebsiteLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var label = sender as Label;
            if (label == null) return;
            try
            {
                Process.Start(label.Tag.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine("WebsiteLinkClicked: " + exception.Message);
            }
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        private void BtnOkClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}