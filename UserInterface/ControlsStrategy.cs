// Forex Strategy Builder - Strategy controls.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Strategy field
    /// </summary>
    public partial class Controls
    {
        private Button ButtonShowJournalByBars { get; set; }
        private Button ButtonShowJournalByPos { get; set; }
        private StrategyLayout StrategyField { get; set; }
        private ToolStripButton ButtonStrategyInfo { get; set; }
        private ToolStripButton ButtonStrategySize1 { get; set; }
        private ToolStripButton ButtonStrategySize2 { get; set; }

        /// <summary>
        /// Initializes the strategy field
        /// </summary>
        private void InitializeStrategy()
        {
            // Button Overview
            var tsbtOverview = new ToolStripButton {Name = "Overview", Text = Language.T("Overview")};
            tsbtOverview.Click += BtnToolsOnClick;
            tsbtOverview.ToolTipText = Language.T("See the strategy overview.");
            ToolStripStrategy.Items.Add(tsbtOverview);

            // Button Generator
            var tsbtGenerator = new ToolStripButton {Name = "Generator", Text = Language.T("Generator")};
            tsbtGenerator.Click += BtnToolsOnClick;
            tsbtGenerator.ToolTipText = Language.T("Generate or improve a strategy.");
            ToolStripStrategy.Items.Add(tsbtGenerator);

            // Button Strategy Size 1
            ButtonStrategySize1 = new ToolStripButton
                                    {
                                        DisplayStyle = ToolStripItemDisplayStyle.Image,
                                        Image = Resources.slot_size_max,
                                        Tag = 1,
                                        ToolTipText = Language.T("Show detailed info in the slots."),
                                        Alignment = ToolStripItemAlignment.Right
                                    };
            ButtonStrategySize1.Click += BtnSlotSizeClick;
            ToolStripStrategy.Items.Add(ButtonStrategySize1);

            // Button Strategy Size 2
            ButtonStrategySize2 = new ToolStripButton
                                    {
                                        DisplayStyle = ToolStripItemDisplayStyle.Image,
                                        Image = Resources.slot_size_min,
                                        Tag = 2,
                                        ToolTipText = Language.T("Show minimum info in the slots."),
                                        Alignment = ToolStripItemAlignment.Right
                                    };
            ButtonStrategySize2.Click += BtnSlotSizeClick;
            ToolStripStrategy.Items.Add(ButtonStrategySize2);

            // Button Strategy Info
            ButtonStrategyInfo = new ToolStripButton
                                   {
                                       DisplayStyle = ToolStripItemDisplayStyle.Image,
                                       Image = Resources.str_info_infook,
                                       Tag = SlotSizeMinMidMax.min,
                                       ToolTipText = Language.T("Show the strategy description."),
                                       Alignment = ToolStripItemAlignment.Right
                                   };
            ButtonStrategyInfo.Click += BtnStrategyDescriptionClick;
            ToolStripStrategy.Items.Add(ButtonStrategyInfo);

            // Button Optimizer
            var tsbtOptimizer = new ToolStripButton {Name = "Optimizer", Text = Language.T("Optimizer")};
            tsbtOptimizer.Click += BtnToolsOnClick;
            tsbtOptimizer.ToolTipText = Language.T("Optimize the strategy parameters.");
            ToolStripStrategy.Items.Add(tsbtOptimizer);

            // Strategy Layout
            StrategyField = new StrategyLayout {Parent = PanelStrategy};
            StrategyField.ButtonAddOpenFilter.Click += BtnAddOpenFilterClick;
            StrategyField.ButtonAddCloseFilter.Click += BtnAddCloseFilterClick;

            ButtonShowJournalByPos = new Button
                                      {
                                          Parent = PanelStrategy,
                                          Text = Language.T("Journal by Positions"),
                                          UseVisualStyleBackColor = true
                                      };
            ButtonShowJournalByPos.Click += BtnShowJournalByPosClick;

            ButtonShowJournalByBars = new Button
                                       {
                                           Parent = PanelStrategy,
                                           Text = Language.T("Journal by Bars"),
                                           UseVisualStyleBackColor = true
                                       };
            ButtonShowJournalByBars.Click += BtnShowJournalByBarsClick;

            PanelStrategy.Resize += PnlStrategyResize;
        }

        /// <summary>
        /// Arranges the controls after resizing.
        /// </summary>
        private void PnlStrategyResize(object sender, EventArgs e)
        {
            if (Configs.ShowJournal)
            {
                ButtonShowJournalByPos.Visible = false;
                ButtonShowJournalByBars.Visible = false;
                StrategyField.Size = new Size(PanelStrategy.ClientSize.Width, PanelStrategy.ClientSize.Height - Gap);
                StrategyField.Location = new Point(0, Gap);
            }
            else
            {
                ButtonShowJournalByPos.Visible = true;
                ButtonShowJournalByBars.Visible = true;
                ButtonShowJournalByPos.Width = ButtonShowJournalByBars.Width = (PanelStrategy.ClientSize.Width - Gap)/2;
                ButtonShowJournalByPos.Location = new Point(0, PanelStrategy.Height - ButtonShowJournalByPos.Height + 1);
                ButtonShowJournalByBars.Location = new Point(ButtonShowJournalByPos.Right + Gap, ButtonShowJournalByPos.Top);
                StrategyField.Size = new Size(PanelStrategy.ClientSize.Width, ButtonShowJournalByPos.Top - Gap - 2);
                StrategyField.Location = new Point(0, Gap);
            }
        }

        /// <summary>
        /// Shows Journal by bars.
        /// </summary>
        private void BtnShowJournalByBarsClick(object sender, EventArgs e)
        {
            Configs.ShowJournal = true;
            Configs.JournalByBars = true;
            MiJournalByPosWithoutTransfers.Checked = false;
            MiJournalByPos.Checked = false;
            MiJournalByBars.Checked = true;

            ResetJournal();
        }

        /// <summary>
        /// Shows Journal by positions.
        /// </summary>
        private void BtnShowJournalByPosClick(object sender, EventArgs e)
        {
            Configs.ShowJournal = true;
            Configs.JournalByBars = false;
            MiJournalByPosWithoutTransfers.Checked = !Configs.JournalShowTransfers;
            MiJournalByPos.Checked = Configs.JournalShowTransfers;
            MiJournalByBars.Checked = false;

            ResetJournal();
        }

        /// <summary>
        /// Creates a new strategy layout using _backtester.Strategy
        /// </summary>
        protected void RebuildStrategyLayout()
        {
            StrategyField.RebuildStrategyControls(Backtester.Strategy.Clone());
            StrategyField.PanelProperties.Click += PnlAveragingClick;
            for (int slot = 0; slot < Backtester.Strategy.Slots; slot++)
            {
                StrategyField.SlotPanelsList[slot].ContextMenuStrip = new ContextMenuStrip();
                StrategyField.SlotPanelsList[slot].ContextMenuStrip.Items.AddRange(GetStrategySlotContextMenuItems(slot));
                StrategyField.SlotPanelsList[slot].MouseClick += PnlSlotMouseUp;
                if (slot != Backtester.Strategy.OpenSlot && slot != Backtester.Strategy.CloseSlot)
                    StrategyField.SlotPanelsList[slot].CloseButton.Click += BtnRemoveSlotClick;
            }

            SetStrategyDescriptionButton();
        }

        private ToolStripItem[] GetStrategySlotContextMenuItems(int slot)
        {
            var miEdit = new ToolStripMenuItem
                             {Text = Language.T("Edit") + "...", Image = Resources.edit, Name = "Edit", Tag = slot};
            miEdit.Click += SlotContextMenuClick;

            var miUpwards = new ToolStripMenuItem
                                {
                                    Text = Language.T("Move Up"),
                                    Image = Resources.up_arrow,
                                    Name = "Upwards",
                                    Tag = slot
                                };
            miUpwards.Click += SlotContextMenuClick;
            miUpwards.Enabled = (slot > 1 &&
                                 Backtester.Strategy.Slot[slot].SlotType == Backtester.Strategy.Slot[slot - 1].SlotType);

            var miDownwards = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Move Down"),
                                      Image = Resources.down_arrow,
                                      Name = "Downwards",
                                      Tag = slot
                                  };
            miDownwards.Click += SlotContextMenuClick;
            miDownwards.Enabled = (slot < Backtester.Strategy.Slots - 1 &&
                                   Backtester.Strategy.Slot[slot].SlotType == Backtester.Strategy.Slot[slot + 1].SlotType);

            var miDuplicate = new ToolStripMenuItem
                                  {
                                      Text = Language.T("Duplicate"),
                                      Image = Resources.duplicate,
                                      Name = "Duplicate",
                                      Tag = slot
                                  };
            miDuplicate.Click += SlotContextMenuClick;
            miDuplicate.Enabled = (Backtester.Strategy.Slot[slot].SlotType == SlotTypes.OpenFilter &&
                                   Backtester.Strategy.OpenFilters < Strategy.MaxOpenFilters ||
                                   Backtester.Strategy.Slot[slot].SlotType == SlotTypes.CloseFilter &&
                                   Backtester.Strategy.CloseFilters < Strategy.MaxCloseFilters);

            var miDelete = new ToolStripMenuItem
                               {
                                   Text = Language.T("Delete"),
                                   Image = Resources.close_button,
                                   Name = "Delete",
                                   Tag = slot
                               };
            miDelete.Click += SlotContextMenuClick;
            miDelete.Enabled = (Backtester.Strategy.Slot[slot].SlotType == SlotTypes.OpenFilter ||
                                Backtester.Strategy.Slot[slot].SlotType == SlotTypes.CloseFilter);

            var itemCollection = new ToolStripItem[]
                                     {
                                         miEdit, miUpwards, miDownwards, miDuplicate,
                                         miDelete
                                     };

            return itemCollection;
        }

        /// <summary>
        /// Repaint the strategy slots without changing its kind and count
        /// </summary>
        protected void RepaintStrategyLayout()
        {
            StrategyField.RepaintStrategyControls(Backtester.Strategy.Clone());
        }

        /// <summary>
        /// Rearranges the strategy slots without changing its kind and count
        /// </summary>
        private void RearangeStrategyLayout()
        {
            StrategyField.RearangeStrategyControls();
        }

        /// <summary>
        /// Opens the averaging parameters dialog.
        /// </summary>
        protected virtual void PnlAveragingClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void PnlSlotMouseUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void SlotContextMenuClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs actions after the button add open filter was clicked.
        /// </summary>
        protected virtual void BtnAddOpenFilterClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs actions after the button add close filter was clicked.
        /// </summary>
        protected virtual void BtnAddCloseFilterClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Removes the corresponding slot.
        /// </summary>
        protected virtual void BtnRemoveSlotClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Changes the slot size
        /// </summary>
        private void BtnSlotSizeClick(object sender, EventArgs e)
        {
            var iTag = (int) ((ToolStripButton) sender).Tag;

            if (iTag == 1)
            {
                if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.min ||
                    StrategyField.SlotMinMidMax == SlotSizeMinMidMax.mid)
                {
                    ButtonStrategySize1.Image = Resources.slot_size_mid;
                    ButtonStrategySize1.ToolTipText = Language.T("Show regular info in the slots.");
                    ButtonStrategySize2.Image = Resources.slot_size_min;
                    ButtonStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.max;
                }
                else if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    ButtonStrategySize1.Image = Resources.slot_size_max;
                    ButtonStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    ButtonStrategySize2.Image = Resources.slot_size_min;
                    ButtonStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.mid;
                }
            }
            else
            {
                if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.min)
                {
                    ButtonStrategySize1.Image = Resources.slot_size_max;
                    ButtonStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    ButtonStrategySize2.Image = Resources.slot_size_min;
                    ButtonStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.mid;
                }
                else if (StrategyField.SlotMinMidMax == SlotSizeMinMidMax.mid ||
                         StrategyField.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    ButtonStrategySize1.Image = Resources.slot_size_max;
                    ButtonStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    ButtonStrategySize2.Image = Resources.slot_size_mid;
                    ButtonStrategySize2.ToolTipText = Language.T("Show regular info in the slots.");
                    StrategyField.SlotMinMidMax = SlotSizeMinMidMax.min;
                }
            }

            RearangeStrategyLayout();
        }

        /// <summary>
        /// View / edit the strategy description
        /// </summary>
        private void BtnStrategyDescriptionClick(object sender, EventArgs e)
        {
            string oldInfo = Backtester.Strategy.Description;
            var si = new StrategyDescription(Backtester);
            si.ShowDialog();
            if (oldInfo == Backtester.Strategy.Description) return;
            //Data.SetStrategyIndicators(); TODO
            SetStrategyDescriptionButton();
            Text = Path.GetFileNameWithoutExtension(Backtester.Strategy.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
        }

        /// <summary>
        /// Sets the strategy description button icon
        /// </summary>
        private void SetStrategyDescriptionButton()
        {
            if (string.IsNullOrEmpty(Backtester.Strategy.Description))
            {
                ButtonStrategyInfo.Image = Resources.str_info_noinfo;
            }
            else
            {
                //ButtonStrategyInfo.Image = Data.IsStrDescriptionRelevant() ? Resources.str_info_infook : Resources.str_info_warning; TODO FIX
                ButtonStrategyInfo.Image = Resources.str_info_infook;
            }
        }
    }
}