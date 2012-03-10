// IDataStore Interface
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder.Interfaces
{
    public interface IDataSet
    {
        string Symbol { get; }
        DataPeriods Period { get; set; }
        int Bars { get; set; }

        // Instrument Properties
        InstrumentProperties InstrProperties { get; set; }
        
        // Market data
        DateTime[] Time { get; set; }
        double[] Open { get; set; }
        double[] High { get; set; }
        double[] Low { get; set; }
        double[] Close { get; set; }
        int[] Volume { get; set; }

        // Intrabar data
        bool IsIntrabarData { get; set; }
        int LoadedIntraBarPeriods { get; set; }
        DataPeriods[] IntraBarsPeriods { get; set; }
        Bar[][] IntraBarData { get; set; }
        int[] IntraBarBars { get; set; }
        int[] IntraBars { get; set; }

        // Tick data
        bool IsTickData { get; set; }
        long Ticks { get; set; }
        double[][] TickData { get; set; }

        string PeriodString { get; }
        string FF { get; }
    }
}