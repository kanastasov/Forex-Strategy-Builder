// Strategy Analyzer class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;
using Forex_Strategy_Builder.Utils;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    public sealed class Analyzer : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Analyzer(string menuItem)
        {
            Text = Language.T("Strategy Analyzer");
            MaximizeBox = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnClose;
            FormClosing += ActionsFormClosing;

            // Button Close
            BtnClose = new Button
                           {
                               Parent = this,
                               Text = Language.T("Close"),
                               DialogResult = DialogResult.Cancel,
                               UseVisualStyleBackColor = true
                           };

            SetupMenuBar();
            SetPanelOptions();
            SetPanelOverOptimization();
            SetPanelCumulativeStrategy();

            var tsMenuItem = new ToolStripMenuItem {Name = menuItem};
            MainMenuClick(tsMenuItem, EventArgs.Empty);
        }

        private Button BtnClose { get; set; }
        private ToolStrip TsMainMenu { get; set; }
        private Options PnlOptions { get; set; }
        private OverOptimization PnlOverOptimization { get; set; }
        private CumulativeStrategy PnlCumulativeStrategy { get; set; }

        public Form SetParrentForm
        {
            set { PnlOptions.SetParrentForm = value; }
        }

        /// <summary>
        /// Sets items in the Main Menu.
        /// </summary>
        private void SetupMenuBar()
        {
            TsMainMenu = new ToolStrip {Parent = this};

            var tsmiAnalysis = new ToolStripMenuItem {Text = Language.T("Analysis")};
            TsMainMenu.Items.Add(tsmiAnalysis);

            var tsmiOverOptimization = new ToolStripMenuItem
                                           {
                                               Text = Language.T("Over-optimization Report"),
                                               Name = "tsmiOverOptimization",
                                               Image = Resources.overoptimization_chart
                                           };
            tsmiOverOptimization.Click += MainMenuClick;
            tsmiAnalysis.DropDownItems.Add(tsmiOverOptimization);

            var tsmiCumulativeStrategy = new ToolStripMenuItem
                                             {
                                                 Text = Language.T("Cumulative Strategy"),
                                                 Name = "tsmiCumulativeStrategy",
                                                 Image = Resources.cumulative_str
                                             };
            tsmiCumulativeStrategy.Click += MainMenuClick;
            //tsmiAnalysis.DropDownItems.Add(tsmiCumulativeStrategy);

            var tsmiTools = new ToolStripMenuItem {Text = Language.T("Tools")};
            TsMainMenu.Items.Add(tsmiTools);

            var tsmiOptions = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Options"),
                                      Name = "tsmiOptions",
                                      Image = Resources.tools
                                  };
            tsmiOptions.Click += MainMenuClick;
            tsmiTools.DropDownItems.Add(tsmiOptions);
        }

        private void MainMenuClick(object sender, EventArgs e)
        {
            if (IsSomethingRunning())
            {
                SystemSounds.Hand.Play();
                return;
            }

            var button = (ToolStripMenuItem) sender;
            string name = button.Name;

            switch (name)
            {
                case "tsmiOptions":
                    PnlOverOptimization.Visible = false;
                    PnlCumulativeStrategy.Visible = false;
                    PnlOptions.Visible = true;
                    break;
                case "tsmiOverOptimization":
                    PnlOptions.Visible = false;
                    PnlCumulativeStrategy.Visible = false;
                    PnlOverOptimization.Visible = true;
                    break;
                case "tsmiCumulativeStrategy":
                    PnlOptions.Visible = false;
                    PnlOverOptimization.Visible = false;
                    PnlCumulativeStrategy.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PnlOptions.SetFSBVisiability();

            ClientSize = new Size(500, 400);
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ColorMagic.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
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

            // Button Close
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // pnlOptions
            PnlOptions.Size = new Size(ClientSize.Width - 2*space,
                                       BtnClose.Top - space - btnVertSpace - TsMainMenu.Bottom);
            PnlOptions.Location = new Point(space, TsMainMenu.Bottom + space);

            // pnlOverOptimization
            PnlOverOptimization.Size = PnlOptions.Size;
            PnlOverOptimization.Location = PnlOptions.Location;

            // pnlCumulativeStrategy
            PnlCumulativeStrategy.Size = PnlOptions.Size;
            PnlCumulativeStrategy.Location = PnlOptions.Location;
        }

        /// <summary>
        /// It shows if some process is running.
        /// </summary>
        /// <returns></returns>
        private bool IsSomethingRunning()
        {
            bool isRunning = PnlOverOptimization.IsRunning;

            return isRunning;
        }

        /// <summary>
        /// Analyzer closes
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsSomethingRunning())
            {
                SystemSounds.Hand.Play();
                e.Cancel = true;
            }
            else
            {
                PnlOptions.ShowFSB();
            }
        }

        private void SetPanelOptions()
        {
            PnlOptions = new Options(Language.T("Options")) {Parent = this, Visible = false};
        }

        private void SetPanelOverOptimization()
        {
            PnlOverOptimization = new OverOptimization(Language.T("Over-optimization Report"))
                                      {Parent = this, Visible = true};
        }

        private void SetPanelCumulativeStrategy()
        {
            PnlCumulativeStrategy = new CumulativeStrategy(Language.T("Cumulative Strategy"))
                                        {Parent = this, Visible = false};
        }
    }
}