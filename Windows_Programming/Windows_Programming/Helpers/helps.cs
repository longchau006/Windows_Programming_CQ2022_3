using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;

namespace Windows_Programming.Helpers
{
    internal class Helps
    {
        public static Dictionary<string, object> ToFirestoreDocument(Account account)
        {
            return new Dictionary<string, object>
            {
                { "id", account.Id },
                { "username", account.Username },
                { "email", account.Email },
                { "fullname", account.Fullname },
                { "address", account.Address }
            };
        }

        public static Account FromFirestoreDocument(Dictionary<string, object> doc)
        {
            return new Account
            {
                Id = Convert.ToInt32(doc["id"]),
                Username = doc["username"].ToString(),
                Email = doc["email"].ToString(),
                Fullname = doc["fullname"].ToString(),
                Address = doc["address"].ToString()
            };
        }

    }
}
