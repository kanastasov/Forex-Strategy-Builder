// StrategyIO Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Forex_Strategy_Builder.Interfaces;

namespace Forex_Strategy_Builder
{
    public static class StrategyIO
    {
        /// <summary>
        /// Saves the strategy in XML format.
        /// </summary>
        public static void Save(Strategy strategy, string filePath)
        {
            strategy.StrategyName = Path.GetFileNameWithoutExtension(filePath);

            XmlDocument xmlDocStrategy = StrategyXML.CreateStrategyXmlDoc(strategy);

            try
            {
                xmlDocStrategy.Save(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Loads the strategy from a file in XML format.
        /// </summary>
        public static Strategy Load(string filename, IDataSet dataSet)
        {
            var xmlDocStrategy = new XmlDocument();

            try
            {
                xmlDocStrategy.Load(filename);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Language.T("Strategy Loading"));

                return null;
            }

            Strategy strategy = StrategyXML.ParseXmlStrategy(xmlDocStrategy, dataSet );

            return strategy;
        }
    }
}