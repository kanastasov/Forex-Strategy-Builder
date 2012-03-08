// Backtester - Way Point
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder
{
    public class WayPoint
    {
        public WayPoint(double price, WayPointType wpType)
        {
            Price = price;
            WPType = wpType;
            OrdNumb = -1;
            PosNumb = -1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public WayPoint(double price, WayPointType wpType, int ordNumb, int posNumb)
        {
            Price  = price;
            WPType = wpType;

            if (wpType == WayPointType.Open || wpType == WayPointType.High  ||
                wpType == WayPointType.Low  || wpType == WayPointType.Close ||
                wpType == WayPointType.None)
                OrdNumb = -1;
            else
                OrdNumb = ordNumb;

            //TODO Check this.
            //if (Backtester.PosFromNumb(posNumb).PosDir == PosDirection.None   ||
            //    Backtester.PosFromNumb(posNumb).PosDir == PosDirection.Closed &&
            //    wpType != WayPointType.Exit && wpType != WayPointType.Reduce)
            //    PosNumb = -1;
            //else
                PosNumb = posNumb;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        private WayPoint(){}

        /// <summary>
        /// Gets or sets the waypoint price
        /// </summary>
        public double Price { get; private set; }

        /// <summary>
        /// Gets or sets the waypoint type
        /// </summary>
        public WayPointType WPType { get; private set; }

        /// <summary>
        /// Gets or sets the waypoint order number
        /// </summary>
        public int OrdNumb { get; set; }

        /// <summary>
        /// Gets or sets the waypoint position number
        /// </summary>
        public int PosNumb { get; private set; }

        /// <summary>
        /// Shows the WayPointType as a string.
        /// </summary>
        public static string WPTypeToString(WayPointType wpType)
        {
            string output;

            switch (wpType)
            {
                case WayPointType.None:
                    output = "None";
                    break;
                case WayPointType.Open:
                    output = "Bar open";
                    break;
                case WayPointType.High:
                    output = "Bar high";
                    break;
                case WayPointType.Low:
                    output = "Bar low";
                    break;
                case WayPointType.Close:
                    output = "Bar close";
                    break;
                case WayPointType.Entry:
                    output = "Entry point";
                    break;
                case WayPointType.Exit:
                    output = "Exit point";
                    break;
                case WayPointType.Add:
                    output = "Adding point";
                    break;
                case WayPointType.Reduce:
                    output = "Reducing point";
                    break;
                case WayPointType.Reverse:
                    output = "Reversing point";
                    break;
                case WayPointType.Cancel:
                    output = "Cancelled order";
                    break;
                default:
                    output = "Error";
                    break;
            }

            return output;
        }

        public WayPoint Copy()
        {
            var wayPoint = new WayPoint
                               {
                                   Price = Price,
                                   WPType = WPType,
                                   OrdNumb = OrdNumb,
                                   PosNumb = PosNumb
                               };
            return wayPoint;
        }
    }
}
