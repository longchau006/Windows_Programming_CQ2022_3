﻿using System;
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
using ApplicationData = Windows.Storage.ApplicationData;
using ApplicationDataContainer = Windows.Storage.ApplicationDataContainer;
using Timestamp = Google.Cloud.Firestore.Timestamp;
using System.Net.Http;
using System.Linq;


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
            storageClient = StorageClient.Create();
        }

        //Init FirestoreDB
        public static FirestoreDb GetFirestoreDb()
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Private.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);
            return FirestoreDb.Create("tripplandatabase-8fbf9");
        }
        // Init storageClient
        public static StorageClient GetStorageClient()
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Private.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);
            return StorageClient.Create();
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


        public async Task SendPasswordResetEmailAsync(string email)
        {
            try
            {
                await authClient.ResetEmailPasswordAsync(email);
            }
            catch (FirebaseAuthException authEx)
            {
                throw new Exception(ParseFirebaseError(authEx.Message));
            }
        }


        //---------------------------------------------------FirestoreDB

        /*public async Task<int> GetAccountsCount()
        {
            var accountsRef = firestoreDb.Collection("accounts");
            var snapshot = await accountsRef.GetSnapshotAsync();

            return snapshot.Documents.Count;
        }*/
        public async Task<int> GetMaxAccountId()
        {
            var accountsRef = firestoreDb.Collection("accounts");
            var snapshot = await accountsRef.GetSnapshotAsync();

            if (!snapshot.Documents.Any())
                return 0;

            var maxId = snapshot.Documents
                .Select(doc => int.Parse(doc.Id))
                .Max();

            return maxId;
        }



        public async Task CreateAccountInFirestore(Account account)
        {
            var accountsRef = firestoreDb.Collection("accounts");
            var docRef = accountsRef.Document(account.Id.ToString());
            //Convert to Dictionary
            var accountJson = Helps.ToFirestoreDocument(account);

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
            System.Diagnostics.Debug.WriteLine($"Trc khi xoa o firebase:  {plan.Id} va bien {planId}");
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
            /*for (int i = 0; i < plans.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Read o db {plans[i].Id}");
            }*/
            foreach (var plan in plans)
            {
                System.Diagnostics.Debug.WriteLine($"Read o db {plan.Id}");
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



        //----------------------------------------Storage in firebase
        public async Task<string> UploadImageToStorage(string localImagePath, int accountId, int planId)
        {

            System.Diagnostics.Debug.WriteLine($"Image vao trong upload len storage {localImagePath}");
            if (string.IsNullOrEmpty(localImagePath))
                return null;

            string storagePath = $"plans/{accountId}/plan{planId}";
            using var stream = File.OpenRead(localImagePath);
            System.Diagnostics.Debug.WriteLine($"ABC");

            var storageObject = await storageClient.UploadObjectAsync(
                "tripplandatabase-8fbf9.appspot.com",
                storagePath,
                "image/jpeg",
                stream
            );

            string encodedPath = Uri.EscapeDataString(storagePath);
            string publicUrl = $"https://firebasestorage.googleapis.com/v0/b/tripplandatabase-8fbf9.appspot.com/o/{encodedPath}?alt=media";

            return publicUrl;
        }

        public async Task DeleteImageFromStorage(int accountId, int planId)
        {
            string storagePath = $"plans/{accountId}/plan{planId}";

            await storageClient.DeleteObjectAsync(
                "tripplandatabase-8fbf9.appspot.com",
                storagePath
            );
        }

        public async Task DeleteAccountFolderFromStorage(int accountId)
        {
            string folderPath = $"plans/{accountId}/";

            var objects = storageClient.ListObjects("tripplandatabase-8fbf9.appspot.com", folderPath);

            foreach (var obj in objects)
            {
                await storageClient.DeleteObjectAsync(obj);
            }
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

        public async Task<MemoryStream> DownloadImageFromClientStorage(string imageName)
        {
            var downloadUri = storageClient.CreateUrlSigner().Sign("tripplandatabase-8fbf9.appspot.com", imageName, TimeSpan.FromMinutes(5), HttpMethod.Get);
            var memoryStream = new MemoryStream();
            await storageClient.DownloadObjectAsync("tripplandatabase-8fbf9.appspot.com", imageName, memoryStream);
            memoryStream.Position = 0; // Reset the stream position to the beginning
            return memoryStream;
        }
        public async Task<List<Blog>> GetAllBlogAsync()
        {
            var docRef = firestoreDb.Collection("blogs");
            var snapshot = await docRef.GetSnapshotAsync();
            List<Blog> blogs = new List<Blog>();
            foreach (var document in snapshot.Documents)
            {
                var blogData = document.ToDictionary();
                Blog blog = new Blog
                {
                    Id = document.Id, 
                    Title = blogData["title"].ToString(),
                    Content = blogData["content"].ToString(),
                    Author = int.Parse(blogData["author"].ToString()),
                    PublishDate = ((Timestamp)blogData["publishdate"]).ToDateTime()
                };
                if (blogData["image"] != null)
                {
                    blog.Image = $"blogs/{document.Id}";
                    MemoryStream dataStream = await DownloadImageFromClientStorage(blog.Image);
                    blog.Image = Convert.ToBase64String(dataStream.ToArray());
                }
                blogs.Add(blog);
            }
            return blogs;
        }

        public async Task<List<Tour>> GetAllTour()
        {
            List<Tour> tours = new List<Tour>();
            var docRef = firestoreDb.Collection("tours");
            var snapshot = await docRef.GetSnapshotAsync();

            foreach (var document in snapshot.Documents)
            {
                var blogData = document.ToDictionary();

                // Xử lý trường 'activities'
                var activities = blogData.ContainsKey("activities") && blogData["activities"] is Dictionary<string, object> activitiesMap
                    ? activitiesMap.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty)
                    : new Dictionary<string, string>();

                // Xử lý transport (có thể chứa map con)
                var transport = blogData.ContainsKey("transport") && blogData["transport"] is Dictionary<string, object> transportMap
                    ? transportMap.ToDictionary(
                        k => k.Key,
                        v => v.Value is Dictionary<string, object> subMap
                            ? subMap.ToDictionary(subKey => subKey.Key, subValue => subValue.Value?.ToString() ?? string.Empty)
                            : new Dictionary<string, string>())
                    : new Dictionary<string, Dictionary<string, string>>();

                Tour tour = new Tour
                {
                    Id = document.Id,
                    Name = blogData["name"].ToString(),
                    Places = blogData["places"].ToString(),
                    Description = blogData["description"].ToString(),
                    Schedule = blogData["schedule"].ToString(),
                    Image = $"{document.Id}{blogData["image"]}",
                    Price = int.Parse(blogData["price"].ToString()),
                    Rating = int.Parse(blogData["rating"].ToString()),
                    Link = blogData["link"].ToString(),
                    Activities = activities,
                    Transport = transport
                };
                MemoryStream dataStream = await DownloadImageFromClientStorage($"tours/{tour.Image}");
                tour.Image = Convert.ToBase64String(dataStream.ToArray());
                tours.Add(tour);
            }
            return tours;
        }

        public async Task<List<Blog>> GetLastestBlog()
        {
            List<Blog> latestBlog = new List<Blog>();
            var docRef = firestoreDb.Collection("blogs").OrderByDescending("publishdate").Limit(3);
            var snapshot = await docRef.GetSnapshotAsync();
            foreach (var document in snapshot.Documents)
            {
                var blogData = document.ToDictionary();
                Blog blog = new Blog
                {
                    Id = document.Id,
                    Title = blogData["title"].ToString(),
                    Content = blogData["content"].ToString(),
                    Author = int.Parse(blogData["author"].ToString()),
                    PublishDate = ((Timestamp)blogData["publishdate"]).ToDateTime()
                };
                if (blogData["image"] != null)
                {
                    MemoryStream dataStream = await DownloadImageFromClientStorage($"blogs/{document.Id}");
                    blog.Image = Convert.ToBase64String(dataStream.ToArray());
                }
                latestBlog.Add(blog);
            }
            return latestBlog;
        }

        public async Task<Blog> GetBlogById(string id)
        {
            var docRef = firestoreDb.Collection("blogs").Document(id);
            var snapshot = docRef.GetSnapshotAsync();
            if (snapshot.Result.Exists)
            {
                var blogData = snapshot.Result.ToDictionary();
                Blog blog = new Blog
                {
                    Id = snapshot.Result.Id,
                    Title = blogData["title"].ToString(),
                    Content = blogData["content"].ToString(),
                    Author = int.Parse(blogData["author"].ToString()),
                    PublishDate = ((Timestamp)blogData["publishdate"]).ToDateTime()
                };
                if (blogData["image"] != null)
                {
                    MemoryStream dataStream = await DownloadImageFromClientStorage($"blogs/{id}");
                    blog.Image = Convert.ToBase64String(dataStream.ToArray());
                }
                return blog;
            }
            return null;
        }

        public async Task<Tour> GetTourById(string id)
        {
            var docRef = firestoreDb.Collection("tours").Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var tourData = snapshot.ToDictionary();
                Tour tour = new Tour
                {
                    Id = snapshot.Id,
                    Name = tourData["name"].ToString(),
                    Places = tourData["places"].ToString(),
                    Description = tourData["description"].ToString(),
                    Schedule = tourData["schedule"].ToString(),
                    Image = $"{snapshot.Id}{tourData["image"]}",
                    Price = int.Parse(tourData["price"].ToString()),
                    Rating = int.Parse(tourData["rating"].ToString()),
                    Link = tourData["link"].ToString()
                };
                MemoryStream dataStream = await DownloadImageFromClientStorage($"tours/{tour.Image}");
                tour.Image = Convert.ToBase64String(dataStream.ToArray());
                return tour;
            }
            return null;    
        }

        public async Task AddBlog(Blog blog)
        {
            var blogsRef = firestoreDb.Collection("blogs");
            var docRef = blogsRef.Document();
            string documentId = docRef.Id;
            var blogData = new Dictionary<string, object>
            {
                { "title", blog.Title },
                { "content", blog.Content },
                { "author", blog.Author },
                { "publishdate", blog.PublishDate.ToUniversalTime() },
                { "image", Path.GetExtension(blog.Image) }
            };
            try
            {
                await AddImageToClientStorage(blog.Image, $"blogs/{documentId}");
                await docRef.SetAsync(blogData);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add blog: " + ex.Message);
            }
        }

        public string GetContentType(string filePath)
        {
            var contentTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
            };

            var extension = Path.GetExtension(filePath);

            if (extension != null && contentTypes.TryGetValue(extension, out string contentType))
            {
                return contentType;
            }

            // Default fallback
            return "application/octet-stream";
        }


        public async Task<List<Blog>> GetOwnBlog(int id) { 
            List<Blog> ownBlogs = new List<Blog>();
            var docRef = firestoreDb.Collection("blogs").WhereEqualTo("author", id);
            var snapshot = await docRef.GetSnapshotAsync();
            foreach (var document in snapshot.Documents)
            {
                var blogData = document.ToDictionary();
                Blog blog = new Blog
                {
                    Id = document.Id, // Change Id to string type in Blog class
                    Title = blogData["title"].ToString(),
                    Content = blogData["content"].ToString(),
                    Author = int.Parse(blogData["author"].ToString()),
                    PublishDate = ((Timestamp)blogData["publishdate"]).ToDateTime()
                };
                if (blogData["image"] != null)
                {
                    blog.Image = $"blogs/{document.Id}";
                    MemoryStream dataStream = await DownloadImageFromClientStorage(blog.Image);
                    blog.Image = Convert.ToBase64String(dataStream.ToArray());
                }
                ownBlogs.Add(blog);
            }
            return ownBlogs;
        }
        public async Task UpdateBlog(Blog blog)
        {
            var docRef = firestoreDb.Collection("blogs").Document(blog.Id);
            var snapshot = await docRef.GetSnapshotAsync();
            Dictionary<string, object> updates = new Dictionary<string, object>();
            if (blog.Title != null)
            {
                updates.Add("title", blog.Title);
            }
            if (blog.Content != null)
            {
                updates.Add("content", blog.Content);
            }
            updates.Add("publishdate", blog.PublishDate.ToUniversalTime());
            try
            {
                if (blog.Image != null)
                {
                    /*                var objectName = $"blogs/{blog.Id}{Path.GetExtension(blog.Image)}";
                                    await storageClient.DeleteObjectAsync("tripplandatabase-8fbf9.appspot.com", objectName);*/
                    updates.Add("image", Path.GetExtension(blog.Image));
                    await AddImageToClientStorage(blog.Image, $"blogs/{blog.Id}");
                }
                await docRef.UpdateAsync(updates);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update blog: " + ex.Message);
            }
        }

        public async Task DeleteBlog(string id)
        {
            var docRef = firestoreDb.Collection("blogs").Document(id);
            var objectName = $"blogs/{id}";
            await storageClient.DeleteObjectAsync("tripplandatabase-8fbf9.appspot.com", objectName);
            await docRef.DeleteAsync();
        }

        public Task<bool> CheckOwnBlog(string id, int accountId)
        {
            var docRef = firestoreDb.Collection("blogs").Document(id);
            var snapshot = docRef.GetSnapshotAsync();
            if (snapshot.Result.Exists)
            {
                var blogData = snapshot.Result.ToDictionary();
                if (int.Parse(blogData["author"].ToString()) == accountId)
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
        public async Task AddImageToClientStorage(string imagePath, string imageNameOnFirebase)
        {
            if (!File.Exists(imagePath))
            {
                return;
            }
            var uploadUri = await storageClient.InitiateUploadSessionAsync("tripplandatabase-8fbf9.appspot.com", imageNameOnFirebase, GetContentType(imagePath), null);
            using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                var uploadInstant = Google.Apis.Upload.ResumableUpload.CreateFromUploadUri(uploadUri, fileStream);
                await uploadInstant.UploadAsync();
            }
        }
        public async Task UpdateFullName(string fullName, int id)
        {
            var docRef = firestoreDb.Collection("accounts").Document(id.ToString());
            var snapshot = docRef.GetSnapshotAsync();
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "fullname", fullName }
            };
            await docRef.UpdateAsync(updates);
        }
        public Task UpdateAddress(string address, int id)
        {
            var docRef = firestoreDb.Collection("accounts").Document(id.ToString());
            var snapshot = docRef.GetSnapshotAsync();
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "address", address }
            };
            return docRef.UpdateAsync(updates);
        }

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
                // delete blog that belongs to the user
                var blogsRef = firestoreDb.Collection("blogs");
                var blogsSnapshot = await blogsRef.GetSnapshotAsync();
                foreach (var blogDoc in blogsSnapshot.Documents)
                {
                    var blogData = blogDoc.ToDictionary();
                    if (blogData["author"].ToString() == id.ToString())
                    {
                        if (blogData["image"] != null)
                        {
                            var objectName = $"blogs/{blogDoc.Id}";
                            await storageClient.DeleteObjectAsync("tripplandatabase-8fbf9.appspot.com", objectName);
                        }
                        await blogDoc.Reference.DeleteAsync();
                    }
                }
                await DeleteAccountFolderFromStorage(id);

                var plansRef = docRef.Collection("plans");
                var plansSnapshot = await plansRef.GetSnapshotAsync();
                foreach (var planDoc in plansSnapshot.Documents)
                {
                    await planDoc.Reference.DeleteAsync();
                }
                await docRef.DeleteAsync();
                await credential.User.DeleteAsync();
            }
            catch (FirebaseAuthException)
            {
                throw new Exception("Failed to authenticate user: Password is incorrect!");
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while deleting the user. Please try again! ");
            }
        }

        public async Task AddTour(Tour tour)
        {
            var toursRef = firestoreDb.Collection("tours");
            var docRef = toursRef.Document();
            string documentId = docRef.Id;
            var tourData = new Dictionary<string, object>
            {
                { "name", tour.Name },
                { "places", tour.Places },
                { "description", tour.Description },
                { "schedule", tour.Schedule },
                { "image", Path.GetExtension(tour.Image) },
                { "price", tour.Price },
                { "rating", tour.Rating },
                { "link", tour.Link }
            };
            try
            {
                await AddImageToClientStorage(tour.Image, $"tours/{documentId}");
                await docRef.SetAsync(tourData);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add tour: " + ex.Message);
            }
            // ...


        }
        
    }
}