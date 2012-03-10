// Instrument class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    public class Instrument
    {
        private readonly InstrumentProperties _instrProperties; // The instrument properties.
        private Bar[] _aBar; // An array containing the data

        /// <summary>
        /// Constructor
        /// </summary>
        public Instrument(InstrumentProperties instrProperties, int period)
        {
            DataDir = "." + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar;
            EndTime = new DateTime(2020, 1, 1, 0, 0, 0);
            StartTime = new DateTime(1990, 1, 1, 0, 0, 0);
            MaxBars = 20000;
            _instrProperties = instrProperties;
            Period = period;
        }

        // Statistical information
        public double MinPrice { get; private set; }
        public double MaxPrice { get; private set; }
        public int MaxGap { get; private set; }
        public int DaysOff { get; private set; }
        public int MaxHighLow { get; private set; }
        public int MaxCloseOpen { get; private set; }
        public int AverageGap { get; private set; }
        public int AverageHighLow { get; private set; }
        public int AverageCloseOpen { get; private set; }
        public bool Cut { get; private set; }


        // General instrument info
        public string Symbol { get { return _instrProperties.Symbol; } }
        private int Period { get; set; }
        public int Bars { get; private set; }
        private double Point { get { return _instrProperties.Point; } }
        public DateTime Update { get; private set; }

        // -------------------------------------------------------------

        public int MaxBars { private get; set; }
        public DateTime StartTime { private get; set; }
        public DateTime EndTime { private get; set; }
        public bool UseEndTime { private get; set; }
        public bool UseStartTime { private get; set; }
        public string DataDir { private get; set; }

        // -------------------------------------------------------------

        // Bar info
        public DateTime Time(int bar)
        {
            return _aBar[bar].Time;
        }

        public double Open(int bar)
        {
            return _aBar[bar].Open;
        }

        public double High(int bar)
        {
            return _aBar[bar].High;
        }

        public double Low(int bar)
        {
            return _aBar[bar].Low;
        }

        public double Close(int bar)
        {
            return _aBar[bar].Close;
        }

        public int Volume(int bar)
        {
            return _aBar[bar].Volume;
        }

        /// <summary>
        /// Loads the data file
        /// </summary>
        /// <returns>0 - success</returns>
        public int LoadData()
        {
            // The source data file full name
            string sourceDataFile = DataDir + _instrProperties.BaseFileName + Period + ".csv";

            // Checks the access to the file
            if (!File.Exists(sourceDataFile))
                return 1;

            var sr = new StreamReader(sourceDataFile);
            string sData = sr.ReadToEnd();
            sr.Close();

            var dp = new DataParser();

            int respond = -1;
            int parsedBars = dp.Parse(sData);

            if (parsedBars > 0)
            {
                _aBar = dp.Bar;
                Bars = parsedBars;
                RefineData();
                DataHorizon();
                CheckMarketData();
                SetDataStats();
                Update = _aBar[Bars - 1].Time;
                respond = 0;
            }

            return respond;
        }

        /// <summary>
        /// Loads the data file
        /// </summary>
        /// <returns>0 - success</returns>
        public int LoadResourceData()
        {
            var dataParser = new DataParser();
            int respond = -1;
            int parsedBars = dataParser.Parse(Resources.EURUSD1440);

            if (parsedBars > 0)
            {
                _aBar = dataParser.Bar;
                Bars = parsedBars;
                RefineData();
                DataHorizon();
                CheckMarketData();
                SetDataStats();
                Update = _aBar[Bars - 1].Time;
                respond = 0;
            }

            return respond;
        }

        /// <summary>
        /// Refines the market data
        /// </summary>
        private void RefineData()
        {
            // Fill In data gaps
            if (Configs.FillInDataGaps)
            {
                for (int bar = 1; bar < Bars; bar++)
                {
                    _aBar[bar].Open = _aBar[bar - 1].Close;
                    if (_aBar[bar].Open > _aBar[bar].High || _aBar[bar].Close > _aBar[bar].High)
                        _aBar[bar].High = _aBar[bar].Open > _aBar[bar].Close ? _aBar[bar].Open : _aBar[bar].Close;
                    if (_aBar[bar].Open < _aBar[bar].Low || _aBar[bar].Close < _aBar[bar].Low)
                        _aBar[bar].Low = _aBar[bar].Open < _aBar[bar].Close ? _aBar[bar].Open : _aBar[bar].Close;
                }
            }

            // Cuts off the bad data
            if (Configs.CutBadData)
            {
                int maxConsecutiveBars = 0;
                int maxConsecutiveBar = 0;
                int consecutiveBars = 0;
                int lastBar = 0;

                for (int bar = 0; bar < Bars; bar++)
                {
                    if (Math.Abs(_aBar[bar].Open - _aBar[bar].Close) < 0.00001)
                    {
                        if (lastBar == bar - 1 || lastBar == 0)
                        {
                            consecutiveBars++;
                            lastBar = bar;

                            if (consecutiveBars > maxConsecutiveBars)
                            {
                                maxConsecutiveBars = consecutiveBars;
                                maxConsecutiveBar = bar;
                            }
                        }
                    }
                    else
                    {
                        consecutiveBars = 0;
                    }
                }

                if (maxConsecutiveBars < 10)
                    maxConsecutiveBar = 0;

                int firstBar = Math.Max(maxConsecutiveBar, 1);
                for (int bar = firstBar; bar < Bars; bar++)
                    if ((Time(bar) - Time(bar - 1)).Days > 5 && Period < 10080)
                        firstBar = bar;

                if (firstBar == 1)
                    firstBar = 0;

                if (firstBar > 0)
                {
                    var aBarCopy = new Bar[Bars];
                    _aBar.CopyTo(aBarCopy, 0);

                    _aBar = new Bar[Bars - firstBar];
                    for (int bar = firstBar; bar < Bars; bar++)
                        _aBar[bar - firstBar] = aBarCopy[bar];

                    Bars = Bars - firstBar;
                    Cut = true;
                }
            }
        }

        /// <summary>
        /// Data Horizon - Cuts some data
        /// </summary>
        private void DataHorizon()
        {
            if (Bars < Configs.MinBars) return;

            int startBar = 0;
            int endBar = Bars - 1;

            // Set the starting date
            if (UseStartTime && _aBar[0].Time < StartTime)
            {
                for (int bar = 0; bar < Bars; bar++)
                {
                    if (_aBar[bar].Time >= StartTime)
                    {
                        startBar = bar;
                        break;
                    }
                }
            }

            // Set the end date
            if (UseEndTime && _aBar[Bars - 1].Time > EndTime)
            {
                // We need to cut out the newest bars
                for (int bar = 0; bar < Bars; bar++)
                {
                    if (_aBar[bar].Time >= EndTime)
                    {
                        endBar = bar - 1;
                        break;
                    }
                }
            }

            if (endBar - startBar > MaxBars - 1)
            {
                startBar = endBar - MaxBars + 1;
            }

            if (endBar - startBar < Configs.MinBars)
            {
                startBar = endBar - Configs.MinBars + 1;
                if (startBar < 0)
                {
                    startBar = 0;
                    endBar = Configs.MinBars - 1;
                }
            }

            // Cut the data
            if (startBar > 0 || endBar < Bars - 1)
            {
                var aBarCopy = new Bar[Bars];
                _aBar.CopyTo(aBarCopy, 0);

                int newBars = endBar - startBar + 1;

                _aBar = new Bar[newBars];
                for (int bar = startBar; bar <= endBar; bar++)
                    _aBar[bar - startBar] = aBarCopy[bar];

                Bars = newBars;
                Update = _aBar[newBars - 1].Time;
                Cut = true;
            }
        }

        /// <summary>
        /// Checks the loaded data
        /// </summary>
        private void CheckMarketData()
        {
            // Not Implemented yet
            //for (int bar = 0; bar < Bars; bar++)
            //{
            //    if (High(bar) < Open(bar) ||
            //        High(bar) < Low(bar) ||
            //        High(bar) < Close(bar) ||
            //        Low(bar) > Open(bar) ||
            //        Low(bar) > Close(bar))
            //    {
            //        return true;
            //    }
            //}
        }

        /// <summary>
        /// Calculate statistics for the loaded data.
        /// </summary>
        private void SetDataStats()
        {
            MinPrice = double.MaxValue;
            MaxPrice = double.MinValue;
            double maxHighLowPrice = double.MinValue;
            double maxCloseOpenPrice = double.MinValue;
            double sumHighLow = 0;
            double sumCloseOpen = 0;
            DaysOff = 0;
            double sumGap = 0;
            double instrMaxGap = double.MinValue;

            for (int bar = 1; bar < Bars; bar++)
            {
                if (High(bar) > MaxPrice)
                    MaxPrice = High(bar);

                if (Low(bar) < MinPrice)
                    MinPrice = Low(bar);

                if (Math.Abs(High(bar) - Low(bar)) > maxHighLowPrice)
                    maxHighLowPrice = Math.Abs(High(bar) - Low(bar));
                sumHighLow += Math.Abs(High(bar) - Low(bar));

                if (Math.Abs(Close(bar) - Open(bar)) > maxCloseOpenPrice)
                    maxCloseOpenPrice = Math.Abs(Close(bar) - Open(bar));
                sumCloseOpen += Math.Abs(Close(bar) - Open(bar));

                int dayDiff = (Time(bar) - Time(bar - 1)).Days;
                if (DaysOff < dayDiff)
                    DaysOff = dayDiff;

                double gap = Math.Abs(Open(bar) - Close(bar - 1));
                sumGap += gap;
                if (instrMaxGap < gap)
                    instrMaxGap = gap;
            }

            MaxHighLow = (int) (maxHighLowPrice/Point);
            AverageHighLow = (int) (sumHighLow/(Bars*Point));
            MaxCloseOpen = (int) (maxCloseOpenPrice/Point);
            AverageCloseOpen = (int) (sumCloseOpen/(Bars*Point));
            MaxGap = (int) (instrMaxGap/Point);
            AverageGap = (int) (sumGap/((Bars - 1)*Point));
        }
    }
}