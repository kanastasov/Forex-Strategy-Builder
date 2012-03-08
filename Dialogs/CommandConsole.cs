// Command Console
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public sealed class CommandConsole : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CommandConsole()
        {
            // The Form
            Text = Language.T("Command Console");
            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;

            // Text Box Input
            TbxInput = new TextBox {BorderStyle = BorderStyle.FixedSingle, Parent = this, Location = Point.Empty};
            TbxInput.KeyUp += TbxInputKeyUp;

            // Text Box Output
            TbxOutput = new TextBox
                            {
                                BorderStyle = BorderStyle.FixedSingle,
                                BackColor = Color.Black,
                                ForeColor = Color.GhostWhite,
                                Parent = this,
                                Location = Point.Empty,
                                Multiline = true,
                                WordWrap = false,
                                Font = new Font("Courier New", 10),
                                ScrollBars = ScrollBars.Vertical
                            };
        }
        private Backtester Backtester { get; set; }

        private TextBox TbxInput { get; set; }
        private TextBox TbxOutput { get; set; }

        /// <summary>
        /// OnLoad
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ClientSize = new Size(400, 505);
            ParseInput("help");
        }

        /// <summary>
        /// OnResize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            const int border = 5;
            TbxInput.Width = ClientSize.Width - 2*border;
            TbxInput.Location = new Point(border, ClientSize.Height - border - TbxInput.Height);
            TbxOutput.Width = ClientSize.Width - 2*border;
            TbxOutput.Height = TbxInput.Top - 2*border;
            TbxOutput.Location = new Point(border, border);
        }

        /// <summary>
        /// Catches the hot keys
        /// </summary>
        private void TbxInputKeyUp(object sender, KeyEventArgs e)
        {
            TbxInput.BackColor = Color.White;
            if (e.KeyCode == Keys.Return)
                ParseInput(TbxInput.Text);
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void ParseInput(string input)
        {
            var asCommands = new[]
            {
                "help           - Shows the commands list.",
                "clr            - Clears the screen.",
                "pos #          - Shows the parameters of position #.",
                "ord #          - Shows the parameters of order #.",
                "bar #          - Shows the prices of bar #.",
                "ind #          - Shows the indicators for bar #.",
                "str            - shows the strategy.",
                "debug          - Turns on debug mode.",
                "undebug        - Turns off debug mode.",
                "loadlang       - Reloads the language file.",
                "missingphrases - Shows all phrases, which are used in the program but are not included in the language file.",
                "genlangfiles   - Regenerates English.xml.",
                "repairlang     - Repairs all the language files.",
                "importlang     - Imports a translation (Read more first).",
                "langtowiki     - Shows translation in wiki format.",
                "resetinstrum   - Resets the instruments list.",
                "speedtest      - Performs a speed test.",
                "reloadtips     - Reloads the starting tips.",
                "showalltips    - Shows all the starting tips."
            };

            if (input.StartsWith("help") || input.StartsWith("?"))
            {
                TbxOutput.Text = "Commands" + Environment.NewLine + "-----------------" + Environment.NewLine;
                foreach (string sCommand in asCommands)
                    TbxOutput.Text += sCommand + Environment.NewLine;
            }
            else if (input.StartsWith("clr"))
            {
                TbxOutput.Text = "";
            }
            else if (input.StartsWith("debug"))
            {
                TbxOutput.Text += "Debug mode - on" + Environment.NewLine;
                Data.Debug = true;
            }
            else if (input.StartsWith("nodebug"))
            {
                TbxOutput.Text += "Debug mode - off" + Environment.NewLine;
                Data.Debug = false;
            }
            else if (input.StartsWith("loadlang"))
            {
                Language.InitLanguages();
                TbxOutput.Text += "Language file loaded." + Environment.NewLine;
            }
            else if (input.StartsWith("importlang"))
            {
                Language.ImportLanguageFile(TbxOutput.Text);
            }
            else if (input.StartsWith("langtowiki"))
            {
                Language.ShowPhrases(4);
            }
            else if (input.StartsWith("genlangfiles"))
            {
                Language.GenerateLangFiles();
                TbxOutput.Text += "Language files generated." + Environment.NewLine;
            }
            else if (input.StartsWith("repairlang"))
            {
                TbxOutput.Text += "Language files repair" + Environment.NewLine + "---------------------" +
                                  Environment.NewLine + "";
                TbxOutput.Text += Language.RapairAllLangFiles();
            }
            else if (input.StartsWith("resetinstrum"))
            {
                Instruments.ResetInstruments();
                TbxOutput.Text += "The instruments are reset." + Environment.NewLine + "Restart the program now!" +
                                  Environment.NewLine;
            }
            else if (input.StartsWith("missingphrases"))
            {
                ShowMissingPhrases();
            }
            else if (input.StartsWith("speedtest"))
            {
                SpeedTest();
            }
            else if (input.StartsWith("str"))
            {
                ShowStrategy();
            }
            else if (input.StartsWith("pos"))
            {
                ShowPosition(input);
            }
            else if (input.StartsWith("ord"))
            {
                ShowOrder(input);
            }
            else if (input.StartsWith("bar"))
            {
                ShowBar(input);
            }
            else if (input.StartsWith("ind"))
            {
                ShowIndicators(input);
            }
            else if (input.StartsWith("reloadtips"))
            {
                var startingTips = new StartingTips();
                startingTips.Show();
            }
            else if (input.StartsWith("showalltips"))
            {
                var startingTips = new StartingTips {ShowAllTips = true};
                startingTips.Show();
            }

            TbxOutput.Focus();
            TbxOutput.ScrollToCaret();

            TbxInput.Focus();
            TbxInput.Text = "";
        }

        /// <summary>
        /// Speed Test
        /// </summary>
        private void SpeedTest()
        {
            DateTime dtStart = DateTime.Now;
            const int rep = 1000;

            for (int i = 0; i < rep; i++)
                Backtester.Strategy.Clone();

            DateTime dtStop = DateTime.Now;
            TimeSpan tsCalcTime = dtStop.Subtract(dtStart);
            TbxOutput.Text += rep + " times strategy clone for Sec: " +
                              tsCalcTime.TotalSeconds.ToString("F4") + Environment.NewLine;
        }

        /// <summary>
        /// Show position
        /// </summary>
        private void ShowPosition(string input)
        {
            const string pattern = @"^pos (?<numb>\d+)$";
            var expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int pos = int.Parse(match.Groups["numb"].Value);
                if (pos < 1 || pos > Backtester.PositionsTotal)
                    return;

                Position position = Backtester.PosFromNumb(pos - 1);
                TbxOutput.Text += "Position" + Environment.NewLine + "-----------------" +
                                  Environment.NewLine + position + Environment.NewLine;
            }
        }

        /// <summary>
        /// Show position
        /// </summary>
        private void ShowOrder(string input)
        {
            const string pattern = @"^ord (?<numb>\d+)$";
            var expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int ord = int.Parse(match.Groups["numb"].Value);
                if (ord < 1 || ord > Backtester.OrdersTotal)
                    return;

                Order order = Backtester.OrdFromNumb(ord - 1);
                TbxOutput.Text += "Order" + Environment.NewLine + "-----------------" +
                                  Environment.NewLine + order + Environment.NewLine;
            }
        }

        /// <summary>
        /// Show bar
        /// </summary>
        private void ShowBar(string input)
        {
            const string pattern = @"^bar (?<numb>\d+)$";
            var expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int bar = int.Parse(match.Groups["numb"].Value);
                if (bar < 1 || bar > Data.DataSet.Bars)
                    return;

                bar--;

                string barInfo = String.Format("Bar No " + (bar + 1) + Environment.NewLine +
                                               "{0:D2}.{1:D2}.{2:D4} {3:D2}:{4:D2}" + Environment.NewLine +
                                               "Open   {5:F4}" + Environment.NewLine +
                                               "High   {6:F4}" + Environment.NewLine +
                                               "Low    {7:F4}" + Environment.NewLine +
                                               "Close  {8:F4}" + Environment.NewLine +
                                               "Volume {9:D6}",
                                               Data.DataSet.Time[bar].Day, Data.DataSet.Time[bar].Month, Data.DataSet.Time[bar].Year,
                                               Data.DataSet.Time[bar].Hour, Data.DataSet.Time[bar].Minute,
                                               Data.DataSet.Open[bar], Data.DataSet.High[bar], Data.DataSet.Low[bar], Data.DataSet.Close[bar],
                                               Data.DataSet.Volume[bar]);

                TbxOutput.Text += "Bar" + Environment.NewLine + "-----------------" +
                                  Environment.NewLine + barInfo + Environment.NewLine;
            }
        }

        /// <summary>
        /// Shows all missing phrases.
        /// </summary>
        private void ShowMissingPhrases()
        {
            TbxOutput.Text += Environment.NewLine +
                              "Missing Phrases" + Environment.NewLine +
                              "---------------------------" + Environment.NewLine;
            foreach (string phrase in Language.MissingPhrases)
                TbxOutput.Text += phrase + Environment.NewLine;
        }

        /// <summary>
        /// Shows the strategy
        /// </summary>
        private void ShowStrategy()
        {
            TbxOutput.Text += "Strategy" + Environment.NewLine + "-----------------" +
                              Environment.NewLine + Backtester.Strategy + Environment.NewLine;
        }

        /// <summary>
        /// Show indicators in the selected bars.
        /// </summary>
        private void ShowIndicators(string input)
        {
            const string pattern = @"^ind (?<numb>\d+)$";
            var expression = new Regex(pattern, RegexOptions.Compiled);
            Match match = expression.Match(input);
            if (match.Success)
            {
                int bar = int.Parse(match.Groups["numb"].Value);
                if (bar < 1 || bar > Data.DataSet.Bars)
                    return;

                bar--;

                var sb = new StringBuilder();
                for (int iSlot = 0; iSlot < Backtester.Strategy.Slots; iSlot++)
                {
                    Indicator indicator = IndicatorStore.ConstructIndicator(Backtester.Strategy.Slot[iSlot].IndicatorName,
                                                                            Backtester.Strategy.Slot[iSlot].SlotType);

                    sb.Append(Environment.NewLine + indicator + Environment.NewLine + "Logic: " +
                              indicator.IndParam.ListParam[0].Text + Environment.NewLine + "-----------------" +
                              Environment.NewLine);
                    foreach (IndicatorComp indComp in Backtester.Strategy.Slot[iSlot].Component)
                    {
                        sb.Append(indComp.CompName + "    ");
                        sb.Append(indComp.Value[bar] + Environment.NewLine);
                    }
                }

                TbxOutput.Text += Environment.NewLine + "Indicators for bar " + (bar + 1) + Environment.NewLine +
                                  "-----------------" + Environment.NewLine + sb;
            }
        }
    }
}