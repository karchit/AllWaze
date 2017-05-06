using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MoreLinq;

namespace AllWaze.Objects
{
    public class AirSegment : Segment
    {
        public double Distance { get; set; }

        public AirSegment(string name, string from, string to, int pLow, int pHigh, int duration, double distance, string image, string mapUrl, string kind, int p = 0) 
            : base(name, from, to, pLow, pHigh, duration, image, mapUrl, kind, p)
        {
            this.Distance = distance;
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