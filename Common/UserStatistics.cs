using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace Forex_Strategy_Builder.Common
{
    public static class UserStatistics
    {
        private static DateTime _startTime;
        public static int GeneratorStarts { get; set; }
        public static int OptimizerStarts { get; set; }
        public static int SavedStrategies { get; set; }

        public static void InitStats()
        {
            _startTime = DateTime.Now;
            GeneratorStarts = 0;
            OptimizerStarts = 0;
            SavedStrategies = 0;
        }

        /// <summary>
        /// Collects usage statistics and sends them if it's allowed.
        /// </summary>
        public static void SendStats()
        {
            const string fileURL = "http://forexsb.com/ustats/set-fsb.php";

            string mac = "";
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up) continue;
                mac = nic.GetPhysicalAddress().ToString();
                break;
            }

            string parameters = string.Empty;

            if (Configs.SendUsageStats)
            {
                parameters = string.Format("?mac={0}&reg={1}&time={2}&gen={3}&opt={4}&str={5}",
                                           mac, RegionInfo.CurrentRegion.EnglishName,
                                           (int) (DateTime.Now - _startTime).TotalSeconds,
                                           GeneratorStarts, OptimizerStarts, SavedStrategies);
            }

            try
            {
                var webClient = new WebClient();
                Stream data = webClient.OpenRead(fileURL + parameters);
                if (data != null) data.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

    }
}
