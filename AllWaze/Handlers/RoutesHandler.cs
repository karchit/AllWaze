using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Route = AllWaze.Objects.Route;

namespace AllWaze.Handlers
{
    public static class RoutesHandler
    {
        private const string R2RApiKey = "ZnzQK7bo";

        public static async Task<string> EntryPoint(string origin, string dest, string currency)
        {
            bool oPos = false;
            bool dPos = false;
            if (origin.Equals("here", StringComparison.InvariantCultureIgnoreCase) ||
                origin.Equals("my location", StringComparison.InvariantCultureIgnoreCase))
                oPos = true;
            if (dest.Equals("here", StringComparison.InvariantCultureIgnoreCase) ||
                dest.Equals("my location", StringComparison.InvariantCultureIgnoreCase))
                dPos = true;

            if (oPos && dPos)
            {
                MessageHandler.SendTextMessage("You are searching between the same two points!");
                return string.Empty;
            }
            var routes = ConstructRouteObjects(origin, dest, oPos, dPos, currency);
            if (routes != null)
            {
                var message = ConstructJsonFromRoutes(routes, currency, origin, dest, oPos || dPos);

                //var message = GetDescription(origin, dest);

                await MessageHandler.SendCustomMessage(message);
            }
            else
            {
                if (!oPos && !dPos)
                {
                    await MessageHandler.SendTextMessage("I am sorry I could not resolve your search query. Please check your response and try again");
                }
            }
            return string.Empty;
        }

        public static IEnumerable<Route> ConstructRouteObjects(string origin, string dest, bool oPos, bool dPos, string currency = "USD")
        {
            var geolocation = "";
            var address = "";
            var url = "";
            if (oPos || dPos)
            {
                var user = new UserHandler().GetUser(MessageHandler.SenderId);
                geolocation = user.Location;
                address = user.LocationString;
                if (string.IsNullOrEmpty(geolocation) || string.IsNullOrEmpty(address))
                {
                    MessageHandler.SendTextMessage(
                        $"I am sorry, I do not know your current location. Please send it via Messenger");
                    return null;
                }
                if (oPos)
                    url =
                        $"http://free.rome2rio.com/api/1.4/json/Search?key={R2RApiKey}&oPos={geolocation}&dName={dest}&currencyCode={currency}";
                if (dPos)
                    url =
                        $"http://free.rome2rio.com/api/1.4/json/Search?key={R2RApiKey}&oName={origin}&dPos={geolocation}&currencyCode={currency}";
            }
            url = string.IsNullOrEmpty(url) ? $"http://free.rome2rio.com/api/1.4/json/Search?key={R2RApiKey}&oName={origin}&dName={dest}&currencyCode={currency}" : url;
            var routesList = new List<Route>();
            JObject responseJson;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url);
                if (response.Result.IsSuccessStatusCode)
                {
                    responseJson = JObject.Parse(response.Result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    return null;
                }
            }

            var routes = (JArray)responseJson["routes"];
            var places = (JArray) responseJson["places"];

            var originLongName = places[0]["longName"];
            var destLongName = places[1]["longName"];

            foreach (JObject route in routes)
            {
                var indicativePrices = route["indicativePrices"] as JArray;
                var majorSegment = (JObject)((JArray)route["segments"]).OrderByDescending(s => (int)s["distance"]).First();

                var name = (string)route["name"];
                var pLow = indicativePrices != null ? (int?)indicativePrices[0]["priceLow"] : 0;
                var pHigh = indicativePrices != null ? (int?)indicativePrices[0]["priceHigh"] : 0;
                var duration = (int)route["totalDuration"];

                var agencyTuple = ((string)majorSegment["segmentKind"]).Equals("surface")
                    ? FetchAgencyImage(majorSegment, responseJson["agencies"] as JArray)
                    : FetchAirlineImage(majorSegment, responseJson["airlines"] as JArray);
                var agencyName = agencyTuple.Item1;
                var agencyImage = agencyTuple.Item2;

                pLow = pLow ?? 0;
                pHigh = pHigh ?? 0;
                int? price = 0;
                if (pLow == 0 && pHigh == 0)
                {
                    price = indicativePrices != null ? (int?)indicativePrices[0]["price"] : 0;
                }
                var routeObject = new Route(name, pLow ?? 0, pHigh ?? 0, duration, string.IsNullOrEmpty(agencyImage) && name.Contains("Drive") ? "http://i.imgur.com/PfC7OYk.jpg" : agencyImage, string.IsNullOrEmpty(agencyName) && name.Contains("Drive") ? "Self-Drive" : agencyName, !((string)majorSegment["segmentKind"]).Equals("surface"), price ?? 0);
                routesList.Add(routeObject);
            }

            var routeList = routesList.OrderBy(r => r.Duration).Take(Math.Min(routesList.Count, 6)).ToList();

