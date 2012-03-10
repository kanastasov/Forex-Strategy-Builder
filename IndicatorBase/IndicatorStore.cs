// IndicatorStore Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.using System;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Forex_Strategy_Builder.Interfaces;

namespace Forex_Strategy_Builder
{
    public static class IndicatorStore
    {
        private static readonly Dictionary<string, Indicator> OriginalIndicators = new Dictionary<string, Indicator>();

        // Stores the custom indicators
        private static readonly SortableDictionary<string, Indicator> CustomIndicators = new SortableDictionary<string, Indicator>();

        // Stores all the indicators
        private static readonly SortableDictionary<string, Indicator> AllIndicators = new SortableDictionary<string, Indicator>();

        /// <summary>
        /// Constructor.
        /// </summary>
        static IndicatorStore()
        {
            ClosingIndicatorsWithClosingFilters = new List<string>();
            CloseFilterIndicators = new List<string>();
            OpenFilterIndicators = new List<string>();
            ClosePointIndicators = new List<string>();
            OpenPointIndicators = new List<string>();
            AddOriginalIndicators();

            foreach (var record in OriginalIndicators)
                AllIndicators.Add(record.Key, record.Value);
        }

        /// <summary>
        /// Gets the names of all the original indicators
        /// </summary>
        public static IEnumerable<string> OriginalIndicatorNames
        {
            get { return new List<string>(OriginalIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all custom indicators
        /// </summary>
        public static List<string> CustomIndicatorNames
        {
            get { return new List<string>(CustomIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all indicators.
        /// </summary>
        public static List<string> IndicatorNames
        {
            get { return new List<string>(AllIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all Opening Point indicators.
        /// </summary>
        public static List<string> OpenPointIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all Closing Point indicators.
        /// </summary>
        public static List<string> ClosePointIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all Opening Filter indicators.
        /// </summary>
        public static List<string> OpenFilterIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all Closing Filter indicators.
        /// </summary>
        public static List<string> CloseFilterIndicators { get; private set; }

        /// <summary>
        /// Gets the names of all losing Point indicators that allow use of Closing Filter indicators.
        /// </summary>
        public static List<string> ClosingIndicatorsWithClosingFilters { get; private set; }

        /// <summary>
        /// Adds all indicators to the store.
        /// </summary>
        private static void AddOriginalIndicators()
        {
            OriginalIndicators.Add("Accelerator Oscillator", new Accelerator_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Account Percent Stop", new Account_Percent_Stop(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Accumulation Distribution", new Accumulation_Distribution(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("ADX", new ADX(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Alligator", new Alligator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Aroon Histogram", new Aroon_Histogram(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("ATR MA Oscillator", new ATR_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("ATR Stop", new ATR_Stop(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Average True Range", new Average_True_Range(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Awesome Oscillator", new Awesome_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Balance of Power", new Balance_of_Power(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Bar Closing", new Bar_Closing(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Bar Opening", new Bar_Opening(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Bar Range", new Bar_Range(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("BBP MA Oscillator", new BBP_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Bears Power", new Bears_Power(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Bollinger Bands", new Bollinger_Bands(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Bulls Bears Power", new Bulls_Bears_Power(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Bulls Power", new Bulls_Power(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("CCI MA Oscillator", new CCI_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Close and Reverse", new Close_and_Reverse(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Commodity Channel Index", new Commodity_Channel_Index(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Cumulative Sum", new Cumulative_Sum(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Data Bars Filter", new DataBarsFilter(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Date Filter", new DateFilter(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Day Closing", new Day_Closing(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Day of Week", new Day_of_Week(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Day Opening", new Day_Opening(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("DeMarker", new DeMarker(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Detrended Oscillator", new Detrended_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Directional Indicators", new Directional_Indicators(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Donchian Channel", new Donchian_Channel(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Ease of Movement", new Ease_of_Movement(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Enter Once", new Enter_Once(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Entry Hour", new Entry_Hour(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Entry Time", new Entry_Time(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Envelopes", new Envelopes(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Exit Hour", new Exit_Hour(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Fisher Transform", new Fisher_Transform(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Force Index", new Force_Index(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Fractal", new Fractal(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Heiken Ashi", new Heiken_Ashi(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Hourly High Low", new Hourly_High_Low(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Ichimoku Kinko Hyo", new Ichimoku_Kinko_Hyo(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Inside Bar", new Inside_Bar(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Keltner Channel", new Keltner_Channel(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Long or Short", new Long_or_Short(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Lot Limiter", new Lot_Limiter(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("MA Oscillator", new MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("MACD Histogram", new MACD_Histogram(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("MACD", new MACD(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Market Facilitation Index", new Market_Facilitation_Index(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Momentum MA Oscillator", new Momentum_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Momentum", new Momentum(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Money Flow Index", new Money_Flow_Index(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Money Flow", new Money_Flow(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Moving Average", new Moving_Average(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Moving Averages Crossover", new Moving_Averages_Crossover(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("N Bars Exit", new N_Bars_Exit(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Narrow Range", new Narrow_Range(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("OBOS MA Oscillator", new OBOS_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("On Balance Volume", new On_Balance_Volume(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of ATR", new Oscillator_of_ATR(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of BBP", new Oscillator_of_BBP(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of CCI", new Oscillator_of_CCI(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of MACD", new Oscillator_of_MACD(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of Momentum", new Oscillator_of_Momentum(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of OBOS", new Oscillator_of_OBOS(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of ROC", new Oscillator_of_ROC(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of RSI", new Oscillator_of_RSI(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Oscillator of Trix", new Oscillator_of_Trix(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Overbought Oversold Index", new Overbought_Oversold_Index(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Parabolic SAR", new Parabolic_SAR(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Percent Change", new Percent_Change(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Pivot Points", new Pivot_Points(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Previous Bar Closing", new Previous_Bar_Closing(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Previous Bar Opening", new Previous_Bar_Opening(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Previous High Low", new Previous_High_Low(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Price Move", new Price_Move(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Price Oscillator", new Price_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Random Filter", new Random_Filter(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Rate of Change", new Rate_of_Change(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Relative Vigor Index", new Relative_Vigor_Index(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("ROC MA Oscillator", new ROC_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Ross Hook", new Ross_Hook(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Round Number", new Round_Number(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("RSI MA Oscillator", new RSI_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("RSI", new RSI(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Standard Deviation", new Standard_Deviation(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Starc Bands", new Starc_Bands(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Steady Bands", new Steady_Bands(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Stochastics", new Stochastics(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Stop Limit", new Stop_Limit(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Stop Loss", new Stop_Loss(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Take Profit", new Take_Profit(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Top Bottom Price", new Top_Bottom_Price(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Trailing Stop Limit", new Trailing_Stop_Limit(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Trailing Stop", new Trailing_Stop(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Trix Index", new Trix_Index(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Trix MA Oscillator", new Trix_MA_Oscillator(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Week Closing", new Week_Closing(null, SlotTypes.NotDefined));
            OriginalIndicators.Add("Williams' Percent Range", new Williams_Percent_Range(null, SlotTypes.NotDefined));
        }

        /// <summary>
        /// Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> GetIndicatorNames(SlotTypes slotType)
        {
            var list = new List<string>();

            foreach (var record in AllIndicators)
                if (record.Value.TestPossibleSlot(slotType))
                    list.Add(record.Value.IndicatorName);

            return list;
        }

        /// <summary>
        /// Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> ListIndicatorNames(SlotTypes slotType)
        {
            switch (slotType)
            {
                case SlotTypes.NotDefined:
                    break;
                case SlotTypes.Open:
                    return OpenPointIndicators;
                case SlotTypes.OpenFilter:
                    return OpenFilterIndicators;
                case SlotTypes.Close:
                    return ClosePointIndicators;
                case SlotTypes.CloseFilter:
                    return CloseFilterIndicators;
            }

            return null;
        }

        /// <summary>
        /// Resets the custom indicators in the custom indicators list.
        /// </summary>
        public static void ResetCustomIndicators(IEnumerable<Indicator> indicatorList)
        {
            CustomIndicators.Clear();

            if (indicatorList == null)
                return;

            foreach (Indicator indicator in indicatorList)
                if (!CustomIndicators.ContainsKey(indicator.IndicatorName))
                    CustomIndicators.Add(indicator.IndicatorName, indicator);

            CustomIndicators.Sort();
        }

        /// <summary>
        /// Clears the indicator list and adds to it the original indicators.
        /// </summary>
        public static void CombineAllIndicators(IDataSet dataSet)
        {
            AllIndicators.Clear();

            foreach (var record in OriginalIndicators)
                if (!AllIndicators.ContainsKey(record.Key))
                    AllIndicators.Add(record.Key, record.Value);

            foreach (var record in CustomIndicators)
                if (!AllIndicators.ContainsKey(record.Key))
                    AllIndicators.Add(record.Key, record.Value);

            AllIndicators.Sort();

            OpenPointIndicators = GetIndicatorNames(SlotTypes.Open);
            ClosePointIndicators = GetIndicatorNames(SlotTypes.Close);
            OpenFilterIndicators = GetIndicatorNames(SlotTypes.OpenFilter);
            CloseFilterIndicators = GetIndicatorNames(SlotTypes.CloseFilter);

            foreach (string indicatorName in ClosePointIndicators)
            {
                Indicator indicator = ConstructIndicator(indicatorName, dataSet, SlotTypes.Close);
                if (indicator.AllowClosingFilters)
                    ClosingIndicatorsWithClosingFilters.Add(indicatorName);
            }
        }

        /// <summary>
        /// Constructs an indicator with specified name and slot type.
        /// </summary>
        public static Indicator ConstructIndicator(string indicatorName, IDataSet dataSet, SlotTypes slotType)
        {
            if (!AllIndicators.ContainsKey(indicatorName))
            {
                MessageBox.Show("There is no indicator named: " + indicatorName);
                return null;
            }

            Type indicatorType = AllIndicators[indicatorName].GetType();
            var parameterType = new[] {dataSet.GetType(), slotType.GetType()};
            ConstructorInfo constructorInfo = indicatorType.GetConstructor(parameterType);
            if (constructorInfo != null)
                return (Indicator)constructorInfo.Invoke(new object[] { dataSet, slotType });

            MessageBox.Show("Error with indicator named: " + indicatorName);
            return null;
        }
    }
}