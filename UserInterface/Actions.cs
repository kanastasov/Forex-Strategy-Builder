// Actions class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Forex_Strategy_Builder.Common;
using Forex_Strategy_Builder.Dialogs.Generator;
using Forex_Strategy_Builder.Dialogs.Optimizer;
using Forex_Strategy_Builder.Interfaces;
using Forex_Strategy_Builder.Market;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public sealed partial class Actions : Controls
    {
        /// <summary>
        /// The starting point of the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            UpdateStatusLabel("Loading...");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UserStatistics.InitStats();
            Data.Start();
            Instruments.LoadInstruments();
            Configs.LoadConfigs();
            Language.InitLanguages();
            LayoutColors.InitColorSchemes();
            Application.Run(new Actions());
        }

        /// <summary>
        /// The default constructor.
        /// </summary>
        private Actions()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            StartPosition = FormStartPosition.CenterScreen;
            Size = GetFormSize();
            MinimumSize = new Size(500, 375);
            Icon = Data.Icon;
            Text = Data.ProgramName;
            FormClosing += ActionsFormClosing;
            Application.Idle += ApplicationIdle;

            PrepareInstruments();
            PrepareCustomIndicators();
            ProvideStrategy();
            Calculate(false);
            CheckUpdate.CheckForUpdate(Data.SystemDir, MiLiveContent, MiForex);
            ShowStartingTips();
            UpdateStatusLabel("Loading user interface...");
        }

        private bool _isDiscardSelectedIndexChange;

        private void PrepareInstruments()
        {
            UpdateStatusLabel("Loading historical data...");
            if (LoadInstrument(false) == 0) return;
            LoadInstrument(true);
            string message = Language.T("Forex Strategy Builder cannot load a historical data file and is going to use integrated data!");
            MessageBox.Show(message, Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void PrepareCustomIndicators()
        {
            if (Configs.LoadCustomIndicators)
            {
                UpdateStatusLabel("Loading custom indicators...");
                CustomIndicators.LoadCustomIndicators(Backtester.DataSet);
            }
            else
                IndicatorStore.CombineAllIndicators(Backtester.DataSet);
        }

        private void ProvideStrategy()
        {
            UpdateStatusLabel("Loading strategy...");
            string strategyPath = Backtester.Strategy.StrategyPath;

            if (Configs.RememberLastStr && Configs.LastStrategy != "")
            {
                string lastStrategy = Path.GetDirectoryName(Configs.LastStrategy);
                if (lastStrategy != "")
                    lastStrategy = Configs.LastStrategy;
                else
                {
                    string path = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    lastStrategy = Path.Combine(path, Configs.LastStrategy);
                }
                if (File.Exists(lastStrategy))
                    strategyPath = lastStrategy;
            }

            if (OpenStrategy(strategyPath))
            {
                AfterStrategyOpening(false);
            }
        }

        private static void ShowStartingTips()
        {
            if (!Configs.ShowStartingTip) return;
            var startingTips = new StartingTips();
            if (startingTips.TipsCount > 0)
                startingTips.Show();
        }

        /// <summary>
        /// Gets the starting size of the main screen.
        /// </summary>
        private Size GetFormSize()
        {
            int width = Math.Min(Configs.MainScreenWidth, SystemInformation.MaxWindowTrackSize.Width);
            int height = Math.Min(Configs.MainScreenHeight, SystemInformation.MaxWindowTrackSize.Height);

            return new Size(width, height);
        }

        /// <summary>
        /// Application idle
        /// </summary>
        private void ApplicationIdle(object sender, EventArgs e)
        {
            Application.Idle -= ApplicationIdle;
            string lockFile = GetLockFile();
            if (!string.IsNullOrEmpty(lockFile))
                File.Delete(lockFile);
        }

        /// <summary>
        /// Updates the splash screen label.
        /// </summary>
        private static void UpdateStatusLabel(string comment)
        {
            try
            {
                TextWriter tw = new StreamWriter(GetLockFile(), false);
                tw.WriteLine(comment);
                tw.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// The lockfile name will be passed automatically by Splash.exe as a
        /// command line arg -lockfile="c:\temp\C1679A85-A4FA-48a2-BF77-E74F73E08768.lock"
        /// </summary>
        /// <returns>Lock file path</returns>
        private static string GetLockFile()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-lockfile="))
                    return arg.Replace("-lockfile=", String.Empty);

            return string.Empty;
        }

        /// <summary>
        /// Checks whether the strategy have been saved or not
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                e.Cancel = true;

            if (e.Cancel) return;

            // Remember the last used strategy
            if (Configs.RememberLastStr)
            {
                if (Data.LoadedSavedStrategy != "")
                {
                    string strategyPath = Path.GetDirectoryName(Data.LoadedSavedStrategy) + Path.DirectorySeparatorChar;
                    string defaultPath = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    if (strategyPath == defaultPath)
                        Data.LoadedSavedStrategy = Path.GetFileName(Data.LoadedSavedStrategy);
                }
                Configs.LastStrategy = Data.LoadedSavedStrategy;
            }

            WindowState = FormWindowState.Normal;
            Configs.MainScreenWidth = Width;
            Configs.MainScreenHeight = Height;

            Configs.SaveConfigs();
            Instruments.SaveInstruments();
#if !DEBUG
            Hide();
            UserStatistics.SendStats();
#endif
        }

// ---------------------------------------------------------- //

        /// <summary>
        /// Edits the Strategy Properties Slot
        /// </summary>
        private void EditStrategyProperties()
        {
            Data.StackStrategy.Push(Backtester.Strategy.Clone());

            var strategyProperties = new StrategyProperties(Backtester);
            strategyProperties.ShowDialog();

            if (strategyProperties.DialogResult == DialogResult.OK)
            {
                StatsBuffer.UpdateStatsBuffer(Backtester);

                Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                RebuildStrategyLayout();
                BalanceChart.SetChartData(Backtester);
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
                HistogramChart.SetChartData();
                HistogramChart.InitChart();
                HistogramChart.Invalidate();
                SetupJournal();
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                                  Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags,
                                                  Language.T("Account Statistics"));
            }
            else
            {
                UndoStrategy();
            }
        }

        /// <summary>
        /// Edits the Strategy Slot
        /// </summary>
        private void EditSlot(int slot)
        {
            SlotTypes slotType = Backtester.Strategy.Slot[slot].SlotType;
            bool isSlotExist = Backtester.Strategy.Slot[slot].IsDefined;
            if (isSlotExist)
                Data.StackStrategy.Push(Backtester.Strategy.Clone());

            var indicatorDialog = new IndicatorDialog(slot, slotType, isSlotExist, Backtester);
            indicatorDialog.ShowDialog();

            if (indicatorDialog.DialogResult == DialogResult.OK)
            {
                StatsBuffer.UpdateStatsBuffer(Backtester);

                Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                IndicatorChart.InitChart();
                IndicatorChart.Invalidate();
                RebuildStrategyLayout();
                BalanceChart.SetChartData(Backtester);
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
                HistogramChart.SetChartData();
                HistogramChart.InitChart();
                HistogramChart.Invalidate();
                SetupJournal();
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                                  Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags,
                                                  Language.T("Account Statistics"));
            }
            else
            {
                // Cancel was pressed
                UndoStrategy();
            }
        }

        /// <summary>
        /// Moves a Slot Upwards
        /// </summary>
        private void MoveSlotUpwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Backtester.Strategy.Clone());
            Backtester.Strategy.MoveFilterUpwards(iSlotToMove);

            Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        /// Moves a Slot Downwards
        /// </summary>
        private void MoveSlotDownwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Backtester.Strategy.Clone());
            Backtester.Strategy.MoveFilterDownwards(iSlotToMove);

            Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        /// Duplicates a Slot
        /// </summary>
        private void DuplicateSlot(int slotToDuplicate)
        {
            Data.StackStrategy.Push(Backtester.Strategy.Clone());
            Backtester.Strategy.DuplicateFilter(slotToDuplicate);

            Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        /// Adds a new Open filter
        /// </summary>
        private void AddOpenFilter()
        {
            Data.StackStrategy.Push(Backtester.Strategy.Clone());
            Backtester.Strategy.AddOpenFilter();
            EditSlot(Backtester.Strategy.OpenFilters);
        }

        /// <summary>
        /// Adds a new Close filter
        /// </summary>
        private void AddCloseFilter()
        {
            Data.StackStrategy.Push(Backtester.Strategy.Clone());
            Backtester.Strategy.AddCloseFilter();
            EditSlot(Backtester.Strategy.Slots - 1);
        }

        /// <summary>
        /// Removes a strategy slot.
        /// </summary>
        /// <param name="slotNumber">Slot to remove</param>
        private void RemoveSlot(int slotNumber)
        {
            Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;

            Data.StackStrategy.Push(Backtester.Strategy.Clone());
            Backtester.Strategy.RemoveFilter(slotNumber);
            RebuildStrategyLayout();

            Calculate(false);
        }

        /// <summary>
        /// Undoes the strategy
        /// </summary>
        private void UndoStrategy()
        {
            if (Data.StackStrategy.Count <= 1)
            {
                Text = Backtester.Strategy.StrategyName + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
            }

            if (Data.StackStrategy.Count > 0)
            {
                Backtester.Strategy = Data.StackStrategy.Pop();

                RebuildStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        /// Performs actions when UPBV has been changed
        /// </summary>
        private void UsePreviousBarValueChange()
        {
            if (MiStrategyAUPBV.Checked == false)
            {
                // Confirmation Message
                string message = Language.T("Are you sure you want to control \"Use previous bar value\" manually?");
                DialogResult dialogResult = MessageBox.Show(message, Language.T("Use previous bar value"),
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // OK, we are sure
                    Data.AutoUsePrvBarValue = false;

                    foreach (IndicatorSlot indicatorSlot in Backtester.Strategy.Slot)
                        foreach (CheckParam checkParam in indicatorSlot.IndParam.CheckParam)
                            if (checkParam.Caption == "Use previous bar value")
                                checkParam.Enabled = true;
                }
                else
                {
                    // Not just now
                    MiStrategyAUPBV.Checked = true;
                }
            }
            else
            {
                Data.AutoUsePrvBarValue = true;
                Backtester.Strategy.AdjustUsePreviousBarValue(Backtester.DataSet);
                RepaintStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        /// Ask for saving the changed strategy
        /// </summary>
        private DialogResult WhetherSaveChangedStrategy()
        {
            DialogResult dr = DialogResult.No;
            if (Data.IsStrategyChanged)
            {
                string message = Language.T("Do you want to save the current strategy?") + Environment.NewLine + Backtester.Strategy.StrategyName;
                dr = MessageBox.Show(message, Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            return dr;
        }

        /// <summary>
        /// LoadInstrument
        /// </summary>
        private int LoadInstrument(bool useResource)
        {
            Cursor = Cursors.WaitCursor;

            //  Takes the instrument symbol and period
            string symbol = ComboBoxSymbol.Text;
            var dataPeriod = (DataPeriods) Enum.GetValues(typeof (DataPeriods)).GetValue(ComboBoxPeriod.SelectedIndex);
            InstrumentProperties instrProperties = Instruments.InstrumentList[symbol].Clone();

            //  Makes an instance of class Instrument
            var instrument = new Instrument(instrProperties, (int) dataPeriod)
                                 {
                                     DataDir = Data.OfflineDataDir,
                                     MaxBars = Configs.MaxBars,
                                     StartTime = Configs.DataStartTime,
                                     EndTime = Configs.DataEndTime,
                                     UseStartTime = Configs.UseStartTime,
                                     UseEndTime = Configs.UseEndTime
                                 };

            // Loads the data
            int loadDataResult = useResource ? instrument.LoadResourceData() : instrument.LoadData();

            if (instrument.Bars > 0 && loadDataResult == 0)
            {
                Backtester.DataSet.InstrProperties = instrProperties.Clone();
                int bars = instrument.Bars;
                Backtester.DataSet.Bars = bars;
                Backtester.DataSet.Period = dataPeriod;
                Backtester.DataSet.Time = new DateTime[bars];
                Backtester.DataSet.Open = new double[bars];
                Backtester.DataSet.High = new double[bars];
                Backtester.DataSet.Low = new double[bars];
                Backtester.DataSet.Close = new double[bars];
                Backtester.DataSet.Volume = new int[bars];

                for (int bar = 0; bar < bars; bar++)
                {
                    Backtester.DataSet.Open[bar] = instrument.Open(bar);
                    Backtester.DataSet.High[bar] = instrument.High(bar);
                    Backtester.DataSet.Low[bar] = instrument.Low(bar);
                    Backtester.DataSet.Close[bar] = instrument.Close(bar);
                    Backtester.DataSet.Time[bar] = instrument.Time(bar);
                    Backtester.DataSet.Volume[bar] = instrument.Volume(bar);
                }

                Backtester.DataStats = new DataStatistics
                                     {
                                         Symbol = instrument.Symbol,
                                         Period = dataPeriod,
                                         Bars = bars,
                                         Beginning = instrument.Time(0),
                                         Update = instrument.Update,
                                         MinPrice = instrument.MinPrice,
                                         MaxPrice = instrument.MaxPrice,
                                         DaysOff = instrument.DaysOff,
                                         AverageGap = instrument.AverageGap,
                                         MaxGap = instrument.MaxGap,
                                         AverageHighLow = instrument.AverageHighLow,
                                         MaxHighLow = instrument.MaxHighLow,
                                         AverageCloseOpen = instrument.AverageCloseOpen,
                                         MaxCloseOpen = instrument.MaxCloseOpen,
                                         DataCut = instrument.Cut
                                     };
                Backtester.DataSet.IsIntrabarData = false;
                Backtester.DataSet.IsTickData = false;
                Backtester.IsData = true;
                Backtester.IsResult = false;

                CheckLoadedData();
                Backtester.DataStats.GenerateMarketStats();
                InfoPanelMarketStatistics.Update(Backtester.DataStats.MarketStatsParam,
                                                 Backtester.DataStats.MarketStatsValue,
                                                 Backtester.DataStats.MarketStatsFlag,
                                                 Language.T("Market Statistics"));
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                                  Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags,
                                                  Language.T("Account Statistics"));
            }
            else if (loadDataResult == -1)
            {
                MessageBox.Show(Language.T("Error in the data file!"), Language.T("Data file loading"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
                return 1;
            }
            else
            {
                MessageBox.Show(
                    Language.T("There is no data for") + " " + symbol + " " + Data.DataPeriodToString(dataPeriod) + " " +
                    Language.T("in folder") + " " + Data.OfflineDataDir + Environment.NewLine + Environment.NewLine +
                    Language.T("Check the offline data directory path (Menu Market -> Data Directory)"),
                    Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Cursor = Cursors.Default;
                return 1;
            }

            Cursor = Cursors.Default;

            return 0;
        }

        /// <summary>
        /// Checks the loaded data
        /// </summary>
        private void CheckLoadedData()
        {
            SetInstrumentDataStatusBar();

            if (!Configs.CheckData)
                return;

            string errorMessage = "";

            // Check for defective data
            int maxConsecutiveBars = 0;
            int maxConsecutiveBar = 0;
            int consecutiveBars = 0;
            int lastBar = 0;
            for (int bar = 0; bar < Backtester.DataSet.Bars; bar++)
            {
                if (Math.Abs(Backtester.DataSet.Open[bar] - Backtester.DataSet.Close[bar]) < Backtester.DataSet.InstrProperties.Point)
                {
                    if (lastBar == bar - 1 || lastBar == 0)
                    {
                        consecutiveBars++;
                        lastBar = bar;

                        if (consecutiveBars > maxConsecutiveBars)
                        {
                            maxConsecutiveBars = consecutiveBars;
                            maxConsecutiveBar = bar;
                        }
                    }
                }
                else
                {
                    consecutiveBars = 0;
                }
            }

            if (maxConsecutiveBars > 10)
            {
                errorMessage += Language.T("Defective till bar number:") + " " + (maxConsecutiveBar + 1) + " - " +
                                Backtester.DataSet.Time[maxConsecutiveBar].ToString(CultureInfo.InvariantCulture) +
                                Environment.NewLine +
                                Language.T("You can try to cut it using \"Data Horizon\".") + Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (Backtester.DataSet.Bars < 300)
            {
                errorMessage += Language.T("Contains less than 300 bars!") + Environment.NewLine +
                                Language.T("Check your data file or the limits in \"Data Horizon\".");
            }

            if (Backtester.DataStats.DaysOff > 5 && Backtester.DataSet.Period != DataPeriods.week)
            {
                errorMessage += Language.T("Maximum days off") + " " + Backtester.DataStats.DaysOff + Environment.NewLine +
                                Language.T("The data is probably incomplete!") + Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (errorMessage != "")
            {
                errorMessage = Language.T("Market") + " " + Backtester.DataSet.Symbol + " " + Data.DataPeriodToString(Backtester.DataSet.Period) +
                               Environment.NewLine + errorMessage;
                MessageBox.Show(errorMessage, Language.T("Data File Loading"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Open a strategy file
        /// </summary>
        private void OpenFile()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();
            switch (dialogResult)
            {
                case DialogResult.Yes:
                    SaveStrategy();
                    break;
                case DialogResult.Cancel:
                    return;
            }

            var opendlg = new OpenFileDialog
                              {
                                  InitialDirectory = Data.StrategyDir,
                                  Filter = Language.T("Strategy file") + " (*.xml)|*.xml",
                                  Title = Language.T("Open Strategy")
                              };

            if (opendlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                OpenStrategy(opendlg.FileName);
                AfterStrategyOpening(true);
                Calculate(false);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        /// New Strategy
        /// </summary>
        private void NewStrategy()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();
            switch (dialogResult)
            {
                case DialogResult.Yes:
                    SaveStrategy();
                    break;
                case DialogResult.Cancel:
                    return;
            }

            Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

            if (!OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml"))) return;
            AfterStrategyOpening(false);
            Calculate(false);
        }

        /// <summary>
        ///Reloads the Custom Indicators.
        /// </summary>
        private void ReloadCustomIndicators()
        {
            // Check if the strategy contains custom indicators
            bool strategyHasCustomIndicator = false;
            foreach (IndicatorSlot slot in Backtester.Strategy.Slot)
            {
                // Searching the strategy slots for a custom indicator
                if (IndicatorStore.CustomIndicatorNames.Contains(slot.IndicatorName))
                {
                    strategyHasCustomIndicator = true;
                    break;
                }
            }

            if (strategyHasCustomIndicator)
            {
                // Save the current strategy
                DialogResult dialogResult = WhetherSaveChangedStrategy();
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        SaveStrategy();
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            // Reload all the custom indicators
            CustomIndicators.LoadCustomIndicators(Backtester.DataSet);

            if (strategyHasCustomIndicator)
            {
                // Load and calculate a new strategy
                Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

                if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")))
                {
                    AfterStrategyOpening(false);
                    Calculate(false);
                }
            }
        }

        /// <summary>
        /// Reads the strategy from a file.
        /// </summary>
        /// <returns>true - success.</returns>
        private bool OpenStrategy(string strategyFilePath)
        {
            try
            {
                bool isLoaded = false;

                if (File.Exists(strategyFilePath))
                {
                    Strategy strategy = StrategyIO.Load(strategyFilePath, Backtester.DataSet);
                    if (strategy != null)
                    {
                        Backtester.Strategy = strategy;
                        Backtester.Strategy.StrategyName = Path.GetFileNameWithoutExtension(strategyFilePath);
                        Data.StrategyDir = Path.GetDirectoryName(strategyFilePath);
                        if (Backtester.Strategy.OpenFilters > Configs.MaxEntryFilters)
                            Configs.MaxEntryFilters = Backtester.Strategy.OpenFilters;
                        if (Backtester.Strategy.CloseFilters > Configs.MaxExitFilters)
                            Configs.MaxExitFilters = Backtester.Strategy.CloseFilters;
                        isLoaded = true;
                    }
                }

                if(!isLoaded)
                {
                    Backtester.Strategy = Strategy.GenerateNew(Backtester.DataSet);
                    Data.LoadedSavedStrategy = "";
                    Text = Data.ProgramName;
                }

                SetStrategyIndicators();
                RebuildStrategyLayout();

                Text = Backtester.Strategy.StrategyName + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Backtester.Strategy.StrategyPath;

                Data.StackStrategy.Clear();
            }
            catch
            {
                Backtester.Strategy = Strategy.GenerateNew(Backtester.DataSet);
                string message = Language.T("The strategy could not be loaded correctly!");
                MessageBox.Show(message, Language.T("Strategy Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Data.LoadedSavedStrategy = "";
                Text = Data.ProgramName;
                RebuildStrategyLayout();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        private void SaveStrategy()
        {
            if (Backtester.Strategy.StrategyName == "New")
            {
                SaveAsStrategy();
            }
            else
            {
                try
                {
                    Backtester.Strategy.Symbol = Backtester.DataSet.Symbol;
                    Backtester.Strategy.DataPeriod = Backtester.DataSet.Period;
                    StrategyIO.Save(Backtester.Strategy, Backtester.Strategy.StrategyPath);
                    Text = Backtester.Strategy.StrategyName + " - " + Data.ProgramName;
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Backtester.Strategy.StrategyPath;
                    UserStatistics.SavedStrategies++;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                }
            }
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        private void SaveAsStrategy()
        {
            // Creates a dialog form SaveFileDialog
            var saveFileDialog = new SaveFileDialog
                              {
                                  InitialDirectory = Data.StrategyDir,
                                  FileName = Path.GetFileName(Backtester.Strategy.StrategyName),
                                  AddExtension = true,
                                  Title = Language.T("Save the Strategy As"),
                                  Filter = Language.T("Strategy file") + " (*.xml)|*.xml"
                              };


            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                Backtester.Strategy.Symbol = Backtester.DataSet.Symbol;
                Backtester.Strategy.DataPeriod = Backtester.DataSet.Period;
                Backtester.Strategy.StrategyName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                Data.StrategyDir = Path.GetDirectoryName(saveFileDialog.FileName);
                StrategyIO.Save(Backtester.Strategy, saveFileDialog.FileName);
                Text = Backtester.Strategy.StrategyName + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Backtester.Strategy.StrategyPath;
                UserStatistics.SavedStrategies++;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        /// Calculates the strategy.
        /// </summary>
        /// <param name="recalcIndicators">true - to recalculate all the indicators.</param>
        private void Calculate(bool recalcIndicators)
        {
            bool isUPBVChanged = Backtester.Strategy.AdjustUsePreviousBarValue(Backtester.DataSet);

            // Calculates the indicators by slots if it's necessary
            if (recalcIndicators)
                foreach (IndicatorSlot indSlot in Backtester.Strategy.Slot)
                {
                    string indicatorName = indSlot.IndicatorName;
                    SlotTypes slotType = indSlot.SlotType;
                    Indicator indicator = IndicatorStore.ConstructIndicator(indicatorName, Backtester.DataSet, slotType);

                    indicator.IndParam = indSlot.IndParam;

                    indicator.Calculate(slotType);

                    indSlot.IndicatorName = indicator.IndicatorName;
                    indSlot.IndParam = indicator.IndParam;
                    indSlot.Component = indicator.Component;
                    indSlot.SeparatedChart = indicator.SeparatedChart;
                    indSlot.SpecValue = indicator.SpecialValues;
                    indSlot.MinValue = indicator.SeparatedChartMinValue;
                    indSlot.MaxValue = indicator.SeparatedChartMaxValue;
                    indSlot.IsDefined = true;
                }

            // Searches the indicators' components to determine the Data.FirstBar
            Backtester.Strategy.SetFirstBar();

            // Calculates the backtest
            Backtester.Calculate(Backtester.Strategy, Backtester.DataSet);
            Backtester.CalculateAccountStats();

            Backtester.IsResult = true;
            StatsBuffer.UpdateStatsBuffer(Backtester);

            if (isUPBVChanged) RebuildStrategyLayout();
            IndicatorChart.InitChart();
            IndicatorChart.Invalidate();
            BalanceChart.SetChartData(Backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
        }

        /// <summary>
        /// Sets the market according to the strategy
        /// </summary>
        private void SetMarket(string symbol, DataPeriods dataPeriod)
        {
            Backtester.DataSet.InstrProperties = Instruments.InstrumentList[symbol].Clone();
            Backtester.DataSet.Period = dataPeriod;

            _isDiscardSelectedIndexChange = true;
            ComboBoxSymbol.SelectedIndex = ComboBoxSymbol.Items.IndexOf(symbol);

            switch (dataPeriod)
            {
                case DataPeriods.min1:
                    ComboBoxPeriod.SelectedIndex = 0;
                    break;
                case DataPeriods.min5:
                    ComboBoxPeriod.SelectedIndex = 1;
                    break;
                case DataPeriods.min15:
                    ComboBoxPeriod.SelectedIndex = 2;
                    break;
                case DataPeriods.min30:
                    ComboBoxPeriod.SelectedIndex = 3;
                    break;
                case DataPeriods.hour1:
                    ComboBoxPeriod.SelectedIndex = 4;
                    break;
                case DataPeriods.hour4:
                    ComboBoxPeriod.SelectedIndex = 5;
                    break;
                case DataPeriods.day:
                    ComboBoxPeriod.SelectedIndex = 6;
                    break;
                case DataPeriods.week:
                    ComboBoxPeriod.SelectedIndex = 7;
                    break;
            }

            _isDiscardSelectedIndexChange = false;
        }

        /// <summary>
        /// Edit the Trading Charges
        /// </summary>
        private void EditTradingCharges()
        {
            var tradingCharges = new TradingCharges(Backtester)
                                     {
                                         Spread = Backtester.DataSet.InstrProperties.Spread,
                                         SwapLong = Backtester.DataSet.InstrProperties.SwapLong,
                                         SwapShort = Backtester.DataSet.InstrProperties.SwapShort,
                                         Commission = Backtester.DataSet.InstrProperties.Commission,
                                         Slippage = Backtester.DataSet.InstrProperties.Slippage
                                     };

            tradingCharges.ShowDialog();

            if (tradingCharges.DialogResult == DialogResult.OK)
            {
                Backtester.DataSet.InstrProperties.Spread = tradingCharges.Spread;
                Backtester.DataSet.InstrProperties.SwapLong = tradingCharges.SwapLong;
                Backtester.DataSet.InstrProperties.SwapShort = tradingCharges.SwapShort;
                Backtester.DataSet.InstrProperties.Commission = tradingCharges.Commission;
                Backtester.DataSet.InstrProperties.Slippage = tradingCharges.Slippage;

                Instruments.InstrumentList[Backtester.DataSet.InstrProperties.Symbol] = Backtester.DataSet.InstrProperties.Clone();

                Calculate(false);

                SetInstrumentDataStatusBar();
            }
            else if (tradingCharges.EditInstrument)
                ShowInstrumentEditor();
        }

        /// <summary>
        /// Check the needed market conditions
        /// </summary>
        /// <param name="isMessage">To show the message or not</param>
        private void AfterStrategyOpening(bool isMessage)
        {
            if (Backtester.Strategy.Symbol != Backtester.DataSet.Symbol || Backtester.Strategy.DataPeriod != Backtester.DataSet.Period)
            {
                bool toReload = true;

                if (isMessage)
                {
                    string message = Language.T("The loaded strategy has been designed for a different market!") +
                                     Environment.NewLine + Environment.NewLine + Backtester.Strategy.Symbol + " " +
                                     Data.DataPeriodToString(Backtester.Strategy.DataPeriod) + Environment.NewLine +
                                     Environment.NewLine + Language.T("Do you want to load this market data?");
                    DialogResult result = MessageBox.Show(message, Backtester.Strategy.StrategyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    toReload = (result == DialogResult.Yes);
                }

                if (toReload)
                {
                    if (!Instruments.InstrumentList.ContainsKey(Backtester.Strategy.Symbol))
                    {
                        string message = Language.T("There is no information for this market!") +
                                         Environment.NewLine + Environment.NewLine + Backtester.Strategy.Symbol + " " +
                                         Data.DataPeriodToString(Backtester.Strategy.DataPeriod);
                        MessageBox.Show(message, Backtester.Strategy.StrategyName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    string symbol = Backtester.DataSet.Symbol;
                    DataPeriods dataPeriod = Backtester.DataSet.Period;

                    SetMarket(Backtester.Strategy.Symbol, Backtester.Strategy.DataPeriod);

                    if (LoadInstrument(false) == 0)
                    {
                        Calculate(true);
                        PrepareScannerCompactMode();
                    }
                    else
                    {
                        SetMarket(symbol, dataPeriod);
                    }
                }
            }
            else if (!Backtester.DataSet.IsIntrabarData)
            {
                PrepareScannerCompactMode();
            }
        }

        /// <summary>
        /// Load intrabar data by using scanner.
        /// </summary>
        private void PrepareScannerCompactMode()
        {
            if (!Configs.Autoscan || (Backtester.DataSet.Period == DataPeriods.min1 && !Configs.UseTickData)) return;
            ComboBoxSymbol.Enabled = false;
            ComboBoxPeriod.Enabled = false;

            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += LoadIntrabarData;
            bgWorker.RunWorkerCompleted += IntrabarDataLoaded;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Starts scanner and loads intrabar data.
        /// </summary>
        private void LoadIntrabarData(object sender, DoWorkEventArgs e)
        {
            var scanner = new Scanner(Backtester) {CompactMode = true};
            scanner.ShowDialog();
        }

        /// <summary>
        /// The intrabar data is loaded. Refresh the program.
        /// </summary>
        private void IntrabarDataLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            Calculate(true);

            ComboBoxSymbol.Enabled = true;
            ComboBoxPeriod.Enabled = true;
        }

        /// <summary>
        /// Generate BBCode for the forum
        /// </summary>
        private void PublishStrategy()
        {
            var publisher = new StrategyPublish(StrategyFormatForum.FormatPost(Backtester, IsStrDescriptionRelevant()));
            publisher.Show();
        }

        /// <summary>
        /// Shows the Account Settings dialog.
        /// </summary>
        private void ShowAccountSettings()
        {
            var accountSettings = new AccountSettings(Backtester)
                                      {
                                          AccountCurrency = Configs.AccountCurrency,
                                          InitialAccount = Configs.InitialAccount,
                                          Leverage = Configs.Leverage,
                                          RateToUSD = Backtester.DataSet.InstrProperties.RateToUSD,
                                          RateToEUR = Backtester.DataSet.InstrProperties.RateToEUR
                                      };

            accountSettings.SetParams();

            if (accountSettings.ShowDialog() != DialogResult.OK) return;
            Configs.AccountCurrency = accountSettings.AccountCurrency;
            Configs.InitialAccount = accountSettings.InitialAccount;
            Configs.Leverage = accountSettings.Leverage;
            Backtester.DataSet.InstrProperties.RateToUSD = accountSettings.RateToUSD;
            Backtester.DataSet.InstrProperties.RateToEUR = accountSettings.RateToEUR;

            Instruments.InstrumentList[Backtester.DataSet.InstrProperties.Symbol] = Backtester.DataSet.InstrProperties.Clone();
            Calculate(false);
        }

        /// <summary>
        ///  Shows Scanner.
        /// </summary>
        private void ShowScanner()
        {
            var scanner = new Scanner(Backtester);
            scanner.ShowDialog();

            StatsBuffer.UpdateStatsBuffer(Backtester);

            MiStrategyAutoscan.Checked = Configs.Autoscan;

            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
            BalanceChart.SetChartData(Backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        /// Perform intrabar scanning.
        /// </summary>
        private void Scan()
        {
            if (!Backtester.DataSet.IsIntrabarData)
                ShowScanner();
            else
                Backtester.Scan(Backtester.Strategy, Backtester.DataSet);

            StatsBuffer.UpdateStatsBuffer(Backtester);

            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
            BalanceChart.SetChartData(Backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        /// Starts Generator.
        /// </summary>
        private void ShowGenerator()
        {
            var generator = new Generator(Backtester) {ParrentForm = this};
            generator.Closed += GeneratorOnClosed;
            generator.Show();
            UserStatistics.GeneratorStarts++;
        }

        private void GeneratorOnClosed(object sender, EventArgs eventArgs)
        {
            var generator = sender as Generator;
            if (generator == null) return;
            if (generator.DialogResult != DialogResult.OK) return;

            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Backtester.Strategy.Clone());

            // We accept the generated strategy
            Backtester.Strategy = generator.Strategy.Clone();
            string orginalDescription = generator.StrategyOriginalDescription;

            Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;

            if (generator.IsStrategyModified)
            {
                string description = orginalDescription != string.Empty
                                         ? orginalDescription + Environment.NewLine + Environment.NewLine +
                                           "-----------" + Environment.NewLine + generator.GeneratedDescription
                                         : generator.GeneratedDescription;
                Backtester.Strategy.Description = (description);
            }
            else
            {
                SetStrategyIndicators();
                Backtester.Strategy.Description = generator.GeneratedDescription;
            }
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        private string[] _asStrategyIndicators;

        /// <summary>
        /// Sets the indicator names
        /// </summary>
        private void SetStrategyIndicators()
        {
            _asStrategyIndicators = new string[Backtester.Strategy.Slots];
            for (int i = 0; i < Backtester.Strategy.Slots; i++)
                _asStrategyIndicators[i] = Backtester.Strategy.Slot[i].IndicatorName;
        }

        /// <summary>
        /// It tells if the strategy description is relevant.
        /// </summary>
        private bool IsStrDescriptionRelevant()
        {
            if (Backtester.Strategy.Slots == _asStrategyIndicators.Length)
            {
                for (int i = 0; i < Backtester.Strategy.Slots; i++)
                    if (_asStrategyIndicators[i] != Backtester.Strategy.Slot[i].IndicatorName)
                        return false;
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Starts the generator
        /// </summary>
        private void ShowOverview()
        {
            var browser = new Browser(Language.T("Strategy Overview"), StrategyFormatOverview.FormatOverview(Backtester, IsStrDescriptionRelevant()));
            browser.Show();
        }

        /// <summary>
        /// Call the Optimizer
        /// </summary>
        private void ShowOptimizer()
        {

            var optimizer = new Optimizer(Backtester) {SetParrentForm = this};
            optimizer.Closed += OptimizerOnClosed;
            optimizer.Show();
            UserStatistics.OptimizerStarts++;
        }

        private void OptimizerOnClosed(object sender, EventArgs e)
        {
            var optimizer = sender as Optimizer;
            if (optimizer == null) return;
            if (optimizer.DialogResult != DialogResult.OK) return;

            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Backtester.Strategy.Clone());

            // We accept the generated strategy
            Backtester.Strategy = optimizer.Strategy.Clone();

            Text = Backtester.Strategy.StrategyName + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RepaintStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        ///  Show the method Comparator
        /// </summary>
        private void ShowComparator()
        {
            // Save the original method to return it later
            InterpolationMethod interpMethodOriginal = Backtester.InterpolationMethod;

            var comparator = new Comparator(Backtester);
            comparator.ShowDialog();

            // Returns the original method
            Backtester.InterpolationMethod = interpMethodOriginal;
            Calculate(false);
        }

        /// <summary>
        ///  Shows the Bar Explorer tool.
        /// </summary>
        private void ShowBarExplorer()
        {
            var barExplorer = new BarExplorer(Backtester, Backtester.Strategy.FirstBar);
            barExplorer.ShowDialog();
        }

        /// <summary>
        ///  Sets the data starting parameters.
        /// </summary>
        private void DataHorizon()
        {
            var horizon = new DataHorizon(Configs.MaxBars,
                                          Configs.DataStartTime,
                                          Configs.DataEndTime,
                                          Configs.UseStartTime,
                                          Configs.UseEndTime);
            horizon.ShowDialog();

            if (horizon.DialogResult != DialogResult.OK) return;
            Configs.MaxBars = horizon.MaxBars;
            Configs.DataStartTime = horizon.StartTime;
            Configs.DataEndTime = horizon.EndTime;
            Configs.UseStartTime = horizon.UseStartTime;
            Configs.UseEndTime = horizon.UseEndTime;

            if (LoadInstrument(false) != 0) return;
            Calculate(true);
            PrepareScannerCompactMode();
        }

        /// <summary>
        ///  Shows the Instrument Editor dialog.
        /// </summary>
        private void ShowInstrumentEditor()
        {
            var instrEditor = new InstrumentEditor(Backtester);
            instrEditor.ShowDialog();

            if (instrEditor.NeedReset)
            {
                _isDiscardSelectedIndexChange = true;

                ComboBoxSymbol.Items.Clear();
                foreach (string symbol in Instruments.SymbolList)
                    ComboBoxSymbol.Items.Add(symbol);
                ComboBoxSymbol.SelectedIndex = ComboBoxSymbol.Items.IndexOf(Backtester.DataSet.Symbol);

                _isDiscardSelectedIndexChange = false;
            }

            Backtester.DataSet.InstrProperties = Instruments.InstrumentList[Backtester.DataSet.InstrProperties.Symbol].Clone();
            SetInstrumentDataStatusBar();
            Calculate(false);
        }

        /// <summary>
        /// Loads a color scheme.
        /// </summary>
        private void LoadColorScheme()
        {
            string colorFile = Path.Combine(Data.ColorDir, Configs.ColorScheme + ".xml");

            if (!File.Exists(colorFile)) return;
            LayoutColors.LoadColorScheme(colorFile);

            PanelWorkspace.BackColor = LayoutColors.ColorFormBack;
            RepaintStrategyLayout(); 
            InfoPanelAccountStatistics.SetColors();
            InfoPanelMarketStatistics.SetColors();
            IndicatorChart.InitChart();
            BalanceChart.SetChartData(Backtester);
            BalanceChart.InitChart();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            SetupJournal();
            PanelWorkspace.Invalidate(true);
        }

        /// <summary>
        /// Sets the Status Bar Data Label
        /// </summary>
        private void SetInstrumentDataStatusBar()
        {
            string swapUnit = "p";
            if (Backtester.DataSet.InstrProperties.SwapType == CommissionType.money)
                swapUnit = "m";
            else if (Backtester.DataSet.InstrProperties.SwapType == CommissionType.percents)
                swapUnit = "%";

            string commUnit = "p";
            if (Backtester.DataSet.InstrProperties.CommissionType == CommissionType.money)
                commUnit = "m";
            else if (Backtester.DataSet.InstrProperties.CommissionType == CommissionType.percents)
                commUnit = "%";

            StatusLabelInstrument =
                Backtester.DataSet.Symbol + " " +
                Backtester.DataSet.PeriodString + " (" +
                Backtester.DataSet.InstrProperties.Spread + ", " +
                Backtester.DataSet.InstrProperties.SwapLong.ToString("F2") + swapUnit + ", " +
                Backtester.DataSet.InstrProperties.SwapShort.ToString("F2") + swapUnit + ", " +
                Backtester.DataSet.InstrProperties.Commission.ToString("F2") + commUnit + ", " +
                Backtester.DataSet.InstrProperties.Slippage + ")" +
                (Backtester.DataStats.DataCut ? " - " + Language.T("Cut") : "") +
                (Configs.FillInDataGaps ? " - " + Language.T("No Gaps") : "") +
                (Configs.CheckData ? "" : " - " + Language.T("Unchecked"));
        }
    }
}
