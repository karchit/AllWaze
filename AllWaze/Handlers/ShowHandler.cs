using System;
using System.Net;
using AllWaze.App_Data;
using AllWaze.Objects;
using Newtonsoft.Json.Linq;

namespace AllWaze.Handlers
{
    public static class ShowHandler
    {
        private static readonly string MessageEndPoint = "http://maps.google.com/maps/api/geocode/json?latlng={0}&sensor=false";
        private static readonly UserHandler db = new UserHandler();

        public static void EntryPoint(string id, string attr)
        {
            var isCurrency = attr.Equals("currency", StringComparison.InvariantCultureIgnoreCase);
            var isLocation = attr.Equals("location", StringComparison.InvariantCultureIgnoreCase);
            if (!isCurrency && !isLocation)
            {
                MessageHandler.SendTextMessage($"I could not identify what you were asking for. Please ask for either location or currency");
                return;
            }
            var name = isLocation ? "location" : "currency";
            var user = db.GetUser(id);
            var toShow = isLocation ? user.LocationString : user.Currency + $" ({Currency.CurrencyNames[user.Currency]})";
            if (string.IsNullOrEmpty(toShow))
            {
                MessageHandler.SendTextMessage($"Your {name} is not currently set.");
                return;
            }
            MessageHandler.SendTextMessage($"Your {name} is currently set as {toShow} ;)");
        }
    }
}