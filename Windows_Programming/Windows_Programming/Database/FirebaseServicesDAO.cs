using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;

using Firebase.Auth.Providers;
using Google.Cloud.Firestore;
using FirebaseUserCredential = Firebase.Auth.UserCredential;
using System.IO;
using Google.Cloud.Storage.V1;
using Windows_Programming.Model;
using Windows_Programming.Configs;
using Windows_Programming.Helpers;
using System.Diagnostics;



namespace Windows_Programming.Database
{
    public class FirebaseServicesDAO : IDao
    {

        private static FirebaseServicesDAO instance;// Singleton pattern
        private readonly FirebaseAuthClient authClient;//Authentication
        private readonly FirestoreDb firestoreDb;//Firestore
        private readonly StorageClient storageClient;//Storage

        //Singleton pattern
        public static FirebaseServicesDAO Instance
        {
            get => instance ??= new FirebaseServicesDAO();
        }


        //Initial Services
        private FirebaseServicesDAO()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = EnvVariables.API_KEY,
                AuthDomain = "tripplandatabase-8fbf9.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                new EmailProvider()
                }
            };

            authClient = new FirebaseAuthClient(config);
            firestoreDb = GetFirestoreDb();
            //storageClient = StorageClient.Create();
        }

        //Init FirestoreDB
        public static FirestoreDb GetFirestoreDb()
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Private.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);
            return FirestoreDb.Create("tripplandatabase-8fbf9");
        }

        //Function of Authentication
        public async Task<FirebaseUserCredential> SignInWithEmailAndPasswordInFireBase(string email, string password)
        {
            return await authClient.SignInWithEmailAndPasswordAsync(email, password);
        }

        public async Task<FirebaseUserCredential> CreateAccountInFireBase(string email, string password)
        {
            return await authClient.CreateUserWithEmailAndPasswordAsync(email, password);
        }

        //FirestoreDB

        public async Task<int> GetAccountsCount()
        {
            var accountsRef = firestoreDb.Collection("accounts");
            var snapshot = await accountsRef.GetSnapshotAsync();

            return snapshot.Documents.Count;
        }



        public async Task CreateAccountInFirestore(Account account){
            var accountsRef = firestoreDb.Collection("accounts");
            var docRef = accountsRef.Document(account.Id.ToString());
            //Convert to Dictionary
            var accountJson=Helps.ToFirestoreDocument(account);

            await docRef.SetAsync(accountJson);

            var planRef = docRef.Collection("plans");
            await planRef.Document("__placeholder").SetAsync(new Dictionary<string, object>());
        }

        public async Task<Account> GetAccountByID(int id)
        {
            var docRef = firestoreDb.Collection("accounts").Document(id.ToString());
            var snapshot = await docRef.GetSnapshotAsync();
            
            if (snapshot.Exists)
            {
                var accountData = snapshot.ToDictionary();
                return Helps.FromFirestoreDocument(accountData);
            }
            
            return null;
        }

        public async Task CreatePlanInFirestore(int accountId, Plan plan)
        {
            var planRef = firestoreDb.Collection("accounts")
                                     .Document(accountId.ToString())
                                     .Collection("plans")
                                     .Document(plan.Id.ToString());

            var planData = Helps.PlanToFirestoreDocument(plan);

     
            await planRef.SetAsync(planData);
            
            var activitiesRef = planRef.Collection("activities");
            await activitiesRef.Document("__placeholder").SetAsync(new Dictionary<string, object>());

        }
        public async Task UpdatePlanInFirestore(int accountId, int planId, Plan plan)
        {
            var planRef = firestoreDb.Collection("accounts")
                                     .Document(accountId.ToString())
                                     .Collection("plans")
                                     .Document(planId.ToString());

            var planData = Helps.UpdatePlanToFirestoreDocument(plan);

            await planRef.UpdateAsync(planData);

        }
        public async Task UpdateWhenDeletePlanInFirestore(int accountId, int planId, Plan plan)
        {
            var planRef = firestoreDb.Collection("accounts")
                                     .Document(accountId.ToString())
                                     .Collection("plans")
                                     .Document(planId.ToString());

            var planData = new Dictionary<string, object>
            {
                { "deleteddate", plan.DeletedDate.HasValue ? plan.DeletedDate.Value.ToString("o") : null}
            };
            await planRef.UpdateAsync(planData);

        }
        public async Task CreateActivityInFirestore(int accountId, int planId, Windows_Programming.Model.Activity activity)
        {
                var activityRef = firestoreDb.Collection("accounts")
                                             .Document(accountId.ToString())
                                             .Collection("plans")
                                             .Document(planId.ToString())
                                             .Collection("activities")
                                             .Document(activity.Id.ToString());
            
                var activityData = Helps.ActivityToFirestoreDocument(activity);

                await activityRef.SetAsync(activityData);
           
        }
        public async Task DeleteActivityInFirestore(int accountId, int planId, int activityId)
        {
   
            var activityRef = firestoreDb.Collection("accounts")
                                         .Document(accountId.ToString())
                                         .Collection("plans")
                                         .Document(planId.ToString())
                                         .Collection("activities")
                                         .Document(activityId.ToString());

            await activityRef.DeleteAsync();      
        }
        public async Task UpdateActivityInFirestore(int accountId, int planId, int activityId, Windows_Programming.Model.Activity activity)
        {
            var activityRef = firestoreDb.Collection("accounts")
                                         .Document(accountId.ToString())
                                         .Collection("plans")
                                         .Document(planId.ToString())
                                         .Collection("activities")
                                         .Document(activityId.ToString());
            var activityData = Helps.ActivityToFirestoreDocument(activity);

            await activityRef.UpdateAsync(activityData);
        }
        public async Task<List<Plan>> GetAllPlan(int id)
        {
            var plansSubcollection = firestoreDb.Collection("accounts")
                                               .Document(id.ToString())
                                               .Collection("plans");
            var plansSnapshot = await plansSubcollection.GetSnapshotAsync();

            List<Plan> plans = new List<Plan>();
            foreach (var planDoc in plansSnapshot.Documents)
            {
                var planData = planDoc.ToDictionary();

                if (planData.TryGetValue("id", out var idObj))
                {
                    var plan = Helps.PlanFromFirestoreDocument(planData);

                    var activitiesSubcollection = plansSubcollection.Document(plan.Id.ToString())
                                                                    .Collection("activities");
                    var activitiesSnapshot = await activitiesSubcollection.GetSnapshotAsync();
                    foreach (var activityDoc in activitiesSnapshot.Documents)
                    {

                        var activityData = activityDoc.ToDictionary();

                        if (activityData.TryGetValue("type", out var typeObj) && int.TryParse(typeObj.ToString(), out int type))
                        {
                            System.Diagnostics.Debug.WriteLine($"{type}");
                            switch (type)
                            {
                                case 1:
                                    var activity1 = Helps.ActivityFromFirestoreDocument(activityData);
                                    plan.Activities.Add(activity1);
                                    break;
                                case 2:
                                    var activity2 = Helps.TransportFromFirestoreDocument(activityData);
                                    plan.Activities.Add(activity2);
                                    break;
                                case 3:
                                    var activity3 = Helps.LodgingFromFirestoreDocument(activityData);
                                    plan.Activities.Add(activity3);
                                    break;
                                case 4:
                                    var activity4 = Helps.ExtendFromFirestoreDocument(activityData);
                                    plan.Activities.Add(activity4);
                                    break;
                            }
                        }
                    }
                    plans.Add(plan);
                }    
            }

            return plans;
        }
        public async Task<int> GetNumAllPlanInHome(int id)
        {
            var plansSubcollection = firestoreDb.Collection("accounts")
                                               .Document(id.ToString())
                                               .Collection("plans");
            var plansSnapshot = await plansSubcollection.GetSnapshotAsync();

            int num = -1;
            foreach (var planDoc in plansSnapshot.Documents)
            {
                var planData = planDoc.ToDictionary();

                if (!planData.TryGetValue("deleteddate", out var deleteDate) || deleteDate == null)
                {
                    num++;
                }
            }
            return num;
        }


        //From IDAO
        public List<Plan> GetAllPlanInHome()
        {
            throw new NotImplementedException();
        }

        public List<Plan> GetAllPlanInTrashCan()
        {
            throw new NotImplementedException();
        }

        public List<Account> GetAllAccount()
        {
            throw new NotImplementedException();
        }

        public List<Blog> GetAllBlog()
        {
            throw new NotImplementedException();
        }

        public List<Tour> GetAllTour()
        {
            throw new NotImplementedException();
        }

        public List<Blog> GetLastestBlog()
        {
            throw new NotImplementedException();
        }

        public Blog GetBlogById(int id)
        {
            throw new NotImplementedException();
        }

        public Tour GetTourById(int id)
        {
            throw new NotImplementedException();
        }
    }
}