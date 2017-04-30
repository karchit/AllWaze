using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllWaze.Controllers
{
    [Route("routes")]
    public class RoutesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {
            var req = Request.Content.ReadAsStreamAsync().Result;
            var body = new StreamReader(req).ReadToEnd();
            dynamic json = JsonConvert.DeserializeObject(body);

            if ((string) json.result.action == "routes")
            {
                var parameters = json.result.parameters;
                var origin = (string )parameters["origin"];
                var dest = (string )parameters["destination"];

                var message = GetDescription(origin, dest);

                var returnJson = new JObject(
                        new JProperty("speech", message),
                        new JProperty("displayText", message),
                        new JProperty("source", "routes")
                    );
                return Json(returnJson);
            }

            return InternalServerError();
            
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

        [HttpGet]
        public IHttpActionResult Get()
        {
            return Content(HttpStatusCode.OK, "Hey there!");

        }
    }
}
