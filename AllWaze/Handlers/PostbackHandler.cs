using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AllWaze.Objects;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Route = AllWaze.Objects.Route;

namespace AllWaze.Handlers
{
    public static class PostbackHandler
    {

        private const string R2RApiKey = "ZnzQK7bo";


        public static void EntryPoint(JToken postback)
        {
            var parsedPostBack = JObject.Parse((string)postback);
            if ((string) parsedPostBack["intent"] == "SegmentInfo")
            {
                var userId = (string)parsedPostBack["recipient"]["id"];
                var currency = new UserHandler().GetCurrency(userId) ?? "USD";
                string currencyCulture;
                Currency.currencies.TryGetValue(currency, out currencyCulture);

                var origin = (string)parsedPostBack["origin"];
                var dest = (string)parsedPostBack["destination"];
                var routeName = (string)parsedPostBack["routeName"];
                var location = (string)parsedPostBack["location"];

                var oPos = false;
                var dPos = false;
                if (origin.Equals("here", StringComparison.InvariantCultureIgnoreCase) ||
                    origin.Equals("my location", StringComparison.InvariantCultureIgnoreCase))
                    oPos = true;
                if (dest.Equals("here", StringComparison.InvariantCultureIgnoreCase) ||
                    dest.Equals("my location", StringComparison.InvariantCultureIgnoreCase))
                    dPos = true;

                var segments = ConstructSegmentObjects(origin, dest, routeName, oPos, dPos, location, currency);
                if (segments != null)
                {
                    var message = ConstructSegmentJson(segments, currencyCulture);
                    MessageHandler.SenderId = userId;
                    MessageHandler.SendCustomMessage(message);
                }
            }
        }

