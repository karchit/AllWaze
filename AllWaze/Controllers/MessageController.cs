using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AllWaze.Handlers;
using ApiAiSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllWaze.Controllers
{
    [Route("")]
    public class MessageController : ApiController
    {
        private static readonly AIConfiguration Config = new AIConfiguration("b889778ac1e84e2885120a47fdec9809", SupportedLanguage.English);
        private static readonly ApiAi ApiAi = new ApiAi(Config);

        // GET / for authenitication
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var qvPairs = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

            if (qvPairs["hub.mode"] == "subscribe" &&
                qvPairs["hub.verify_token"] == "tuxedo_monkey")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(qvPairs["hub.challenge"], Encoding.UTF8, "text/plain")
                };
                return resp;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        // POST /. Start of messaging. :)
        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            var req = Request.Content.ReadAsStreamAsync().Result;
            var json = new StreamReader(req).ReadToEnd();
            dynamic x = JsonConvert.DeserializeObject(json);

            foreach (dynamic entry in (JArray)x.entry)
            {
                foreach (JObject eve in (JArray)entry.messaging)
                {
                    await MessageHandler.SendTypingNotification((string)eve["sender"]["id"]);
                    if (eve["postback"] != null)
                    {
                        PostbackHandler.EntryPoint(eve["postback"]["payload"]);
                    }
                    else
                    {
                        await SendMessage((string) eve["message"]["text"], (string) eve["sender"]["id"]);
                    }
                }

            }

            return Ok();
        }

        private static async Task SendMessage(string message, string sender)
        {
            MessageHandler.SenderId = sender;

            var aiResponse = ApiAi.TextRequest(message);
            message = aiResponse.Result.Fulfillment.Speech;
            var intentName = aiResponse.Result.Action;

            if (string.IsNullOrWhiteSpace(message)) message = "I am sorry, I could not resolve your query. :( Please check your input and try again.";

            if (intentName != null && intentName.StartsWith("smalltalk."))
            {
                await MessageHandler.SendTextMessage(message);
            }
        }

    }
}