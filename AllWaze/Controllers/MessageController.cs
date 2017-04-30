using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ApiAiSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllWaze.Controllers
{
    [Route("")]
    public class MessageController : ApiController
    {
        private const string PageToken = "EAADm6heZBUeUBAAwA6Y2eagDmIc753LA3udfZCWND1yFVgAGga4TtvC3o2Be7mZBNXcZBlJllbzDjVeZAa853hGT2y8g2ZBhmR94RPSvj0rHwTJf0bX1rbBCz7ajVJEjQ0NYXZBq2oSX9tk5iRpPvrdwZBGDda0DR3KriejGHqXaMAZDZD";
        private static readonly AIConfiguration Config = new AIConfiguration("b889778ac1e84e2885120a47fdec9809", SupportedLanguage.English);
        private static readonly ApiAi ApiAi = new ApiAi(Config);

        // GET / for authenitication
        [HttpGet]
        public IHttpActionResult Get()
        {
            var qvPairs = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

            if (qvPairs["hub_mode"] == "subscribe" &&
                qvPairs["hub.verify_token"] == "tuxedo_monkey")
            {
                return Content(HttpStatusCode.Accepted , qvPairs["hub.challenge"]);
            }
            else
            {
                return Content(HttpStatusCode.Unauthorized, "Not Authorized!");
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
                foreach (dynamic eve in (JArray)entry.messaging)
                {
                    await SendMessage((string)eve.message.text, (string)eve.sender.id);
                }

            }

            return Ok();
        }

        private static async Task SendMessage(string message, string sender)
        {
            var aiResponse = ApiAi.TextRequest(message);
            message = aiResponse.Result.Fulfillment.Speech;

            var json = $"{{'recipient': {{ 'id': '{sender}' }}, 'message': {{ 'text': '{message}' }} }}";


            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var requestUri = $"https://graph.facebook.com/v2.6/me/messages?access_token={PageToken}";
                var response = await client.PostAsync(requestUri, content);
            }
        }
    }
}