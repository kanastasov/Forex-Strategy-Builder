// Forex Strategy Builder - Journal controls.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Controls Journal: Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        private Panel PanelJournalRight { get; set; }
        private JournalByPositions JournalByPositions { get; set; }
        private JournalByBars JournalByBars { get; set; }
        private JournalOrders JournalOrders { get; set; }
        private JournalPositions JournalPositions { get; set; }
        private Splitter VerticalSplitter { get; set; }
        private int SelectedBarNumber { get; set; }
        private int WidthOldJournal { get; set; }

        /// <summary>
        /// Initializes the controls in panel pnlJournal.
        /// </summary>
        private void InitializeJournal()
        {
            var toolTip = new ToolTip();

            // Journal Right
            PanelJournalRight = new Panel {Parent = PanelJournal, Dock = DockStyle.Fill};

            // Journal Orders
            JournalOrders = new JournalOrders {Parent = PanelJournalRight, Dock = DockStyle.Fill, Cursor = Cursors.Hand};
            JournalOrders.Click += PnlJournalMouseClick;
            JournalOrders.CloseButton.Visible = true;
            JournalOrders.CloseButton.Click += ContextMenuCloseJournalClick;
            toolTip.SetToolTip(JournalOrders, Language.T("Click to view Bar Explorer."));
            toolTip.SetToolTip(JournalOrders.CloseButton, Language.T("Close Journal"));

            new Splitter {Parent = PanelJournalRight, Dock = DockStyle.Bottom, Height = Gap};

            // Journal Position
            JournalPositions = new JournalPositions {Parent = PanelJournalRight, Dock = DockStyle.Bottom, Cursor = Cursors.Hand};
            JournalPositions.Click += PnlJournalMouseClick;
            toolTip.SetToolTip(JournalPositions, Language.T("Click to view Bar Explorer."));

            VerticalSplitter = new Splitter {Parent = PanelJournal, Dock = DockStyle.Left, Width = Gap};

            // Journal by Bars
            JournalByBars = new JournalByBars {Name = "JournalByBars", Parent = PanelJournal, Dock = DockStyle.Left};
            JournalByBars.PopUpContextMenu.Items.AddRange(GetJournalContextMenuItems());
            JournalByBars.IsContextButtonVisible = true;
            JournalByBars.SelectedBarChange += PnlJournalSelectedBarChange;
            JournalByBars.MouseDoubleClick += PnlJournalMouseDoubleClick;
            toolTip.SetToolTip(JournalByBars, Language.T("Click to select a bar.") + Environment.NewLine + Language.T("Double click to view Bar Explorer."));

            // Journal by Positions
            JournalByPositions = new JournalByPositions {Name = "JournalByPositions", Parent = PanelJournal, Dock = DockStyle.Fill};
            JournalByPositions.PopUpContextMenu.Items.AddRange(GetJournalContextMenuItems());
            JournalByPositions.IsContextButtonVisible = true;
            JournalByPositions.CloseButton.Visible = true;
            JournalByPositions.CloseButton.Click += ContextMenuCloseJournalClick;
            JournalByPositions.SelectedBarChange += PnlJournalSelectedBarChange;
            JournalByPositions.MouseDoubleClick += PnlJournalMouseDoubleClick;
            toolTip.SetToolTip(JournalByPositions, Language.T("Click to select a bar.") + Environment.NewLine + Language.T("Double click to view Bar Explorer."));
            toolTip.SetToolTip(JournalByPositions.CloseButton, Language.T("Close Journal"));

            PanelJournal.Resize += PnlJournalResize;

            PanelJournalRight.Visible = Configs.JournalByBars;
            JournalByBars.Visible = Configs.JournalByBars;
            VerticalSplitter.Visible = Configs.JournalByBars;
            JournalByPositions.Visible = !Configs.JournalByBars;
            JournalByPositions.ShowTransfers = Configs.JournalShowTransfers;
        }

        private ToolStripItem[] GetJournalContextMenuItems()
        {
            var mi1 = new ToolStripMenuItem
            {
                Image = Resources.pos_buy,
                Text = Language.T("Journal by Positions") + " " + Language.T("without Transfers")
            };
            mi1.Click += ContextMenuJournalByPosWithoutTransfersClick;

            var mi2 = new ToolStripMenuItem
            {
                Image = Resources.pos_transfer_long,
                Text = Language.T("Journal by Positions")
            };
            mi2.Click += ContextMenuJournalByPositionsClick;

            var mi3 = new ToolStripMenuItem
            {
                Image = Resources.pos_square,
                Text = Language.T("Journal by Bars")
            };
            mi3.Click += ContextMenuJournalByBarsClick;

            var mi4 = new ToolStripMenuItem
            {
                Image = Resources.bar_explorer,
                Text = Language.T("Bar Explorer") + "..."
            };
            mi4.Click += ContextMenuBarExplorerClick;

            var mi5 = new ToolStripMenuItem
            {
                Image = Resources.close_button,
                Text = Language.T("Close Journal")
            };
            mi5.Click += ContextMenuCloseJournalClick;

            var itemCollection = new ToolStripItem[]
            {
                mi1, mi2, mi3, new ToolStripSeparator(), mi4, new ToolStripSeparator(), mi5
            };

            return itemCollection;
        }

        /// <summary>
        /// Sets the journal data
        /// </summary>
        protected void SetupJournal()
        {
            if (!Configs.ShowJournal) return;
            if (Configs.JournalByBars)
            {
                JournalByBars.SetUpJournal();
                JournalByBars.UpdateJournalData();
                JournalByBars.Invalidate();
                SelectedBarNumber = JournalByBars.SelectedBar;
                JournalOrders.SelectedBar = SelectedBarNumber;
                JournalOrders.SetUpJournal();
                JournalOrders.Invalidate();
                JournalPositions.SelectedBar = SelectedBarNumber;
                JournalPositions.SetUpJournal();
                JournalPositions.Invalidate();
            }
            else
            {
                JournalByPositions.ShowTransfers = Configs.JournalShowTransfers;
                JournalByPositions.SetUpJournal();
                JournalByPositions.Invalidate();
                SelectedBarNumber = JournalByBars.SelectedBar;
            }
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        private void PnlJournalResize(object sender, EventArgs e)
        {
            if (Configs.ShowJournal && Configs.JournalByBars)
            {
                if (WidthOldJournal != PanelJournal.Width)
                    JournalByBars.Width = 2*ClientSize.Width/3;
                JournalPositions.Height = (PanelJournal.ClientSize.Height - Gap)/2;
            }
            WidthOldJournal = PanelJournal.Width;
        }

        /// <summary>
        /// Sets the selected bar number
        /// </summary>
        private void PnlJournalSelectedBarChange(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            switch (panel.Name)
            {
                case "JournalByBars":
                    SelectedBarNumber = JournalByBars.SelectedBar;
                    JournalOrders.SelectedBar = SelectedBarNumber;
                    JournalOrders.SetUpJournal();
                    JournalOrders.Invalidate();
                    JournalPositions.SelectedBar = SelectedBarNumber;
                    JournalPositions.SetUpJournal();
                    JournalPositions.Invalidate();
                    break;
                case "JournalByPositions":
                    SelectedBarNumber = JournalByPositions.SelectedBar;
                    break;
            }
        }

        private void PnlJournalMouseClick(object sender, EventArgs e)
        {
            ShowBarExplorer();
        }

        private void PnlJournalMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowBarExplorer();
        }

        private void ContextMenuBarExplorerClick(object sender, EventArgs e)
        {
            ShowBarExplorer();
        }

        /// <summary>
        /// Shows the Bar Explorer
        /// </summary>
        private void ShowBarExplorer()
        {
            var barExplorer = new BarExplorer(Backtester, SelectedBarNumber);
            barExplorer.ShowDialog();
        }

        private void ContextMenuJournalByPosWithoutTransfersClick(object sender, EventArgs e)
        {
            Configs.JournalByBars = false;
            Configs.JournalShowTransfers = false;
            MiJournalByPosWithoutTransfers.Checked = true;
            MiJournalByPos.Checked = false;
            MiJournalByBars.Checked = false;
            ResetJournal();
        }

        private void ContextMenuJournalByPositionsClick(object sender, EventArgs e)
        {
            Configs.JournalByBars = false;
            Configs.JournalShowTransfers = true;
            MiJournalByPosWithoutTransfers.Checked = false;
            MiJournalByPos.Checked = true;
            MiJournalByBars.Checked = false;
            ResetJournal();
        }

        private void ContextMenuJournalByBarsClick(object sender, EventArgs e)
        {
            Configs.JournalByBars = true;
            MiJournalByPosWithoutTransfers.Checked = false;
            MiJournalByPos.Checked = false;
            MiJournalByBars.Checked = true;
            ResetJournal();
        }

        private void ContextMenuCloseJournalClick(object sender, EventArgs e)
        {
            MiJournalByPosWithoutTransfers.Checked = false;
            MiJournalByPos.Checked = false;
            MiJournalByBars.Checked = false;
            Configs.ShowJournal = false;
            OnResize(EventArgs.Empty);
        }

        /// <summary>
        /// Resets the journal layout.
        /// </summary>
        protected void ResetJournal()
        {
            SetupJournal();

            PanelJournalRight.Visible = Configs.JournalByBars;
            VerticalSplitter.Visible = Configs.JournalByBars;
            JournalByBars.Visible = Configs.JournalByBars;
            JournalByPositions.Visible = !Configs.JournalByBars;
            if (Configs.ShowJournal && Configs.JournalByBars)
            {
                JournalByBars.Width = 2*ClientSize.Width/3;
                JournalPositions.Height = PanelJournal.ClientSize.Height/2;
            }

            OnResize(EventArgs.Empty);
        }
    }
}