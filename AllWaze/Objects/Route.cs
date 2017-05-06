using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MoreLinq;

namespace AllWaze.Objects
{
    public class Route
    {
        public string Name { get; set; }
        public int PriceLow { get; set; }
        public int PriceHigh { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
        public string AgencyName { get; set; }
        public string Image { get; set; }
        public bool IsFastest { get; set; }
        public bool IsCheapest { get; set; }
        public bool IsAirline { get; set; }

        public Route(string name, int pLow, int pHigh, int duration, string image, string agencyName, bool isAirline, int p = 0)
        {
            this.Name = name;
            this.PriceLow = pLow;
            this.PriceHigh = pHigh;
            this.Price = p == 0 ? (PriceLow + PriceHigh)/2 : p;
            this.Duration = duration;
            this.Image = image;
            this.AgencyName = agencyName;
            this.IsFastest = false;
            this.IsCheapest = false;
            this.IsAirline = isAirline;
        }

        public override bool Equals(object obj)
        {
            // If parameter cannot be cast to Point return false.
            var route = obj as Route;
            if (route == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Name == route.Name);
        }
    }
}