using System;
using System.Globalization;

namespace Forex_Strategy_Builder.Market
{
    public class DataStatistics
    {
        public DataStatistics()
        {
            InitMarketStatistic();
        }

        // Statistical information for the instrument data
        public string Symbol { get; set; }
        public DataPeriods Period { get; set; }
        public int Bars { get; set; }
        public DateTime Beginning { get; set; }
        public DateTime Update { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int DaysOff { get; set; }
        public int AverageGap { private get; set; }
        public int MaxGap { private get; set; }
        public int AverageHighLow { private get; set; }
        public int MaxHighLow { private get; set; }
        public int AverageCloseOpen { private get; set; }
        public int MaxCloseOpen { private get; set; }
        public bool DataCut { get; set; }

        /// <summary>
        /// Gets the market stats parameters
        /// </summary>
        public string[] MarketStatsParam { get; private set; }

        /// <summary>
        /// Gets the market stats values
        /// </summary>
        public string[] MarketStatsValue { get; private set; }

        /// <summary>
        /// Gets the market stats flags
        /// </summary>
        public bool[] MarketStatsFlag { get; private set; }

        /// <summary>
        /// Initializes the stats names
        /// </summary>
        public void InitMarketStatistic()
        {
            MarketStatsParam = new[]
            {
                Language.T("Symbol"),
                Language.T("Period"),
                Language.T("Number of bars"),
                Language.T("Date of updating"),
                Language.T("Time of updating"),
                Language.T("Date of beginning"),
                Language.T("Time of beginning"),
                Language.T("Minimum price"),
                Language.T("Maximum price"),
                Language.T("Average Gap"),
                Language.T("Maximum Gap"),
                Language.T("Average High-Low"),
                Language.T("Maximum High-Low"),
                Language.T("Average Close-Open"),
                Language.T("Maximum Close-Open"),
                Language.T("Maximum days off"),
                Language.T("Maximum data bars"),
                Language.T("No data older than"),
                Language.T("No data newer than"),
                Language.T("Fill In Data Gaps"),
                Language.T("Cut Off Bad Data")
            };

            MarketStatsValue = new string[21];
            MarketStatsFlag = new bool[21];
        }

        /// <summary>
        /// Generate the Market Statistics
        /// </summary>
        public void GenerateMarketStats()
        {
            MarketStatsValue[0] = Symbol;
            MarketStatsValue[1] = Data.DataPeriodToString(Period);
            MarketStatsValue[2] = Bars.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[3] = Update.ToString(Data.DF);
            MarketStatsValue[4] = Update.ToString("HH:mm");
            MarketStatsValue[5] = Beginning.ToString(Data.DF);
            MarketStatsValue[6] = Beginning.ToString("HH:mm");
            MarketStatsValue[7] = MinPrice.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[8] = MaxPrice.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[9] = AverageGap + " " + Language.T("pips");
            MarketStatsValue[10] = MaxGap + " " + Language.T("pips");
            MarketStatsValue[11] = AverageHighLow + " " + Language.T("pips");
            MarketStatsValue[12] = MaxHighLow + " " + Language.T("pips");
            MarketStatsValue[13] = AverageCloseOpen + " " + Language.T("pips");
            MarketStatsValue[14] = MaxCloseOpen + " " + Language.T("pips");
            MarketStatsValue[15] = DaysOff.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[16] = Configs.MaxBars.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[17] = Configs.UseStartTime ? Configs.DataStartTime.ToShortDateString() : Language.T("No limits");
            MarketStatsValue[18] = Configs.UseEndTime ? Configs.DataEndTime.ToShortDateString() : Language.T("No limits");
            MarketStatsValue[19] = Configs.FillInDataGaps ? Language.T("Accomplished") : Language.T("Switched off");
            MarketStatsValue[20] = Configs.CutBadData ? Language.T("Accomplished") : Language.T("Switched off");
        }
    }
}
