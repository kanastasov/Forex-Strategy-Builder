// Strategy Optimizer - Math
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The Optimizer - Math
    /// </summary>
    public sealed partial class Optimizer
    {
        private double[] _initialValues;
        private Backtester Backtester { get; set; }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        private void SetIndicatorParams()
        {
            _protections = 0;
            _parameters = 0;

            CountParameters();
            CountPermanentProtections();
            CreateControls();
            CreateProtectionParameters();

            for (int prot = 0; prot < _protections; prot++)
            {
                SetParametersValues(prot);
            }

            int param = _protections;
            for (int slot = 0; slot < Backtester.Strategy.Slots; slot++)
            {
                for (int numParam = 0; numParam < 6; numParam++)
                {
                    if (!Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled) continue;
                    AParameter[param] = new Parameter(Backtester, OptimizerParameterType.Indicator, slot, numParam);
                    SetParametersValues(param);
                    param++;
                }
            }

            int totalHeight = ArrangeControls();

            if (_parameters == 0)
            {
                PnlParams.Height = 50;
                LblNoParams.Visible = true;
            }
            else
            {
                PnlParams.Height = totalHeight;
            }

            BtnOptimize.Focus();
        }

        /// <summary>
        /// Counts the strategy's numeric parameters and Indicators.
        /// </summary>
        private void CountParameters()
        {
            for (int slot = 0; slot < Backtester.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        _parameters++;
        }

        /// <summary>
        /// Counts Permanent SL, TP and Breakeven
        /// </summary>
        private void CountPermanentProtections()
        {
            if (Backtester.Strategy.UsePermanentSL)
            {
                _parameters++;
                _protections++;
            }
            if (Backtester.Strategy.UsePermanentTP)
            {
                _parameters++;
                _protections++;
            }
            if (Backtester.Strategy.UseBreakEven)
            {
                _parameters++;
                _protections++;
            }
        }

        /// <summary>
        /// Creates control arrays.
        /// </summary>
        private void CreateControls()
        {
            AParameter = new Parameter[_parameters];
            AchbxParameterName = new CheckBox[_parameters];
            AlblInitialValue = new Label[_parameters];
            AlblParameterValue = new Label[_parameters];
            AnudParameterMin = new NumericUpDown[_parameters];
            AnudParameterMax = new NumericUpDown[_parameters];
            AnudParameterStep = new NumericUpDown[_parameters];
            AlblIndicatorName = new Label[_parameters];

            for (int param = 0; param < _parameters; param++)
            {
                AlblIndicatorName[param] = new Label();
                AchbxParameterName[param] = new CheckBox();
                AlblInitialValue[param] = new Label();
                AlblParameterValue[param] = new Label();
                AnudParameterMin[param] = new NumericUpDown();
                AnudParameterMax[param] = new NumericUpDown();
                AnudParameterStep[param] = new NumericUpDown();
            }

            _initialValues = new double[_parameters];
        }

        /// <summary>
        /// Initializes protection parameter.
        /// </summary>
        private void CreateProtectionParameters()
        {
            int par = 0;
            if (Backtester.Strategy.UsePermanentSL)
            {
                AParameter[par] = new Parameter(Backtester, OptimizerParameterType.PermanentSL, -1, 0);
                par++;
            }
            if (Backtester.Strategy.UsePermanentTP)
            {
                AParameter[par] = new Parameter(Backtester, OptimizerParameterType.PermanentTP, -1, 0);
                par++;
            }
            if (Backtester.Strategy.UseBreakEven)
            {
                AParameter[par] = new Parameter(Backtester,OptimizerParameterType.BreakEven, -1, 0);
            }
        }

        /// <summary>
        /// Sets values of optimizer controls.
        /// </summary>
        private void SetParametersValues(int param)
        {
            AlblIndicatorName[param].Parent = PnlParams;
            AlblIndicatorName[param].Text = AParameter[param].GroupName;
            AlblIndicatorName[param].Font = _fontIndicator;
            AlblIndicatorName[param].ForeColor = LayoutColors.ColorSlotIndicatorText;
            AlblIndicatorName[param].BackColor = Color.Transparent;
            AlblIndicatorName[param].Height = 18;
            AlblIndicatorName[param].Width = 200;

            AchbxParameterName[param].ForeColor = _colorText;
            AchbxParameterName[param].BackColor = Color.Transparent;
            AchbxParameterName[param].Text = AParameter[param].ParameterName;
            AchbxParameterName[param].CheckedChanged += OptimizerCheckedChanged;
            AchbxParameterName[param].Parent = PnlParams;
            AchbxParameterName[param].Width = 155;
            AchbxParameterName[param].TextAlign = ContentAlignment.MiddleLeft;

            int point = AParameter[param].Point;
            double value = AParameter[param].Value;
            double min = AParameter[param].Minimum;
            double max = AParameter[param].Maximum;
            double step = AParameter[param].Step;
            string stringFormat = "{0:F" + point + "}";

            AlblInitialValue[param].ForeColor = _colorText;
            AlblInitialValue[param].BackColor = Color.Transparent;
            AlblInitialValue[param].Text = string.Format(stringFormat, value);
            AlblInitialValue[param].Parent = PnlParams;
            AlblInitialValue[param].Width = 55;
            AlblInitialValue[param].TextAlign = ContentAlignment.MiddleCenter;

            AlblParameterValue[param].ForeColor = _colorText;
            AlblParameterValue[param].BackColor = Color.Transparent;
            AlblParameterValue[param].Text = string.Format(stringFormat, value);
            AlblParameterValue[param].Parent = PnlParams;
            AlblParameterValue[param].Width = 55;
            AlblParameterValue[param].TextAlign = ContentAlignment.MiddleCenter;

            _fontParamValueRegular = AlblParameterValue[param].Font;
            _fontParamValueBold = new Font(_fontParamValueRegular, FontStyle.Bold);

            AnudParameterMin[param].BeginInit();
            AnudParameterMin[param].Minimum = (decimal) min;
            AnudParameterMin[param].Maximum = (decimal) max;
            AnudParameterMin[param].Value = (decimal) Math.Round(Math.Max(value - _lastSetStepButtonValue*step, min), point);
            AnudParameterMin[param].DecimalPlaces = point;
            AnudParameterMin[param].Increment = (decimal) step;
            AnudParameterMin[param].EndInit();
            AnudParameterMin[param].Parent = PnlParams;
            AnudParameterMin[param].Width = 70;
            AnudParameterMin[param].TextAlign = HorizontalAlignment.Center;
            _toolTip.SetToolTip(AnudParameterMin[param], Language.T("Minimum value."));

            AnudParameterMax[param].BeginInit();
            AnudParameterMax[param].Minimum = (decimal) min;
            AnudParameterMax[param].Maximum = (decimal) max;
            AnudParameterMax[param].Value = (decimal) Math.Round(Math.Min(value + _lastSetStepButtonValue*step, max), point);
            AnudParameterMax[param].DecimalPlaces = point;
            AnudParameterMax[param].Increment = (decimal) step;
            AnudParameterMax[param].EndInit();
            AnudParameterMax[param].Parent = PnlParams;
            AnudParameterMax[param].Width = 70;
            AnudParameterMax[param].TextAlign = HorizontalAlignment.Center;
            _toolTip.SetToolTip(AnudParameterMax[param], Language.T("Maximum value."));

            AnudParameterStep[param].BeginInit();
            AnudParameterStep[param].Minimum = (decimal) step;
            AnudParameterStep[param].Maximum = (decimal) Math.Max(step, Math.Abs(max - min));
            AnudParameterStep[param].Value = (decimal) step;
            AnudParameterStep[param].DecimalPlaces = point;
            AnudParameterStep[param].Increment = (decimal) step;
            AnudParameterStep[param].EndInit();
            AnudParameterStep[param].Parent = PnlParams;
            AnudParameterStep[param].Width = 70;
            AnudParameterStep[param].TextAlign = HorizontalAlignment.Center;
            _toolTip.SetToolTip(AnudParameterStep[param], Language.T("Step of change."));

            _initialValues[param] = value;
        }

        /// <summary>
        /// Arranges the controls
        /// </summary>
        private int ArrangeControls()
        {
            const int vertMargin = 3;
            const int horizMargin = 5;
            int vertPosition = vertMargin - ScrollBar.Value;
            int totalHeight = vertMargin;

            int slotNumber = int.MinValue;
            for (int param = 0; param < _parameters; param++)
            {
                if (AParameter[param].SlotNumber != slotNumber)
                {
                    slotNumber = AParameter[param].SlotNumber;
                    AlblIndicatorName[param].Location = new Point(horizMargin, vertPosition);
                    vertPosition += AlblIndicatorName[param].Height + vertMargin;
                    totalHeight += AlblIndicatorName[param].Height + vertMargin;
                }
                else
                {
                    AlblIndicatorName[param].Visible = false;
                }
                AchbxParameterName[param].Location = new Point(horizMargin, vertPosition);
                AlblInitialValue[param].Location = new Point(AchbxParameterName[param].Right + horizMargin, vertPosition - 1);
                AlblParameterValue[param].Location = new Point(AlblInitialValue[param].Right + horizMargin, vertPosition - 1);
                AnudParameterMin[param].Location = new Point(AlblParameterValue[param].Right + horizMargin, vertPosition + 2);
                AnudParameterMax[param].Location = new Point(AnudParameterMin[param].Right + horizMargin, vertPosition + 2);
                AnudParameterStep[param].Location = new Point(AnudParameterMax[param].Right + horizMargin, vertPosition + 2);
                vertPosition += AchbxParameterName[param].Height + vertMargin;
                totalHeight += AchbxParameterName[param].Height + vertMargin;
            }

            return totalHeight;
        }

        /// <summary>
        /// Sets parameters' Min / Max values
        /// </summary>
        private void SetParamsMinMax(int deltaStep)
        {
            for (int param = 0; param < _parameters; param++)
            {
                int point = AParameter[param].Point;
                double value = AParameter[param].Value;
                double min = AParameter[param].Minimum;
                double max = AParameter[param].Maximum;
                double step = AParameter[param].Step;

                AnudParameterMin[param].Value = (decimal) Math.Round(Math.Max(value - deltaStep*step, min), point);
                AnudParameterMax[param].Value = (decimal) Math.Round(Math.Min(value + deltaStep*step, max), point);
                AlblParameterValue[param].Text = GetParameterText(param);
            }
        }

        /// <summary>
        /// Select / unselect parameters
        /// </summary>
        private void SelectParameters(OptimizerButtons button)
        {
            for (int param = 0; param < _parameters; param++)
            {
                if (button == OptimizerButtons.SelectAll)
                    AchbxParameterName[param].Checked = true;
                else if (button == OptimizerButtons.SelectNone)
                    AchbxParameterName[param].Checked = false;
                else if (button == OptimizerButtons.SelectRandom)
                    SelectRandomParameters();
            }
        }

        /// <summary>
        /// Selects parameters randomly.
        /// </summary>
        private void SelectRandomParameters()
        {
            for (int param = 0; param < _protections; param++)
            {
                AchbxParameterName[param].Checked = _rand.Next(100) > 40;
            }

            int checkedChBoxes = 0;
            for (int param = _protections; param < _parameters; param++)
            {
                int slot = AParameter[param].SlotNumber;
                int numParam = AParameter[param].NumParam;

                AchbxParameterName[param].Checked = false;

                if (Backtester.Strategy.Slot[slot].IndParam.NumParam[numParam].Caption == "Level")
                {
                    if (Backtester.Strategy.Slot[slot].IndParam.ListParam[0].Text.Contains("Level"))
                    {
                        if (_rand.Next(100) > 20 && checkedChBoxes < 6)
                        {
                            AchbxParameterName[param].Checked = true;
                            checkedChBoxes++;
                        }
                    }
                    else
                    {
                        AchbxParameterName[param].Checked = false;
                    }
                }
                else
                {
                    if (_rand.Next(100) > 40 && checkedChBoxes < 6)
                    {
                        AchbxParameterName[param].Checked = true;
                        checkedChBoxes++;
                    }
                }
            }
        }

        /// <summary>
        /// Optimize.
        /// </summary>
        private void BtnOptimizeClick(object sender, EventArgs e)
        {
            if (_isOptimizing)
            {
                // Cancel the asynchronous operation.
                BgWorker.CancelAsync();
                return;
            }

            if (ChbOptimizerWritesReport.Checked)
                InitReport();

            // Counts the checked parameters
            _checkedParams = 0;
            for (int i = 0; i < _parameters; i++)
                if (AchbxParameterName[i].Checked)
                    _checkedParams++;

            // If there are no checked returns
            if (_checkedParams < 1)
            {
                SystemSounds.Hand.Play();
                return;
            }

            // Contains the checked parameters only
            _aiChecked = new int[_checkedParams];
            int indexChecked = 0;
            for (int i = 0; i < _parameters; i++)
                if (AchbxParameterName[i].Checked)
                    _aiChecked[indexChecked++] = i;

            SetNecessaryCycles();

            Cursor = Cursors.WaitCursor;
            ProgressBar.Value = 1;
            _progressPercent = 0;
            _computedCycles = 0;
            _isOptimizing = true;
            BtnCancel.Enabled = false;
            BtnAccept.Enabled = false;
            BtnOptimize.Text = Language.T("Stop");

            for (int i = 0; i <= (int) OptimizerButtons.ResetStrategy; i++)
                AOptimizerButtons[i].Enabled = false;

            foreach (Control control in PnlParams.Controls)
                if (control.GetType() != typeof (Label))
                    control.Enabled = false;

            foreach (Control control in PnlLimitations.Controls)
                control.Enabled = false;

            foreach (Control control in PnlSettings.Controls)
                control.Enabled = false;
            ChbHideFSB.Enabled = true;

            // Start the bgWorker
            BgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Counts the necessary optimization cycles.
        /// </summary>
        private void SetNecessaryCycles()
        {
            _cycles = 0;
            for (int i = 0; i < _checkedParams; i++)
            {
                var min = (double) AnudParameterMin[_aiChecked[i]].Value;
                var max = (double) AnudParameterMax[_aiChecked[i]].Value;
                var step = (double) AnudParameterStep[_aiChecked[i]].Value;

                for (double value = min; value <= max; value += step)
                    _cycles += 1;
            }

            for (int i = 0; i < _checkedParams - 1; i++)
            {
                for (int j = 0; j < _checkedParams; j++)
                {
                    if (i >= j) continue;

                    var min1 = (double) AnudParameterMin[_aiChecked[i]].Value;
                    var max1 = (double) AnudParameterMax[_aiChecked[i]].Value;
                    var step1 = (double) AnudParameterStep[_aiChecked[i]].Value;
                    var min2 = (double) AnudParameterMin[_aiChecked[j]].Value;
                    var max2 = (double) AnudParameterMax[_aiChecked[j]].Value;
                    var step2 = (double) AnudParameterStep[_aiChecked[j]].Value;

                    for (double value1 = min1; value1 <= max1; value1 += step1)
                        for (double value2 = min2; value2 <= max2; value2 += step2)
                            _cycles += 1;
                }
            }
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;

            // Optimize all Parameters
            OptimizeParams(worker, e);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Backtester.Calculate(Backtester.Strategy, Data.DataSet);
            Backtester.CalculateAccountStats();
            BalanceChart.SetChartData(Backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();

            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            _isOptimizing = false;
            BtnCancel.Enabled = true;
            BtnAccept.Enabled = true;
            BtnOptimize.Text = Language.T("Optimize");
            ProgressBar.Value = 1;

            if (PnlParams.Visible)
                for (int i = 0; i <= (int) OptimizerButtons.ResetStrategy; i++)
                    AOptimizerButtons[i].Enabled = true;

            for (int i = 0; i < _parameters; i++)
                AlblParameterValue[i].Text = GetParameterText(i);

            foreach (Control control in PnlParams.Controls)
                control.Enabled = true;

            foreach (Control control in PnlLimitations.Controls)
                control.Enabled = true;

            foreach (Control control in PnlSettings.Controls)
                control.Enabled = true;

            if (ChbOptimizerWritesReport.Checked)
                SaveReport();

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Saves the Generator History
        /// </summary>
        private void SetStrategyToGeneratorHistory()
        {
            Data.GeneratorHistory.Add(Backtester.Strategy.Clone());

            if (Data.GeneratorHistory.Count >= 110)
                Data.GeneratorHistory.RemoveRange(0, 10);

            Data.GenHistoryIndex = Data.GeneratorHistory.Count - 1;
        }

        /// <summary>
        /// Optimize all the checked parameters
        /// </summary>
        private void OptimizeParams(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int bestBalance = (_isOOS ? Backtester.Balance(_barOOS) : Backtester.NetBalance);

            // First Optimization Cycle
            for (int round = 0; round < _checkedParams && _isOptimizing; round++)
            {
                if (worker.CancellationPending) break;

                int param = _aiChecked[round];

                var min = (double) AnudParameterMin[param].Value;
                var max = (double) AnudParameterMax[param].Value;
                var step = (double) AnudParameterStep[param].Value;

                for (double value = min; value <= max; value += step)
                {
                    if (worker.CancellationPending) break;

                    AParameter[param].Value = value;
                    if (AParameter[param].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(AParameter[param].SlotNumber);

                    Backtester.Calculate(Backtester.Strategy, Data.DataSet);
                    Backtester.CalculateAccountStats();
                    if (ChbOptimizerWritesReport.Checked)
                        FillInReport();

                    int balance = _isOOS ? Backtester.Balance(_barOOS) : Backtester.NetBalance;
                    if (balance > bestBalance && IsLimitationsFulfilled())
                    {
                        bestBalance = balance;
                        AParameter[param].BestValue = value;
                        ShowParamBestValue(param);
                        BalanceChart.SetChartData(Backtester);
                        BalanceChart.InitChart();
                        BalanceChart.Invalidate();
                        _isStartegyChanged = true;
                        SetStrategyToGeneratorHistory();
                    }

                    // Report progress as a percentage of the total task.
                    _computedCycles++;
                    int percentComplete = 100*_computedCycles/_cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > _progressPercent)
                    {
                        _progressPercent = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }

                AParameter[param].Value = AParameter[param].BestValue;
                if (AParameter[param].Type == OptimizerParameterType.Indicator)
                    CalculateIndicator(AParameter[param].SlotNumber);
                Backtester.Calculate(Backtester.Strategy, Data.DataSet);
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (_checkedParams < 2)
                return;

            // Counts the necessary round
            int rounds = 0;
            for (int i = 0; i < _checkedParams - 1; i++)
                for (int j = 0; j < _checkedParams; j++)
                    if (i < j) rounds++;

            var aCP = new CoupleOfParams[rounds];
            var aCPTemp = new CoupleOfParams[rounds];

            rounds = 0;
            for (int i = 0; i < _checkedParams - 1; i++)
                for (int j = 0; j < _checkedParams; j++)
                    if (i < j)
                    {
                        aCPTemp[rounds].Param1 = _aiChecked[i];
                        aCPTemp[rounds].Param2 = _aiChecked[j];
                        aCPTemp[rounds].IsPassed = false;
                        rounds++;
                    }

            // Shaking the parameters
            for (int round = 0; round < rounds; round++)
            {
                int couple;
                do
                {
                    couple = _rand.Next(rounds);
                } while (aCPTemp[couple].IsPassed);
                aCPTemp[couple].IsPassed = true;
                aCP[round] = aCPTemp[couple];
            }

            // The Optimization Cycle
            for (int round = 0; round < rounds; round++)
            {
                if (worker.CancellationPending) break;

                int param1 = aCP[round].Param1;
                int param2 = aCP[round].Param2;
                bool isOneIndicator = (AParameter[param1].Type == OptimizerParameterType.Indicator &&
                                       AParameter[param2].Type == OptimizerParameterType.Indicator &&
                                       AParameter[param1].IndParam.IndicatorName ==
                                       AParameter[param2].IndParam.IndicatorName);

                var min1 = (double) AnudParameterMin[param1].Value;
                var max1 = (double) AnudParameterMax[param1].Value;
                var step1 = (double) AnudParameterStep[param1].Value;

                var min2 = (double) AnudParameterMin[param2].Value;
                var max2 = (double) AnudParameterMax[param2].Value;
                var step2 = (double) AnudParameterStep[param2].Value;

                for (double value1 = min1; value1 <= max1; value1 += step1)
                {
                    if (worker.CancellationPending) break;

                    if (!isOneIndicator)
                    {
                        AParameter[param1].Value = value1;
                        if (AParameter[param1].Type == OptimizerParameterType.Indicator)
                            CalculateIndicator(AParameter[param1].SlotNumber);
                    }

                    for (double value2 = min2; value2 <= max2; value2 += step2)
                    {
                        if (worker.CancellationPending) break;

                        if (isOneIndicator)
                        {
                            AParameter[param1].Value = value1;
                            AParameter[param2].Value = value2;
                            if (AParameter[param1].Type == OptimizerParameterType.Indicator)
                                CalculateIndicator(AParameter[param1].SlotNumber);
                        }
                        else
                        {
                            AParameter[param2].Value = value2;
                            if (AParameter[param2].Type == OptimizerParameterType.Indicator)
                                CalculateIndicator(AParameter[param2].SlotNumber);
                        }

                        // Calculates the Strategy
                        Backtester.Calculate(Backtester.Strategy, Data.DataSet);
                        Backtester.CalculateAccountStats();
                        if (ChbOptimizerWritesReport.Checked)
                            FillInReport();

                        int balance = _isOOS ? Backtester.Balance(_barOOS) : Backtester.NetBalance;
                        if (balance > bestBalance && IsLimitationsFulfilled())
                        {
                            bestBalance = balance;
                            AParameter[param1].BestValue = value1;
                            AParameter[param2].BestValue = value2;
                            ShowParamBestValue(param1);
                            ShowParamBestValue(param2);
                            BalanceChart.SetChartData(Backtester);
                            BalanceChart.InitChart();
                            BalanceChart.Invalidate();
                            _isStartegyChanged = true;
                            SetStrategyToGeneratorHistory();
                        }

                        // Report progress as a percentage of the total task.
                        _computedCycles++;
                        int percentComplete = 100*_computedCycles/_cycles;
                        percentComplete = percentComplete > 100 ? 100 : percentComplete;
                        if (percentComplete > _progressPercent)
                        {
                            _progressPercent = percentComplete;
                            worker.ReportProgress(percentComplete);
                        }
                    }
                }

                AParameter[param1].Value = AParameter[param1].BestValue;
                AParameter[param2].Value = AParameter[param2].BestValue;

                if (isOneIndicator)
                {
                    if (AParameter[param1].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(AParameter[param1].SlotNumber);
                }
                else
                {
                    if (AParameter[param1].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(AParameter[param1].SlotNumber);
                    if (AParameter[param2].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(AParameter[param2].SlotNumber);
                }

                Backtester.Calculate(Backtester.Strategy, Data.DataSet);
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Calculates the indicator in the designated slot
        /// </summary>
        private void CalculateIndicator(int slot)
        {
            IndicatorParam ip = Backtester.Strategy.Slot[slot].IndParam;
            Indicator indicator = IndicatorStore.ConstructIndicator(ip.IndicatorName, ip.SlotType);

            // List parameters
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index = ip.ListParam[i].Index;
                indicator.IndParam.ListParam[i].Text = ip.ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ip.ListParam[i].Enabled;
            }

            // Numeric parameters
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value = ip.NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = ip.NumParam[i].Enabled;
            }

            // Check parameters
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = ip.CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            indicator.Calculate(ip.SlotType);

            // Sets Backtester.Strategy
            Backtester.Strategy.Slot[slot].IndicatorName = indicator.IndicatorName;
            Backtester.Strategy.Slot[slot].IndParam = indicator.IndParam;
            Backtester.Strategy.Slot[slot].Component = indicator.Component;
            Backtester.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Backtester.Strategy.Slot[slot].SpecValue = indicator.SpecialValues;
            Backtester.Strategy.Slot[slot].MinValue = indicator.SeparatedChartMinValue;
            Backtester.Strategy.Slot[slot].MaxValue = indicator.SeparatedChartMaxValue;
            Backtester.Strategy.Slot[slot].IsDefined = true;

            // Search the indicators' components to determine the Backtester.Strategy.FirstBar
            Backtester.Strategy.SetFirstBar();
        }

        /// <summary>
        /// Calculates the Limitations Criteria
        /// </summary>
        private bool IsLimitationsFulfilled()
        {
            // Limitation Max Ambiguous Bars
            if (ChbAmbiguousBars.Checked && Backtester.AmbiguousBars > NUDAmbiguousBars.Value)
                return false;

            // Limitation Max Equity Drawdown
            double maxEquityDrawdown = Configs.AccountInMoney
                                           ? Backtester.MaxMoneyEquityDrawdown
                                           : Backtester.MaxEquityDrawdown;
            if (ChbMaxDrawdown.Checked && maxEquityDrawdown > (double) NUDMaxDrawdown.Value)
                return false;

            // Limitation Max Equity percent drawdown
            if (ChbEquityPercent.Checked && Backtester.MoneyEquityPercentDrawdown > (double) NUDEquityPercent.Value)
                return false;

            // Limitation Min Trades
            if (ChbMinTrades.Checked && Backtester.ExecutedOrders < NUDMinTrades.Value)
                return false;

            // Limitation Max Trades
            if (ChbMaxTrades.Checked && Backtester.ExecutedOrders > NUDMaxTrades.Value)
                return false;

            // Limitation Win / Loss ratio
            if (ChbWinLossRatio.Checked && Backtester.WinLossRatio < (double) NUDWinLossRatio.Value)
                return false;

            // OOS Pattern filter
            if (ChbOOSPatternFilter.Checked && ChbOutOfSample.Checked)
            {
                int netBalance = Backtester.NetBalance;
                int oosBalance = Backtester.Balance(_barOOS);
                float targetBalanceRatio = 1 + (int) NUDOutOfSample.Value/100.0F;
                var targetBalance = (int) (oosBalance*targetBalanceRatio);
                var minBalance = (int) (targetBalance*(1 - NUDOOSPatternPercent.Value/100));
                if (netBalance < oosBalance || netBalance < minBalance)
                    return false;
            }

            // Smooth Balance Line
            if (ChbSmoothBalanceLines.Checked)
            {
                var checkPoints = (int) NUDSmoothBalanceCheckPoints.Value;
                var maxPercentDeviation = (double) (NUDSmoothBalancePercent.Value/100);

                for (int i = 1; i <= checkPoints; i++)
                {
                    int firstBar = Backtester.Strategy.FirstBar;
                    int bar = Backtester.Strategy.FirstBar + i*(Data.DataSet.Bars - firstBar)/(checkPoints + 1);
                    double netBalance = Backtester.NetMoneyBalance;
                    double startBalance = Backtester.MoneyBalance(firstBar);
                    double checkPointBalance = Backtester.MoneyBalance(bar);
                    double targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                    double minBalance = targetBalance*(1 - maxPercentDeviation);
                    double maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                        return false;

                    if (Configs.AdditionalStatistics)
                    {
                        // Long balance line
                        netBalance = Backtester.NetLongMoneyBalance;
                        checkPointBalance = Backtester.LongMoneyBalance(bar);
                        startBalance = Backtester.LongMoneyBalance(firstBar);
                        targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                        minBalance = targetBalance*(1 - maxPercentDeviation);
                        maxBalance = targetBalance*(1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;

                        // Short balance line
                        netBalance = Backtester.NetShortMoneyBalance;
                        checkPointBalance = Backtester.ShortMoneyBalance(bar);
                        startBalance = Backtester.ShortMoneyBalance(firstBar);
                        targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                        minBalance = targetBalance*(1 - maxPercentDeviation);
                        maxBalance = targetBalance*(1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Shows the best value of a parameter during optimization.
        /// </summary>
        private void ShowParamBestValue(int param)
        {
            if (AlblParameterValue[param].InvokeRequired)
            {
                BeginInvoke(new ShowParamBestValueCallback(ShowParamBestValue), new object[] {param});
            }
            else
            {
                AlblParameterValue[param].Text = GetParameterText(param);
                AlblParameterValue[param].Font = _fontParamValueBold;
                if (AParameter[param].OldBestValue < AParameter[param].BestValue)
                    AlblParameterValue[param].ForeColor = LayoutColors.ColorTradeLong;
                if (AParameter[param].OldBestValue > AParameter[param].BestValue)
                    AlblParameterValue[param].ForeColor = LayoutColors.ColorTradeShort;

                var timer = new Timer {Interval = 1500, Tag = param};
                timer.Tick += TimerTick;
                timer.Start();
            }
        }

        /// <summary>
        /// Generates the parameter text.
        /// </summary>
        private string GetParameterText(int param)
        {
            string stringFormat = "{0:F" + AParameter[param].Point + "}";
            string newText = string.Format(stringFormat, AParameter[param].BestValue);

            if (AlblParameterValue[param].Text == newText)
                return newText;

            var bestValue = AParameter[param].BestValue;
            if (Math.Abs(bestValue - (double) AnudParameterMin[param].Value) < 0.000001)
                newText = "[" + newText;
            if (Math.Abs(bestValue - AParameter[param].Minimum) < 0.000001)
                newText = "[" + newText;
            if (Math.Abs(bestValue - (double) AnudParameterMax[param].Value) < 0.000001)
                newText = newText + "]";
            if (Math.Abs(bestValue - AParameter[param].Maximum) < 0.000001)
                newText = newText + "]";

            return newText;
        }

        /// <summary>
        /// Recovers the font of a parameter value label.
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            var timer = (Timer) sender;
            var param = (int) timer.Tag;

            AlblParameterValue[param].ForeColor = LayoutColors.ColorControlText;
            AlblParameterValue[param].Font = _fontParamValueRegular;

            timer.Stop();
            timer.Dispose();
        }

        /// <summary>
        /// Resets parameters and recalculates the strategy.
        /// </summary>
        private void ResetStrategyParameters()
        {
            for (int param = 0; param < _parameters; param++)
            {
                int point = AParameter[param].Point;
                string stringFormat = "{0:F" + point + "}";
                double value = _initialValues[param];

                AParameter[param].Value = value;
                AlblParameterValue[param].Text = string.Format(stringFormat, value);
            }

            int lastSlot = -1;
            for (int param = 0; param < _parameters; param++)
            {
                if (AParameter[param].Type != OptimizerParameterType.Indicator) continue;
                if (AParameter[param].SlotNumber == lastSlot) continue;
                lastSlot = AParameter[param].SlotNumber;
                CalculateIndicator(lastSlot);
            }

            Backtester.Calculate(Backtester.Strategy, Data.DataSet);
            Backtester.CalculateAccountStats();
            BalanceChart.SetChartData(Backtester);
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            _isStartegyChanged = false;
        }

        #region Nested type: ShowParamBestValueCallback

        private delegate void ShowParamBestValueCallback(int param);

        #endregion
    }
}