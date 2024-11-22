using Google.Cloud.Firestore.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Sensors;
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
        public static Dictionary<string, object> ActivityToFirestoreDocument(Model.Activity activity)
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
            else if (activity is Model.Activity)
            {
                data.Add("venue", activity.Venue);
                data.Add("address", activity.Address);
            }    

                return data;
        }

        public static Plan PlanFromFirestoreDocument(Dictionary<string, object> documentData)
        {
            if (documentData == null)
            {
                throw new ArgumentNullException(nameof(documentData));
            }

            var plan = new Plan();

            // Mapping các trường
            if (documentData.TryGetValue("id", out var idValue) && idValue is long id)
                plan.Id = (int)id;

            if (documentData.TryGetValue("name", out var nameValue) && nameValue is string name)
                plan.Name = name;

            if (documentData.TryGetValue("planimage", out var imageValue) && imageValue is string planImage)
                plan.PlanImage = planImage;

            if (documentData.TryGetValue("startdate", out var startDateValue) && startDateValue is string startDateString)
                plan.StartDate = DateTime.Parse(startDateString);

            if (documentData.TryGetValue("enddate", out var endDateValue) && endDateValue is string endDateString)
                plan.EndDate = DateTime.Parse(endDateString);

            if (documentData.TryGetValue("description", out var descriptionValue) && descriptionValue is string description)
                plan.Description = description;

            if (documentData.TryGetValue("startlocation", out var startLocationValue) && startLocationValue is string startLocation)
                plan.StartLocation = startLocation;

            if (documentData.TryGetValue("endlocation", out var endLocationValue) && endLocationValue is string endLocation)
                plan.EndLocation = endLocation;

            if (documentData.TryGetValue("type", out var typeValue) && typeValue is bool type)
                plan.Type = type;

            if (documentData.TryGetValue("deleteddate", out var deletedDateValue) && deletedDateValue is string deletedDateString)
                plan.DeletedDate = DateTime.TryParse(deletedDateString, out var deletedDate) ? deletedDate : (DateTime?)null;

            plan.Activities = new List<Model. Activity>();

            return plan;
        }

        public static Model.Activity ActivityFromFirestoreDocument(Dictionary<string, object> documentData)
        {
            if (documentData == null)
            {
                throw new ArgumentNullException(nameof(documentData));
            }

            var activity = new Model.Activity();
            if (documentData.TryGetValue("id", out var idValue) && idValue is long id)
                activity.Id = (int)id;
            if (documentData.TryGetValue("type", out var typeValue) && typeValue is long type)
                activity.Type = (int)type;
            if (documentData.TryGetValue("name", out var nameValue) && nameValue is string name)
                activity.Name = name;
            if (documentData.TryGetValue("startdate", out var startDateValue) && startDateValue is string startDateString)
                activity.StartDate = DateTime.Parse(startDateString);
            if (documentData.TryGetValue("enddate", out var endDateValue) && endDateValue is string endDateString)
                activity.EndDate = DateTime.Parse(endDateString);
            if (documentData.TryGetValue("description", out var descriptionValue) && descriptionValue is string description)
                activity.Description = description;
            if (documentData.TryGetValue("venue", out var venueValue) && venueValue is string venue)
                activity.Venue = venue;
            if (documentData.TryGetValue("address", out var addressValue) && addressValue is string address)
                activity.Address = address;

            
            return activity;
        }
        public static Transport TransportFromFirestoreDocument(Dictionary<string, object> documentData)
        {
            if (documentData == null)
            {
                throw new ArgumentNullException(nameof(documentData));
            }

            var activity2 = new Transport();
            if (documentData.TryGetValue("id", out var idValue) && idValue is long id)
                activity2.Id = (int)id;
            if (documentData.TryGetValue("type", out var typeValue) && typeValue is long type)
                activity2.Type = (int)type;
            if (documentData.TryGetValue("name", out var nameValue) && nameValue is string name)
                activity2.Name = name;
            if (documentData.TryGetValue("startdate", out var startDateValue) && startDateValue is string startDateString)
                activity2.StartDate = DateTime.Parse(startDateString);
            if (documentData.TryGetValue("enddate", out var endDateValue) && endDateValue is string endDateString)
                activity2.EndDate = DateTime.Parse(endDateString);
            if (documentData.TryGetValue("description", out var descriptionValue) && descriptionValue is string description)
                activity2.Description = description;
            if (documentData.TryGetValue("vehicle", out var vehicleValue) && nameValue is string vehicle)
                activity2.Vehicle = vehicle;
            if (documentData.TryGetValue("startlocation", out var startLocationValue) && startLocationValue is string startLocation)
                activity2.StartLocation = startLocation;
            if (documentData.TryGetValue("endlocation", out var endLocationValue) && endLocationValue is string endLocation)
                activity2.EndLocation = endLocation;
            return activity2;
        }
        public static Lodging LodgingFromFirestoreDocument(Dictionary<string, object> documentData)
        {
            if (documentData == null)
            {
                throw new ArgumentNullException(nameof(documentData));
            }

            var activity3 = new Lodging();
            if (documentData.TryGetValue("id", out var idValue) && idValue is long id)
                activity3.Id = (int)id;
            if (documentData.TryGetValue("type", out var typeValue) && typeValue is long type)
                activity3.Type = (int)type;
            if (documentData.TryGetValue("name", out var nameValue) && nameValue is string name)
                activity3.Name = name;
            if (documentData.TryGetValue("startdate", out var startDateValue) && startDateValue is string startDateString)
                activity3.StartDate = DateTime.Parse(startDateString);
            if (documentData.TryGetValue("enddate", out var endDateValue) && endDateValue is string endDateString)
                activity3.EndDate = DateTime.Parse(endDateString);
            if (documentData.TryGetValue("description", out var descriptionValue) && descriptionValue is string description)
                activity3.Description = description;
            if (documentData.TryGetValue("roominfo", out var roominfoValue) && roominfoValue is string roominfo)
                activity3.RoomInfo = roominfo;
            if (documentData.TryGetValue("address", out var addressValue) && addressValue is string address)
                activity3.Address = address;
            return activity3;
        }
        public static Extend ExtendFromFirestoreDocument(Dictionary<string, object> documentData)
        {
            if (documentData == null)
            {
                throw new ArgumentNullException(nameof(documentData));
            }

            var activity4 = new Extend();
            if (documentData.TryGetValue("id", out var idValue) && idValue is long id)
                activity4.Id = (int)id;
            if (documentData.TryGetValue("type", out var typeValue) && typeValue is long type)
                activity4.Type = (int)type;
            if (documentData.TryGetValue("name", out var nameValue) && nameValue is string name)
                activity4.Name = name;
            if (documentData.TryGetValue("startdate", out var startDateValue) && startDateValue is string startDateString)
                activity4.StartDate = DateTime.Parse(startDateString);
            if (documentData.TryGetValue("enddate", out var endDateValue) && endDateValue is string endDateString)
                activity4.EndDate = DateTime.Parse(endDateString);
            if (documentData.TryGetValue("description", out var descriptionValue) && descriptionValue is string description)
                activity4.Description = description;
            if (documentData.TryGetValue("namemore", out var namemoreValue) && namemoreValue is string namemore)
                activity4.NameMore = namemore;
            if (documentData.TryGetValue("venue", out var venueValue) && venueValue is string venue)
                activity4.Venue = venue;
            if (documentData.TryGetValue("address", out var addressValue) && addressValue is string address)
                activity4.Address = address;
            return activity4;
        }

    }
}
