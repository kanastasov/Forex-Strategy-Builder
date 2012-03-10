// N Bars Exit Indicator
// Contributed by Krog
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using Forex_Strategy_Builder.Interfaces;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// N Bars Stop Indicator
    /// Exit N Bars after entry
    /// The implementation of logic is in Market.AnalyseClose(int iBar)
    /// </summary>
    public class N_Bars_Exit : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public N_Bars_Exit(IDataSet dataSet, SlotTypes slotType)
            : base(dataSet)
        {
            // General properties
            IndicatorName = "N Bars Exit";
            PossibleSlots = SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Exit N Bars after entry",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "N Bars";
            IndParam.NumParam[0].Value   = 10;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 10000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The number of bars after entry to exit the position.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            int nExit = (int)IndParam.NumParam[0].Value;

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "N Bars Exit (" + nExit.ToString() + ")";
            Component[0].DataType      = IndComponentType.ForceClose;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = true;
            Component[0].FirstBar      = 1;
            Component[0].Value         = new double[Bars];
            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            int nExit = (int)IndParam.NumParam[0].Value;

            ExitFilterLongDescription  = nExit + " bars passed after the entry";
            ExitFilterShortDescription = nExit + " bars passed after the entry";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ")"; // Number of Bars

            return sString;
        }
    }
}
