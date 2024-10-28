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

        //Function
        public async Task<FirebaseUserCredential> SignInWithEmailAndPasswordInFireBase(string email, string password)
        {
            return await authClient.SignInWithEmailAndPasswordAsync(email, password);
        }

        public async Task<FirebaseUserCredential> CreateAccountInFireBase(string email, string password)
        {
            return await authClient.CreateUserWithEmailAndPasswordAsync(email, password);
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