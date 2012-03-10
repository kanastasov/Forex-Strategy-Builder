// StrategyFormatOverview
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Forex_Strategy_Builder
{
    public static class StrategyFormatOverview
    {
        private static Backtester _backtester;

        /// <summary>
        /// Generate Overview in HTML code
        /// </summary>
        /// <returns>HTML code</returns>
        public static string FormatOverview(Backtester backtester, bool isDescriptionRelevant)
        {
            _backtester = backtester;

            var sb = new StringBuilder();

            // Header
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">");
            sb.AppendLine("<head><meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" />");
            sb.AppendLine("<title>" + _backtester.Strategy.StrategyName + "</title>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {padding: 0 10px 10px 10px; margin: 0px; background-color: #fff; color: #003; font-size: 16px}");
            sb.AppendLine(".content h1 {font-size: 1.4em;}");
            sb.AppendLine(".content h2 {font-size: 1.2em;}");
            sb.AppendLine(".content h3 {font-size: 1em;}");
            sb.AppendLine(".content p { }");
            sb.AppendLine(".content p.fsb_go_top {text-align: center;}");
            sb.AppendLine(".fsb_strategy_slot {font-family: sans-serif; width: 30em; margin: 2px auto; text-align: center; background-color: #f3ffff; }");
            sb.AppendLine(".fsb_strategy_slot table tr td {text-align: left; color: #033; font-size: 75%;}");
            sb.AppendLine(".fsb_properties_slot {color: #fff; padding: 2px 0px; background: #966; }");
            sb.AppendLine(".fsb_open_slot {color: #fff; padding: 2px 0; background: #693; }");
            sb.AppendLine(".fsb_close_slot {color: #fff; padding: 2px 0; background: #d63; }");
            sb.AppendLine(".fsb_open_filter_slot {color: #fff; padding: 2px 0; background: #699;}");
            sb.AppendLine(".fsb_close_filter_slot {color: #fff; padding: 2px 0; background: #d99;}");
            sb.AppendLine(".fsb_str_indicator {padding: 5px 0; color: #6090c0;}");
            sb.AppendLine(".fsb_str_logic {padding-bottom: 5px; color: #066;}");
            sb.AppendLine(".fsb_table {margin: 0 auto; border: 2px solid #003; border-collapse: collapse;}");
            sb.AppendLine(".fsb_table th {border: 1px solid #006; text-align: center; background: #ccf; border-bottom-width: 2px;}");
            sb.AppendLine(".fsb_table td {border: 1px solid #006;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"content\" id=\"fsb_header\">");

            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            sb.AppendLine("<h1>" + Language.T("Strategy Overview") + "</h1>");
            sb.AppendLine("<p>");
            sb.AppendLine("\t" + Language.T("Strategy name") + ": <strong>" + _backtester.Strategy.StrategyName + "</strong><br />");
            sb.AppendLine("\t" + Data.ProgramName + " v" + Data.ProgramVersion + stage + "<br />");
            sb.AppendLine("\t" + Language.T("Date") + ": " + DateTime.Now);
            sb.AppendLine("</p>");

            // Contents
            sb.AppendLine("<h2 id=\"contents\">" + Language.T("Table of Contents") + "</h2>");
            sb.AppendLine("<a href=\"#description\">" + Language.T("Description") + "</a><br />");
            sb.AppendLine("<a href=\"#logic\">" + Language.T("Logic") + "</a><br />");
            sb.AppendLine("<a href=\"#environment\">" + Language.T("Environment") + "</a><br />");
            sb.AppendLine("<a href=\"#properties\">" + Language.T("Strategy Properties") + "</a><br />");
            sb.AppendLine("<a href=\"#indicators\">" + Language.T("Indicator Slots") + "</a><br />");

            if (Configs.AdditionalStatistics)
            {
                sb.AppendLine("<a href=\"#statistics\">" + Language.T("Statistic Information") + "</a><br />");
                sb.AppendLine("<a href=\"#addstats\">" + Language.T("Additional Statistics") + "</a>");
            }
            else
            {
                sb.AppendLine("<a href=\"#statistics\">" + Language.T("Statistic Information") + "</a>");
            }

            // Description
            sb.AppendLine("<h2 id=\"description\">" + Language.T("Description") + "</h2>");
            if (_backtester.Strategy.Description != String.Empty)
            {
                string strStrategyDescription = _backtester.Strategy.Description.Replace(Environment.NewLine, "<br />");
                strStrategyDescription = strStrategyDescription.Replace("&", "&amp;");
                strStrategyDescription = strStrategyDescription.Replace("\"", "&quot;");

                if (isDescriptionRelevant)
                    sb.AppendLine("<p style=\"color: #a00\">" + "(" + Language.T("This description might be outdated!") + ")" + "</p>");

                sb.AppendLine("<p>" + strStrategyDescription + "</p>");

                sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");
            }
            else
                sb.AppendLine("<p>" + Language.T("None") + ".</p>");

            // Strategy Logic
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"logic\">" + Language.T("Logic") + "</h2>");

            // Opening
            sb.AppendLine("<h3>" + Language.T("Opening (Entry Signal)") + "</h3>");
            sb.AppendLine(OpeningLogicHTMLReport().ToString());

            // Closing
            sb.AppendLine("<h3>" + Language.T("Closing (Exit Signal)") + "</h3>");
            sb.AppendLine(ClosingLogicHTMLReport().ToString());

            // Averaging
            sb.AppendLine("<h3>" + Language.T("Handling of Additional Entry Signals") + "**</h3>");
            sb.AppendLine(AveragingHTMLReport().ToString());

            // Trading Sizes
            sb.AppendLine("<h3>" + Language.T("Trading Size") + "</h3>");
            sb.AppendLine(TradingSizeHTMLReport().ToString());

            // Protection
            sb.AppendLine("<h3>" + Language.T("Permanent Protection") + "</h3>");
            if (!_backtester.Strategy.UsePermanentSL)
            {
                sb.AppendLine("<p>" + Language.T("The strategy does not provide a permanent loss limitation.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Stop Loss limits the loss of a position to") +
                              (_backtester.Strategy.PermanentSLType == PermanentProtectionType.Absolute
                                   ? " (Abs) "
                                   : " ") + _backtester.Strategy.PermanentSL);
                sb.AppendLine(Language.T("pips per open lot (plus the charged spread and rollover).") + "</p>");
            }

            if (!_backtester.Strategy.UsePermanentTP)
            {
                sb.AppendLine("<p>" + Language.T("The strategy does not use a Permanent Take Profit.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Take Profit closes a position at") +
                              (_backtester.Strategy.PermanentTPType == PermanentProtectionType.Absolute
                                   ? " (Abs) "
                                   : " ") + _backtester.Strategy.PermanentTP);
                sb.AppendLine(Language.T("pips profit.") + "</p>");
            }

            if (_backtester.Strategy.UseBreakEven)
            {
                sb.AppendLine("<p>" +
                              Language.T("The position's Stop Loss will be set to Break Even price when the profit reaches") +
                              " " + _backtester.Strategy.BreakEven);
                sb.AppendLine(Language.T("pips") + "." + "</p>");
            }

            sb.AppendLine("<p>--------------<br />");
            sb.AppendLine("* " + Language.T("Use the indicator value from the previous bar for all asterisk-marked indicators!") + "<br />");
            sb.AppendLine("** " + Language.T("The averaging rules apply to the entry signals only. Exit signals close a position. They cannot open, add or reduce one."));
            sb.AppendLine("</p>");
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Environment
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"environment\">" + Language.T("Environment") + "</h2>");
            sb.AppendLine(EnvironmentHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Strategy properties
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"properties\">" + Language.T("Strategy Properties") + "</h2>");
            sb.AppendLine(StrategyPropertiesHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Indicator Slots
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"indicators\">" + Language.T("Indicator Slots") + "</h2>");
            sb.AppendLine("<p>" + Language.T("The slots show the logic for the long positions only.") + " ");
            sb.AppendLine(Language.T("Forex Strategy Builder automatically computes the proper logic for the short positions.") + "</p>");
            sb.AppendLine(StrategySlotsHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Statistic information
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"statistics\">" + Language.T("Statistic Information") + "</h2>" + Environment.NewLine);
            sb.AppendLine(TradingStatsHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Additional Statistics
            if (Configs.AdditionalStatistics)
            {
                sb.AppendLine();
                sb.AppendLine("<h2 id=\"addstats\">" + Language.T("Additional Statistics") + "</h2>" + Environment.NewLine);
                sb.AppendLine(AdditionalStatsHTMLReport().ToString());
                sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");
            }

            // Footer
            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a HTML report about the opening logic.
        /// </summary>
        private static StringBuilder OpeningLogicHTMLReport()
        {
            var sb = new StringBuilder();
            string indicatorName = _backtester.Strategy.Slot[0].IndicatorName;

            Indicator indOpen = IndicatorStore.ConstructIndicator(indicatorName, _backtester.DataSet, SlotTypes.Open);
            indOpen.IndParam = _backtester.Strategy.Slot[0].IndParam;
            indOpen.SetDescription(SlotTypes.Open);

            // Logical groups of the opening conditions.
            var opengroups = new List<string>();
            for (int slot = 1; slot <= _backtester.Strategy.OpenFilters; slot++)
            {
                string group = _backtester.Strategy.Slot[slot].LogicalGroup;
                if (!opengroups.Contains(group) && group != "All")
                    opengroups.Add(group); // Adds all groups except "All"
            }
            if (opengroups.Count == 0 && _backtester.Strategy.OpenFilters > 0)
                opengroups.Add("All"); // If all the slots are in "All" group, adds "All" to the list.

            // Long position
            string openLong = "<p>";

            if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Add)
                openLong = Language.T("Open a new long position or add to an existing position");
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                openLong = Language.T("Open a new long position or add to a winning position");
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                openLong = Language.T("Open a new long position");

            if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Close)
                openLong += " " + Language.T("or close a short position");
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                openLong += " " + Language.T("or reduce a short position");
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                openLong += " " + Language.T("or reverse a short position");
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Nothing)
                openLong += "";

            openLong += " " + indOpen.EntryPointLongDescription;

            if (_backtester.Strategy.OpenFilters == 0)
                openLong += ".</p>";
            else if (_backtester.Strategy.OpenFilters == 1)
                openLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                openLong += " " + Language.T("when") + ":</p>";
            else
                openLong += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(openLong);

            // Open Filters
            if (_backtester.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") +
                                      Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int slot = 1; slot <= _backtester.Strategy.OpenFilters; slot++)
                        if (_backtester.Strategy.Slot[slot].LogicalGroup == group ||
                            _backtester.Strategy.Slot[slot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = 1; slot <= _backtester.Strategy.OpenFilters; slot++)
                    {
                        if (_backtester.Strategy.Slot[slot].LogicalGroup != group &&
                            _backtester.Strategy.Slot[slot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter =
                            IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                              _backtester.DataSet,
                                                              SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = _backtester.Strategy.Slot[slot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + "; " + Language.T("and") +
                                          "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }

                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            // Short position
            string openShort = "<p>";

            if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Add)
                openShort = Language.T("Open a new short position or add to an existing position");
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                openShort = Language.T("Open a new short position or add to a winning position");
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                openShort = Language.T("Open a new short position");

            if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Close)
                openShort += " " + Language.T("or close a long position");
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                openShort += " " + Language.T("or reduce a long position");
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                openShort += " " + Language.T("or reverse a long position");
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Nothing)
                openShort += "";

            openShort += " " + indOpen.EntryPointShortDescription;

            if (_backtester.Strategy.OpenFilters == 0)
                openShort += ".</p>";
            else if (_backtester.Strategy.OpenFilters == 1)
                openShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                openShort += " " + Language.T("when") + ":</p>";
            else
                openShort += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(openShort);

            // Open Filters
            if (_backtester.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") +
                                      Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int slot = 1; slot <= _backtester.Strategy.OpenFilters; slot++)
                        if (_backtester.Strategy.Slot[slot].LogicalGroup == group ||
                            _backtester.Strategy.Slot[slot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = 1; slot <= _backtester.Strategy.OpenFilters; slot++)
                    {
                        if (_backtester.Strategy.Slot[slot].LogicalGroup != group &&
                            _backtester.Strategy.Slot[slot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter =
                            IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                              _backtester.DataSet,
                                                              SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = _backtester.Strategy.Slot[slot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + "; " + Language.T("and") +
                                          "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }
                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the closing logic.
        /// </summary>
        private static StringBuilder ClosingLogicHTMLReport()
        {
            var sb = new StringBuilder();

            int closingSlotNmb = _backtester.Strategy.CloseSlot;
            string indicatorName = _backtester.Strategy.Slot[closingSlotNmb].IndicatorName;

            Indicator indClose = IndicatorStore.ConstructIndicator(indicatorName, _backtester.DataSet, SlotTypes.Close);
            indClose.IndParam = _backtester.Strategy.Slot[closingSlotNmb].IndParam;
            indClose.SetDescription(SlotTypes.Close);

            bool isGroups = false;
            var closegroups = new List<string>();

            if (_backtester.Strategy.CloseFilters > 0)
                foreach (IndicatorSlot slot in _backtester.Strategy.Slot)
                {
                    if (slot.SlotType == SlotTypes.CloseFilter)
                    {
                        if (slot.LogicalGroup == "all" && _backtester.Strategy.CloseFilters > 1)
                            isGroups = true;

                        if (closegroups.Contains(slot.LogicalGroup))
                            isGroups = true;
                        else if (slot.LogicalGroup != "all")
                            closegroups.Add(slot.LogicalGroup);
                    }
                }

            if (closegroups.Count == 0 && _backtester.Strategy.CloseFilters > 0)
                closegroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.


            // Long position
            string closeLong = "<p>" + Language.T("Close an existing long position") + " " +
                               indClose.ExitPointLongDescription;

            if (_backtester.Strategy.CloseFilters == 0)
                closeLong += ".</p>";
            else if (_backtester.Strategy.CloseFilters == 1)
                closeLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (isGroups)
                closeLong += " " + Language.T("when") + ":</p>";
            else
                closeLong += " " + Language.T("when at least one of the following logic conditions is satisfied") +
                             ":</p>";

            sb.AppendLine(closeLong);

            // Close Filters
            if (_backtester.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (isGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") +
                                      Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int slot = closingSlotNmb + 1; slot < _backtester.Strategy.Slots; slot++)
                        if (_backtester.Strategy.Slot[slot].LogicalGroup == group ||
                            _backtester.Strategy.Slot[slot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = closingSlotNmb + 1; slot < _backtester.Strategy.Slots; slot++)
                    {
                        if (_backtester.Strategy.Slot[slot].LogicalGroup != group &&
                            _backtester.Strategy.Slot[slot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter =
                            IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                              _backtester.DataSet,
                                                              SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = _backtester.Strategy.Slot[slot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (isGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " +
                                              Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        else
                        {
                            if (slot < _backtester.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " +
                                              Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (isGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            // Short position
            string closeShort = "<p>" + Language.T("Close an existing short position") + " " +
                                indClose.ExitPointShortDescription;

            if (_backtester.Strategy.CloseFilters == 0)
                closeShort += ".</p>";
            else if (_backtester.Strategy.CloseFilters == 1)
                closeShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (isGroups)
                closeShort += " " + Language.T("when") + ":</p>";
            else
                closeShort += " " + Language.T("when at least one of the following logic conditions is satisfied") +
                              ":</p>";

            sb.AppendLine(closeShort);

            // Close Filters
            if (_backtester.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (isGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") +
                                      Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int slot = closingSlotNmb + 1; slot < _backtester.Strategy.Slots; slot++)
                        if (_backtester.Strategy.Slot[slot].LogicalGroup == group ||
                            _backtester.Strategy.Slot[slot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = closingSlotNmb + 1; slot < _backtester.Strategy.Slots; slot++)
                    {
                        if (_backtester.Strategy.Slot[slot].LogicalGroup != group &&
                            _backtester.Strategy.Slot[slot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter =
                            IndicatorStore.ConstructIndicator(_backtester.Strategy.Slot[slot].IndicatorName,
                                                              _backtester.DataSet,
                                                              SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = _backtester.Strategy.Slot[slot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (isGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " +
                                              Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        else
                        {
                            if (slot < _backtester.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " +
                                              Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (isGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the averaging logic.
        /// </summary>
        private static StringBuilder AveragingHTMLReport()
        {
            var sb = new StringBuilder();

            // Same direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the direction of the present position:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                sb.AppendLine(
                    Language.T("No averaging is allowed. Cancel any additional orders which are in the same direction."));
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                sb.AppendLine(
                    Language.T(
                        "Add to a winning position but not to a losing one. If the position is at a loss, cancel the additional entry order. Do not exceed the maximum allowed number of lots to open."));
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Add)
                sb.AppendLine(
                    Language.T(
                        "Add to the position no matter if it is at a profit or loss. Do not exceed the maximum allowed number of lots to open."));
            sb.AppendLine("</li></ul>");

            // Opposite direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the opposite direction:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Nothing)
                sb.AppendLine(
                    Language.T(
                        "No modification of the present position is allowed. Cancel any additional orders which are in the opposite direction."));
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                sb.AppendLine(
                    Language.T(
                        "Reduce the present position. If its amount is lower than or equal to the specified reducing lots, close it."));
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Close)
                sb.AppendLine(
                    Language.T(
                        "Close the present position regardless of its amount or result. Do not open a new position until the next entry signal has been raised."));
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                sb.AppendLine(
                    Language.T(
                        "Close the existing position and open a new one in the opposite direction using the entry rules."));
            sb.AppendLine("</li></ul>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading sizes.
        /// </summary>
        private static StringBuilder TradingSizeHTMLReport()
        {
            var sb = new StringBuilder();

            if (_backtester.Strategy.UseAccountPercentEntry)
            {
                sb.AppendLine("<p>" + Language.T("Trade percent of your account.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " + _backtester.Strategy.EntryLots +
                              Language.T("% of the account equity") + ".</li>");
                if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " +
                                  _backtester.Strategy.AddingLots + Language.T("% of the account equity") + ". " +
                                  Language.T("Do not open more than") + " " +
                                  Plural("lot", _backtester.Strategy.MaxOpenLots) + ".</li>");
                if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " + _backtester.Strategy.AddingLots +
                                  Language.T("% of the account equity") + ". " + Language.T("Do not open more than") +
                                  " " + Plural("lot", _backtester.Strategy.MaxOpenLots) + ".</li>");
                if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " + _backtester.Strategy.ReducingLots +
                                  Language.T("% of the account equity") + ".</li>");
                if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " + _backtester.Strategy.EntryLots +
                                  Language.T("% of the account equity") + " " + Language.T("in the opposite direction.") +
                                  "</li>");
                sb.AppendLine("</ul>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("Always trade a constant number of lots.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " +
                              Plural("lot", _backtester.Strategy.EntryLots) + ".</li>");
                if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " +
                                  Plural("lot", _backtester.Strategy.AddingLots) + ". " +
                                  Language.T("Do not open more than") + " " +
                                  Plural("lot", _backtester.Strategy.MaxOpenLots) + ".</li>");
                if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " +
                                  Plural("lot", _backtester.Strategy.AddingLots) + ". " +
                                  Language.T("Do not open more than") + " " +
                                  Plural("lot", _backtester.Strategy.MaxOpenLots) + ".</li>");
                if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " +
                                  Plural("lot", _backtester.Strategy.ReducingLots) + " " +
                                  Language.T("from the current position.") + "</li>");
                if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " +
                                  Plural("lot", _backtester.Strategy.EntryLots) + " " +
                                  Language.T("in the opposite direction.") + "</li>");
                sb.AppendLine("</ul>");
            }

            if (_backtester.Strategy.UseMartingale)
            {
                sb.AppendLine("<p>" + Language.T("Apply Martingale money management system with multiplier of") + " " +
                              _backtester.Strategy.MartingaleMultiplier.ToString("F2") + "." + "</p>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading environment.
        /// </summary>
        private static StringBuilder EnvironmentHTMLReport()
        {
            var sb = new StringBuilder();

            string swapUnit = (_backtester.DataSet.InstrProperties.SwapType == CommissionType.money
                                   ? _backtester.DataSet.InstrProperties.PriceIn
                                   : Language.T(_backtester.DataSet.InstrProperties.SwapType.ToString()));
            string commission = Language.T("Commission") + " " + _backtester.DataSet.InstrProperties.CommissionScopeToString +
                                " " + _backtester.DataSet.InstrProperties.CommissionTimeToString;
            string commUnit = (_backtester.DataSet.InstrProperties.CommissionType == CommissionType.money
                                   ? _backtester.DataSet.InstrProperties.PriceIn
                                   : Language.T(_backtester.DataSet.InstrProperties.CommissionType.ToString()));

            sb.AppendLine("<h3>" + Language.T("Market") + "</h3>");
            sb.AppendLine("<table class=\"fsb_table_cleen\" cellspacing=\"0\">");
            sb.AppendLine("<tr><td>" + Language.T("Symbol") + "</td><td> - <strong>" + _backtester.DataSet.Symbol +
                          "</strong></td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Time frame") + "</td><td> - <strong>" + _backtester.DataSet.PeriodString +
                          "</strong></td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>" + Language.T("Account") + "</h3>");
            sb.AppendLine("<table class=\"fsb_table_cleen\" cellspacing=\"0\">");
            sb.AppendLine("<tr><td>" + Language.T("Initial account") + "</td><td> - " +
                          Configs.InitialAccount.ToString("F2") + " " + Configs.AccountCurrency + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Lot size") + "</td><td> - " + _backtester.DataSet.InstrProperties.LotSize +
                          "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Leverage") + "</td><td> - 1/" + Configs.Leverage + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Required margin") + "</td><td> - " +
                          _backtester.RequiredMargin(1, _backtester.DataSet.Bars - 1).ToString("F2") + " " +
                          Configs.AccountCurrency + "* " + Language.T("for each open lot") + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>" + Language.T("Charges") + "</h3>");
            sb.AppendLine("<table class=\"fsb_table_cleen\" cellspacing=\"0\">");
            sb.AppendLine("<tr><td>" + Language.T("Spread") + "</td><td> - " +
                          _backtester.DataSet.InstrProperties.Spread.ToString("F2") + " " + Language.T("pips") + "</td><td>(" +
                          _backtester.PipsToMoney(_backtester.DataSet.InstrProperties.Spread, _backtester.DataSet.Bars - 1).ToString(
                              "F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Swap number for a long position rollover") + "</td><td> - " +
                          _backtester.DataSet.InstrProperties.SwapLong.ToString("F2") + " " + swapUnit + "</td><td>(" +
                          _backtester.RolloverInMoney(PosDirection.Long, 1, 1, _backtester.DataSet.Close[_backtester.DataSet.Bars - 1])
                              .ToString("F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Swap number for a short position rollover") + "</td><td> - " +
                          _backtester.DataSet.InstrProperties.SwapShort.ToString("F2") + " " + swapUnit + "</td><td>(" +
                          _backtester.RolloverInMoney(PosDirection.Short, 1, 1,
                                                      _backtester.DataSet.Close[_backtester.DataSet.Bars - 1]).ToString("F2") + " " +
                          Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + commission + "</td><td> - " +
                          _backtester.DataSet.InstrProperties.Commission.ToString("F2") + " " + commUnit + "</td><td>(" +
                          _backtester.CommissionInMoney(1, _backtester.DataSet.Close[_backtester.DataSet.Bars - 1], false).ToString(
                              "F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Slippage") + "</td><td> - " +
                          Plural("pip", _backtester.DataSet.InstrProperties.Slippage) + "</td><td>(" +
                          _backtester.PipsToMoney(_backtester.DataSet.InstrProperties.Slippage, _backtester.DataSet.Bars - 1).ToString
                              ("F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<p>--------------<br />");
            sb.AppendLine("* " + Language.T("This value may vary!"));
            sb.AppendLine("</p>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the strategy properties.
        /// </summary>
        private static StringBuilder StrategyPropertiesHTMLReport()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h3>" + Language.T("Handling of Additional Entry Signals") + "</h3>");
            sb.AppendLine("<p>");
            sb.AppendLine(Language.T("Next same direction signal behavior") + " - ");
            if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Add)
                sb.AppendLine(Language.T("Adds to the position"));
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                sb.AppendLine(Language.T("Adds to a winning position"));
            else if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                sb.AppendLine(Language.T("Does nothing"));
            sb.AppendLine("<br />");
            sb.AppendLine(Language.T("Next opposite direction signal behavior") + " - ");
            if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Close)
                sb.AppendLine(Language.T("Closes the position"));
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                sb.AppendLine(Language.T("Reduces the position"));
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                sb.AppendLine(Language.T("Reverses the position"));
            else if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Nothing)
                sb.AppendLine(Language.T("Does nothing"));
            sb.AppendLine("</p>");

            sb.AppendLine("<h3>" + Language.T("Trading Size") + "</h3>");
            if (_backtester.Strategy.UseAccountPercentEntry)
                sb.AppendLine("<p>" +
                              Language.T("Trade percent of your account. The percentage values show the part of the account equity used to cover the required margin.") +
                              "</p>");

            sb.AppendLine("<table cellspacing=\"0\">");
            string sTradingUnit = _backtester.Strategy.UseAccountPercentEntry
                                      ? Language.T("% of the account equity")
                                      : "";
            sb.AppendLine("<tr><td>" + Language.T("Maximum number of open lots") + "</td><td> - " +
                          _backtester.Strategy.MaxOpenLots + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Number of entry lots for a new position") + "</td><td> - " +
                          _backtester.Strategy.EntryLots + sTradingUnit + "</td></tr>");
            if (_backtester.Strategy.UseMartingale)
                sb.AppendLine("<tr><td>" + Language.T("Martingale money management multiplier") + "</td><td> - " +
                              _backtester.Strategy.MartingaleMultiplier.ToString("F2") + "</td></tr>");
            if (_backtester.Strategy.SameSignalAction == SameDirSignalAction.Add ||
                _backtester.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                sb.AppendLine("<tr><td>" + Language.T("In case of addition - number of lots to add") + "</td><td> - " +
                              _backtester.Strategy.AddingLots + sTradingUnit + "</td></tr>");
            if (_backtester.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                sb.AppendLine("<tr><td>" + Language.T("In case of reduction - number of lots to close") + "</td><td> - " +
                              _backtester.Strategy.ReducingLots + sTradingUnit + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>" + Language.T("Permanent Protection") + "</h3>");
            string permSL = _backtester.Strategy.UsePermanentSL
                                ? (_backtester.Strategy.PermanentSLType == PermanentProtectionType.Absolute
                                       ? "(Abs) "
                                       : "") + _backtester.Strategy.PermanentSL + " " + Language.T("pips")
                                : "- " + Language.T("None");
            string permTP = _backtester.Strategy.UsePermanentTP
                                ? (_backtester.Strategy.PermanentTPType == PermanentProtectionType.Absolute
                                       ? "(Abs) "
                                       : "") + _backtester.Strategy.PermanentTP + " " + Language.T("pips")
                                : "- " + Language.T("None");
            string brkEven = _backtester.Strategy.UseBreakEven
                                 ? _backtester.Strategy.BreakEven + " " + Language.T("pips")
                                 : "- " + Language.T("None");
            sb.AppendLine("<table cellspacing=\"0\">");
            sb.AppendLine("<tr><td>" + Language.T("Permanent Stop Loss") + "</td><td>" + permSL + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Permanent Take Profit") + "</td><td>" + permTP + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Break Even") + "</td><td>" + brkEven + "</td></tr>");
            sb.AppendLine("</table>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the strategy slots
        /// </summary>
        private static StringBuilder StrategySlotsHTMLReport()
        {
            var sb = new StringBuilder();

            // Strategy Properties
            string sameDir = Language.T(_backtester.Strategy.SameSignalAction.ToString());
            string oppDir = Language.T(_backtester.Strategy.OppSignalAction.ToString());
            string permSL = _backtester.Strategy.UsePermanentSL
                                ? (_backtester.Strategy.PermanentSLType == PermanentProtectionType.Absolute
                                       ? "(Abs) "
                                       : "") + _backtester.Strategy.PermanentSL
                                : Language.T("None");
            string permTP = _backtester.Strategy.UsePermanentTP
                                ? (_backtester.Strategy.PermanentTPType == PermanentProtectionType.Absolute
                                       ? "(Abs) "
                                       : "") + _backtester.Strategy.PermanentTP
                                : Language.T("None");
            string brkEven = _backtester.Strategy.UseBreakEven
                                 ? _backtester.Strategy.BreakEven.ToString(CultureInfo.InvariantCulture)
                                 : Language.T("None");
            sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #966\">");
            sb.AppendLine("\t<div class=\"fsb_properties_slot\">" + Language.T("Strategy Properties") + "</div>");
            sb.AppendLine("\t<table style=\"margin-left: auto; margin-right: auto;\">");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Same direction signal") + "</td><td> - " + sameDir + "</td></tr>");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Opposite direction signal") + "</td><td> - " + oppDir + "</td></tr>");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Permanent Stop Loss") + "</td><td> - " + permSL + "</td></tr>");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Permanent Take Profit") + "</td><td> - " + permTP + "</td></tr>");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Break Even") + "</td><td> - " + brkEven + "</td></tr>");
            sb.AppendLine("\t</table>");
            sb.AppendLine("</div>");

            // Add the slots
            for (int slot = 0; slot < _backtester.Strategy.Slots; slot++)
            {
                IndicatorSlot indSlot = _backtester.Strategy.Slot[slot];
                string slotType = "";
                switch (indSlot.SlotType)
                {
                    case SlotTypes.Open:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #693\">");
                        slotType = "\t<div class=\"fsb_open_slot\">" + Language.T("Opening Point of the Position") +
                                   "</div>";
                        break;
                    case SlotTypes.OpenFilter:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #699\">");
                        slotType = "\t<div class=\"fsb_open_filter_slot\">" + Language.T("Opening Logic Condition") +
                                   "</div>";
                        break;
                    case SlotTypes.Close:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #d63\">");
                        slotType = "\t<div class=\"fsb_close_slot\">" + Language.T("Closing Point of the Position") +
                                   "</div>";
                        break;
                    case SlotTypes.CloseFilter:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #d99\">");
                        slotType = "\t<div class=\"fsb_close_filter_slot\">" + Language.T("Closing Logic Condition") +
                                   "</div>";
                        break;
                }

                sb.AppendLine(slotType);
                sb.AppendLine("\t<div class=\"fsb_str_indicator\">" + indSlot.IndicatorName + "</div>");

                // Add the logic
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled && listParam.Caption == "Logic")
                        sb.AppendLine("\t<div class=\"fsb_str_logic\">" +
                                      (Configs.UseLogicalGroups &&
                                       (indSlot.SlotType == SlotTypes.OpenFilter ||
                                        indSlot.SlotType == SlotTypes.CloseFilter)
                                           ? "<span style=\"float: left; margin-left: 5px; margin-right: -25px\">" +
                                             "[" + indSlot.LogicalGroup + "]" + "</span>"
                                           : "") +
                                      listParam.Text + "</div>");

                // Adds the parameters
                var sbParams = new StringBuilder();

                // Add the list params
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled && listParam.Caption != "Logic")
                        sbParams.AppendLine("\t\t<tr><td>" + listParam.Caption + "</td><td> - " + listParam.Text +
                                            "</td></tr>");

                // Add the num params
                foreach (NumericParam numParam in indSlot.IndParam.NumParam)
                    if (numParam.Enabled)
                        sbParams.AppendLine("\t\t<tr><td>" + numParam.Caption + "</td><td> - " + numParam.ValueToString +
                                            "</td></tr>");

                // Add the check params
                foreach (CheckParam checkParam in indSlot.IndParam.CheckParam)
                    if (checkParam.Enabled)
                        sbParams.AppendLine("\t\t<tr><td>" + checkParam.Caption + "</td><td> - " +
                                            (checkParam.Checked ? "Yes" : "No") + "</td></tr>");

                // Adds the params if there are any
                if (sbParams.Length > 0)
                {
                    sb.AppendLine("\t<table style=\"margin-left: auto; margin-right: auto;\">");
                    sb.AppendLine(sbParams.ToString());
                    sb.AppendLine("\t</table>");
                }

                sb.AppendLine("</div>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading stats
        /// </summary>
        private static StringBuilder TradingStatsHTMLReport()
        {
            var sb = new StringBuilder();
            int rows = Math.Max(_backtester.DataStats.MarketStatsParam.Length, _backtester.AccountStatsParam.Length);

            sb.AppendLine("<table class=\"fsb_table\" cellspacing=\"0\" cellpadding=\"5\">");
            sb.AppendLine("<tr><th colspan=\"2\">" + Language.T("Market") + "</th><th colspan=\"2\">" +
                          Language.T("Account") + "</th></tr>");
            for (int i = 0; i < rows; i++)
            {
                sb.Append("<tr>");
                if (i < _backtester.DataStats.MarketStatsParam.Length)
                    sb.Append("<td><strong>" + _backtester.DataStats.MarketStatsParam[i] + "</strong></td><td>" +
                              _backtester.DataStats.MarketStatsValue[i] + "</td>");
                else
                    sb.Append("<td></td><td></td>");
                if (i < _backtester.AccountStatsParam.Length)
                    sb.Append("<td><strong>" + _backtester.AccountStatsParam[i] + "</strong></td><td>" +
                              _backtester.AccountStatsValue[i] + "</td>");
                else
                    sb.Append("<td></td><td></td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the additional stats
        /// </summary>
        private static StringBuilder AdditionalStatsHTMLReport()
        {
            var sb = new StringBuilder();
            int rows = _backtester.AdditionalStatsParamName.Length;

            sb.AppendLine("<table class=\"fsb_table\" cellspacing=\"0\" cellpadding=\"5\">");
            sb.AppendLine("<tr><th>" + Language.T("Parameter") + "</th><th>" + Language.T("Long") + " + " +
                          Language.T("Short") + "</th><th>" + Language.T("Long") + "</th><th>" + Language.T("Short") +
                          "</th></tr>");
            for (int i = 0; i < rows; i++)
            {
                sb.Append("<tr>");
                sb.Append("<td><strong>" + _backtester.AdditionalStatsParamName[i] + "</strong></td>" +
                          "<td>" + _backtester.AdditionalStatsValueTotal[i] + "</td>" +
                          "<td>" + _backtester.AdditionalStatsValueLong[i] + "</td>" +
                          "<td>" + _backtester.AdditionalStatsValueShort[i] + "</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            return sb;
        }

        /// <summary>
        /// Appends "s" when plural
        /// </summary>
        private static string Plural(string unit, double number)
        {
            if (Math.Abs(number - 1) > 0.0000001 && Math.Abs(number + 1) > 0.0000001)
                unit += "s";

            return number + " " + Language.T(unit);
        }
    }
}