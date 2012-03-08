// Strategy Generator - Optimization
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    /// <summary>
    /// Strategy Generator
    /// </summary>
    public sealed partial class Generator
    {
        /// <summary>
        /// Initial Optimization
        /// </summary>
        void PerformInitialOptimization(BackgroundWorker worker, bool isBetter)
        {
            bool secondChance = (_random.Next(100) < 10 && _backtester.NetBalance > 500);
            int maxCycles = isBetter ? 3 : 1;

            if (isBetter || secondChance)
            {
                for (int cycle = 0; cycle < maxCycles; cycle++)
                {
                    // Change parameters
                    ChangeNumericParameters(worker);

                    // Change Permanent Stop Loss
                    ChangePermanentSL(worker);

                    // Change Permanent Take Profit
                    ChangePermanentTP(worker);

                    // Change BreakEven
                    ChangeBreakEven(worker);
                }

                // Remove needless filters
                RemoveNeedlessFilters(worker);

                // Tries to clear the Same / Opposite Signals
                NormalizeSameOppositeSignalBehaviour(worker);

                // Remove Permanent Stop Loss
                if (!ChbPreservPermSL.Checked && _strategyBest.PropertiesStatus == StrategySlotStatus.Open && _backtester.Strategy.UsePermanentSL && !worker.CancellationPending)
                    RemovePermanentSL();

                // Remove Permanent Take Profit
                if (!ChbPreservPermTP.Checked && _strategyBest.PropertiesStatus == StrategySlotStatus.Open && _backtester.Strategy.UsePermanentTP && !worker.CancellationPending)
                    RemovePermanentTP();

                // Remove Break Even
                if (!ChbPreservBreakEven.Checked && _strategyBest.PropertiesStatus == StrategySlotStatus.Open && _backtester.Strategy.UseBreakEven && !worker.CancellationPending)
                    RemoveBreakEven();

                // Reduce the value of numeric parameters
                if (!ChbUseDefaultIndicatorValues.Checked)
                    ReduceTheValuesOfNumericParams(worker);
           }
        }

        /// <summary>
        /// Tries to clear the Same / Opposite Signals
        /// </summary>
        void NormalizeSameOppositeSignalBehaviour(BackgroundWorker worker)
        {
            if (_strategyBest.PropertiesStatus != StrategySlotStatus.Open) return;

            if (_backtester.Strategy.SameSignalAction != SameDirSignalAction.Nothing)
            {
                if (!worker.CancellationPending)
                {
                    _backtester.Strategy.SameSignalAction = SameDirSignalAction.Nothing;
                    bool isBetterOrSame = CalculateTheResult(true);
                    if (!isBetterOrSame)
                        RestoreFromBest();
                }
            }

            if (_backtester.Strategy.OppSignalAction != OppositeDirSignalAction.Nothing &&
                _backtester.Strategy.Slot[_backtester.Strategy.CloseSlot].IndicatorName != "Close and Reverse")
            {
                if (!worker.CancellationPending)
                {
                    _backtester.Strategy.OppSignalAction = OppositeDirSignalAction.Nothing;
                    bool isBetterOrSame = CalculateTheResult(true);
                    if (!isBetterOrSame)
                        RestoreFromBest();
                }
            }
        }

        /// <summary>
        /// Removes the excessive filter.
        /// </summary>
        void RemoveNeedlessFilters(BackgroundWorker worker)
        {
            for (int slot = 1; slot < _backtester.Strategy.Slots; slot++)
            {
                if (_backtester.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked || _backtester.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    continue;

                if (_backtester.Strategy.Slot[slot].SlotType == SlotTypes.OpenFilter || _backtester.Strategy.Slot[slot].SlotType == SlotTypes.CloseFilter)
                {
                    if (worker.CancellationPending) break;

                    _backtester.Strategy.RemoveFilter(slot);
                    bool isBetterOrSame = CalculateTheResult(true);
                    if (!isBetterOrSame)
                        RestoreFromBest();
                }
            }
        }

        /// <summary>
        /// Change Numeric Parameters
        /// </summary>
        void ChangeNumericParameters(BackgroundWorker worker)
        {
            bool isDoAgain;
            int repeats = 0;
            do
            {
                isDoAgain = repeats < 4;
                repeats++;
                for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
                {
                    if (_backtester.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked) continue;
                    if (worker.CancellationPending) break;

                    GenerateIndicatorParameters(slot);
                    RecalculateSlots();
                    isDoAgain = CalculateTheResult(false);
                    if (!isDoAgain)
                        RestoreFromBest();
                }
            } while (isDoAgain);
        }

        /// <summary>
        /// Change Permanent Stop Loss
        /// </summary>
        void ChangePermanentSL(BackgroundWorker worker)
        {
            bool isDoAgain;
            do
            {
                if (worker.CancellationPending) break;
                if (ChbPreservPermSL.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                    break;

                int oldPermSL = _backtester.Strategy.PermanentSL;
                _backtester.Strategy.UsePermanentSL = true;
                int multiplier = _backtester.DataSet.InstrProperties.IsFiveDigits ? 50 : 5;
                _backtester.Strategy.PermanentSL = multiplier * _random.Next(5, 100);

                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    _backtester.Strategy.PermanentSL = oldPermSL;
            } while (isDoAgain);
        }

        /// <summary>
        /// Remove Permanent Stop Loss
        /// </summary>
        void RemovePermanentSL()
        {
            int oldPermSL = _backtester.Strategy.PermanentSL;
            _backtester.Strategy.UsePermanentSL = false;
            _backtester.Strategy.PermanentSL    = _backtester.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterOrSame = CalculateTheResult(true);
            if (isBetterOrSame) return;
            _backtester.Strategy.UsePermanentSL = true;
            _backtester.Strategy.PermanentSL = oldPermSL;
        }

        /// <summary>
        ///  Change Permanent Take Profit
        /// </summary>
        void ChangePermanentTP(BackgroundWorker worker)
        {
            bool isDoAgain;
            int  multiplier = _backtester.DataSet.InstrProperties.IsFiveDigits ? 50 : 5;
            do
            {
                if (worker.CancellationPending) break;
                if (ChbPreservPermTP.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked || !_backtester.Strategy.UsePermanentTP)
                    break;

                int oldPermTP = _backtester.Strategy.PermanentTP;
                _backtester.Strategy.UsePermanentTP = true;
                _backtester.Strategy.PermanentTP = multiplier * _random.Next(5, 100);

                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    _backtester.Strategy.PermanentTP = oldPermTP;
            } while (isDoAgain);
        }

        /// <summary>
        /// Removes the Permanent Take Profit
        /// </summary>
        void RemovePermanentTP()
        {
            int oldPermTP = _backtester.Strategy.PermanentTP;
            _backtester.Strategy.UsePermanentTP = false;
            _backtester.Strategy.PermanentTP = _backtester.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterOrSame = CalculateTheResult(true);
            if (isBetterOrSame) return;
            _backtester.Strategy.UsePermanentTP = true;
            _backtester.Strategy.PermanentTP = oldPermTP;
        }

        /// <summary>
        /// Change Break Even
        /// </summary>
        void ChangeBreakEven(BackgroundWorker worker)
        {
            bool isDoAgain;
            do
            {
                if (worker.CancellationPending) break;
                if (ChbPreservBreakEven.Checked || _strategyBest.PropertiesStatus == StrategySlotStatus.Locked || !_backtester.Strategy.UseBreakEven)
                    break;

                int oldBreakEven = _backtester.Strategy.BreakEven;
                _backtester.Strategy.UseBreakEven = true;
                int multiplier = _backtester.DataSet.InstrProperties.IsFiveDigits ? 50 : 5;
                _backtester.Strategy.BreakEven = multiplier * _random.Next(5, 100);

                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    _backtester.Strategy.BreakEven = oldBreakEven;
            } while (isDoAgain);
        }

        /// <summary>
        /// Removes the Break Even
        /// </summary>
        void RemoveBreakEven()
        {
            int oldBreakEven = _backtester.Strategy.BreakEven;
            _backtester.Strategy.UseBreakEven = false;
            _backtester.Strategy.BreakEven = _backtester.DataSet.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterOrSame = CalculateTheResult(true);
            if (isBetterOrSame) return;
            _backtester.Strategy.UseBreakEven = true;
            _backtester.Strategy.BreakEven = oldBreakEven;
        }

        /// <summary>
        /// Normalizes the numeric parameters.
        /// </summary>
        void ReduceTheValuesOfNumericParams(BackgroundWorker worker)
        {
            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                if (_bestBalance < 500) break;
                if (_backtester.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked) continue;

                // Numeric parameters
                for (int param = 0; param < 6; param++)
                {
                    if (!_backtester.Strategy.Slot[slot].IndParam.NumParam[param].Enabled) continue;

                    bool isDoAgain;
                    do
                    {
                        if (worker.CancellationPending) break;

                        IndicatorSlot indSlot = _backtester.Strategy.Slot[slot];
                        NumericParam num = _backtester.Strategy.Slot[slot].IndParam.NumParam[param];
                        if (num.Caption == "Level" && !indSlot.IndParam.ListParam[0].Text.Contains("Level")) break;

                        Indicator indicator = IndicatorStore.ConstructIndicator(indSlot.IndicatorName, indSlot.SlotType);
                        double defaultValue = indicator.IndParam.NumParam[param].Value;

                        double numOldValue = num.Value;
                        if (Math.Abs(num.Value - defaultValue) < 0.00001) break;

                        double value = num.Value;
                        double delta = (defaultValue - value) * 3 / 4;
                        value += delta;
                        value = Math.Round(value, num.Point);

                        if (Math.Abs(value - numOldValue) < value) break;

                        num.Value = value;

                        RecalculateSlots();
                        isDoAgain = CalculateTheResult(true);
                        if (!isDoAgain) RestoreFromBest();

                    } while (isDoAgain);
                }
            }
        }
    }
}