            var cheapestRoute = routesList.MinBy(x => x.Price);
            cheapestRoute.IsCheapest = true;
            var fastestRoute = routesList.MinBy(x => x.Duration);
            fastestRoute.IsFastest = true;

            if(!routeList.Contains(cheapestRoute)) routeList.Add(cheapestRoute);
            if (oPos)
            {
                MessageHandler.SendTextMessage(
                    $"I have found {routeList.Count} routes from {address} to {destLongName}");
            }
            else if (dPos)
            {
                MessageHandler.SendTextMessage(
                    $"I have found {routeList.Count} routes from {originLongName} to {address}");

            }
            else
            {
                MessageHandler.SendTextMessage(
                    $"I have found {routeList.Count} routes from {originLongName} to {destLongName}");

            }

            return routeList; // Returns maximum of 7 routes. 
        }

        #region Route Helper Methods

        public static Tuple<string, string> FetchAgencyImage(JObject majorSegment, JArray agencies)
        {
            var segmentAgencies = majorSegment["agencies"] as JArray;
            if (segmentAgencies == null) return new Tuple<string, string>(string.Empty, string.Empty);

            var index = (int)segmentAgencies.ElementAt(0)["agency"];
            var agencyName = (string)agencies.ElementAt(index)["name"];
            var agencyUrl = new Uri((string)agencies.ElementAt(index)["url"]).Host.Replace("www.", "");
            agencyUrl = FixAgencyUrls(agencyUrl);
            var fixedUrl = ClearbitNoLogo(agencyUrl);
            if(!string.IsNullOrWhiteSpace(fixedUrl)) return new Tuple<string, string>(agencyName, fixedUrl);

            using (var client = new WebClient())
            {
                var url = $"https://autocomplete.clearbit.com/v1/companies/suggest?query={agencyUrl}";

                var companiesArray = JArray.Parse(client.DownloadString(url));

                return new Tuple<string, string>(agencyName, companiesArray.Any()
                    ? (string)companiesArray[0]["logo"] + "?size=220"
                    : "www.rome2rio.com/" + (string)agencies.ElementAt(index)["icon"]["url"]);

            }
        }

        private static string ClearbitNoLogo(string agencyUrl)
        {
            switch(agencyUrl)
            {
                case "hyperdia.com":
                    return "https://japabanchel.files.wordpress.com/2015/06/hyperdia-portada.jpg";
                case "sydneytrains.info":
                    return "https://media.licdn.com/mpr/mpr/shrink_200_200/AAEAAQAAAAAAAAhpAAAAJGM2M2VlNGIxLTdmZjgtNDRjMS05NDZkLTE5OWQ5ODRmMWQ1Mg.png";
                default:
                    return string.Empty;

            }
        }

        private static string FixAgencyUrls(string agencyUrl)
        {
            switch (agencyUrl)
            {
                case "bahn.com":
                    return "bahn.de";
                case "indianrail.gov.in":
                    return "indianrailways.gov.in";
                default:
                    return agencyUrl;
            }
        }

        public static Tuple<string, string> FetchAirlineImage(JObject majorSegment, JArray airlines)
        {
            var hops = (JArray)((JArray)majorSegment["outbound"]).ElementAt(0)["hops"];
            var index = (int)hops.ElementAt(0)["airline"];
            var airlineName = (string)airlines.ElementAt(index)["name"];
            var airlineCode = (string)airlines.ElementAt(index)["code"];

            var fixedUrl = FixAirlineUrls(airlineCode);
            if (!string.IsNullOrEmpty(fixedUrl)) return new Tuple<string, string>(airlineName, fixedUrl);

            using (var client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var companiesArray = JArray.Parse(client.DownloadString($"https://autocomplete.clearbit.com/v1/companies/suggest?query={airlineName}"));
                return new Tuple<string, string>(airlineName, companiesArray.Any()
                    ? (string)companiesArray[0]["logo"] + "?size=220"
                    : $"http://pics.avs.io/200/200/{airlineCode}.png");

            }
        }

        private static string FixAirlineUrls(string airlineCode)
        {
            switch (airlineCode)
            {
                case "BA":
                    return "http://www.kongresniturizam.com/Media/uploads/avio_kompanije/23/logo.png";
                case "TT":
                    return "https://pbs.twimg.com/profile_images/605039566593007616/JyE-Rr_W.jpg";
                case "UA":
                    return "https://lunardream.files.wordpress.com/2012/01/continental-airlines-inc.png";
                case "QF":
                    return "https://logo.clearbit.com/qantas.com?size=220";
                default:
                    return string.Empty;
            }
        }
        
        #endregion

