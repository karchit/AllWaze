using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MoreLinq;

namespace AllWaze.Objects
{
    public class SurfaceSegment : Segment
    {
        public string DefaultUrl { get; set; }
        public string BookUrl { get; set; }
        public string ScheduleUrl { get; set; }
        public string PhoneNumber { get; set; }

        public SurfaceSegment(string name, string from, string to, int pLow, int pHigh, int duration, string image, string defaultUrl, string bookUrl, string scheduleUrl, string phoneNumber, string mapUrl, string kind, int p = 0) 
            : base(name, from, to, pLow, pHigh, duration, image, mapUrl, kind, p)
        {
            this.DefaultUrl = defaultUrl;
            this.BookUrl = bookUrl;
            this.ScheduleUrl = scheduleUrl;
            this.PhoneNumber = phoneNumber;
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