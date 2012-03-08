using System;
using System.Globalization;
using System.Text;

namespace Forex_Strategy_Builder
{
    public static class ForumFormatReport
    {
        public static string GetReport(Backtester backtester, bool isDescriptionRelevant)
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            var sb = new StringBuilder();

            sb.AppendLine("Strategy name: [b]" + backtester.Strategy.StrategyName + "[/b]");
            sb.AppendLine(Data.ProgramName + " v" + Data.ProgramVersion + stage);
            sb.AppendLine("Exported on: " + DateTime.Now);
            sb.AppendLine();

            // Description
            sb.AppendLine("Description:");

            if (backtester.Strategy.Description != "")
            {
                if (isDescriptionRelevant)
                {
                    sb.AppendLine("(This description might be outdated!)");
                    sb.AppendLine();
                }

                sb.AppendLine(backtester.Strategy.Description);
            }
            else
                sb.AppendLine("   None.");

            sb.AppendLine();
            sb.AppendLine("Market: " + Data.Symbol + " " + Data.PeriodString);
            sb.AppendLine("Spread in pips: " + Data.DataSet.InstrProperties.Spread.ToString("F2"));
            sb.AppendLine("Swap Long in " +
                (Data.DataSet.InstrProperties.SwapType == CommissionType.money ? Data.DataSet.InstrProperties.PriceIn : Data.DataSet.InstrProperties.SwapType.ToString()) + ": " +
                Data.DataSet.InstrProperties.SwapLong.ToString("F2"));
            sb.AppendLine("Swap Short in " +
                (Data.DataSet.InstrProperties.SwapType == CommissionType.money ? Data.DataSet.InstrProperties.PriceIn : Data.DataSet.InstrProperties.SwapType.ToString()) + ": " +
                Data.DataSet.InstrProperties.SwapShort.ToString("F2"));
            sb.AppendLine("Commission per " +
                Data.DataSet.InstrProperties.CommissionScope.ToString() + " at " +
                (Data.DataSet.InstrProperties.CommissionTime == CommissionTime.open ? "opening" : "opening and closing") + " in " +
                (Data.DataSet.InstrProperties.CommissionType == CommissionType.money ? Data.DataSet.InstrProperties.PriceIn : Data.DataSet.InstrProperties.CommissionType.ToString()) + ": " +
                Data.DataSet.InstrProperties.Commission.ToString("F2"));
            sb.AppendLine("Slippage in pips: " + Data.DataSet.InstrProperties.Slippage);

