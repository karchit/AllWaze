using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using AllWaze.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllWaze.Controllers
{
    [Route("routes")]
    public class RoutesController : ApiController
    {
        private const string R2RApiKey = "ZnzQK7bo";

        [HttpPost]
        public IHttpActionResult Post()
        {
            var req = Request.Content.ReadAsStreamAsync().Result;
            var body = new StreamReader(req).ReadToEnd();
            dynamic json = JsonConvert.DeserializeObject(body);

            if ((string) json.result.action == "routes")
            {
                var parameters = json.result.parameters;
                var origin = (string) parameters["origin"];
                var dest = (string)parameters["destination"];

                if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(dest)) return Ok();
                var routes = ConstructRouteObjects(origin, dest);
                var message = ConstructJsonFromRoutes(routes);

                //var message = GetDescription(origin, dest);


                var returnJson = new JObject(
                        new JProperty("speech", message),
                        new JProperty("displayText", message),
                        new JProperty("source", "routes")
                    );
                return Json(returnJson);
            }

            return Ok();
            
        }

        private string GetDescription(string origin, string dest)
        {
            using (var client = new WebClient())
            {
                var url = $"https://www.rome2rio.com/s/{origin}/{dest}".Replace(" ", "-");
                var htmlCode = client.DownloadString(url);
                var regex = new Regex("<meta id='metadescription' content='(?<metadescription>.+)' name.+");
                var match = regex.Match(htmlCode).Groups["metadescription"];
                return match.Value;
            }
        }

        private IEnumerable<Route> ConstructRouteObjects(string origin, string dest)
        {
            var url = $"http://free.rome2rio.com/api/1.4/json/Search?key={R2RApiKey}&oName={origin}&dName={dest}";
            var routesList = new List<Route>();
            JObject responseJson;

            using (var client = new WebClient())
            {
                var response = client.DownloadString(url);
                responseJson = JObject.Parse(response);
            }

            var routes = (JArray) responseJson["routes"];

            foreach (JObject route in routes)
            {
                var indicativePrices = route["indicativePrices"] as JArray;
                var majorSegment = (JObject)((JArray) route["segments"]).OrderByDescending(s => (int) s["distance"]).First();

                var name = (string) route["name"];
                var pLow = indicativePrices != null ? (int?) indicativePrices[0]["priceLow"]: 0;
                var pHigh = indicativePrices != null ? (int?) indicativePrices[0]["priceHigh"] : 0;
                var duration = (int) route["totalDuration"];

                var image = ((string) majorSegment["segmentKind"]).Equals("surface")
                    ? FetchAgencyImage(majorSegment, responseJson["agencies"] as JArray)
                    : FetchAirlineImage(majorSegment, responseJson["airlines"] as JArray);


                routesList.Add(new Route(name, pLow ?? 0, pHigh ?? 0, duration, string.IsNullOrEmpty(image) && name.Contains("Drive") ? "http://i.imgur.com/PfC7OYk.jpg" : image));
            }

            return routesList.OrderBy(r => r.Duration).Take(Math.Min(routesList.Count, 6)); // Return maximum of 6 routes. 
        }

        private string FetchAgencyImage(JObject majorSegment, JArray agencies)
        {
            var segmentAgencies = majorSegment["agencies"] as JArray;
            if (segmentAgencies == null) return string.Empty;

            var index = (int)segmentAgencies.ElementAt(0)["agency"];
            var agencyUrl = new Uri((string) agencies.ElementAt(index)["url"]).Host.Replace("www.", "");
            agencyUrl = FixAgencyUrls(agencyUrl);

            using (var client = new WebClient())
            {
                var url = $"https://autocomplete.clearbit.com/v1/companies/suggest?query={agencyUrl}";

                var companiesArray = JArray.Parse(client.DownloadString(url));

                return companiesArray.Any()
                    ? (string) companiesArray[0]["logo"] + "?size=220"
                    : "www.rome2rio.com/" + (string) agencies.ElementAt(index)["icon"]["url"];

            }
        }

        private string FixAgencyUrls(string agencyUrl)
        {
            return agencyUrl.Contains("bahn.com") ? "bahn.de" : agencyUrl;
        }

        private string FetchAirlineImage(JObject majorSegment, JArray airlines)
        {
            var hops = (JArray)((JArray) majorSegment["outbound"]).ElementAt(0)["hops"];
            var index = (int) hops.ElementAt(0)["airline"];
            var airlineName = (string) airlines.ElementAt(index)["name"];
            var airlineCode = (string) airlines.ElementAt(index)["code"];

            using (var client = new WebClient())
            {
                var companiesArray = JArray.Parse(client.DownloadString($"https://autocomplete.clearbit.com/v1/companies/suggest?query={airlineName}"));
                return companiesArray.Any()
                    ? (string)companiesArray[0]["logo"] + "?size=220"
                    : $"http://pics.avs.io/220/200/{airlineCode}.png";

            }
        }

        private string ConstructJsonFromRoutes(IEnumerable<Route> routes){
            var routesList = routes.ToList();

            var elementsJson = new JArray();
            foreach(var route in routesList)
            {
                var defaultAction = new JObject(
                    new JProperty("type", "web_url"),
                    new JProperty("url", "https://www.rome2rio.com"),
                    new JProperty("webview_height_ratio", "compact")
                );

                var buttons = new JArray
                {
                    new JObject(new JProperty("type", "web_url"), 
                    new JProperty("url", "https://www.rome2rio.com"),
                    new JProperty("title", "View Website"))
                };

                var element = new JObject(
                    new JProperty("title", route.Name),
                    new JProperty("image_url", route.Image),
                    new JProperty("subtitle", $"Price: {route.PriceLow} - {route.PriceHigh}\nDuration: {route.Duration} minutes"),
                    new JProperty("default_action", defaultAction),
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


        [HttpGet]
        public IHttpActionResult Get()
        {
            //Use this for testing purposes ~ make sure the web server is up. 
            return Content(HttpStatusCode.OK, "Hey there!");

        }
    }
}
