// Date Filter Indicator
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using Forex_Strategy_Builder.Interfaces;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Date Filter Indicator
    /// </summary>
    public class DateFilter : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public DateFilter(IDataSet dataSet, SlotTypes slotType)
            : base(dataSet)
        {
            // General properties
            IndicatorName = "Date Filter";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam
                           {
                               IndicatorName = IndicatorName,
                               SlotType = slotType,
                               IndicatorType = TypeOfIndicator.DateTime
                           };

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new[]
            {
                "Do not open positions before",
                "Do not open positions after"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of the date filter.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Year";
            IndParam.NumParam[0].Value   = 2000;
            IndParam.NumParam[0].Min     = 1900;
            IndParam.NumParam[0].Max     = 2100;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The year.";

            IndParam.NumParam[1].Caption = "Month";
            IndParam.NumParam[1].Value   = 1;
            IndParam.NumParam[1].Min     = 1;
            IndParam.NumParam[1].Max     = 12;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The month.";
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            var year  = (int)IndParam.NumParam[0].Value;
            var month = (int)IndParam.NumParam[1].Value;
            var keyDate = new DateTime(year, month, 1);

            // Calculation
            int firstBar = 0;
            var values = new double[Bars];

            // Calculation of the logic.
            switch (IndParam.ListParam[0].Text)
            {
                case "Do not open positions after":
                    for (int bar = firstBar; bar < Bars; bar++)
                        if (Time[bar] < keyDate)
                            values[bar] = 1;

                    break;

                case "Do not open positions before":
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        if (Time[bar] >= keyDate)
                        {
                            firstBar = bar;
                            break;
                        }
                    }

                    firstBar = Math.Min(firstBar, Bars - Configs.MinBars);

                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        values[bar] = 1;
                    }

                    break;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp
                               {
                                   CompName = "Allow Open Long",
                                   DataType = IndComponentType.AllowOpenLong,
                                   ChartType = IndChartType.NoChart,
                                   ShowInDynInfo = false,
                                   FirstBar = firstBar,
                                   Value = values
                               };

            Component[1] = new IndicatorComp
                               {
                                   CompName = "Allow Open Short",
                                   DataType = IndComponentType.AllowOpenShort,
                                   ChartType = IndChartType.NoChart,
                                   ShowInDynInfo = false,
                                   FirstBar = firstBar,
                                   Value = values
                               };
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            var year  = (int)IndParam.NumParam[0].Value;
            var month = (int)IndParam.NumParam[1].Value;
            var keyDate = new DateTime(year, month, 1);

            EntryFilterLongDescription  = "(a back tester limitation) Do not open positions ";
            EntryFilterShortDescription = "(a back tester limitation) Do not open positions ";

            switch (IndParam.ListParam[0].Text)
            {
                case "Do not open positions before":
                    EntryFilterLongDescription  += "before " + keyDate.ToShortDateString();
                    EntryFilterShortDescription += "before " + keyDate.ToShortDateString();
                    break;

                case "Do not open positions after":
                    EntryFilterLongDescription  += "after " + keyDate.ToShortDateString();
                    EntryFilterShortDescription += "after " + keyDate.ToShortDateString();
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
