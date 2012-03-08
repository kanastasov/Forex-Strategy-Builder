// Forex Strategy Builder - Controls class.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : MenuAndStatusBar
    {
        protected Backtester Backtester;
        
        /// <summary>
        /// The default constructor.
        /// </summary>
        protected Controls()
        {
            InitializeMarket();
            InitializeStrategy();
            InitializeAccount();
            InitializeJournal();
        }
    }
}
