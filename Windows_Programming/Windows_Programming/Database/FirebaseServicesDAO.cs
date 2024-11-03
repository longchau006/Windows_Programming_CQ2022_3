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

        public void addBlog(Blog blog)
        {
            throw new NotImplementedException();
        }
    }
}