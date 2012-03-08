// Data class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Forex_Strategy_Builder.Interfaces;
using Forex_Strategy_Builder.Market;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    /// <summary>
    ///  Base class containing the data.
    /// </summary>
    public static class Data
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        static Data()
        {
            Icon = Resources.Icon;
            PointChar = '.';
            DFS = "dd.MM";
            DF = "dd.MM.yy";
            AutoUsePrvBarValue = true;
            PeriodColor = new Dictionary<DataPeriods, Color>();
            AdditionalFolder = "Additional" + Path.DirectorySeparatorChar;
            SourceFolder = "Custom Indicators" + Path.DirectorySeparatorChar;
            DefaultStrategyDir = "Strategies" + Path.DirectorySeparatorChar;
            ColorDir = "Colors" + Path.DirectorySeparatorChar;
            LanguageDir = "Languages" + Path.DirectorySeparatorChar;
            SystemDir = "System" + Path.DirectorySeparatorChar;
            ProgramDir = "";
            ProgramName = "Forex Strategy Builder";
            IsProgramRC = false;
            IsProgramBeta = false;
            LoadedSavedStrategy = "";
            StrategyDir = "Strategies" + Path.DirectorySeparatorChar;
            OfflineDocsDir = "Docs" + Path.DirectorySeparatorChar;
            DefaultOfflineDataDir = "Data" + Path.DirectorySeparatorChar;
            OfflineDataDir = "Data" + Path.DirectorySeparatorChar;
            Debug = false;
            IsData = false;
            IsResult = false;
            IsStrategyChanged = false;
            StackStrategy = new Stack<Strategy>();
            GeneratorHistory = new List<Strategy>();
            DataSet = new DataSet {IsIntrabarData = false};
            DataStats = new DataStatistics();

            // Program's Major, Minor, Version and Build numbers must be <= 99.
            ProgramVersion = Application.ProductVersion;
            string[] version = ProgramVersion.Split('.');
            ProgramID = 1000000*int.Parse(version[0]) +
                        10000*int.Parse(version[1]) +
                        100*int.Parse(version[2]) +
                        1*int.Parse(version[3]);

            if (int.Parse(version[1])%2 != 0)
                IsProgramBeta = true;
        }

        /// <summary>
        /// Gets the program name.
        /// </summary>
        public static string ProgramName { get; private set; }

        /// <summary>
        /// Programs icon.
        /// </summary>
        public static Icon Icon { get; private set; }

        /// <summary>
        /// Gets the program version.
        /// </summary>
        public static string ProgramVersion { get; private set; }

        /// <summary>
        /// Gets the program Beta state.
        /// </summary>
        public static bool IsProgramBeta { get; private set; }

        /// <summary>
        /// Gets the program Release Candidate.
        /// </summary>
        public static bool IsProgramRC { get; private set; }

        /// <summary>
        /// Gets the program ID
        /// </summary>
        public static int ProgramID { get; private set; }

        /// <summary>
        /// Gets the program current working directory.
        /// </summary>
        public static string ProgramDir { get; private set; }

        /// <summary>
        /// Gets the path to System Dir.
        /// </summary>
        public static string SystemDir { get; private set; }

        /// <summary>
        /// Gets the path to LanguageDir Dir.
        /// </summary>
        public static string LanguageDir { get; private set; }

        /// <summary>
        /// Gets the path to Color Scheme Dir.
        /// </summary>
        public static string ColorDir { get; private set; }

        /// <summary>
        /// Gets or sets the data directory.
        /// </summary>
        public static string OfflineDataDir { get; set; }

        /// <summary>
        /// Gets the default data directory.
        /// </summary>
        public static string DefaultOfflineDataDir { get; private set; }

        /// <summary>
        /// Gets or sets the docs directory.
        /// </summary>
        private static string OfflineDocsDir { get; set; }

        /// <summary>
        /// Gets the path to Default Strategy Dir.
        /// </summary>
        public static string DefaultStrategyDir { get; private set; }

        /// <summary>
        /// Gets or sets the path to dir Strategy.
        /// </summary>
        public static string StrategyDir { get; set; }

        /// <summary>
        /// Gets or sets the custom indicators folder
        /// </summary>
        public static string SourceFolder { get; private set; }

        /// <summary>
        /// Gets or sets the Additional  folder
        /// </summary>
        public static string AdditionalFolder { get; private set; }

        /// <summary>
        /// Gets or sets the strategy name for Configs.LastStrategy
        /// </summary>
        public static string LoadedSavedStrategy { get; set; }

        /// <summary>
        /// The current strategy undo
        /// </summary>
        public static Stack<Strategy> StackStrategy { get; private set; }

        /// <summary>
        /// The Generator History
        /// </summary>
        public static List<Strategy> GeneratorHistory { get; private set; }

        /// <summary>
        /// The Generator History current strategy
        /// </summary>
        public static int GenHistoryIndex { get; set; }

        /// <summary>
        /// The scanner colors
        /// </summary>
        public static Dictionary<DataPeriods, Color> PeriodColor { get; private set; }

        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool Debug { get; set; }

        public static string Symbol
        {
            get { return DataSet.InstrProperties.Symbol; }
        }

        public static string PeriodString
        {
            get { return DataPeriodToString(DataSet.Period); }
        }

        public static bool IsData { get; set; }
        public static bool IsResult { get; set; }
        public static bool IsStrategyChanged { get; set; }

        /// <summary>
        /// Sets or gets value of the AutoUsePrvBarValue
        /// </summary>
        public static bool AutoUsePrvBarValue { get; set; }

        /// <summary>
        /// Gets the number format.
        /// </summary>
        public static string FF
        {
            get { return "F" + DataSet.InstrProperties.Digits; }
        }

        /// <summary>
        /// Gets the date format.
        /// </summary>
        public static string DF { get; private set; }

        /// <summary>
        /// Gets the short date format.
        /// </summary>
        public static string DFS { get; private set; }

        /// <summary>
        /// Gets the point character
        /// </summary>
        public static char PointChar { get; private set; }

        /// <summary>
        /// Relative font height
        /// </summary>
        public static float VerticalDLU { get; set; }

        /// <summary>
        /// Relative font width
        /// </summary>
        public static float HorizontalDLU { get; set; }

        /// <summary>
        /// Loaded historical data.
        /// </summary>
        public static IDataSet DataSet { get; private set; }

        /// <summary>
        /// Market statistics about the loaded data.
        /// </summary>
        public static DataStatistics DataStats { get; set; }

        /// <summary>
        /// Initial settings.
        /// </summary>
        public static void Start()
        {
            // Sets the date format.
            if (DateTimeFormatInfo.CurrentInfo != null)
                DF = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            if (DF == "dd/MM yyyy") DF = "dd/MM/yyyy"; // Fixes the Uzbek (Latin) issue
            DF = DF.Replace(" ", ""); // Fixes the Slovenian issue
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                char[] acDS = DateTimeFormatInfo.CurrentInfo.DateSeparator.ToCharArray();
                string[] asSS = DF.Split(acDS, 3);
                asSS[0] = asSS[0].Substring(0, 1) + asSS[0].Substring(0, 1);
                asSS[1] = asSS[1].Substring(0, 1) + asSS[1].Substring(0, 1);
                asSS[2] = asSS[2].Substring(0, 1) + asSS[2].Substring(0, 1);
                DF = asSS[0] + acDS[0] + asSS[1] + acDS[0] + asSS[2];

                if (asSS[0].ToUpper() == "YY")
                    DFS = asSS[1] + acDS[0] + asSS[2];
                else if (asSS[1].ToUpper() == "YY")
                    DFS = asSS[0] + acDS[0] + asSS[2];
                else
                    DFS = asSS[0] + acDS[0] + asSS[1];
            }

            // Point character
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            PointChar = cultureInfo.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];

            // Set the working directories
            ProgramDir = Directory.GetCurrentDirectory();
            DefaultOfflineDataDir = Path.Combine(ProgramDir, OfflineDataDir);
            OfflineDataDir = DefaultOfflineDataDir;
            OfflineDocsDir = Path.Combine(ProgramDir, OfflineDocsDir);
            StrategyDir = Path.Combine(ProgramDir, DefaultStrategyDir);
            SourceFolder = Path.Combine(ProgramDir, SourceFolder);
            SystemDir = Path.Combine(ProgramDir, SystemDir);
            LanguageDir = Path.Combine(SystemDir, LanguageDir);
            ColorDir = Path.Combine(SystemDir, ColorDir);

            // Scanner colors
            PeriodColor.Add(DataPeriods.min1, Color.Yellow);
            PeriodColor.Add(DataPeriods.min5, Color.Lime);
            PeriodColor.Add(DataPeriods.min15, Color.Green);
            PeriodColor.Add(DataPeriods.min30, Color.Orange);
            PeriodColor.Add(DataPeriods.hour1, Color.DarkSalmon);
            PeriodColor.Add(DataPeriods.hour4, Color.Peru);
            PeriodColor.Add(DataPeriods.day, Color.Red);
            PeriodColor.Add(DataPeriods.week, Color.DarkViolet);
        }

        /// <summary>
        /// Converts a data period from DataPeriods type to string.
        /// </summary>
        public static string DataPeriodToString(DataPeriods dataPeriod)
        {
            switch (dataPeriod)
            {
                case DataPeriods.min1:
                    return "1 " + Language.T("Minute");
                case DataPeriods.min5:
                    return "5 " + Language.T("Minutes");
                case DataPeriods.min15:
                    return "15 " + Language.T("Minutes");
                case DataPeriods.min30:
                    return "30 " + Language.T("Minutes");
                case DataPeriods.hour1:
                    return "1 " + Language.T("Hour");
                case DataPeriods.hour4:
                    return "4 " + Language.T("Hours");
                case DataPeriods.day:
                    return "1 " + Language.T("Day");
                case DataPeriods.week:
                    return "1 " + Language.T("Week");
                default:
                    return String.Empty;
            }
        }
    }
}