            sb.AppendLine(backtester.Strategy.UseAccountPercentEntry ? "Use account % for margin round to whole lots" : "");
            string tradingUnit = backtester.Strategy.UseAccountPercentEntry ? "% of the account for margin" : "";
            sb.AppendLine("Maximum open lots: " + backtester.Strategy.MaxOpenLots.ToString("F2"));
            sb.AppendLine("Entry lots: " + backtester.Strategy.EntryLots.ToString("F2") + tradingUnit);
            if (backtester.Strategy.SameSignalAction == SameDirSignalAction.Add || backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                sb.AppendLine("Adding lots: " + backtester.Strategy.AddingLots.ToString("F2") + tradingUnit);
            if (backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                sb.AppendLine("Reducing lots: " + backtester.Strategy.ReducingLots.ToString("F2") + tradingUnit);
            if (backtester.Strategy.UseMartingale)
                sb.AppendLine("Martingale money management multiplier: " + backtester.Strategy.MartingaleMultiplier.ToString("F2"));

            sb.AppendLine();
            sb.AppendLine("Intrabar scanning: " + (backtester.IsScanPerformed ? "Accomplished" : "Not accomplished"));
            sb.AppendLine("Interpolation method: " + backtester.InterpolationMethodToString());
            sb.AppendLine("Ambiguous bars: " + backtester.AmbiguousBars);
            sb.AppendLine("Tested bars: " + (Data.DataSet.Bars - backtester.Strategy.FirstBar));
            sb.AppendLine("Balance: [b]" + backtester.NetBalance + " pips (" + backtester.NetMoneyBalance.ToString("F2") + " " + Data.DataSet.InstrProperties.PriceIn + ")[/b]");
            sb.AppendLine("Minimum account: " + backtester.MinBalance + " pips (" + backtester.MinMoneyBalance.ToString("F2") + " " + Data.DataSet.InstrProperties.PriceIn + ")");
            sb.AppendLine("Maximum drawdown: " + backtester.MaxDrawdown + " pips (" + backtester.MaxMoneyDrawdown.ToString("F2") + " " + Data.DataSet.InstrProperties.PriceIn + ")");
            sb.AppendLine("Time in position: " + backtester.TimeInPosition + " %");
            sb.AppendLine();

            sb.AppendLine("[b][color=#966][Strategy Properties][/color][/b]");
            if (backtester.Strategy.SameSignalAction == SameDirSignalAction.Add)
                sb.AppendLine("     A same direction signal - Adds to the position");
            else if (backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                sb.AppendLine("     A same direction signal - Adds to a winning position");
            else if (backtester.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                sb.AppendLine("     A same direction signal - Does nothing");

            if (backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Close)
                sb.AppendLine("     An opposite direction signal - Closes the position");
            else if (backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                sb.AppendLine("     An opposite direction signal - Reduces the position");
            else if (backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                sb.AppendLine("     An opposite direction signal - Reverses the position");
            else
                sb.AppendLine("     An opposite direction signal - Does nothing");

            sb.AppendLine("     Permanent Stop Loss - " + (backtester.Strategy.UsePermanentSL ? (backtester.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") + backtester.Strategy.PermanentSL.ToString(CultureInfo.InvariantCulture) : "None") + "");
            sb.AppendLine("     Permanent Take Profit - " + (backtester.Strategy.UsePermanentTP ? (backtester.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") + backtester.Strategy.PermanentTP.ToString(CultureInfo.InvariantCulture) : "None") + "");
            sb.AppendLine("     Break Even - " + (backtester.Strategy.UseBreakEven ? backtester.Strategy.BreakEven.ToString(CultureInfo.InvariantCulture) : "None") + "");
            sb.AppendLine();

            // Add the slots.
            foreach (IndicatorSlot indSlot in backtester.Strategy.Slot)
            {
                string slotTypeName;
                string slotColor;
                switch (indSlot.SlotType)
                {
                    case SlotTypes.Open:
                        slotTypeName = "Opening Point of the Position";
                        slotColor = "#693";
                        break;
                    case SlotTypes.OpenFilter:
                        slotTypeName = "Opening Logic Condition";
                        slotColor = "#699";
                        break;
                    case SlotTypes.Close:
                        slotTypeName = "Closing Point of the Position";
                        slotColor = "#d63";
                        break;
                    case SlotTypes.CloseFilter:
                        slotTypeName = "Closing Logic Condition";
                        slotColor = "#d99";
                        break;
                    default:
                        slotTypeName = "";
                        slotColor = "#000";
                        break;
                }

                sb.AppendLine("[b][color=" + slotColor + "][" + slotTypeName + "][/color][/b]");
                sb.AppendLine("     [b][color=blue]" + indSlot.IndicatorName + "[/color][/b]");

                // Add the list params.
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled)
                    {
                        if (listParam.Caption == "Logic")
                            sb.AppendLine("     [b][color=#066]" +
                                (Configs.UseLogicalGroups && (indSlot.SlotType == SlotTypes.OpenFilter || indSlot.SlotType == SlotTypes.CloseFilter) ?
                                "[" + (indSlot.LogicalGroup.Length == 1 ? " " + indSlot.LogicalGroup + " " : indSlot.LogicalGroup) + "]   " : "") + listParam.Text + "[/color][/b]");
                        else
                            sb.AppendLine("     " + listParam.Caption + "  -  " + listParam.Text);
                    }

                // Add the num params.
                foreach (NumericParam numParam in indSlot.IndParam.NumParam)
                    if (numParam.Enabled)
                        sb.AppendLine("     " + numParam.Caption + "  -  " + numParam.ValueToString);

                // Add the check params.
                foreach (CheckParam checkParam in indSlot.IndParam.CheckParam)
                    if (checkParam.Enabled)
                        sb.AppendLine("     " + checkParam.Caption + "  -  " + (checkParam.Checked ? "Yes" : "No"));

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
