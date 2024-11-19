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


        public static Dictionary<string, object> PlanToFirestoreDocument(Plan plan)
        {
            return new Dictionary<string, object>
            {
                {"id",plan.Id},
                { "name", plan.Name },
                { "planimage", plan.PlanImage },
                { "startdate", plan.StartDate.ToString("o") }, 
                { "enddate", plan.EndDate.ToString("o") },    
                { "description", plan.Description },
                { "startlocation", plan.StartLocation },
                { "endlocation", plan.EndLocation },
                { "type", plan.Type },
                { "deleteddate", plan.DeletedDate.HasValue ? plan.DeletedDate.Value.ToString("o") : null }
            };
        }

        public static Dictionary<string, object> UpdatePlanToFirestoreDocument(Plan plan)
        {
            return new Dictionary<string, object>
            {
                { "name", plan.Name },
                { "planimage", plan.PlanImage },
                { "startdate", plan.StartDate.ToString("o") },
                { "enddate", plan.EndDate.ToString("o") },
                { "description", plan.Description },
                { "startlocation", plan.StartLocation },
                { "endlocation", plan.EndLocation }
            };
        }
        public static Dictionary<string, object> ActivityToFirestoreDocument(Activity activity)
        {
            var data = new Dictionary<string, object>
            {
                {"id", activity.Id },
                {"type", activity.Type},
                { "name", activity.Name },
                { "startdate", activity.StartDate?.ToString("o") },
                { "enddate", activity.EndDate?.ToString("o") },
                { "description", activity.Description }
            };
            if (activity is Transport transport)
            {
                data.Add("vehicle", transport.Vehicle);
                data.Add("startlocation", transport.StartLocation);
                data.Add("endlocation", transport.EndLocation);
            }
            else if (activity is Lodging lodging)
            {
                data.Add("roominfo", lodging.RoomInfo);
                data.Add("address", lodging.Address);
            }
            else if (activity is Extend extend)
            {
                data.Add("namemore", extend.NameMore);
                data.Add("venue", extend.Venue);
                data.Add("address", extend.Address);
            }
            else if (activity is Activity)
            {
                data.Add("venue", activity.Venue);
                data.Add("address", activity.Address);
            }    

                return data;
        }



    }
}
