// Data Bars Filter Indicator
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using Forex_Strategy_Builder.Interfaces;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Data Bars Filter Indicator
    /// </summary>
    public class DataBarsFilter : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public DataBarsFilter(IDataSet dataSet, SlotTypes slotType)
            : base(dataSet)
        {
            // General properties
            IndicatorName = "Data Bars Filter";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam
                           {
                               IndicatorName = IndicatorName,
                               SlotType = slotType,
                               IndicatorType = TypeOfIndicator.Additional
                           };

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new[]
            {
                "Do not use the newest bars",
                "Do not use the oldest bars",
                "Do not use the newest bars and oldest bars",
                "Use the newest bars only",
                "Use the oldest bars only"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Specify the entry bars.";

            // The NumericUpDown parameters.
            IndParam.NumParam[0].Caption = "Newest bars";
            IndParam.NumParam[0].Value   = 1000;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = Configs.MaxBarsLimit;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The number of newest bars.";

            IndParam.NumParam[1].Caption = "Oldest bars";
            IndParam.NumParam[1].Value   = 0;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = Configs.MaxBarsLimit;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The number of oldest bars.";
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            var newest = (int)IndParam.NumParam[0].Value;
            var oldest = (int)IndParam.NumParam[1].Value;

            // Calculation
            int firstBar = 0;
            var adBars = new double[Bars];

            // Calculation of the logic
            switch (IndParam.ListParam[0].Text)
            {
                case "Do not use the newest bars":
                    for (int iBar = firstBar; iBar < Bars - newest; iBar++)
                    {
                        adBars[iBar] = 1;
                    }

                    break;

                case "Do not use the oldest bars":
                    firstBar = Math.Min(oldest, Bars - Configs.MinBars);

                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        adBars[bar] = 1;
                    }

                    break;

                case "Do not use the newest bars and oldest bars":
                    firstBar = Math.Min(oldest, Bars - Configs.MinBars);
                    int iLastBar = Math.Max(firstBar + Configs.MinBars, Bars - newest);

                    for (int bar = firstBar; bar < iLastBar; bar++)
                    {
                        adBars[bar] = 1;
                    }

                    break;

                case "Use the newest bars only":
                    firstBar = Math.Max(0, Bars - newest);
                    firstBar = Math.Min(firstBar, Bars - Configs.MinBars);

                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        adBars[bar] = 1;
                    }

                    break;

                case "Use the oldest bars only":
                    oldest = Math.Max(Configs.MinBars, oldest);

                    for (int bar = firstBar; bar < oldest; bar++)
                    {
                        adBars[bar] = 1;
                    }

                    break;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp
                               {
                                   CompName = "(No) Used bars",
                                   DataType = IndComponentType.AllowOpenLong,
                                   ChartType = IndChartType.NoChart,
                                   ShowInDynInfo = false,
                                   FirstBar = firstBar,
                                   Value = adBars
                               };

            Component[1] = new IndicatorComp
                               {
                                   CompName = "(No) Used bars",
                                   DataType = IndComponentType.AllowOpenShort,
                                   ChartType = IndChartType.NoChart,
                                   ShowInDynInfo = false,
                                   FirstBar = firstBar,
                                   Value = adBars
                               };
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            var newest = (int)IndParam.NumParam[0].Value;
            var oldest = (int)IndParam.NumParam[1].Value;

            EntryFilterLongDescription  = "(a back tester limitation) ";
            EntryFilterShortDescription = "(a back tester limitation) ";

            switch (IndParam.ListParam[0].Text)
            {
                case "Do not use the newest bars":
                    EntryFilterLongDescription  += "Do not use the newest " + newest + " bars";
                    EntryFilterShortDescription += "Do not use the newest " + newest + " bars";
                    break;

                case "Do not use the oldest bars":
                    EntryFilterLongDescription  += "Do not use the oldest " + oldest + " bars";
                    EntryFilterShortDescription += "Do not use the oldest " + oldest + " bars";
                    break;

                case "Do not use the newest bars and oldest bars":
                    EntryFilterLongDescription  += "Do not use the newest " + newest + " bars and oldest " + oldest + " bars";
                    EntryFilterShortDescription += "Do not use the newest " + newest + " bars and oldest " + oldest + " bars";
                    break;

                case "Use the newest bars only":
                    EntryFilterLongDescription  += "Use the newest " + newest + " bars only";
                    EntryFilterShortDescription += "Use the newest " + newest + " bars only";
                    break;

                case "Use the oldest bars only":
                    EntryFilterLongDescription  += "Use the oldest " + newest + " bars only";
                    EntryFilterShortDescription += "Use the oldest " + newest + " bars only";
                    break;
            }
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            return IndicatorName;
        }
    }
}
