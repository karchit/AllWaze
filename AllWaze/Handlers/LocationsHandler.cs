using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AllWaze.App_Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Route = AllWaze.Objects.Route;

namespace AllWaze.Handlers
{
    public static class LocationsHandler
    {
        private static string MessageEndPoint = "http://maps.google.com/maps/api/geocode/json?latlng={0}&sensor=false";
        private static readonly UserHandler db = new UserHandler();

        public static void UpdateUserLocation(string id, string location)
        {
            MessageHandler.SenderId = id;
            string address;
            using (var client = new WebClient())
            {
                var json = JObject.Parse(client.DownloadString(string.Format(MessageEndPoint, location)));
                var results = (JArray) json["results"];
                address = (string) results[0]["formatted_address"];
            }
            var user = db.GetUser(id);
            if (user == null)
            {
                var newUser = new FBUser
                {
                    SessionId = id,
                    Location = location,
                    LocationString = address,
                    Currency = "USD",
                };
                db._db.FBUsers.InsertOnSubmit(newUser);

                db._db.SubmitChanges();

            }
            else
            {
                user.LocationString = address;
                user.Location = location;
                db._db.SubmitChanges();
            }
            MessageHandler.SendTextMessage($"Location updated to {location} or you may be better know it as {address} :D");
        }



    }
}