        public static string ConstructJsonFromRoutes(IEnumerable<Route> routes, string currCulture, string origin, string dest, bool isHere)
        {
            var routesList = routes.ToList();

            var elementsJson = new JArray();
            foreach (var route in routesList)
            {
                //var payLoad = new JObject(
                //    new JObject("recipient", new JProperty("id", MessageHandler.SenderId)),
                //    new JProperty("intent", "SegmentInfo"),
                //    new JProperty("origin", origin),
                //    new JProperty("destination", dest),
                //    new JProperty("routeName", route.Name)
                //);

                dynamic recipient = new JObject();
                recipient.id = MessageHandler.SenderId;

                dynamic payLoad = new JObject();
                payLoad.recipient = recipient;
                payLoad.intent = "SegmentInfo";
                payLoad.origin = origin;
                payLoad.destination = dest;
                payLoad.routeName = route.Name;
                payLoad.location = new UserHandler().GetUser(MessageHandler.SenderId).Location;
                
                var p = payLoad.ToString();

                //var p = $"\"payload\":{{\"recipient\" : {{\"id\" : \"{{{MessageHandler.SenderId}}}\"}},\"intent\":\"SegmentInfo\", \"origin\":\"{origin}\", \"destination\":\"{dest}\", \"routeName\":\"{route.Name}\"}}";
                var buttons = new JArray(
                    new JObject(new JProperty("type", "postback"),
                    new JProperty("title", "View More Info"),
                    new JProperty("payload", p))
                    );
                //var buttons = new JArray
                //{
                //    new JProperty("type", "postback"),
                //    new JProperty("title", "View More Info"),
                //    new JProperty("payload", p)
                //};

                route.Name = AddEmojiToName(route.Name);

                var priceLow = route.PriceLow.ToString("C0", new CultureInfo(currCulture));
                var priceHigh = route.PriceHigh.ToString("C0", new CultureInfo(currCulture));
                var duration = FormatDuration(route.Duration);

                var priceString = (route.PriceLow == 0 && route.PriceHigh == 0)
                    ? route.Price.ToString("C0", new CultureInfo(currCulture))
                    : $"{priceLow} - {priceHigh}";

                var attribute = "";
                if (route.IsFastest && route.IsCheapest)
                {
                    attribute = "\n🔥 Fastest & Cheapest 💲 Route!";
                }
                else if (route.IsFastest)
                {
                    attribute = "\n🔥 Fastest Route! 🔥";
                }
                else if (route.IsCheapest)
                {
                    attribute = "\n💲 Cheapest Route! 💲";
                }

                string agencyName;
                if (route.AgencyName.Equals("Self-Drive"))
                {
                    agencyName = "Self-Drive";
                }
                else if (route.IsAirline)
                {
                    agencyName = "Airline: " + route.AgencyName.Replace("Airways", "").Trim();
                }
                else
                {
                    agencyName = "Agency: " + route.AgencyName;
                }

                var element = new JObject(
                    new JProperty("title", route.Name),
                    new JProperty("image_url", route.Image),
                    new JProperty("subtitle", $"{agencyName}\nPrice: {priceString} \nDuration: {duration}{attribute}"),
                    //new JProperty("default_action", defaultAction),
                    new JProperty("buttons", buttons)
                );


                elementsJson.Add(element);
            }
            var payload = new JObject(
                new JProperty("template_type", "generic"),
                new JProperty("image_aspect_ratio", "square"),
                new JProperty("elements", elementsJson)
            );

            var attachment = new JObject(
                new JProperty("type", "template"),
                new JProperty("payload", payload)
            );

            var returnJson = new JObject(
                new JProperty("attachment", attachment)
            );

            return returnJson.ToString(Formatting.None);
        }

        public static string FormatDuration(int duration)
        {
            var numHours = duration/60;
            var numMinutes = duration - (numHours*60);

            var dayString = numHours < 48 ? string.Empty : $"{numHours/24}d";
            var hourString = numHours == 0 ? string.Empty : $"{numHours}h";
            var minuteString = numMinutes == 0 ? string.Empty : $"{numMinutes}min";

            var dhSpaceString = string.IsNullOrEmpty(dayString) ? string.Empty : " ";
            var hmSpaceString = string.IsNullOrEmpty(hourString) ? string.Empty : " ";

            return $"{dayString}{dhSpaceString}{hourString}{hmSpaceString}{minuteString}";
        }

        private static string AddEmojiToName(string name)
        {
            var emojiDictionary = FindIndexes(name, "fly").ToDictionary(index => index, index => "✈️");
            foreach (var index in FindIndexes(name, "train")) emojiDictionary.Add(index, "🚆");
            foreach (var index in FindIndexes(name, "bus")) emojiDictionary.Add(index, "🚌");
            foreach (var index in FindIndexes(name, "drive")) emojiDictionary.Add(index, "🚗");
            foreach (var index in FindIndexes(name, "ferry")) emojiDictionary.Add(index, "⛴️");
            foreach (var index in FindIndexes(name, "taxi")) emojiDictionary.Add(index, "🚕");

            var sorted = from entry in emojiDictionary orderby entry.Key ascending select entry;
            name = sorted.Aggregate(name, (current, entry) => current + (" " + entry.Value));

            return name;
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