using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using AllWaze.Handlers;
using AllWaze.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllWaze.Controllers
{
    [Route("intents")]
    public class IntentsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {
            var req = Request.Content.ReadAsStreamAsync().Result;
            var body = new StreamReader(req).ReadToEnd();
            dynamic json = JsonConvert.DeserializeObject(body);

            var parameters = json.result.parameters;
            var userId = MessageHandler.SenderId;
            var userHandler = new UserHandler();

            if ((string) json.result.action == "routes")
            {
                var origin = (string)parameters["origin"];
                var dest = (string)parameters["destination"];
                var currency = userHandler.GetCurrency(userId);
                string currencyCulture;
                Currency.currencies.TryGetValue(currency, out currencyCulture);

                RoutesHandler.EntryPoint(origin, dest, currencyCulture ?? "chr-Cher-US");

                return Ok();
            }
            else if ((string) json.result.action == "setCurrency")
            {
                var currency = (string)parameters["currency"];
                
                var newCurrency = userHandler.SetCurrency(userId, currency);

                var message = !string.IsNullOrEmpty(newCurrency)
                    ? $"Currency updated to {newCurrency}"
                    : $"Sorry but {currency} is not currently supported :/. Try again with another one.";

                MessageHandler.SendTextMessage(message);
                
                return Ok();

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


        [HttpGet]
        public IHttpActionResult Get()
        {
            //Use this for testing purposes ~ make sure the web server is up. 
            return Content(HttpStatusCode.OK, "Hey there!");

        }
    }
}
