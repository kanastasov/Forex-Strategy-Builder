// Forex Strategy Builder - Controls class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using Forex_Strategy_Builder.Market;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : MenuAndStatusBar
    {
        protected readonly Backtester Backtester;
        
        /// <summary>
        /// The default constructor.
        /// </summary>
        protected Controls()
        {
            Backtester = new Backtester();
            Backtester.DataSet = new DataSet();
            Backtester.Strategy = Strategy.GenerateNew(Backtester.DataSet);
            Backtester.DataSet.InstrProperties = Instruments.InstrumentList[Backtester.Strategy.Symbol].Clone();

            InitializeMarket();
            InitializeStrategy();
            InitializeAccount();
            InitializeJournal();
        }
    }
}
