using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllWaze.Objects
{
    public class Route
    {
        public string Name { get; set; }
        public int PriceLow { get; set; }
        public int PriceHigh { get; set; }
        public int Duration { get; set; }
        public string Image { get; set; }

        public Route(string name, int pLow, int pHigh, int duration, string image)
        {
            this.Name = name;
            this.PriceLow = pLow;
            this.PriceHigh = pHigh;
            this.Duration = duration;
            this.Image = image;
        }
    }
}