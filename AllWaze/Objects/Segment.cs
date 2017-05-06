using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MoreLinq;

namespace AllWaze.Objects
{
    public class Segment
    {
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Image { get; set; }
        public int PriceLow { get; set; }
        public int PriceHigh { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
        public string MapUrl  { get; set; }
        public string Kind  { get; set; }

        public Segment(string name, string from, string to, int pLow, int pHigh, int duration, string image, string mapUrl, string kind, int p = 0)
        {
            this.Name = name;
            this.From = from;
            this.To = to;
            this.PriceLow = pLow;
            this.PriceHigh = pHigh;
            this.Price = p == 0 ? (PriceLow + PriceHigh)/2 : p;
            this.Duration = duration;
            this.Image = image;
            this.MapUrl = mapUrl;
            this.Kind = kind;

        }

        //public override bool Equals(object obj)
        //{
        //    // If parameter cannot be cast to Point return false.
        //    var route = obj as Segment;
        //    if (route == null)
        //    {
        //        return false;
        //    }

        //    // Return true if the fields match:
        //    return (Name == route.Name);
        //}
    }
}