        private static string ConstructSegmentJson(IEnumerable<Segment> segments, string currency)
        {
            var segmentList = segments.ToList();
            var elementsJson = new JArray();

            foreach (var segment in segmentList)
            {
                var emoji = GetEmojiSegment(segment.Kind);
                var name = segment.Name + emoji;
                var priceLow = segment.PriceLow.ToString("C0", new CultureInfo(currency));
                var priceHigh = segment.PriceHigh.ToString("C0", new CultureInfo(currency));
                var duration = RoutesHandler.FormatDuration(segment.Duration);
                var image = segment.Image;

                var priceString = (segment.PriceLow == 0 && segment.PriceHigh == 0)
                    ? segment.Price.ToString("C0", new CultureInfo(currency))
                    : $"{priceLow} - {priceHigh}";

                if (segment is SurfaceSegment)
                {
                    var buttons = new JArray();

                    var surSegment = (SurfaceSegment) segment;
                    buttons.Add(new JObject(
                        new JProperty("type", "web_url"),
                        new JProperty("url", surSegment.BookUrl),
                        new JProperty("title", "Go To Agency Page")
                        )
                        );
                    if (!string.IsNullOrEmpty(surSegment.ScheduleUrl))
                    {
                        buttons.Add(new JObject(
                            new JProperty("type", "web_url"),
                            new JProperty("url", surSegment.ScheduleUrl),
                            new JProperty("title", "Go To Booking Page")
                            )
                        );
                    }
                    if (!string.IsNullOrEmpty(surSegment.PhoneNumber))
                    {
                        buttons.Add(new JObject(
                            new JProperty("type", "phone_number"),
                            new JProperty("payload", surSegment.PhoneNumber),
                            new JProperty("title", "Call Agency 📞")
                            )
                        );
                    }

                    var element = new JObject(
                        new JProperty("title", name),
                        new JProperty("image_url", image),
                        new JProperty("subtitle",
                            $"From: {segment.From}\nTo:{segment.To}\nPrice: {priceString}"),
                        //new JProperty("default_action", defaultAction),
                        new JProperty("buttons", buttons)
                    );
                    elementsJson.Add(element);
                }
                else
                {
                    var airSegment = (AirSegment) segment;
                    var buttons = new JArray();

                    buttons.Add(new JObject(
                        new JProperty("type", "web_url"),
                        new JProperty("url", "www.google.com"),
                        new JProperty("title", "View More Info")
                        )
                    );


                    var element = new JObject(
                        new JProperty("title", name),
                        new JProperty("image_url", image),
                        new JProperty("subtitle", $"From: {segment.From}\nTo:{segment.To}\nPrice: {priceString} \nDuration: {duration}"),
                        //new JProperty("default_action", defaultAction),
                        new JProperty("buttons", buttons)
                    );
                    elementsJson.Add(element);
                }
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

        private static string GetEmojiSegment(string kind)
        {
            switch (kind)
            {
                //"unknown, plane, helicopter, car, bus, taxi, rideshare, shuttle, towncar, train, tram, cablecar, subway, ferry, foot, animal, bicycle"
                case "subway":
                case "train":
                    return "🚆";
                case "plane":
                    return"✈️";
                case "car":
                    return "🚗";
                case "bus":
                case "shuttle":
                    return "🚌";
                case "ferry":
                    return "⛴️";
                case "bicycle":
                    return "🚲";
                case "foot":
                    return "🚶";
                default:
                    return "";
            }
        }

        public static IEnumerable<Segment> ConstructSegmentObjects(string origin, string dest, string routeName, bool oPos, bool dPos, string location, string currency)
        {
            var geolocation = "";
            var url = "";
            if (oPos || dPos)
            {
                var user = new UserHandler().GetUser(MessageHandler.SenderId);
                if (string.IsNullOrEmpty(location))
                {
                    MessageHandler.SendTextMessage(
                        $"I am sorry there has been an error!");
                    return null;
                }
                if (oPos)
                    url =
                        $"http://free.rome2rio.com/api/1.4/json/Search?key={R2RApiKey}&oPos={location}&dName={dest}&currencyCode={currency}";
                if (dPos)
                    url =
                        $"http://free.rome2rio.com/api/1.4/json/Search?key={R2RApiKey}&oName={origin}&dPos={location}&currencyCode={currency}";
            }
            url = string.IsNullOrEmpty(url) ? $"http://free.rome2rio.com/api/1.4/json/Search?key={R2RApiKey}&oName={origin}&dName={dest}&currencyCode={currency}" : url;

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
            var places = (JArray)responseJson["places"];
            var agencies = (JArray)responseJson["agencies"];
            var airlines = (JArray)responseJson["airlines"];
            var vehicles = (JArray)responseJson["vehicles"];

            var originLongName = places[0]["longName"];
            var destLongName = places[1]["longName"];
            var segmentList = new List<Segment>();

            foreach (JObject route in routes)
            {
                var name = (string)route["name"];
                if (name != routeName) continue;

                foreach (JObject segment in route["segments"])
                {
                    var kind = (string)segment["segmentKind"];
                    segmentList.Add(kind == "surface" ? HandleSurfaceSegment(segment, agencies, places, vehicles) : HandleAirSegment(segment, airlines, places, vehicles));
                }
            }

            return segmentList;
        }



        public static IEnumerable<Tuple<double, double>> Decode(string encodedPoints)
        {
            var polylineChars = encodedPoints.ToCharArray();
            var index = 0;

            var currentLat = 0;
            var currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                yield return new Tuple<double, double>(Convert.ToDouble(currentLat)/1E5, Convert.ToDouble(currentLng)/1E5);
            }
        }

        public static Segment HandleSurfaceSegment(JObject segment, JArray agencies, JArray places, JArray vehicles)
        {
            var indicativePrices = segment["indicativePrices"] as JArray;
            var pLow = indicativePrices != null ? (int?)indicativePrices[0]["priceLow"] : 0;
            var pHigh = indicativePrices != null ? (int?)indicativePrices[0]["priceHigh"] : 0;
            var agency = (JObject)((JArray)segment["agencies"]).ElementAt(0);
            var vehicleJson = (JObject) vehicles.ElementAt((int) segment["vehicle"]);
            var kind = (string)vehicleJson["kind"];
            var agencyJson = agencies.ElementAt((int)agency["agency"]);
            var links = segment["links"] as JArray;
            var bookUrl = "";
            var scheUrl = "";

            if (links != null)
            {
                foreach (JObject link in links)
                {
                    if (((string)link["text"]).StartsWith("Book"))
                    {
                        bookUrl = (string)link["displayUrl"];
                    };
                    if (((string)link["text"]).StartsWith("Schedule"))
                    {
                        scheUrl = (string)link["displayUrl"];
                    };
                }
            }

            var defaultUrl = (string)agencyJson["url"];
            bookUrl = string.IsNullOrEmpty(bookUrl) ? defaultUrl : bookUrl;

            var segName = (string)agencyJson["name"];
            var phone = (string)agencyJson["phone"];
            var duration = (int)segment["transitDuration"] + (int)segment["transferDuration"];

            var image = RoutesHandler.FetchAgencyImage(segment, agencies);

            var from = (string)((JObject)places.ElementAt((int)segment["depPlace"]))["shortName"];
            var to = (string)((JObject)places.ElementAt((int)segment["arrPlace"]))["shortName"];
            var latlongs = Decode((string)segment["path"]);

            pLow = pLow ?? 0;
            pHigh = pHigh ?? 0;
            int? price = 0;
            if (pLow == 0 && pHigh == 0)
            {
                price = indicativePrices != null ? (int?)indicativePrices[0]["price"] : 0;
            }

            return new SurfaceSegment(segName, from, to, pLow ?? 0, pHigh ?? 0, duration, image.Item2, defaultUrl, bookUrl, scheUrl, phone, "", kind, price ?? 0);
        }

        private static Segment HandleAirSegment(JObject segment, JArray airlines, JArray places, JArray vehicles)
        {
            var indicativePrices = segment["indicativePrices"] as JArray;
            var pLow = indicativePrices != null ? (int?)indicativePrices[0]["priceLow"] : 0;
            var pHigh = indicativePrices != null ? (int?)indicativePrices[0]["priceHigh"] : 0;
            var vehicleJson = (JObject)vehicles.ElementAt((int)segment["vehicle"]);
            var kind = (string)vehicleJson["kind"];

            var duration = (int)segment["transitDuration"] + (int)segment["transferDuration"];
            var distance = (double) segment["distance"];

            var tuple = RoutesHandler.FetchAirlineImage(segment, airlines);
            var image = tuple.Item2;
            var segName = tuple.Item1;
            var from = (string)((JObject)places.ElementAt((int)segment["depPlace"]))["shortName"];
            var to = (string)((JObject)places.ElementAt((int)segment["arrPlace"]))["shortName"];

            pLow = pLow ?? 0;
            pHigh = pHigh ?? 0;
            int? price = 0;
            if (pLow == 0 && pHigh == 0)
            {
                price = indicativePrices != null ? (int?)indicativePrices[0]["price"] : 0;
            }

            return new AirSegment(segName, from, to, pLow ?? 0, pHigh ?? 0, duration, distance, image, "", kind, price ?? 0);
        }


    }
}