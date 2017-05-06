using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Route = AllWaze.Objects.Route;

namespace AllWaze.Handlers
{
    public static class MessageHandler
    {
        private const string PageToken = "EAAEZCJ5a9RvkBAOimeVfFotC57ZC67x0e6gXVqfRqejNtzmaPMgvnhFgZCE8ZBqqbKC1qhE2uRvPcdlqdBlqlZCMFjQbpwZCdaV0JugAggp5fzZAITlodw83kMBJMs3sTpng8aTCcZBvNpQvNcF8aOcN056mxZChXMbhpkClovNktBQZDZD";
        private static readonly string MessageEndPoint = $"https://graph.facebook.com/v2.6/me/messages?access_token={PageToken}";

        public static string SenderId = string.Empty;

        public static async Task SendTextMessage(string message)
        {
            var json = $"{{\"recipient\": {{ \"id\": \"{SenderId}\" }}, \"message\": {{ \"text\": \"{message}\" }} }}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(MessageEndPoint, content);
            }

        }

        public static async Task SendCustomMessage(string message)
        {
            var json = $"{{\"recipient\": {{ \"id\": \"{SenderId}\" }}, \"message\": {message} }}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(MessageEndPoint, content);
                response.EnsureSuccessStatusCode();
            }
        }

        public static async Task SendTypingNotification(string sender)
        {

            var json = $"{{'recipient': {{ 'id': '{sender}' }}, 'sender_action': 'typing_on'}}";


            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var requestUri = $"https://graph.facebook.com/v2.6/me/messages?access_token={PageToken}";
                var response = await client.PostAsync(requestUri, content);
            }
        }

    }
}