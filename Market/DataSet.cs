// DataStore Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using Forex_Strategy_Builder.Interfaces;

namespace Forex_Strategy_Builder.Market
{

    public class DataSet : IDataSet
    {
        public string Symbol { get { return InstrProperties.Symbol; } }
        public DataPeriods Period { get; set; }
        public int Bars { get; set; }

        // Instrument Properties
        public InstrumentProperties InstrProperties { get; set; }
        
        // Market data
        public DateTime[] Time { get; set; }
        public double[] Open { get; set; }
        public double[] High { get; set; }
        public double[] Low { get; set; }
        public double[] Close { get; set; }
        public int[] Volume { get; set; }

        // Intrabar data
        public bool IsIntrabarData { get; set; }
        public int LoadedIntraBarPeriods { get; set; }
        public DataPeriods[] IntraBarsPeriods { get; set; }
        public Bar[][] IntraBarData { get; set; }
        public int[] IntraBarBars { get; set; }
        public int[] IntraBars { get; set; }

        // Tick data
        public bool IsTickData { get; set; }
        public long Ticks { get; set; }
        public double[][] TickData { get; set; }

        public string PeriodString
        {
            get { return Data.DataPeriodToString(Period); }
        }

        /// <summary>
        /// Gets the number format.
        /// </summary>
        public string FF
        {
            get { return "F" + InstrProperties.Digits; }
        }
    }
}
