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
using Windows_Programming.View;
using Microsoft.Windows.Storage;
using Windows.Storage;
using ApplicationData = Windows.Storage.ApplicationData;
using ApplicationDataContainer = Windows.Storage.ApplicationDataContainer;



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
        //public async Task<FirebaseUserCredential> SignInWithEmailAndPasswordInFireBase(string email, string password)
        //{
        //    return await authClient.SignInWithEmailAndPasswordAsync(email, password);
        //}

        public async Task<FirebaseUserCredential> SignInWithEmailAndPasswordInFireBase(string email, string password)
        {
            try
            {
                var credential = await authClient.SignInWithEmailAndPasswordAsync(email, password);
                if (credential?.User != null)
                {
                    string token = await credential.User.GetIdTokenAsync();
                    return credential;
                }
                throw new Exception("Authentication failed");
            }
            catch (FirebaseAuthException authEx)
            {
                throw new Exception(ParseFirebaseError(authEx.Message));
            }
        }
        private string ParseFirebaseError(string responseData)
        {
            System.Diagnostics.Debug.WriteLine("bbbbb");
            System.Diagnostics.Debug.WriteLine(responseData);
            System.Diagnostics.Debug.WriteLine("eeee");
            if (responseData.Contains("INVALID_LOGIN_CREDENTIALS"))
                return "Invalid email or password";
            if (responseData.Contains("EMAIL_NOT_FOUND"))
                return "Account does not exist";
            if (responseData.Contains("INVALID_PASSWORD"))
                return "Incorrect password";
            if (responseData.Contains("USER_DISABLED"))
                return "This account has been disabled";
            if (responseData.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
                return "Too many attempts. Please try again later";

            return "An error occurred during login";
        }


        //public async Task<FirebaseUserCredential> CreateAccountInFireBase(string email, string password)
        //{
        //    return await authClient.CreateUserWithEmailAndPasswordAsync(email, password);
        //}
        public async Task<FirebaseUserCredential> CreateAccountInFireBase(string email, string password)
        {
            try
            {
                var credential = await authClient.CreateUserWithEmailAndPasswordAsync(email, password);
                if (credential?.User != null)
                {
                    string token = await credential.User.GetIdTokenAsync();
                    return credential;
                }
                throw new Exception("Account creation failed");
            }
            catch (FirebaseAuthException authEx)
            {
                if (authEx.Message.Contains("EMAIL_EXISTS"))
                    throw new Exception("This email is already registered");
                if (authEx.Message.Contains("WEAK_PASSWORD"))
                    throw new Exception("Password should be at least 6 characters");
                if (authEx.Message.Contains("INVALID_EMAIL"))
                    throw new Exception("Please enter a valid email address");

                throw new Exception("Failed to create account: " + authEx.Message);
            }
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
        public async Task<Account> GetAccountByEmail(string email)
        {
            try
            {
                var accountsRef = firestoreDb.Collection("accounts");
                var query = accountsRef.WhereEqualTo("email", email);
                var querySnapshot = await query.GetSnapshotAsync();

                if (querySnapshot.Count > 0)
                {
                    var document = querySnapshot.Documents[0];
                    var accountData = document.ToDictionary();
                    
                    return new Account
                    {
                        Id = int.Parse(document.Id),
                        Email = accountData["email"].ToString(),
                        Username = accountData["username"].ToString(),
                        Address = accountData["address"].ToString(),
                        Fullname = accountData["fullname"].ToString(),
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving account: {ex.Message}");
            }
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
            System.Diagnostics.Debug.WriteLine($"AAAAAAAAA:  {plan.DeletedDate}");
            var planData = new Dictionary<string, object>
            {
                { "deleteddate", plan.DeletedDate.HasValue ? plan.DeletedDate.Value.ToString("o") : null}
            };
            await planRef.UpdateAsync(planData);

        }
        
        public async Task DeleteImediatelyPlanInFirestore(int accountId, Plan plan)
        {

            var planRef = firestoreDb.Collection("accounts")
                                         .Document(accountId.ToString())
                                         .Collection("plans")
                                         .Document(plan.Id.ToString());

            await planRef.DeleteAsync();
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

        public async Task addBlog(Blog blog, string imagePath)
        {
            var blogsRef = firestoreDb.Collection("blogs");
            var docRef = blogsRef.Document();
            var blogData = new Dictionary<string, object>
            {
                { "title", blog.Title },
                { "content", blog.Content },
                { "author", blog.Author },
                { "publishdate", blog.PublishDate.ToString("o") },
                { "image", blog.Image }
            };
            await addImageToClientStorage(imagePath);

            await docRef.SetAsync(blogData);
        }

        public async Task addImageToClientStorage(string path)
        {
            var bucketName = "tripplan-8fbf9.appspot.com";
            var objectName = Path.GetFileName(path);
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                await storageClient.UploadObjectAsync(bucketName, objectName, null, fileStream);
            }
        }

        public async Task UpdateFullName(string fullName, int id) {
            var docRef = firestoreDb.Collection("accounts").Document(id.ToString());
            var snapshot = docRef.GetSnapshotAsync();
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "fullname", fullName }
            };
            await docRef.UpdateAsync(updates);
        }
        public Task UpdateAddress(string address, int id) { 
            var docRef = firestoreDb.Collection("accounts").Document(id.ToString());
            var snapshot = docRef.GetSnapshotAsync();
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "address", address }
            };
            return docRef.UpdateAsync(updates);
        }

        // ...

        public async Task UpdatePassword(string oldPassword, string newPassword)
        {
            try
            {
                // Re-authenticate the user with the old password
                FirebaseUserCredential credential;
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                string email = localSettings.Values["email"].ToString();
                try
                {
                    credential = await authClient.SignInWithEmailAndPasswordAsync(email, oldPassword);
                    if (credential?.User == null)
                        throw new Exception("The old password is incorrect");
                }
                catch (FirebaseAuthException authEx)
                {
                    if (authEx.Message.Contains("INVALID_PASSWORD"))
                        throw new Exception("The old password is incorrect");
                    throw new Exception("Failed to re-authenticate user: The old password is incorrect");
                }

                // Update the password
                await credential.User.ChangePasswordAsync(newPassword);
            }
            catch (FirebaseAuthException authEx)
            {
                if (authEx.Message.Contains("WEAK_PASSWORD"))
                    throw new Exception("Password should be at least 6 characters");
                if (authEx.Message.Contains("INVALID_PASSWORD"))
                    throw new Exception("The old password is incorrect");

                throw new Exception("Failed to update password: " + authEx.Message);
            }
        }

        public async Task DeleteUser(string email, string password, int id)
        {
            try
            {
                var credential = await authClient.SignInWithEmailAndPasswordAsync(email, password);
                var docRef = firestoreDb.Collection("accounts").Document(id.ToString());
                await docRef.DeleteAsync();
                await credential.User.DeleteAsync();
            }
            catch (FirebaseAuthException)
            {
                throw new Exception("Failed to authenticate user: Password is incorrect!" );
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while deleting the user. Please try again! ");
            }
        }
    }
}