// Parameter class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// Provide links to the Parameter's fields
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Parameter(Backtester backtester, OptimizerParameterType type, int slotNumber, int paramNumber)
        {
            _backtester = backtester;
            Type = type;
            SlotNumber = slotNumber;
            NumParam = paramNumber;
            _bestValue = Value;
            OldBestValue = Value;
        }

        private readonly Backtester _backtester;

        /// <summary>
        /// Type of the parameter
        /// </summary>
        public OptimizerParameterType Type { get; private set; }

        /// <summary>
        /// The number of the indicator slot
        /// </summary>
        public int SlotNumber { get; private set; }

        /// <summary>
        /// The number of NumericParam
        /// </summary>
        public int NumParam { get; private set; }

        /// <summary>
        /// The IndicatorParameters
        /// </summary>
        public IndicatorParam IndParam
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? _backtester.Strategy.Slot[SlotNumber].IndParam
                           : null;
            }
        }

        /// <summary>
        /// Parameter group name
        /// </summary>
        public string GroupName
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? _backtester.Strategy.Slot[SlotNumber].IndicatorName
                           : Language.T("Permanent Protection");
            }
        }

        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName
        {
            get
            {
                string name = string.Empty;
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        name = _backtester.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Caption;
                        break;
                    case OptimizerParameterType.PermanentSL:
                        name = Language.T("Permanent Stop Loss");
                        break;
                    case OptimizerParameterType.PermanentTP:
                        name = Language.T("Permanent Take Profit");
                        break;
                    case OptimizerParameterType.BreakEven:
                        name = Language.T("Break Even");
                        break;
                }
                return name;
            }
        }

        /// <summary>
        /// The current value of the parameter
        /// </summary>
        public double Value
        {
            get
            {
                double value = 0.0;
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        value = _backtester.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Value;
                        break;
                    case OptimizerParameterType.PermanentSL:
                        value = _backtester.Strategy.PermanentSL;
                        break;
                    case OptimizerParameterType.PermanentTP:
                        value = _backtester.Strategy.PermanentTP;
                        break;
                    case OptimizerParameterType.BreakEven:
                        value = _backtester.Strategy.BreakEven;
                        break;
                }
                return value;
            }
            set
            {
                switch (Type)
                {
                    case OptimizerParameterType.Indicator:
                        _backtester.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Value = value;
                        break;
                    case OptimizerParameterType.PermanentSL:
                        _backtester.Strategy.PermanentSL = (int) value;
                        break;
                    case OptimizerParameterType.PermanentTP:
                        _backtester.Strategy.PermanentTP = (int) value;
                        break;
                    case OptimizerParameterType.BreakEven:
                        _backtester.Strategy.BreakEven = (int) value;
                        break;
                }
            }
        }

        /// <summary>
        /// The minimum value of the parameter
        /// </summary>
        public double Minimum
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? _backtester.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Min
                           : 5;
            }
        }

        /// <summary>
        /// The maximum value of the parameter
        /// </summary>
        public double Maximum
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? _backtester.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Max
                           : 5000;
            }
        }

        /// <summary>
        /// The number of significant digits
        /// </summary>
        public int Point
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? _backtester.Strategy.Slot[SlotNumber].IndParam.NumParam[NumParam].Point
                           : 0;
            }
        }

        /// <summary>
        /// The maximum value of the parameter
        /// </summary>
        public double Step
        {
            get
            {
                return Type == OptimizerParameterType.Indicator
                           ? Math.Round(Math.Pow(10, -Point), Point)
                           : (_backtester.DataSet.InstrProperties.IsFiveDigits ? 10 : 1);
            }
        }

        /// <summary>
        /// The best value of the parameter
        /// </summary>
        public double BestValue
        {
            get { return _bestValue; }
            set
            {
                OldBestValue = _bestValue;
                _bestValue = value;
            }
        }

        /// <summary>
        /// The previous best value of the parameter
        /// </summary>
        public double OldBestValue { get; private set; }

        private double _bestValue;
    }
}