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
            var emojiDictionary = FindIndexes(this.Name, "fly").ToDictionary(index => index, index => "✈️");
            foreach(var index in FindIndexes(this.Name, "train")) emojiDictionary.Add(index, "🚆");
            foreach(var index in FindIndexes(this.Name, "bus")) emojiDictionary.Add(index, "🚌");
            foreach(var index in FindIndexes(this.Name, "drive")) emojiDictionary.Add(index, "🚗");
            foreach(var index in FindIndexes(this.Name, "ferry")) emojiDictionary.Add(index, "⛴️");
            foreach(var index in FindIndexes(this.Name, "taxi")) emojiDictionary.Add(index, "🚕");

            var sorted = from entry in emojiDictionary orderby entry.Key ascending select entry;
            foreach (var entry in sorted) this.Name += " " + entry.Value;
            this.Name = this.Name.Trim();

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

        private static IEnumerable<int> FindIndexes(string text, string query)
        {
            query = Regex.Escape(query);
            foreach (Match match in Regex.Matches(text, query, RegexOptions.IgnoreCase))
            {
                yield return match.Index;
            }
        }
    }
}