using System.Linq;
using AllWaze.App_Data;
using AllWaze.Objects;

namespace AllWaze.Handlers
{
    public class UserHandler
    {
        private readonly FBUserDataContext _db = new FBUserDataContext();

        public void InsertUser(string id)
        {
            var fbUser = new FBUser {SessionId = id};

            if (_db.FBUsers.FirstOrDefault(u => u.SessionId == id) != null) return;

            _db.FBUsers.InsertOnSubmit(fbUser);

            _db.SubmitChanges();
        }

        public FBUser GetUser(string id)
        {
            return _db.FBUsers.FirstOrDefault(u => u.SessionId == id);
        }

        public string SetCurrency(string id, string currency)
        {
            if (!Currency.currencies.ContainsKey(currency)) return string.Empty;

            var user = GetUser(id);
            if (user != null) user.Currency = currency;
            else
            {
                user = new FBUser() {Currency = currency, SessionId = id};
                _db.FBUsers.InsertOnSubmit(user);
            }

            _db.SubmitChanges();
            return user.Currency;
        }

        public string GetCurrency(string id)
        {
            var user = GetUser(id);
            return user != null ? user.Currency : "USD";
        }
    }
}